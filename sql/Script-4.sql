create or replace function check_user_role(p_user_id int, p_required_role text)
returns void
language plpgsql
as $$
declare
    v_role text;
begin
    select role into v_role
    from users
    where user_id = p_user_id;

    if v_role is null then
        raise exception 'Пользователь с id % не найден', p_user_id;
    end if;

    if v_role != p_required_role then
        raise exception 'Доступ запрещен для роли %', v_role;
    end if;
end;
$$;

create or replace function set_copy_status(
    p_copy_id int,
    p_status text
)
returns void
language plpgsql
as $$
declare
    v_status_id int;
begin
    -- Находим статус
    select status_id into v_status_id
    from copy_status
    where name = p_status;

    if v_status_id is null then
        raise exception 'Неизвестный статус экземпляра: %', p_status;
    end if;

    -- Применяем
    update copy_books
    set status_id = v_status_id
    where copy_id = p_copy_id;
end;
$$;

begin;
select set_copy_status(10, 'забронирована');
rollback;

begin;
select set_copy_status(10, 'забронирована2');
rollback;


create or replace function get_book_copies_info(
    p_book_id int,
    p_is_worker bool
)
returns table(
    copy_id int,
    book_title text,
    publisher text,
    year_publish int,
    country text,
    section text,
    closet int,
    shelf int,
    place int,
    status text,
    authors text,
    genres text
)
language plpgsql
as $$
begin
    return query
    select
        cb.copy_id,
        b.title::text,
        p.name::text as publisher,
        cb.year_publish,
        c.name::text as country,
        s.name::text as section,
        cb.closet,
        cb.shelf,
        cb.place,
        cs.name::text as status,
        string_agg(a.full_name, ', ')::text as authors,
		string_agg(g.name, ', ')::text as genres
    from copy_books cb
    join books b on cb.book_id = b.book_id
    join publishers p on cb.publisher_id = p.publisher_id
    join countries c on p.country_id = c.country_id
    join sections s on cb.section_id = s.section_id
    join copy_status cs on cb.status_id = cs.status_id
    join book_authors ba on b.book_id = ba.book_id
    join authors a on ba.author_id = a.author_id
	join book_genres bg on b.book_id = bg.book_id
    join genres g on bg.genre_id = g.genre_id
    where cb.book_id = p_book_id
      and (true = p_is_worker or cs.name = 'в наличии') and is_deleted = false
    group by 
        cb.copy_id, b.title, p.name, cb.year_publish,
        c.name, s.name, cb.closet, cb.shelf, cb.place, cs.name;
end;
$$;

select * from get_book_copies_info(2, true);
select * from get_book_copies_info(2, false);

-- Возвращает все брони пользователя
create or replace function get_user_requests(p_user_id int)
returns table (
    request_id int,
    copy_id int,
    book_title text,
    status text,
    request_date timestamp,
    delete_date timestamp,
    days_left int
) as $$
begin
	perform check_user_role(p_user_id, 'reader');
    return query
    select 
        r.request_id::int,
        r.copy_id::int,
        b.title::text,
        r.status::text,
        r.created_at::timestamp,
        (r.created_at + interval '7 days')::timestamp as delete_date,
        greatest(0, date_part('day', (r.created_at + interval '7 days' - now())))::int as days_left
    from requests r
    join copy_books c on c.copy_id = r.copy_id
    join books b on b.book_id = c.book_id
    where r.reader_id = p_user_id and (r.status != 'отклонено')
    order by r.created_at desc;
end;
$$ language plpgsql;

select * from get_user_requests(6);


-- Процедура добавления бронирования
create or replace procedure create_reservation(p_reader_id int, p_copy_id int)
language plpgsql
as $$
declare
    copy_status_id int;
begin
	perform check_user_role(p_reader_id, 'reader');
    -- Получаем текущий статус копии
    select status_id into copy_status_id
    from copy_books
    where copy_id = p_copy_id;

    if copy_status_id != (select status_id from copy_status where name = 'в наличии') then
        -- Если копия недоступна, создаём отклонённый запрос
        insert into requests(reader_id, copy_id, status, created_at)
        values (p_reader_id, p_copy_id, 'отклонено', now());
    else
        -- Если доступна, создаём запрос и меняем статус копии
        insert into requests(reader_id, copy_id, status, created_at)
        values (p_reader_id, p_copy_id, 'на рассмотрении', now());

		perform set_copy_status(p_copy_id, 'забронирована');
    end if;
end;
$$;

begin;
call create_reservation(1, 5);
rollback;
begin;
call create_reservation(6, 5);
rollback;
--Добавление пользователя
create or replace procedure create_reader(
    p_login text,
    p_email text,
    p_password text,
    p_address text,
    p_phone_number text
)
language plpgsql
as $$
declare
    new_user_id int;
begin
    -- проверяем уникальность email и логина
    if exists (select 1 from users where email = p_email) then
        raise exception 'email уже используется';
    end if;

    if exists (select 1 from users where login = p_login) then
        raise exception 'логин уже используется';
    end if;

    -- вставляем нового пользователя с ролью reader
    insert into users (login, email, password_hash, role)
    values (p_login, p_email, p_password, 'reader')
    returning user_id into new_user_id;

    -- вставляем запись в таблицу readers
    insert into readers (user_id, address, phone_number)
    values (new_user_id, p_address, p_phone_number);
end;
$$;
begin;
call create_reader(
    'Test',
    'test@mail.com',
    'hash',
    'address test',
    '+79116300903'
);
rollback;

--Удаление брони пользователем
create or replace procedure cancel_reservation(p_request_id int)
language plpgsql
as $$
declare
    v_copy_id int;
begin
    select copy_id into v_copy_id
    from requests
    where request_id = p_request_id;

    if v_copy_id is null then
        raise notice 'Бронь с таким ID не найдена';
        return;
    end if;

    perform set_copy_status(v_copy_id, 'в наличии');

    update requests
	set status = 'отклонено'
    where request_id = p_request_id;
end;
$$;

begin;
call cancel_reservation(42);
rollback;
--Добавление экземпляра
create or replace procedure add_book_copy(
    p_book_id int,
    p_publisher text,
    p_section text,
    p_year_publish int,
    p_closet int,
    p_shelf int,
    p_place int,
    p_status text,
    p_num_of_pages int,
    p_user_id int
)
language plpgsql
as $$
declare
	v_book_id int;
    v_publisher_id int;
    v_section_id int;
    v_status_id int;
begin
    -- Проверка роли (доступ только для сотрудника)
    perform check_user_role(p_user_id, 'worker');

    -- Проверка корректности издателя
    select publisher_id into v_publisher_id
    from publishers
    where upper(name) = upper(p_publisher);

    if v_publisher_id is null then
        raise exception 'Издатель "%" не найден', p_publisher;
    end if;

    -- Проверка корректности раздела
    select section_id into v_section_id
    from sections
    where upper(name) = upper(p_section);

    if v_section_id is null then
        raise exception 'Раздел "%" не найден', p_section;
    end if;

    -- Проверка статуса экземпляра
    select status_id into v_status_id
    from copy_status
    where upper(name) = upper(p_status);

    if v_status_id is null then
        raise exception 'Статус экземпляра "%" не найден', p_status;
    end if;
    -- Проверяем, существует ли книга
    select book_id into v_book_id
    from books
    where book_id = p_book_id;

    if v_book_id is null then
        raise exception 'Книга "%" не найдена в каталоге. Добавьте книгу перед созданием экземпляра.', p_title;
    end if;
    -- Добавление экземпляра
    insert into copy_books(
        book_id,
        publisher_id,
        section_id,
        year_publish,
        closet,
        shelf,
        place,
        status_id,
        num_of_pages
    )
    values (
        p_book_id,
        v_publisher_id,
        v_section_id,
        p_year_publish,
        p_closet,
        p_shelf,
        p_place,
        v_status_id,
        p_num_of_pages
    );
end;
$$;


begin;
call add_book_copy(3,'Эксмо','История', 1866, 1, 2, 3, 'в наличии', 432, 6);
rollback;

begin;
call add_book_copy(3,'Эксмо','История', 1866, 1, 2, 3, 'в наличии', 432, 10);
rollback;

-- удаление экземпляра
create or replace procedure delete_copy(p_copy_id int, p_user_id int)
language plpgsql
as $$
declare
    v_status text;
begin
    perform check_user_role(p_user_id, 'worker');

    select s.name into v_status
    from copy_books cb
    join copy_status s on s.status_id = cb.status_id
    where cb.copy_id = p_copy_id;

    if v_status in ('выдана', 'забронирована') then
        raise exception 'Cannot delete copy with status: %', v_status;
    end if;
    update copy_books
    set is_deleted = true
    where copy_id = p_copy_id;

    update requests
    set is_deleted = true
    where copy_id = p_copy_id;
end;
$$;

alter table copy_books add column is_deleted bool;
alter table requests add column is_deleted bool;
begin;
call delete_copy(7, 6);
rollback;
begin;
call delete_copy(7, 10);
rollback;

--Редактирование экземпляра
create or replace procedure update_copy(
    p_copy_id int,
    p_publisher_name text,
    p_section_name text,
    p_year int,
    p_closet int,
    p_shelf int,
    p_place int,
    p_status_name text,
    p_user_id int
)
language plpgsql
as $$
declare
    v_publisher_id int;
    v_section_id int;
    v_status_id int;
begin
	perform check_user_role(p_user_id, 'worker');
    -- Издательство
    select publisher_id into v_publisher_id
    from publishers
    where upper(name) = upper(p_publisher_name);
    -- Раздел
    select section_id into v_section_id
    from sections
    where upper(name) = upper(p_section_name);
    -- Статус
    select status_id into v_status_id
    from copy_status
    where upper(name) = upper(p_status_name);
    if v_status_id is null then
        raise exception 'Unknown copy status: %', p_status_name;
    end if;
    -- Обновляем
    update copy_books
    set
        publisher_id = v_publisher_id,
        section_id = v_section_id,
        year_publish = p_year,
        closet = p_closet,
        shelf = p_shelf,
        place = p_place,
        status_id = v_status_id
    where copy_id = p_copy_id;
end;
$$;

begin;
call update_copy(7,'Эксмо','История', 1866, 1, 2, 3, 'в наличии', 6);
rollback;

begin;
call update_copy(7,'Эксмо','История', 1866, 1, 2, 3, 'в наличии', 10);
rollback;
rollback;

create or replace procedure approve_request(p_worker_id int,
    p_request_id int
)
language plpgsql
as $$
declare
    v_copy_id int;
    v_reader_id int;
    v_status_issued_id int;
begin
	perform check_user_role(p_worker_id, 'worker');
    -- получаем копию и читателя из запроса
    select copy_id, reader_id into v_copy_id, v_reader_id
    from requests
    where request_id = p_request_id;

    if v_copy_id is null then
        raise exception 'Запрос с id % не найден', p_request_id;
    end if;

    -- создаём выдачу
    insert into book_loans(worker_id, copy_id, reader_id, loan_date, status, request_id)
    values(p_worker_id, v_copy_id, v_reader_id, current_date, 'выдана', p_request_id);
    -- обновляем статус копии
    perform set_copy_status(v_copy_id, 'выдана');
    -- обновляем статус запроса
    update requests
    set status = 'подтверждено'
    where request_id = p_request_id;
end;
$$;
begin;
call approve_request(6,31);
rollback;
begin;
call approve_request(10,31);
rollback;
create or replace procedure reject_request(
    p_request_id int
)
language plpgsql
as $$
declare
    v_copy_id int;
begin
    select copy_id
    into v_copy_id
    from requests
    where request_id = p_request_id;
    if v_copy_id is null then
        raise exception 'Запрос % не найден', p_request_id;
    end if;
    perform set_copy_status(v_copy_id, 'в наличии');
    update requests
    set status = 'отклонено'
    where request_id = p_request_id;

end;
$$;
begin;
call reject_request(31);
rollback;

create or replace function set_due_date()
returns trigger
language plpgsql
as $$
begin
    if new.due_date is null then
        new.due_date := new.loan_date + interval '14 days';
    end if;
    return new;
end;
$$;

create trigger trg_set_due_date
before insert on book_loans
for each row
execute function set_due_date();


create or replace procedure return_book(
    p_loan_id int
)
language plpgsql
as $$
declare
    v_copy_id int;
    v_request_id int;
    v_status_available int;
	v_due_date date;
begin
    -- Берём данные из выдачи
    select copy_id, request_id, due_date
        into v_copy_id, v_request_id, v_due_date
    from book_loans
    where loan_id = p_loan_id;

    if v_copy_id is null then
        raise exception 'Loan with id % not found', p_loan_id;
    end if;

    -- Обновляем loan
	if(current_date <= v_due_date) then 
	    update book_loans
	    set status = 'возвращена',
	        return_date = current_date
	    where loan_id = p_loan_id;
	else
		update book_loans
	    set status = 'просрочена',
	        return_date = current_date
	    where loan_id = p_loan_id;
	end if;
	update requests
	    set status = 'книга получена'
	    where request_id = v_request_id;
    perform set_copy_status(v_copy_id, 'в наличии');
end;
$$;
begin;
call return_book(17);
rollback;

create table copy_books_log (
    log_id serial primary key,
    copy_id int not null,
    book_title text not null,
    status text not null,
    action text not null,
    changed_at timestamp default now()
);

create or replace rule log_copy_books_insert as
on insert to copy_books
do also
insert into copy_books_log(copy_id, book_title, status, action)
select
    new.copy_id,
    b.title,
    cs.name,
    'insert'
from books b, copy_status cs
where b.book_id = new.book_id
  and cs.status_id = new.status_id;

create or replace rule log_copy_books_update as
on update to copy_books
do also
insert into copy_books_log(copy_id, book_title, status, action)
select
    new.copy_id,
    b.title,
    cs.name,
    'update'
from books b, copy_status cs
where b.book_id = new.book_id
  and cs.status_id = new.status_id;

create or replace rule log_copy_books_delete as
on delete to copy_books
do also
insert into copy_books_log(copy_id, book_title, status, action)
select
    old.copy_id,
    b.title,
    cs.name,
    'delete'
from books b, copy_status cs
where b.book_id = old.book_id
  and cs.status_id = old.status_id;

-- Добавление читателя
create or replace procedure create_worker(
    p_login text,
    p_email text,
    p_password text,
    p_address text,
    p_phone_number text,
    p_inn text default null,
    p_passport text default null,
    p_full_name text default null,
    p_gender text default null,
    p_position text default null,
    p_salary real default null
)
language plpgsql
as $$
declare
    new_user_id int;
begin
    -- проверки
    if exists (select 1 from users where email = p_email and is_deleted = false) then
        raise exception 'email уже используется';
    end if;

    if exists (select 1 from users where login = p_login and is_deleted = false) then
        raise exception 'логин уже используется';
    end if;
    if p_passport is not null and length(p_passport) <> 11 then
        raise exception 'passport должен состоять из 11 символов';
    end if;
    if p_inn is not null and length(p_inn) <> 10 then
        raise exception 'inn должен состоять из 10 символов';
    end if;
    insert into users (login, email, password_hash, role)
    values (p_login, p_email, p_password, 'worker')
    returning user_id into new_user_id;
    insert into workers (user_id, address, phone_number, inn, passport, full_name, gender, position, salary)
    values (new_user_id, p_address, p_phone_number, p_inn, p_passport, p_full_name, p_gender, p_position, p_salary);
end;
$$;

begin;
call create_worker(
    'worker100', 
    'worker523@test.com', 
    'pass321', 
    'ул. Победы, 5', 
    '+79995556677', 
    '9876543210', 
    '10987654321', 
    'Алексей Кузнецов', 
    'М', 
    'Библиотекарь', 
    50000
);
rollback;

create or replace procedure delete_worker(
    p_worker_id int, p_user_id int  
)
language plpgsql
as $$
begin
    if p_user_id = p_worker_id then
        raise exception 'работнику запрещено удалять себя же';
    end if;

	perform check_user_role(p_user_id, 'worker');
    update users
    set is_deleted = true
    where user_id = p_worker_id;

	update workers
    set is_deleted = true
    where user_id = p_worker_id;
end;
$$;

begin;
call delete_worker(4, 5);
rollback;

begin;
call delete_worker(4, 4);
rollback;

create or replace procedure update_worker(
    p_user_id int,
    p_worker_id int,
    p_login text,
    p_email text,
    p_address text,
    p_phone_number text,
    p_inn text default null,
    p_passport text default null,
    p_full_name text default null,
    p_gender text default null,
    p_position text default null,
    p_salary real default null
)
language plpgsql
as $$
begin
    perform check_user_role(p_user_id, 'worker');
    if exists (
        select 1 from users 
        where email = p_email and user_id <> p_worker_id and is_deleted = false
    ) then
        raise exception 'email уже используется';
    end if;
    if exists (
        select 1 from users 
        where login = p_login and user_id <> p_worker_id and is_deleted = false
    ) then
        raise exception 'логин уже используется';
    end if;
    if p_passport is not null and length(p_passport) <> 11 then
        raise exception 'passport должен состоять из 11 символов';
    end if;
    if p_inn is not null and length(p_inn) <> 10 then
        raise exception 'inn должен состоять из 10 символов';
    end if;
    update users
    set login = p_login,
        email = p_email
    where user_id = p_worker_id;
    update workers
    set address = p_address,
        phone_number = p_phone_number,
        inn = p_inn,
        passport = p_passport,
        full_name = p_full_name,
        gender = p_gender,
        position = p_position,
        salary = p_salary
    where user_id = p_worker_id;
end;
$$;

begin;
call update_worker(
    4,
    'worker1_updated',
    'worker1_new@example.com',
    'New Address',
    '+79990001122',
    '1234567890',
    '11111111111',
    'John Updated',
    'M',
    'Senior Manager',
    60000
);
rollback;

begin;
call update_worker(
    1,
    'worker1_updated',
    'worker1_new@example.com',
    'New Address',
    '+79990001122',
    '1234567890',
    '11111111111',
    'John Updated',
    'M',
    'Senior Manager',
    60000
);
rollback;