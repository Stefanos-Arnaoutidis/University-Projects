-- Δημιουργία της Βάσης Δεδομένων
CREATE DATABASE UniversityGradesDB;
GO

-- Επιλογή της Βάσης
USE UniversityGradesDB;
GO

-- Δημιουργία Πινάκων

-- Users
CREATE TABLE Users (
    username VARCHAR(45) NOT NULL PRIMARY KEY,
    password VARCHAR(100) NOT NULL,
    role VARCHAR(45) NOT NULL
);

-- Professors
CREATE TABLE Professors (
    AFM INT NOT NULL PRIMARY KEY,
    Name VARCHAR(45) NOT NULL,
    Surname VARCHAR(45) NOT NULL,
    Department VARCHAR(45) NOT NULL,
    USERS_username VARCHAR(45) NOT NULL,
    CONSTRAINT FK_Professors_Users FOREIGN KEY (USERS_username) 
        REFERENCES Users(username)
);

-- Students
CREATE TABLE Students (
    RegistrationNumber INT NOT NULL PRIMARY KEY,
    Name VARCHAR(45) NOT NULL,
    Surname VARCHAR(45) NOT NULL,
    Department VARCHAR(45) NOT NULL,
    USERS_username VARCHAR(45) NOT NULL,
    CONSTRAINT FK_Students_Users FOREIGN KEY (USERS_username) 
        REFERENCES Users(username)
);

-- Secretaries
CREATE TABLE Secretaries (
    Phonenumber INT NOT NULL PRIMARY KEY,
    Name VARCHAR(45) NOT NULL,
    Surname VARCHAR(45) NOT NULL,
    Department VARCHAR(45) NOT NULL,
    USERS_username VARCHAR(45) NOT NULL,
    CONSTRAINT FK_Secretaries_Users FOREIGN KEY (USERS_username) 
        REFERENCES Users(username)
);

-- Course
CREATE TABLE Course (
    idCOURSE INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    CourseTitle VARCHAR(60) NOT NULL,
    CourseSemester VARCHAR(25) NOT NULL,
    PROFESSORS_AFM INT NULL,
    CONSTRAINT FK_Course_Professors FOREIGN KEY (PROFESSORS_AFM) 
        REFERENCES Professors(AFM)
);

-- Course_has_Students
CREATE TABLE Course_has_Students (
    COURSE_idCOURSE INT NOT NULL,
    STUDENTS_RegistrationNumber INT NOT NULL,
    GradeCourseStudent INT NULL,
    PRIMARY KEY (COURSE_idCOURSE, STUDENTS_RegistrationNumber),
    CONSTRAINT FK_ChS_Course FOREIGN KEY (COURSE_idCOURSE) 
        REFERENCES Course(idCOURSE),
    CONSTRAINT FK_ChS_Students FOREIGN KEY (STUDENTS_RegistrationNumber) 
        REFERENCES Students(RegistrationNumber)
);