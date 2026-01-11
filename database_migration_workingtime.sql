-- ====================================================================
-- Database Migration Script for WorkingTime Table & Appointment Updates
-- HealthTech Medical - Doctor Availability System
-- Date: January 11, 2026
-- ====================================================================

-- Step 1: Create WorkingTime table for doctor availability
-- ====================================================================
CREATE TABLE IF NOT EXISTS WorkingTime (
    workingTimeID INT PRIMARY KEY AUTO_INCREMENT,
    doctorID INT NOT NULL,
    startTime TIME NOT NULL,
    endTime TIME NOT NULL,
    date DATE NULL,  -- NULL for recurring weekly schedule, specific date for overrides
    day VARCHAR(20) NOT NULL,  -- Monday, Tuesday, Wednesday, etc.
    type VARCHAR(20) NOT NULL DEFAULT 'Working',  -- 'Working', 'OnLeave', 'OffDay'
    FOREIGN KEY (doctorID) REFERENCES User(userID) ON DELETE CASCADE,
    INDEX idx_doctor_date (doctorID, date),
    INDEX idx_doctor_day (doctorID, day)
);

-- Step 2: Insert sample working hours for doctors
-- ====================================================================
-- Example: Doctor ID 10 works Mon-Fri, 9 AM to 5 PM
-- IMPORTANT: Update doctorID values to match your actual doctor IDs

-- Doctor 1 - Regular working hours (Monday to Friday, 9 AM - 5 PM)
INSERT INTO WorkingTime (doctorID, startTime, endTime, date, day, type) VALUES
(10, '09:00:00', '17:00:00', NULL, 'Monday', 'Working'),
(10, '09:00:00', '17:00:00', NULL, 'Tuesday', 'Working'),
(10, '09:00:00', '17:00:00', NULL, 'Wednesday', 'Working'),
(10, '09:00:00', '17:00:00', NULL, 'Thursday', 'Working'),
(10, '09:00:00', '17:00:00', NULL, 'Friday', 'Working');

-- Doctor 2 - Different schedule (Monday to Saturday, 10 AM - 6 PM)
INSERT INTO WorkingTime (doctorID, startTime, endTime, date, day, type) VALUES
(11, '10:00:00', '18:00:00', NULL, 'Monday', 'Working'),
(11, '10:00:00', '18:00:00', NULL, 'Tuesday', 'Working'),
(11, '10:00:00', '18:00:00', NULL, 'Wednesday', 'Working'),
(11, '10:00:00', '18:00:00', NULL, 'Thursday', 'Working'),
(11, '10:00:00', '18:00:00', NULL, 'Friday', 'Working'),
(11, '10:00:00', '14:00:00', NULL, 'Saturday', 'Working');

-- Step 3: Example - Set specific date as on-leave
-- ====================================================================
-- Example: Doctor 10 is on leave on January 15, 2026
INSERT INTO WorkingTime (doctorID, startTime, endTime, date, day, type) VALUES
(10, '00:00:00', '00:00:00', '2026-01-15', 'Wednesday', 'OnLeave');

-- Step 4: Example - Set specific date as off day
-- ====================================================================
-- Example: Doctor 11 has off day on January 20, 2026
INSERT INTO WorkingTime (doctorID, startTime, endTime, date, day, type) VALUES
(11, '00:00:00', '00:00:00', '2026-01-20', 'Monday', 'OffDay');

-- ====================================================================
-- Step 5: Verify the data
-- ====================================================================
SELECT * FROM WorkingTime ORDER BY doctorID, day;

-- ====================================================================
-- Step 6: Query to check doctor availability for a specific date
-- ====================================================================
-- Example: Check Doctor 10's availability for today
SELECT 
    wt.*,
    u.Name as DoctorName
FROM WorkingTime wt
JOIN User u ON wt.doctorID = u.userID
WHERE wt.doctorID = 10
  AND (wt.date = CURDATE() OR (wt.date IS NULL AND wt.day = DAYNAME(CURDATE())))
ORDER BY wt.date DESC, wt.startTime;

-- ====================================================================
-- NOTES & USAGE GUIDE:
-- ====================================================================
-- 1. WorkingTime Table Structure:
--    - workingTimeID: Primary key
--    - doctorID: Reference to User table (doctors only)
--    - startTime: Working hours start time (e.g., 09:00:00)
--    - endTime: Working hours end time (e.g., 17:00:00)
--    - date: NULL for recurring schedule, specific date for overrides
--    - day: Day of week (Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday)
--    - type: 'Working', 'OnLeave', 'OffDay'
--
-- 2. How the system works:
--    - Regular schedule: date=NULL, day=DayOfWeek, type='Working'
--    - Specific date override: date=SpecificDate, type='OnLeave' or 'OffDay'
--    - System checks specific date first, then falls back to regular schedule
--
-- 3. Adding new doctor schedules:
--    UPDATE doctorID values (10, 11) to match your actual doctor IDs from User table
--
-- 4. Time slot duration:
--    Default is 30 minutes (configurable in GetAvailableTimeSlots method)
--
-- 5. Appointment booking process:
--    a) Select doctor
--    b) Select date
--    c) System calculates available time slots based on:
--       - Doctor's working hours for that day
--       - Existing appointments (already booked slots)
--       - Past time slots are excluded
--    d) Select available time slot
--    e) Book appointment
-- ====================================================================

-- ====================================================================
-- Quick Setup Commands (Copy-paste to terminal):
-- ====================================================================
-- 1. Find your doctor IDs:
SELECT userID, Name, Role FROM User WHERE Role = 'Doctor';

-- 2. Add working hours for a specific doctor (replace 10 with actual doctorID):
-- INSERT INTO WorkingTime (doctorID, startTime, endTime, date, day, type) VALUES
-- (10, '09:00:00', '17:00:00', NULL, 'Monday', 'Working'),
-- (10, '09:00:00', '17:00:00', NULL, 'Tuesday', 'Working'),
-- (10, '09:00:00', '17:00:00', NULL, 'Wednesday', 'Working'),
-- (10, '09:00:00', '17:00:00', NULL, 'Thursday', 'Working'),
-- (10, '09:00:00', '17:00:00', NULL, 'Friday', 'Working');

-- 3. Mark a specific date as on-leave:
-- INSERT INTO WorkingTime (doctorID, startTime, endTime, date, day, type) 
-- VALUES (10, '00:00:00', '00:00:00', '2026-01-15', 'Wednesday', 'OnLeave');

-- ====================================================================
