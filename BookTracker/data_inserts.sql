USE booktracker;

SET FOREIGN_KEY_CHECKS = 0;  -- temporarily disable

TRUNCATE TABLE activity;
TRUNCATE TABLE book;
TRUNCATE TABLE user;
TRUNCATE TABLE author;

SET FOREIGN_KEY_CHECKS = 1;  -- re-enable

-- =================== AUTHORS ===================
INSERT INTO author (first_name, middle_name, last_name, birth_year) VALUES
('J.R.R.', NULL, 'Tolkien', 1892),
('George', 'R.R.', 'Martin', 1948),
('Terry', NULL, 'Pratchett', 1948),
('Stephen', NULL, 'King', 1947),
('Robert', NULL, 'Jordan', 1948),
('Isaac', NULL, 'Asimov', 1920),
('Arthur', 'C.', 'Clarke', 1917),
('H.G.', NULL, 'Wells', 1866),
('Mary', NULL, 'Shelley', 1797),
('Leo', NULL, 'Tolstoy', 1828),
('Victor', NULL, 'Hugo', 1802),
('Alexandre', NULL, 'Dumas', 1802),
('Philip', NULL, 'Dick', 1928),
('Arthur', NULL, 'Conan Doyle', 1859);

-- =================== BOOKS (50+) ===================
INSERT IGNORE INTO book (title, page_count, genre, publishing_house, year_of_release, isbn, author_id) VALUES
('The Fellowship of the Ring', 423, 'Fantasy', 'Allen & Unwin', 1954, '9780544003415', 1),
('The Two Towers', 352, 'Fantasy', 'Allen & Unwin', 1954, '9780544003416', 1),
('The Return of the King', 416, 'Fantasy', 'Allen & Unwin', 1955, '9780544003417', 1),
('The Hobbit', 310, 'Fantasy', 'George Allen & Unwin', 1937, '9780547928228', 1),
('The Silmarillion', 365, 'Fantasy', 'Allen & Unwin', 1977, '9780618391111', 1),

('A Game of Thrones', 694, 'Fantasy', 'Bantam Books', 1996, '9780553103540', 2),
('A Clash of Kings', 768, 'Fantasy', 'Bantam Books', 1998, '9780553108032', 2),
('A Storm of Swords', 973, 'Fantasy', 'Bantam Books', 2000, '9780553106631', 2),
('A Feast for Crows', 753, 'Fantasy', 'Bantam Books', 2005, '9780553801507', 2),
('A Dance with Dragons', 1016, 'Fantasy', 'Bantam Books', 2011, '9780553801477', 2),

('The Colour of Magic', 300, 'Fantasy', 'Corgi', 1983, '9780552136701', 3),
('The Light Fantastic', 320, 'Fantasy', 'Corgi', 1986, '9780552136718', 3),
('Equal Rites', 256, 'Fantasy', 'Corgi', 1987, '9780552136725', 3),
('Mort', 240, 'Fantasy', 'Corgi', 1987, '9780552136732', 3),
('Sourcery', 280, 'Fantasy', 'Corgi', 1988, '9780552136749', 3),

('The Shining', 447, 'Horror', 'Doubleday', 1977, '9780385121675', 4),
('It', 1138, 'Horror', 'Viking', 1986, '9780670813025', 4),
('Misery', 320, 'Horror', 'Viking', 1987, '9780670813643', 4),
('Carrie', 199, 'Horror', 'Doubleday', 1974, '9780307743664', 4),
('The Stand', 1152, 'Horror', 'Doubleday', 1978, '9780385121682', 4),

('The Eye of the World', 814, 'Fantasy', 'Tor Books', 1990, '9780812511819', 5),
('The Great Hunt', 705, 'Fantasy', 'Tor Books', 1990, '9780812511826', 5),
('The Dragon Reborn', 704, 'Fantasy', 'Tor Books', 1991, '9780812511833', 5),
('The Shadow Rising', 1007, 'Fantasy', 'Tor Books', 1992, '9780812511840', 5),
('The Fires of Heaven', 992, 'Fantasy', 'Tor Books', 1993, '9780812511857', 5),

('Foundation', 255, 'Science Fiction', 'Gnome Press', 1951, '9780553294385', 6),
('Foundation and Empire', 247, 'Science Fiction', 'Gnome Press', 1952, '9780553294392', 6),
('Second Foundation', 240, 'Science Fiction', 'Gnome Press', 1953, '9780553294408', 6),
('I, Robot', 224, 'Science Fiction', 'Gnome Press', 1950, '9780553294415', 6),
('The Caves of Steel', 270, 'Science Fiction', 'Doubleday', 1953, '9780553294422', 6),

('2001: A Space Odyssey', 350, 'Science Fiction', 'Ballantine Books', 1968, '9780345347954', 7),
('Childhood\'s End', 224, 'Science Fiction', 'Ballantine Books', 1953, '9780345347955', 7),
('Rendezvous with Rama', 288, 'Science Fiction', 'HarperCollins', 1973, '9780345347956', 7),
('The Fountains of Paradise', 337, 'Science Fiction', 'Harper & Row', 1979, '9780345347957', 7),
('Imperial Earth', 325, 'Science Fiction', 'Harper & Row', 1976, '9780345347958', 7),

('The War of the Worlds', 290, 'Science Fiction', 'Penguin', 1895, '9780451530707', 8),
('The Time Machine', 118, 'Science Fiction', 'Penguin', 1895, '9780451530714', 8),
('The Invisible Man', 149, 'Science Fiction', 'Penguin', 1897, '9780451530721', 8),
('The Island of Doctor Moreau', 192, 'Science Fiction', 'Penguin', 1896, '9780451530738', 8),
('The First Men in the Moon', 192, 'Science Fiction', 'Penguin', 1901, '9780451530745', 8),

('Frankenstein', 280, 'Gothic', 'Lackington', 1818, '9780141439471', 9),
('The Last Man', 300, 'Science Fiction', 'Henry Colburn', 1826, '9780141439488', 9),
('Mathilda', 80, 'Gothic', 'Lackington', 1819, '9780141439495', 9),
('Valperga', 400, 'Historical', 'Henry Colburn', 1823, '9780141439501', 9),
('Lodore', 320, 'Gothic', 'Henry Colburn', 1835, '9780141439518', 9),

('War and Peace', 1225, 'Historical', 'The Russian Messenger', 1869, '9780199232765', 10),
('Anna Karenina', 864, 'Historical', 'The Russian Messenger', 1878, '9780199232766', 10),
('Resurrection', 572, 'Historical', 'The Russian Messenger', 1899, '9780199232767', 10),
('The Cossacks', 232, 'Historical', 'The Russian Messenger', 1863, '9780199232768', 10),
('Childhood', 192, 'Autobiographical', 'The Russian Messenger', 1852, '9780199232769', 10),

('Les Misérables', 1463, 'Historical', 'A. Lacroix', 1862, '9780451419439', 11),
('The Hunchback of Notre-Dame', 940, 'Gothic', 'Gosselin', 1831, '9780141439644', 11),

('The Three Musketeers', 700, 'Adventure', 'Penguin', 1844, '9780140449266', 12),
('The Count of Monte Cristo', 1276, 'Adventure', 'Penguin', 1844, '9780140449267', 12),

('Do Androids Dream of Electric Sheep?', 210, 'Science Fiction', 'Doubleday', 1968, '9780345404473', 13),
('Ubik', 224, 'Science Fiction', 'Doubleday', 1969, '9780345410030', 13),

('A Study in Scarlet', 188, 'Mystery', 'Ward, Lock & Co', 1887, '9781512091003', 14),
('The Hound of the Baskervilles', 256, 'Mystery', 'George Newnes', 1902, '9781512091010', 14),
('The Sign of the Four', 164, 'Mystery', 'Spencer Blackett', 1890, '9781512091027', 14);

-- =================== USERS ===================
INSERT INTO user (username, email, dob) VALUES
('user1','user1@example.com','1990-01-01'),
('user2','user2@example.com','1985-05-12'),
('user3','user3@example.com','2000-07-20'),
('user4','user4@example.com','1995-03-15'),
('user5','user5@example.com','1988-11-23'),
('user6','user6@example.com','1992-08-10'),
('user7','user7@example.com','1980-06-05'),
('user8','user8@example.com','1999-12-31'),
('user9','user9@example.com','1993-09-21'),
('user10','user10@example.com','1987-04-18');

-- =================== READING ACTIVITY (50+) ===================
INSERT INTO activity (user_id, book_id, book_status, progress_completed, start_date, end_date) VALUES
(1,1,'completed',100,'2023-01-01','2023-01-20'),
(1,2,'reading',40,'2023-02-01',NULL),
(1,3,'toread',0,NULL,NULL),
(1,4,'dropped',25,'2023-03-01','2023-03-05'),
(1,5,'completed',100,'2023-03-10','2023-03-25'),
(2,6,'reading',60,'2023-04-01',NULL),
(2,7,'toread',0,NULL,NULL),
(2,8,'completed',100,'2023-05-01','2023-05-15'),
(2,9,'reading',30,'2023-06-01',NULL),
(2,10,'toread',0,NULL,NULL),
(3,11,'completed',100,'2023-01-15','2023-02-05'),
(3,12,'reading',50,'2023-02-10',NULL),
(3,13,'toread',0,NULL,NULL),
(3,14,'completed',100,'2023-03-01','2023-03-20'),
(3,15,'reading',20,'2023-03-25',NULL),
(4,16,'completed',100,'2023-01-05','2023-01-25'),
(4,17,'reading',45,'2023-02-05',NULL),
(4,18,'toread',0,NULL,NULL),
(4,19,'completed',100,'2023-03-10','2023-03-30'),
(4,20,'reading',30,'2023-04-01',NULL),
(5,21,'toread',0,NULL,NULL),
(5,22,'completed',100,'2023-01-12','2023-02-01'),
(5,23,'reading',60,'2023-02-10',NULL),
(5,24,'toread',0,NULL,NULL),
(5,25,'completed',100,'2023-03-05','2023-03-25'),
(6,26,'reading',40,'2023-01-10',NULL),
(6,27,'completed',100,'2023-01-20','2023-02-05'),
(6,28,'toread',0,NULL,NULL),
(6,29,'reading',50,'2023-02-15',NULL),
(6,30,'completed',100,'2023-03-01','2023-03-20'),
(7,31,'toread',0,NULL,NULL),
(7,32,'completed',100,'2023-01-05','2023-01-25'),
(7,33,'reading',30,'2023-02-01',NULL),
(7,34,'toread',0,NULL,NULL),
(7,35,'completed',100,'2023-03-10','2023-03-30'),
(8,36,'reading',45,'2023-01-15',NULL),
(8,37,'toread',0,NULL,NULL),
(8,38,'completed',100,'2023-02-05','2023-02-25'),
(8,39,'reading',35,'2023-03-01',NULL),
(8,40,'toread',0,NULL,NULL),
(9,41,'completed',100,'2023-01-20','2023-02-10'),
(9,42,'reading',50,'2023-02-15',NULL),
(9,43,'toread',0,NULL,NULL),
(9,44,'completed',100,'2023-03-05','2023-03-25'),
(9,45,'reading',30,'2023-04-01',NULL),
(10,46,'toread',0,NULL,NULL),
(10,47,'completed',100,'2023-01-10','2023-01-30'),
(10,48,'reading',40,'2023-02-05',NULL),
(10,49,'toread',0,NULL,NULL),
(10,50,'completed',100,'2023-03-01','2023-03-20');