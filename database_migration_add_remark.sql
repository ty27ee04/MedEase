-- ====================================================================
-- Database Migration Script: Add Remark Column to Appointment Table
-- HealthTech Medical System
-- Date: January 22, 2026
-- ====================================================================

USE medeasedb;

-- Step 1: Add remark column to appointment table
-- ====================================================================
ALTER TABLE appointment 
ADD COLUMN remark TEXT NULL AFTER link;

-- Step 2: Update existing appointments with sample remarks (optional)
-- ====================================================================
UPDATE appointment 
SET remark = 'Follow-up appointment' 
WHERE appointmentID = 1;

UPDATE appointment 
SET remark = 'Regular checkup' 
WHERE appointmentID = 2;

UPDATE appointment 
SET remark = 'Fever and flu symptoms' 
WHERE appointmentID = 3;

-- Step 3: Verify the changes
-- ====================================================================
SELECT 'Remark column added successfully' AS Status;

-- Show updated table structure
DESCRIBE appointment;

-- Show sample data with remarks
SELECT appointmentID, patientID, doctorID, dateTime, status, type, remark 
FROM appointment 
LIMIT 5;

-- ====================================================================
-- Migration Complete!
-- ====================================================================
