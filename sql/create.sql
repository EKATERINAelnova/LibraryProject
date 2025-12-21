create table countries (
    country_id serial primary key,
    name varchar(100) not null UNIQUE
);

create table publishers (
    publisher_id serial primary key,
    name varchar(100) not null,
    country_id int references countries(country_id)
);

create table sections (
    section_id serial primary key,
    name varchar(50) not null
);

create table genres (
    genre_id serial primary key,
    name varchar(50) not null,
    section_id int references sections(section_id)
);

create table books (
    book_id serial primary key,
    title varchar(100) not null,
    first_year_publish int,
    annotation text
);

create table copy_status (
    status_id serial primary key,
    name varchar(40) not null UNIQUE
);

insert into copy_status (name) VALUES 
('в наличии'),('выдана'),('в ремонте'),('списана');

create table copy_books (
    copy_id serial primary key,
    book_id int not null references books(book_id),
    publisher_id int not null references publishers(publisher_id),
    section_id int not null references sections(section_id),
    year_publish int,
    num_of_pages int,
    closet int not null,
    shelf int not null,
    place int not null,
    status_id int not null default 1 references copy_status(status_id),
    library_cipher text generated always as (
        section_id::text || '-' || 
        closet::text || '-' || 
        shelf::text || '-' || 
        place::text
    ) STORED
);
create table authors (
    author_id serial primary key,
    full_name varchar(100) not null
);

-- Таблица для связи Books - Authors (Many-to-Many)
create table book_authors (
    book_id int references books(book_id) on delete cascade,
    author_id int references authors(author_id) on delete cascade,
    primary key (book_id, author_id)
);

-- Таблица для связи Books - Genres (Many-to-Many)
create table book_genres (
    book_id int references books(book_id) on delete cascade,
    genre_id int references genres(genre_id) on delete cascade,
    primary key (book_id, genre_id)
);

create type user_role as enum ('reader', 'worker');

alter table users drop column role;
drop type user_role;

alter table users add column role varchar(20) default 'reader';

create table users (
    user_id serial primary key,
    login varchar(50) not null unique,
    password_hash text not null,
    email varchar(100) not null unique,
    role user_role not null
);

create table readers (
    user_id int primary key references users(user_id),
    address varchar(200),
    phone_number varchar(20)
);

create table workers (
    user_id int primary key references users(user_id),
    full_name varchar(100),
    gender char(1),
    passport varchar(20),
    inn varchar(20),
    position varchar(50),
    salary NUMERIC,
    address varchar(200),
    phone_number varchar(20)
);
create table book_loans (
    loan_id serial primary key,
    reader_id int not null references readers(user_id),
    copy_id int not null references copy_books(copy_id),
    worker_id int not null references workers(user_id),
    loan_date date not null default current_date,
    due_date date not null,
    return_date date,
    status varchar(20) default 'выдана'
        CHECK (status IN ('выдана', 'возвращена', 'просрочена'))
);

create table requests (
	request_id serial primary key,
    reader_id int not null references readers(user_id),
    copy_id int not null references copy_books(copy_id),
    created_at date not null default current_date,
    status varchar(30) not null default 'на рассмотрении',
        CHECK (status IN ('на рассмотрении', 'отклонено', 'подтверждено','книга получена'))
);

alter table book_loans
    add column request_id int references requests(request_id);

-- 1. countries
insert into countries (name) values
('Россия'),
('США'),
('Великобритания'),
('Германия'),
('Франция'),
('Италия'),
('Испания');

-- 2. publishers
insert into publishers (name, country_id) values
('Эксмо', 1),
('АСТ', 1),
('Penguin Random House', 2),
('HarperCollins', 2),
('Oxford University Press', 3),
('Deutsche Verlags-Anstalt', 4),
('Hachette Livre', 5);

-- 3. sections
insert into sections (name, description) values
('Художественная литература', 'Раздел с художественными произведениями'),
('Научная литература', 'Раздел с научной литературой'),
('Детская литература', 'Книги для детей'),
('История', 'Исторические книги'),
('Психология', 'Книги о психологии');

-- 4. genres
insert into genres (name, section_id) values
('Фантастика', 1),
('Роман', 1),
('Биология', 2),
('Физика', 2),
('Сказки', 3),
('Энциклопедии', 3),
('Средневековая история', 4),
('Новая история', 4),
('Психоанализ', 5),
('Саморазвитие', 5);

-- 5. books
insert into books (book_id, title, first_year_publish, annotation) values
(1, 'ВОЙНА И МИР', 1869, 'Эпический роман Льва Толстого'),
(2, 'ГАРРИ ПОТТЕР И ФИЛОСОФСКИЙ КАМЕНЬ', 1997, 'Фэнтези о мальчике-волшебнике'),
(3, 'КРАТКАЯ ИСТОРИЯ ВРЕМЕНИ', 1988, 'Научно-популярная книга Стивена Хокинга'),
(4, 'ЭНЦИКЛОПЕДИЯ ЖИВОТНЫХ', 2010, 'Справочник по животным'),
(5, 'ПСИХОЛОГИЯ ОБЩЕНИЯ', 2015, 'Книга по психологии общения'),
(6, 'ТОНКОЕ ИСКУССТВО ПОПЫТКИ НЕ СДАВАТЬСЯ', 2016, 'Саморазвитие и мотивация');
insert into books (title, first_year_publish, annotation) values
(7, 'ДЮНА', 2011, 'Путешествия на другой планете');

-- 6. copy_status
insert into copy_status (status_id, name) values 
(1, 'в наличии'),
(2, 'выдана'),
(3, 'в ремонте'),
(4, 'списана');
insert into copy_status (name) values 
(9, 'забронирована');

-- 7. copy_books
insert into copy_books (copy_id, book_id, publisher_id, section_id, year_publish, num_of_pages, closet, shelf, place, status_id) values
(1, 1, 1, 1, 1869, 1225, 1, 1, 1, 1),
(2, 2, 3, 1, 1997, 320, 1, 1, 2, 1),
(3, 3, 4, 2, 1988, 256, 2, 1, 1, 1),
(4, 4, 2, 3, 2010, 500, 2, 2, 1, 1),
(5, 5, 5, 5, 2015, 350, 3, 1, 1, 1),
(6, 6, 6, 5, 2016, 270, 3, 1, 2, 1),
(7, 4, 1, 3, 2001, 500, 4, 2, 1, 9),
(8, 5, 4, 5, 2005, 350, 5, 3, 2, 9),
(9, 6, 5, 5, 2020, 270, 6, 1, 2, 3),
(10, 7, 2, 4, 2021, 420, 2, 6, 9, 9);

-- 8. authors
insert into authors (full_name) values
(1, 'Лев Толстой'),
(2, 'Джоан Роулинг'),
(3, 'Стивен Хокинг'),
(4, 'Иван Петров'),
(5, 'Мария Смирнова'),
(6, 'Марк Леви');

insert into authors (full_name) values
(7, 'Фрэнк Герберт'),
(8, 'Соловэй');

-- 9. book_authors
insert into book_authors (book_id, author_id) values
(1, 1),
(2, 2),
(3, 3),
(5, 4),
(6, 5);

insert into book_authors (book_id, author_id) values
(7, 7),
(7, 8);

insert into book_authors (book_id, author_id) values
(4, 8);

-- 10. book_genres
insert into book_genres (book_id, genre_id) values
(1, 2),
(2, 1),
(3, 4),
(4, 6),
(5, 9),
(6, 10);

insert into book_genres (book_id, genre_id) values
(7, 1);
-- 11. users
insert into users (login, password_hash, email, role) values
('reader1', 'hash1', 'reader1@mail.com', 'reader'),
('reader2', 'hash2', 'reader2@mail.com', 'reader'),
('reader3', 'hash3', 'reader3@mail.com', 'reader'),
('worker1', 'hash4', 'worker1@mail.com', 'worker'),
('worker2', 'hash5', 'worker2@mail.com', 'worker');

insert into users (login, password_hash, email, role) values
('reader4', 'hash6', 'reader4@mail.com', 'reader');
-- 12. readers
insert into readers (user_id, address, phone_number) values
(1, 'ул. Ленина, д.1', '+7-900-111-22-33'),
(2, 'ул. Пушкина, д.5', '+7-900-222-33-44'),
(3, 'ул. Садовая, д.10', '+7-900-333-55-66');
insert into readers (user_id, address, phone_number) values
(6, 'ул. Ленина, д.1', '+7-900-111-22-11');

-- 13. workers
insert into workers (user_id, full_name, gender, passport, inn, position, salary, address, phone_number) values
(4, 'Иван Иванов', 'М', '1234 567890', '7701234567', 'Библиотекарь', 50000, 'ул. Советская, д.10', '+7-900-333-44-55'),
(5, 'Мария Кузнецова', 'Ж', '2345 678901', '7712345678', 'Заведующая', 60000, 'ул. Пушкина, д.12', '+7-900-444-55-66');

-- 14. book_loan
insert into book_loans (reader_id, copy_id, worker_id, loan_date, due_date, return_date, status, request_id) values
(1, 1, 4, '2025-11-01', '2025-11-15', null, 'выдана', 35),
(2, 2, 4, '2025-11-05', '2025-11-20', '2025-11-18', 'возвращена', 34),
(3, 3, 5, '2025-11-10', '2025-11-25', null, 'выдана', 36),
(1, 4, 5, '2025-11-12', '2025-11-27', null, 'выдана', 37);

insert into requests (request_id, reader_id, copy_id, created_at, status) values
(32, 1, 7, '2025-11-01','на рассмотрении'),
(33, 2, 8, '2025-11-05','на рассмотрении'),
(34, 3, 10, '2025-11-10', 'книга получена'),
(35, 1, 1, '2025-11-12','книга получена');

insert into requests (request_id, reader_id, copy_id, created_at, status) values
(36, 3, 3, '2025-11-01','книга получена'),
(37, 1, 4, '2025-11-01','книга получена');

insert into requests (request_id, reader_id, copy_id, created_at, status) values
(38, 3, 3, '2025-11-01','отклонено'),
(39, 1, 4, '2025-11-01','отклонено');

alter table users
add column is_deleted boolean not null default false;

alter table workers
add column is_deleted boolean not null default false;

alter table readers
add column is_deleted boolean not null default false;

alter table copy_books
add column is_deleted boolean not null default false;

alter table requests
add column is_deleted boolean not null default false;
