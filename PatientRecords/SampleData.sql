-- Sample Data for Patient Record Management (Composite Pattern Demo)
-- Run this in your MedEaseDB database

-- Make sure you have a patient with ID = 2
-- If not, insert a test patient first:
-- INSERT INTO User (name, email, password, role, phone) 
-- VALUES ('John Doe', 'john@test.com', 'password123', 'Patient', '1234567890');

-- Clear existing records for patient ID 2 (optional)
DELETE FROM PatientRecord WHERE patientID = 2;

-- ROOT LEVEL: Laboratory Results Folder
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES (2, NULL, 'Folder', 'Laboratory Results', 'All lab test results');

-- Get the ID of the folder we just created
SET @labFolderId = LAST_INSERT_ID();

-- Add records inside Laboratory Results folder
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES 
(2, @labFolderId, 'File', 'Blood Test - Jan 2026', 'Hemoglobin: 14.5 g/dL, WBC: 7500, RBC: 5.2M'),
(2, @labFolderId, 'File', 'Urine Analysis - Jan 2026', 'pH: 6.5, Protein: Negative, Glucose: Normal'),
(2, @labFolderId, 'File', 'Lipid Panel - Dec 2025', 'Total Cholesterol: 180 mg/dL, LDL: 100, HDL: 60');

-- ROOT LEVEL: Imaging Reports Folder
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES (2, NULL, 'Folder', 'Imaging Reports', 'X-rays, MRIs, CT scans');

SET @imagingFolderId = LAST_INSERT_ID();

-- Add records inside Imaging Reports folder
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES 
(2, @imagingFolderId, 'File', 'Chest X-Ray - Dec 2025', 'Result: Clear, no abnormalities detected'),
(2, @imagingFolderId, 'File', 'Brain MRI - Nov 2025', 'Result: Normal brain structure, no lesions');

-- ROOT LEVEL: Prescriptions Folder
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES (2, NULL, 'Folder', 'Prescriptions', 'All medication records');

SET @prescriptionsFolderId = LAST_INSERT_ID();

-- NESTED: Current Medications Folder (inside Prescriptions)
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES (2, @prescriptionsFolderId, 'Folder', 'Current Medications', 'Active prescriptions');

SET @currentMedsFolderId = LAST_INSERT_ID();

-- Add records inside Current Medications
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES 
(2, @currentMedsFolderId, 'File', 'Metformin 500mg', 'Take twice daily with meals. For diabetes management. Prescribed: Jan 2025'),
(2, @currentMedsFolderId, 'File', 'Lisinopril 10mg', 'Take once daily in the morning. For blood pressure. Prescribed: Jan 2025'),
(2, @currentMedsFolderId, 'File', 'Atorvastatin 20mg', 'Take once daily at bedtime. For cholesterol. Prescribed: Feb 2025');

-- NESTED: Past Medications Folder (inside Prescriptions)
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES (2, @prescriptionsFolderId, 'Folder', 'Past Medications', 'Completed or discontinued medications');

SET @pastMedsFolderId = LAST_INSERT_ID();

-- Add records inside Past Medications
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES 
(2, @pastMedsFolderId, 'File', 'Amoxicillin 500mg', 'Completed course. For bacterial infection. Duration: Dec 2025 (7 days)'),
(2, @pastMedsFolderId, 'File', 'Ibuprofen 400mg', 'Discontinued. For pain relief. Used: Nov 2025');

-- ROOT LEVEL: Vaccination Records Folder
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES (2, NULL, 'Folder', 'Vaccination Records', 'Immunization history');

SET @vaccineFolderId = LAST_INSERT_ID();

INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES 
(2, @vaccineFolderId, 'File', 'COVID-19 Vaccine', 'Pfizer-BioNTech, 2nd dose: March 2025'),
(2, @vaccineFolderId, 'File', 'Flu Vaccine', 'Annual flu shot: October 2025'),
(2, @vaccineFolderId, 'File', 'Tetanus Booster', 'Last administered: January 2024');

-- ROOT LEVEL: Individual Files (not in folders)
INSERT INTO PatientRecord (patientID, parentFolderID, recordType, title, details)
VALUES 
(2, NULL, 'File', 'General Checkup - Jan 10, 2026', 'BP: 120/80, Weight: 75kg, Height: 175cm. All vitals normal.'),
(2, NULL, 'File', 'Dental Checkup - Dec 15, 2025', 'Routine cleaning performed. No cavities. Next visit: June 2026'),
(2, NULL, 'File', 'Eye Examination - Nov 20, 2025', 'Vision: 20/20. No prescription changes needed.');

-- Verify the data
SELECT 
    recordID,
    patientID,
    parentFolderID,
    recordType,
    title,
    SUBSTRING(details, 1, 50) as details_preview
FROM PatientRecord 
WHERE patientID = 2
ORDER BY recordID;

-- Show the hierarchy structure
SELECT 
    CASE 
        WHEN parentFolderID IS NULL THEN '📋 ROOT'
        ELSE CONCAT('  └─ Parent: ', parentFolderID)
    END as hierarchy,
    recordID,
    CASE 
        WHEN recordType = 'Folder' THEN CONCAT('📂 ', title)
        ELSE CONCAT('📄 ', title)
    END as record_title
FROM PatientRecord 
WHERE patientID = 2
ORDER BY COALESCE(parentFolderID, 0), recordID;
