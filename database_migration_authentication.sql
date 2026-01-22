-- ============================================
-- Authentication System Database Migration
-- ============================================
-- This script updates the User table to support
-- the new authentication system with email login
-- and enhanced registration fields.
-- ============================================

USE MedEaseDB;

-- Check if columns already exist before adding them
-- This prevents errors if script is run multiple times

-- Add specialization column (for Doctors)
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM information_schema.COLUMNS 
WHERE TABLE_SCHEMA = 'MedEaseDB' 
AND TABLE_NAME = 'User' 
AND COLUMN_NAME = 'specialization';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE User ADD COLUMN specialization VARCHAR(100) NULL COMMENT "Doctor specialization (e.g., Cardiology)"',
    'SELECT "Column specialization already exists" AS message');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Add licenseNumber column (for Doctors)
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM information_schema.COLUMNS 
WHERE TABLE_SCHEMA = 'MedEaseDB' 
AND TABLE_NAME = 'User' 
AND COLUMN_NAME = 'licenseNumber';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE User ADD COLUMN licenseNumber VARCHAR(50) NULL COMMENT "Medical license number"',
    'SELECT "Column licenseNumber already exists" AS message');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Add createdAt column (timestamp)
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM information_schema.COLUMNS 
WHERE TABLE_SCHEMA = 'MedEaseDB' 
AND TABLE_NAME = 'User' 
AND COLUMN_NAME = 'createdAt';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE User ADD COLUMN createdAt DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT "Account creation timestamp"',
    'SELECT "Column createdAt already exists" AS message');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Ensure password column exists and has correct length
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM information_schema.COLUMNS 
WHERE TABLE_SCHEMA = 'MedEaseDB' 
AND TABLE_NAME = 'User' 
AND COLUMN_NAME = 'password';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE User ADD COLUMN password VARCHAR(255) NOT NULL DEFAULT "" COMMENT "User password (should be hashed in production)"',
    'ALTER TABLE User MODIFY COLUMN password VARCHAR(255) NOT NULL COMMENT "User password (should be hashed in production)"');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Ensure email column has UNIQUE constraint
SET @idx_exists = 0;
SELECT COUNT(*) INTO @idx_exists
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = 'MedEaseDB'
AND TABLE_NAME = 'User'
AND INDEX_NAME = 'email'
AND NON_UNIQUE = 0;

SET @query = IF(@idx_exists = 0,
    'ALTER TABLE User ADD UNIQUE INDEX email (email)',
    'SELECT "UNIQUE constraint on email already exists" AS message');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Display final table structure
DESCRIBE User;

-- Display sample data (if any)
SELECT 
    userID,
    name,
    email,
    role,
    specialization,
    licenseNumber,
    age,
    gender,
    createdAt
FROM User
LIMIT 5;

SELECT '✅ Authentication database migration completed successfully!' AS status;

-- ============================================
-- Optional: Insert test users for development
-- ============================================
-- Uncomment the following section to insert test users

/*
-- Test Doctor Account
INSERT INTO User (name, email, password, phone, role, specialization, licenseNumber, age, gender)
VALUES (
    'Dr. Sarah Johnson',
    'sarah.johnson@hospital.com',
    'doctor123',
    '555-0101',
    'Doctor',
    'Cardiology',
    'MD-001',
    42,
    'Female'
) ON DUPLICATE KEY UPDATE name = name; -- Skip if email exists

-- Test Receptionist Account
INSERT INTO User (name, email, password, phone, role, age, gender)
VALUES (
    'John Smith',
    'john.smith@hospital.com',
    'receptionist123',
    '555-0102',
    'Receptionist',
    28,
    'Male'
) ON DUPLICATE KEY UPDATE name = name; -- Skip if email exists

-- Test Patient Account (for backward compatibility)
INSERT INTO User (name, email, password, phone, role, age, gender)
VALUES (
    'Jane Doe',
    'jane.doe@example.com',
    'patient123',
    '555-0103',
    'Patient',
    35,
    'Female'
) ON DUPLICATE KEY UPDATE name = name; -- Skip if email exists

SELECT '✅ Test users inserted successfully!' AS status;
SELECT 'Login credentials:' AS info;
SELECT 
    CONCAT('Doctor: ', email, ' / doctor123') AS credentials
FROM User WHERE email = 'sarah.johnson@hospital.com'
UNION
SELECT 
    CONCAT('Receptionist: ', email, ' / receptionist123')
FROM User WHERE email = 'john.smith@hospital.com'
UNION
SELECT 
    CONCAT('Patient: ', email, ' / patient123')
FROM User WHERE email = 'jane.doe@example.com';
*/

-- ============================================
-- Verification Queries
-- ============================================

-- Check all required columns exist
SELECT 
    CASE 
        WHEN COUNT(*) = 11 THEN '✅ All required columns exist'
        ELSE '❌ Some columns are missing'
    END AS verification_status,
    COUNT(*) AS column_count
FROM information_schema.COLUMNS 
WHERE TABLE_SCHEMA = 'MedEaseDB' 
AND TABLE_NAME = 'User'
AND COLUMN_NAME IN (
    'userID', 'name', 'email', 'password', 'phone', 
    'role', 'specialization', 'licenseNumber', 
    'age', 'gender', 'createdAt'
);

-- Check email UNIQUE constraint exists
SELECT 
    CASE 
        WHEN COUNT(*) > 0 THEN '✅ Email UNIQUE constraint exists'
        ELSE '❌ Email UNIQUE constraint missing'
    END AS email_constraint_status
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = 'MedEaseDB'
AND TABLE_NAME = 'User'
AND INDEX_NAME = 'email'
AND NON_UNIQUE = 0;

-- Show total users by role
SELECT 
    role,
    COUNT(*) AS user_count
FROM User
GROUP BY role
ORDER BY user_count DESC;
