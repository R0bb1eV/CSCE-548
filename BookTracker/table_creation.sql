USE booktracker;

-- Author table
CREATE TABLE author (
    author_id INT AUTO_INCREMENT PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    middle_name VARCHAR(50),
    last_name VARCHAR(50) NOT NULL,
    birth_year INT NOT NULL
);

-- Book table
CREATE TABLE book (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    page_count INT NOT NULL,
    genre VARCHAR(50) NOT NULL,
    publishing_house VARCHAR(100) NOT NULL,
    year_of_release INT NOT NULL,
    isbn VARCHAR(20) NOT NULL UNIQUE,
    author_id INT NOT NULL,
    FOREIGN KEY (author_id) REFERENCES author(author_id) ON DELETE CASCADE
);

-- User table
CREATE TABLE user (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    dob DATE NOT NULL,
    account_creation_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- ReadingActivity table
CREATE TABLE activity (
    activity_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    book_id INT NOT NULL,
    book_status ENUM('toread','reading','completed','dropped') NOT NULL,
    progress_completed INT DEFAULT 0,
    start_date DATE,
    end_date DATE,
    FOREIGN KEY (user_id) REFERENCES user(user_id) ON DELETE CASCADE,
    FOREIGN KEY (book_id) REFERENCES book(id) ON DELETE CASCADE
);