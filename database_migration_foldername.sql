-- Migration to add FolderName column to PatientRecord table
-- This allows doctors to assign names to folders for better organization

-- Add FolderName column to PatientRecord table
ALTER TABLE PatientRecord 
ADD COLUMN FolderName VARCHAR(255) DEFAULT '' AFTER Details;

-- Update existing folders to have a default folder name based on their title
UPDATE PatientRecord 
SET FolderName = Title 
WHERE RecordType = 'Folder' AND (FolderName IS NULL OR FolderName = '');
