-- ====================================================================
-- Database Migration Script for Notification System
-- HealthTech Medical - Appointment Scheduling + Notifications
-- Date: January 11, 2026
-- ====================================================================

-- Step 1: Add Email and Phone columns to User table
-- ====================================================================
ALTER TABLE User 
ADD COLUMN IF NOT EXISTS Email VARCHAR(255) NOT NULL DEFAULT '',
ADD COLUMN IF NOT EXISTS Phone VARCHAR(20) NOT NULL DEFAULT '';

-- Step 2: Update existing users with sample contact information
-- ====================================================================
-- IMPORTANT: Replace these with REAL email addresses and phone numbers
-- Phone numbers MUST be in E.164 format: +[country code][number]
-- Examples: +60123456789 (Malaysia), +14155552671 (USA)

-- Update all users with a generated email (you should update with real emails)
UPDATE User 
SET Email = CONCAT(LOWER(REPLACE(Name, ' ', '')), '@example.com')
WHERE Email = '' OR Email IS NULL;

-- Update all users with a sample phone number (you should update with real numbers)
UPDATE User 
SET Phone = '+60123456789'
WHERE Phone = '' OR Phone IS NULL;

-- ====================================================================
-- Step 3 (RECOMMENDED): Update specific users with REAL data
-- ====================================================================
-- Uncomment and modify these lines with actual patient/doctor contact info:

/*
-- Example: Update Patient #1
UPDATE User 
SET Email = 'john.doe@gmail.com', 
    Phone = '+60123456789'
WHERE UserID = 1 AND Role = 'Patient';

-- Example: Update Patient #2
UPDATE User 
SET Email = 'jane.smith@gmail.com', 
    Phone = '+60198765432'
WHERE UserID = 2 AND Role = 'Patient';

-- Example: Update Doctor #1
UPDATE User 
SET Email = 'dr.sarah@healthtech.com', 
    Phone = '+60187654321'
WHERE UserID = 10 AND Role = 'Doctor';
*/

-- ====================================================================
-- Step 4: Verify the changes
-- ====================================================================
SELECT UserID, Name, Role, Email, Phone 
FROM User 
ORDER BY Role, UserID;

-- ====================================================================
-- NOTES:
-- ====================================================================
-- 1. Email format: any valid email address
-- 2. Phone format: MUST be E.164 format (+[country code][number])
--    - Malaysia: +60XXXXXXXXX (e.g., +60123456789)
--    - USA: +1XXXXXXXXXX (e.g., +14155552671)
--    - Singapore: +65XXXXXXXX (e.g., +6591234567)
-- 3. For Twilio trial accounts, you may need to verify phone numbers
-- 4. Gmail may require "App Password" instead of regular password
-- ====================================================================
