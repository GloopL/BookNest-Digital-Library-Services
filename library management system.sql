CREATE TABLE students(
    StudentID int NOT NULL AUTO_INCREMENT,
    FirstName varchar(50),
    LastName varchar(50),
    Program varchar(50),
    Section varchar(50),
    ContactNumber varchar(11),
    SRCode varchar(100),
    Username varchar(255),
    Password varchar(255),
    loggedIn BOOLEAN DEFAULT False,
    PRIMARY KEY (StudentID)
);

CREATE TABLE books(
    BookID int NOT NULL AUTO_INCREMENT,
    Title varchar(255),
    Author varchar(50),
    Genre varchar(50),
    PublicationDate date,
    PRIMARY KEY (BookID)
);

CREATE TABLE admins(

    AdminID int NOT NULL AUTO_INCREMENT,
    Username varchar(50),
    Password varchar(255),
    PRIMARY KEY (AdminID)
);

CREATE TABLE transactions(
    TransactionID int NOT NULL AUTO_INCREMENT,
    BookID int,
    StudentID int,
    BorrowDate datetime,
    ReturnDate datetime,
    PRIMARY KEY (TransactionID),
    FOREIGN KEY (BookID) REFERENCES books (BookID)
);

CREATE TABLE BorrowedBooks(

	BorrowID int NOT NULL AUTO_INCREMENT,
	BookID int, 
    BookTitle varchar(50),
	StudentID int, 
    StudentsFullName varchar(50),
	BorrowDate datetime,
	PRIMARY KEY(BorrowID),
	FOREIGN KEY (BookID) REFERENCES Books(BookID),
    	FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ,
        FOREIGN KEY (BookTitle) REFERENCES Books(Title),
        FOREIGN KEY (StudentsFullName) REFERENCES students(FirstName + LastName)
	);