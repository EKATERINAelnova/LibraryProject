--Запросы
--1. Все книги
select 
    b.book_id,
    b.title,
    string_agg(a.full_name, ', ') as authors,
    g.name as genres,
    first_year_publish, 
    annotation,
    count(b.book_id)
from books b
join book_authors ba on b.book_id = ba.book_id
join authors a on ba.author_id = a.author_id
join book_genres bg on b.book_id = bg.book_id
join genres g on bg.genre_id = g.genre_id
group by 
    b.book_id, b.title, g.name;



--2. Информация о копиях определённой книги
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
        string_agg(a.full_name, ', ')::text as authors
    from copy_books cb
    join books b on cb.book_id = b.book_id
    join publishers p on cb.publisher_id = p.publisher_id
    join countries c on p.country_id = c.country_id
    join sections s on cb.section_id = s.section_id
    join book_authors ba on b.book_id = ba.book_id
    join authors a on ba.author_id = a.author_id
    where cb.book_id = 2
    group by cb.copy_id, b.title, p.name, cb.year_publish, c.name, s.name, cb.closet, cb.shelf, cb.place;

--3. Запросы читателя
select 
    r.request_id,
    b.title,
    cb.library_cipher,
    r.status,
    r.created_at,
    7 - (current_date - r.created_at) as days_left
from requests r
join copy_books cb on r.copy_id = cb.copy_id
join books b on cb.book_id = b.book_id
where r.reader_id = 10;

--4. Все запросы
--5. Брони для определённого работника
--6. 

select 
    r.request_id,
    u.login,
    b.title,
    r.created_at,
    (r.created_at + interval '7 days' - current_date) as days_left,
    case 
        when (r.created_at + interval '7 days') < current_date then 'просрочен'
        else 'активен'
    end as status_calc
from requests r
join readers rd on r.reader_id = rd.user_id
join users u on rd.user_id = u.user_id
join copy_books cb on r.copy_id = cb.copy_id
join books b on cb.book_id = b.book_id
where r.status = 'на рассмотрении'
order by days_left;


select b.book_id, cb.copy_id, cb.library_cipher
from books b
join copy_books cb on b.book_id = cb.book_id
where cb.status_id = 1 -- свободная
and not exists (
    select 1 
    from requests r
    where r.copy_id = cb.copy_id
    and r.status in ('на рассмотрении', 'подтверждено')
);

select 
    r.request_id,
    b.title,
    cb.library_cipher,
    r.status,
    r.created_at,
    7 - (current_date - r.created_at) as days_left
from requests r
join copy_books cb on r.copy_id = cb.copy_id
join books b on cb.book_id = b.book_id
where r.reader_id = 1;


select 
    r.request_id,
    u.login,
    b.title,
    r.created_at,
    (r.created_at + interval '7 days' - current_date) as days_left,
    case 
        when (r.created_at + interval '7 days') < current_date then 'просрочен'
        else 'активен'
    end as status_calc
from requests r
join readers rd on r.reader_id = rd.user_id
join users u on rd.user_id = u.user_id
join copy_books cb on r.copy_id = cb.copy_id
join books b on cb.book_id = b.book_id
where r.status = 'на рассмотрении'
order by days_left;

select 
    b.title,
    count(ba.author_id) as author_count
from books b
join book_authors ba on b.book_id = ba.book_id
group by b.title
having count(ba.author_id) > 1;


select u.user_id, u.login
from users u
join readers r on u.user_id = r.user_id
except
select bl.reader_id, u2.login
from book_loans bl
join users u2 on bl.reader_id = u2.user_id;

select b.title, cb.copy_id
from books b
join copy_books cb on b.book_id = cb.book_id
where cb.status_id = 1 
and not exists (
    select 1 
    from requests r
    where r.copy_id = cb.copy_id
    and r.status in ('на рассмотрении')
);

select 
    s.name as section,
    avg(cb.num_of_pages) as avg_pages
from copy_books cb
join sections s on s.section_id = cb.section_id
group by s.name;

select b.title, g.name as genre, cb.year_publish
from books b
join copy_books cb on b.book_id = cb.book_id
join book_genres bg on b.book_id = bg.book_id
join genres g on g.genre_id = bg.genre_id
where cb.year_publish = (
    select max(cb2.year_publish)
    from copy_books cb2
    join book_genres bg2 on cb2.book_id = bg2.book_id
    where bg2.genre_id = g.genre_id
);

select g.name, count(bg.book_id) as cnt
from genres g
join book_genres bg on g.genre_id = bg.genre_id
group by g.name
having count(bg.book_id) < (
    select avg(cnt)
    from (
        select count(*) as cnt 
        from book_genres 
        group by genre_id
    ) t
);



--1
select * from books
where first_year_publish > 2010;

--2
select b.title, num_of_pages
from copy_books cb
join books b on b.book_id = cb.book_id
where num_of_pages > (select avg(num_of_pages) from copy_books);

--3
select b.title, cb.library_cipher 
from copy_books cb
join books b on b.book_id = cb.book_id
where cb.copy_id in (
    select copy_id from requests where status = 'на рассмотрении'
);
--4
select b.*
from books b
where not exists (
    select 1
    from copy_books cb
    where cb.book_id = b.book_id
      and cb.status_id != 1
);
--5
select user_id, phone_number from readers
except
select reader_id, r.phone_number from book_loans bl
join readers r on r.user_id = bl.reader_id;
--6
with authors_count as (
    select 
        a.full_name,
        count(ba.book_id) as books_cnt,
        row_number() over (order by count(ba.book_id) desc) as rn
    from authors a
    join book_authors ba on a.author_id = ba.author_id
    group by a.full_name
)
select full_name, books_cnt
from authors_count
where rn <= 3;
--7
select s.name, avg(cb.num_of_pages) as avg_pages
from copy_books cb
join sections s on cb.section_id = s.section_id
group by s.name;
--8
select r.request_id, b.title, r.status
from requests r
join copy_books cb on r.copy_id = cb.copy_id
join books b on cb.book_id = b.book_id
where r.reader_id = 1;
--9
select login || ' <' || email || '>' as user_info
from users;
--10
select *,
    row_number() over (partition by reader_id order by loan_date desc) as rn
from book_loans;