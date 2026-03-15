-- 创建数据库
CREATE DATABASE IF NOT EXISTS face_recognition DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE face_recognition;

-- 创建用户表
CREATE TABLE IF NOT EXISTS user (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    face_feature TEXT NOT NULL,
    create_time DATETIME NOT NULL,
    INDEX idx_username (username)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
