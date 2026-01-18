-- ====================================================================
-- Database Migration Script for Patient Records Timestamps
-- HealthTech Medical - Add created_at and updated_at to PatientRecord
-- Date: January 18, 2026
-- ====================================================================

-- Step 1: Add timestamp columns to PatientRecord table
-- ====================================================================
ALTER TABLE PatientRecord 
ADD COLUMN IF NOT EXISTS created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;

-- Step 2: Update existing records with current timestamp
-- ====================================================================
UPDATE PatientRecord 
SET created_at = NOW(), 
    updated_at = NOW()
WHERE created_at IS NULL OR updated_at IS NULL;

-- ====================================================================
-- Step 3: Verify the changes
-- ====================================================================
SELECT recordID, patientID, title, recordType, created_at, updated_at 
FROM PatientRecord 
ORDER BY created_at DESC
LIMIT 10;

-- ====================================================================
-- NOTES:
-- ====================================================================
-- 1. created_at: Automatically set when record is created
-- 2. updated_at: Automatically updated when record is modified
-- 3. Both columns use DATETIME format (YYYY-MM-DD HH:MM:SS)
-- 4. Existing records will have their timestamps set to the migration time
-- ====================================================================
