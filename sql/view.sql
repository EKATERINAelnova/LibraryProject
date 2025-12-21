create or replace view books_info as
select 
    b.book_id,
    b.title,
    string_agg(distinct a.full_name, ', ') as authors,
    string_agg(distinct g.name, ', ') as genres,
    b.first_year_publish, 
    b.annotation,
    count(cb.copy_id) as book_count
from books b
join copy_books cb on cb.book_id = b.book_id
join book_authors ba on b.book_id = ba.book_id
join authors a on ba.author_id = a.author_id
join book_genres bg on b.book_id = bg.book_id
join genres g on bg.genre_id = g.genre_id
where cb.is_deleted = false
group by 
    b.book_id, g.name;


create or replace view all_user_requests_info as
select
    r.request_id,
    r.reader_id,
    us.login as reader_name,
    r.copy_id,
    b.book_id,
    b.title as book_title,
    string_agg(a.full_name, ', ') as authors,
    r.created_at,
    r.status
from requests r
join readers u on u.user_id = r.reader_id
join users us on us.user_id = u.user_id
join copy_books cb on cb.copy_id = r.copy_id
join books b on b.book_id = cb.book_id
join book_authors ba on ba.book_id = b.book_id
join authors a on ba.author_id = a.author_id
where r.status = 'на рассмотрении' and r.is_deleted = false
group by r.request_id, r.reader_id, us.login, b.book_id
order by r.created_at desc;


create or replace view all_book_loans_info as
select
    bl.loan_id,
    bl.request_id,
    bl.copy_id,
    cb.book_id,
    b.title as book_title,
    string_agg(a.full_name, ', ') as authors,
    r.user_id,
    u.login as reader_name,
    bl.loan_date,
   	bl.due_date,
    bl.return_date,
    bl.status
from book_loans bl
join copy_books cb on cb.copy_id = bl.copy_id
join books b on b.book_id = cb.book_id
join book_authors ba on ba.book_id = b.book_id
join authors a on ba.author_id = a.author_id
join readers r on r.user_id = bl.reader_id
join users u on u.user_id = r.user_id
join copy_status cs on cs.status_id = cb.status_id
group by bl.loan_id, bl.request_id, bl.copy_id, cb.book_id, b.title, r.user_id, u.login
order by bl.loan_date desc;

create or replace view stat_workers as
	select distinct full_name,
					login,
					count(bl.loan_id) over (partition by full_name) as c_loans
	from workers w
	join book_loans bl on bl.worker_id = w.user_id
	join users u on u.user_id = w.user_id;

create or replace view stat_readers as
	select distinct count(request_id) as c_all,
				(select count(request_id) from requests where status in ('подтверждено','книга получена') ) as c_yes
	from requests;

drop view stat_workers;
drop view stat_readers;

create or replace view stat_workers as
select 
    w.full_name as full_name,
    u.login as login,
    count(bl.loan_id) as loans_count,
    count(distinct bl.reader_id) as unique_workers_count,
    avg(bl.return_date - bl.loan_date) as avg_loan_days
from workers w
join users u on u.user_id = w.user_id
left join book_loans bl on bl.worker_id = w.user_id
where w.is_deleted = false and u.is_deleted = false
group by u.user_id, w.full_name, u.login;

drop view stat_workers;
drop view stat_readers;

-- представление для читателей
create or replace view stat_readers as
select 
    'подтвержденные' as Category,
    count(*) 
from requests 
where status in ('подтверждено', 'книга получена')
union all
select 
    'отклоненные',
    count(*) 
from requests 
where status = 'отклонено'
union all
select 
    'ожидают рассмотрения',
    count(*) 
from requests 
where status = 'на рассмотрении';

