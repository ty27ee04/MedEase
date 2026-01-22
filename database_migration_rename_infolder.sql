-- Migration to rename parentFolderID to InFolder in PatientRecord table
-- This makes the relationship more intuitive

-- Rename the column
ALTER TABLE PatientRecord 
CHANGE COLUMN parentFolderID InFolder INT NULL;
