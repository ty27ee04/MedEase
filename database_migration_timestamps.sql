-- ====================================================================
-- Database Migration Script for Patient Records Timestamps
-- HealthTech Medical - Add createdDate and lastUpdated to PatientRecord
-- Date: January 18, 2026
-- ====================================================================

-- Step 1: Add timestamp columns to PatientRecord table
-- ====================================================================
ALTER TABLE PatientRecord 
ADD COLUMN IF NOT EXISTS createdDate DATETIME DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS lastUpdated DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;

-- Step 2: Update existing records with current timestamp
-- ====================================================================
UPDATE PatientRecord 
SET createdDate = NOW(), 
    lastUpdated = NOW()
WHERE createdDate IS NULL OR lastUpdated IS NULL;

-- ====================================================================
-- Step 3: Verify the changes
-- ====================================================================
SELECT recordID, patientID, title, recordType, createdDate, lastUpdated 
FROM PatientRecord 
ORDER BY createdDate DESC
LIMIT 10;

-- ====================================================================
-- NOTES:
-- ====================================================================
-- 1. created_at: Automatically set when record is created
-- 2. updated_at: Automatically updated when record is modified
-- 3. Both columns use DATETIME format (YYYY-MM-DD HH:MM:SS)
-- 4. Existing records will have their timestamps set to the migration time
-- ====================================================================
