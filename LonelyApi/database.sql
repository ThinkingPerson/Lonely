-- 创建数据库
CREATE DATABASE IF NOT EXISTS lonely;

-- 使用数据库
USE lonely;

-- 用户表
CREATE TABLE IF NOT EXISTS Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AnonUID VARCHAR(64) NOT NULL UNIQUE,
    Nickname VARCHAR(50) NOT NULL,
    Avatar VARCHAR(20) NOT NULL,
    Phone VARCHAR(20) NULL,
    LoginType ENUM('anonymous', 'quick') NOT NULL DEFAULT 'anonymous',
    LastLoginTime DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- 漂流瓶表
CREATE TABLE IF NOT EXISTS Bottles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    Content TEXT NOT NULL,
    VoiceUrl VARCHAR(255) NULL,
    ImageUrl VARCHAR(255) NULL,
    Emotion VARCHAR(20) NULL,
    Topics JSON NULL,
    Status ENUM('floating', 'received', 'expired') NOT NULL DEFAULT 'floating',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ExpiredAt DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- 树洞表
CREATE TABLE IF NOT EXISTS TreeHoles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    Content TEXT NOT NULL,
    VoiceUrl VARCHAR(255) NULL,
    Status ENUM('active', 'expired') NOT NULL DEFAULT 'active',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ExpiredAt DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- 匹配表
CREATE TABLE IF NOT EXISTS Matches (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId1 INT NOT NULL,
    UserId2 INT NOT NULL,
    Status ENUM('pending', 'matched', 'expired') NOT NULL DEFAULT 'pending',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ExpiredAt DATETIME NOT NULL,
    FOREIGN KEY (UserId1) REFERENCES Users(Id),
    FOREIGN KEY (UserId2) REFERENCES Users(Id)
);

-- 消息表
CREATE TABLE IF NOT EXISTS Messages (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FromUserId INT NOT NULL,
    ToUserId INT NOT NULL,
    Content TEXT NOT NULL,
    Type ENUM('text', 'voice', 'image') NOT NULL DEFAULT 'text',
    IsRead BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (FromUserId) REFERENCES Users(Id),
    FOREIGN KEY (ToUserId) REFERENCES Users(Id)
);

-- 举报表
CREATE TABLE IF NOT EXISTS Reports (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ReporterId INT NOT NULL,
    ReportedUserId INT NOT NULL,
    ReportType ENUM('spam', 'inappropriate', 'harassment') NOT NULL,
    Content TEXT NOT NULL,
    Status ENUM('pending', 'processed', 'dismissed') NOT NULL DEFAULT 'pending',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ReporterId) REFERENCES Users(Id),
    FOREIGN KEY (ReportedUserId) REFERENCES Users(Id)
);

-- 创建索引
CREATE INDEX idx_users_anonuid ON Users(AnonUID);
CREATE INDEX idx_bottles_status ON Bottles(Status);
CREATE INDEX idx_treeholes_status ON TreeHoles(Status);
CREATE INDEX idx_matches_status ON Matches(Status);
CREATE INDEX idx_messages_fromuserid ON Messages(FromUserId);
CREATE INDEX idx_messages_touserid ON Messages(ToUserId);
CREATE INDEX idx_reports_status ON Reports(Status);
