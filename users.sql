-- Drop and recreate the schema
DROP DATABASE IF EXISTS campusehr;
CREATE DATABASE campusehr;
USE campusehr;

-- Users table with merged columns (Gender, Phone) and default ProfilePicture
CREATE TABLE Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    LastName VARCHAR(255) NOT NULL,
    FirstName VARCHAR(255) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Gender ENUM('Male', 'Female', 'Other') NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Role ENUM('Admin', 'Doctor') NOT NULL,
    ProfilePicture VARCHAR(255) DEFAULT 'default.png',
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Patients table with updated column order (FirstName and LastName added instead of FullName)
CREATE TABLE Patients (
    PatientID INT AUTO_INCREMENT PRIMARY KEY,
    LastName VARCHAR(255) NOT NULL,
    FirstName VARCHAR(255) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender ENUM('Male', 'Female', 'Other') NOT NULL,
    MatricNumber VARCHAR(100),
    Phone VARCHAR(20),
    Email VARCHAR(255) UNIQUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Prescriptions table with ConsultationID column added
CREATE TABLE Prescriptions (
    PrescriptionID INT AUTO_INCREMENT PRIMARY KEY,
    ConsultationID INT,
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    MedicationName VARCHAR(255) NOT NULL,
    Dosage VARCHAR(100) NOT NULL,
    Instructions TEXT NOT NULL,
    DatePrescribed TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    SentToPharmacy BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (ConsultationID) REFERENCES Consultations(ConsultationID) ON DELETE CASCADE,
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID) ON DELETE CASCADE,
    FOREIGN KEY (DoctorID) REFERENCES Users(UserID) ON DELETE CASCADE
);

-- Pharmacy table
CREATE TABLE Pharmacy (
    MedicationID INT AUTO_INCREMENT PRIMARY KEY,
    MedicationName VARCHAR(255) NOT NULL UNIQUE,
    StockQuantity INT DEFAULT 0,
    ExpiryDate DATE NOT NULL
);

-- AuditLogs table
CREATE TABLE AuditLogs (
    LogID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    Action VARCHAR(255) NOT NULL,
    Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

-- Notifications table
CREATE TABLE Notifications (
    NotificationID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    Message TEXT NOT NULL,
    IsRead BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

-- Consultations table
CREATE TABLE Consultations (
    ConsultationID INT AUTO_INCREMENT PRIMARY KEY,
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    VisitReason TEXT NOT NULL,
    Diagnosis TEXT,
    Vitals TEXT,
    LabSummary TEXT,
    FollowUpRequired BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID) ON DELETE CASCADE,
    FOREIGN KEY (DoctorID) REFERENCES Users(UserID) ON DELETE CASCADE
);

-- Lab results table
CREATE TABLE Lab_Results (
    LabResultID INT AUTO_INCREMENT PRIMARY KEY,
    PatientID INT,
    ConsultationID INT,
    FileName VARCHAR(255),
    FilePath VARCHAR(500),
    UploadedAt DATETIME,
    UploadedBy INT,
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    FOREIGN KEY (ConsultationID) REFERENCES Consultations(ConsultationID),
    FOREIGN KEY (UploadedBy) REFERENCES Users(UserID) ON DELETE SET NULL
);
