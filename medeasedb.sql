-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- 主机： 127.0.0.1
-- 生成日期： 2026-01-19 02:57:39
-- 服务器版本： 10.4.28-MariaDB
-- PHP 版本： 8.2.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- 数据库： `medeasedb`
--

-- --------------------------------------------------------

--
-- 表的结构 `appointment`
--

CREATE TABLE `appointment` (
  `appointmentID` int(11) NOT NULL,
  `patientID` int(11) NOT NULL,
  `doctorID` int(11) NOT NULL,
  `roomID` int(11) DEFAULT NULL,
  `dateTime` datetime NOT NULL,
  `status` varchar(50) DEFAULT 'Pending',
  `type` varchar(50) NOT NULL,
  `link` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 转存表中的数据 `appointment`
--

INSERT INTO `appointment` (`appointmentID`, `patientID`, `doctorID`, `roomID`, `dateTime`, `status`, `type`, `link`) VALUES
(1, 2, 1, NULL, '2026-01-17 09:00:00', 'Confirmed', 'Online', 'https://zoom.us/j/198817396'),
(2, 2, 1, NULL, '2026-01-12 09:30:00', 'Confirmed', 'Physical', NULL),
(3, 2, 1, NULL, '2026-01-14 18:35:00', 'ReminderSent', 'Physical', NULL);

-- --------------------------------------------------------

--
-- 表的结构 `availabilityslot`
--

CREATE TABLE `availabilityslot` (
  `slotID` int(11) NOT NULL,
  `doctorID` int(11) NOT NULL,
  `startTime` datetime NOT NULL,
  `endTime` datetime NOT NULL,
  `isBooked` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 表的结构 `notification`
--

CREATE TABLE `notification` (
  `notificationID` int(11) NOT NULL,
  `appointmentID` int(11) NOT NULL,
  `message` text NOT NULL,
  `sendTime` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 表的结构 `notificationlog`
--

CREATE TABLE `notificationlog` (
  `logID` int(11) NOT NULL,
  `appointmentID` int(11) NOT NULL,
  `channel` varchar(50) NOT NULL,
  `recipient` varchar(255) NOT NULL,
  `message` text NOT NULL,
  `sentAt` datetime NOT NULL,
  `status` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 转存表中的数据 `notificationlog`
--

INSERT INTO `notificationlog` (`logID`, `appointmentID`, `channel`, `recipient`, `message`, `sentAt`, `status`) VALUES
(1, 1, 'Email', 'Patient@example.com', 'Appointment confirmation for January 17, 2026 at 09:00 AM', '2026-01-09 15:51:37', 'Sent'),
(2, 2, 'Email', 'Patient@example.com', 'Appointment confirmation for January 10, 2026 at 09:30 AM', '2026-01-09 15:51:37', 'Sent'),
(4, 2, 'Email', 'Patient@test.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 15:53:13', 'Sent'),
(5, 2, 'WhatsApp', '0123456789', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 15:53:13', 'Sent'),
(6, 2, 'SMS', '0123456789', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 15:53:13', 'Sent'),
(7, 2, 'In-App', 'Patient@test.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 15:53:13', 'Displayed'),
(8, 2, 'Email', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 15:55:52', 'Sent'),
(9, 2, 'WhatsApp', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 15:55:52', 'Sent'),
(10, 2, 'SMS', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 15:55:52', 'Sent'),
(11, 2, 'In-App', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 15:55:52', 'Displayed'),
(12, 2, 'Email', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:09:56', 'Sent'),
(13, 2, 'WhatsApp', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:09:56', 'Sent'),
(14, 2, 'SMS', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:09:56', 'Sent'),
(15, 2, 'In-App', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:09:56', 'Displayed'),
(16, 2, 'Email', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:11:00', 'Sent'),
(17, 2, 'WhatsApp', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:11:00', 'Sent'),
(18, 2, 'SMS', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:11:00', 'Sent'),
(19, 2, 'In-App', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:11:00', 'Displayed'),
(20, 2, 'Email', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:18:36', 'Sent'),
(21, 2, 'WhatsApp', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:18:37', 'Failed: WhatsApp sending failed: The \'To\' number 01153558939 is not a valid phone number.'),
(22, 2, 'SMS', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:18:37', 'Failed: SMS sending failed: Invalid \'To\' Phone Number: 0115355XXXX'),
(23, 2, 'In-App', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:18:37', 'Displayed'),
(24, 2, 'Email', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:18:40', 'Sent'),
(25, 2, 'WhatsApp', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:18:40', 'Failed: WhatsApp sending failed: Account ACe86544e92049f712390cb1732e6ba0f6 exceeded the 5 daily mes'),
(26, 2, 'SMS', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:18:41', 'Failed: SMS sending failed: Account ACe86544e92049f712390cb1732e6ba0f6 exceeded the 5 daily messages'),
(27, 2, 'In-App', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:18:41', 'Displayed'),
(28, 2, 'Email', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:18:43', 'Sent'),
(29, 2, 'WhatsApp', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:18:44', 'Failed: WhatsApp sending failed: Account ACe86544e92049f712390cb1732e6ba0f6 exceeded the 5 daily mes'),
(30, 2, 'SMS', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:18:44', 'Failed: SMS sending failed: Account ACe86544e92049f712390cb1732e6ba0f6 exceeded the 5 daily messages'),
(31, 2, 'In-App', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:18:44', 'Displayed'),
(32, 2, 'Email', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:27:28', 'Sent'),
(33, 2, 'WhatsApp', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:27:28', 'Failed: WhatsApp sending failed: Account ACe86544e92049f712390cb1732e6ba0f6 exceeded the 5 daily mes'),
(34, 2, 'SMS', '01153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:27:28', 'Failed: SMS sending failed: Account ACe86544e92049f712390cb1732e6ba0f6 exceeded the 5 daily messages'),
(35, 2, 'In-App', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:27:28', 'Displayed'),
(36, 2, 'Email', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:27:50', 'Sent'),
(37, 2, 'WhatsApp', '+601153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:27:51', 'Failed: WhatsApp sending failed: Twilio could not find a Channel with the specified From address'),
(38, 2, 'SMS', '+601153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:27:51', 'Failed: SMS sending failed: The number +60115355XXXX is unverified. Trial accounts cannot send messa'),
(39, 2, 'In-App', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:27:51', 'Displayed'),
(40, 2, 'Email', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:28:53', 'Sent'),
(41, 2, 'WhatsApp', '+601153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:28:54', 'Failed: WhatsApp sending failed: Twilio could not find a Channel with the specified From address'),
(42, 2, 'SMS', '+601153558939', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:28:54', 'Failed: SMS sending failed: The number +60115355XXXX is unverified. Trial accounts cannot send messa'),
(43, 2, 'In-App', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:28:54', 'Displayed'),
(44, 2, 'Email', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:31:33', 'Sent'),
(45, 2, 'WhatsApp', '+60167255620', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:31:33', 'Failed: WhatsApp sending failed: Twilio could not find a Channel with the specified From address'),
(46, 2, 'SMS', '+60167255620', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:31:34', 'Failed: SMS sending failed: Account AC4032607bc50c981b1c14fca1bb8b577b exceeded the 5 daily messages'),
(47, 2, 'In-App', 'tyongye04@gmail.com', 'Your appointment with Dr. Yong Ye is confirmed for Jan 10, 2026 at 09:30 AM. Type: Physical', '2026-01-09 16:31:34', 'Displayed');

-- --------------------------------------------------------

--
-- 表的结构 `patientrecord`
--

CREATE TABLE `patientrecord` (
  `recordID` int(11) NOT NULL,
  `patientID` int(11) NOT NULL,
  `doctorID` int(11) DEFAULT NULL,
  `parentFolderID` int(11) DEFAULT NULL,
  `recordType` varchar(50) NOT NULL,
  `title` varchar(100) NOT NULL,
  `details` text DEFAULT NULL,
  `createdDate` datetime DEFAULT current_timestamp(),
  `lastUpdated` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 转存表中的数据 `patientrecord`
--

INSERT INTO `patientrecord` (`recordID`, `patientID`, `doctorID`, `parentFolderID`, `recordType`, `title`, `details`, `createdDate`, `lastUpdated`) VALUES
(1, 2, NULL, NULL, 'File', '1', '1', '2026-01-19 00:17:54', '2026-01-19 00:17:54');

-- --------------------------------------------------------

--
-- 表的结构 `report`
--

CREATE TABLE `report` (
  `reportID` int(11) NOT NULL,
  `doctorID` int(11) NOT NULL,
  `reportType` varchar(50) DEFAULT NULL,
  `generatedDate` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- 表的结构 `room`
--

CREATE TABLE `room` (
  `roomID` int(11) NOT NULL,
  `roomNumber` varchar(20) NOT NULL,
  `roomType` varchar(50) DEFAULT NULL,
  `status` varchar(20) DEFAULT 'Available',
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 转存表中的数据 `room`
--

INSERT INTO `room` (`roomID`, `roomNumber`, `roomType`, `status`, `created_at`) VALUES
(1, '101', 'Consultation', 'Available', '2026-01-12 02:35:21'),
(2, '102', 'Consultation', 'Available', '2026-01-12 02:35:21'),
(3, '103', 'Emergency', 'Available', '2026-01-12 02:35:21'),
(4, '104', 'Surgery', 'Available', '2026-01-12 02:35:21'),
(5, '105', 'ICU', 'Available', '2026-01-12 02:35:21'),
(6, '201', 'Consultation', 'Available', '2026-01-12 02:35:21'),
(7, '202', 'Lab', 'Available', '2026-01-12 02:35:21'),
(8, '203', 'X-Ray', 'Available', '2026-01-12 02:35:21');

-- --------------------------------------------------------

--
-- 表的结构 `user`
--

CREATE TABLE `user` (
  `userID` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `email` varchar(100) NOT NULL,
  `password` varchar(255) NOT NULL,
  `phone` varchar(20) DEFAULT NULL,
  `role` varchar(20) NOT NULL,
  `specialization` varchar(100) DEFAULT NULL,
  `licenseNumber` varchar(50) DEFAULT NULL,
  `createdAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 转存表中的数据 `user`
--

INSERT INTO `user` (`userID`, `name`, `email`, `password`, `phone`, `role`, `specialization`, `licenseNumber`, `createdAt`) VALUES
(1, 'Yong Ye', 'yongyeyongye70@gmail.com', '12345', '+60167255620', 'Doctor', NULL, NULL, '2026-01-09 14:27:38'),
(2, 'Patient', 'tyongye04@gmail.com', '12345', '+60167255620', 'Patient', NULL, NULL, '2026-01-09 14:28:22');

-- --------------------------------------------------------

--
-- 表的结构 `workingtime`
--

CREATE TABLE `workingtime` (
  `workingID` int(11) NOT NULL,
  `doctorID` int(11) NOT NULL,
  `startTime` time DEFAULT NULL,
  `endTime` time DEFAULT NULL,
  `date` date DEFAULT NULL,
  `day` varchar(15) DEFAULT NULL,
  `type` enum('working','on leave','off day') DEFAULT 'working'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 转存表中的数据 `workingtime`
--

INSERT INTO `workingtime` (`workingID`, `doctorID`, `startTime`, `endTime`, `date`, `day`, `type`) VALUES
(1, 1, '08:00:00', '17:00:00', NULL, 'Monday', 'working'),
(2, 1, '08:00:00', '17:00:00', NULL, 'Tuesday', 'working'),
(3, 1, '08:00:00', '17:00:00', NULL, 'Wednesday', 'working'),
(4, 1, '08:00:00', '17:00:00', NULL, 'Friday', 'working'),
(5, 1, '08:00:00', '17:00:00', NULL, 'Saturday', 'working'),
(6, 1, '17:00:00', '04:00:00', NULL, 'Sunday', 'working'),
(7, 1, '00:00:00', '00:00:00', NULL, 'Thursday', 'off day'),
(8, 1, '00:00:00', '00:00:00', '2026-01-14', '', 'on leave');

--
-- 转储表的索引
--

--
-- 表的索引 `appointment`
--
ALTER TABLE `appointment`
  ADD PRIMARY KEY (`appointmentID`),
  ADD KEY `patientID` (`patientID`),
  ADD KEY `doctorID` (`doctorID`),
  ADD KEY `roomID` (`roomID`);

--
-- 表的索引 `availabilityslot`
--
ALTER TABLE `availabilityslot`
  ADD PRIMARY KEY (`slotID`),
  ADD KEY `doctorID` (`doctorID`);

--
-- 表的索引 `notification`
--
ALTER TABLE `notification`
  ADD PRIMARY KEY (`notificationID`),
  ADD KEY `appointmentID` (`appointmentID`);

--
-- 表的索引 `notificationlog`
--
ALTER TABLE `notificationlog`
  ADD PRIMARY KEY (`logID`),
  ADD KEY `idx_appointment` (`appointmentID`),
  ADD KEY `idx_sentAt` (`sentAt`);

--
-- 表的索引 `patientrecord`
--
ALTER TABLE `patientrecord`
  ADD PRIMARY KEY (`recordID`),
  ADD KEY `patientID` (`patientID`),
  ADD KEY `doctorID` (`doctorID`),
  ADD KEY `parentFolderID` (`parentFolderID`);

--
-- 表的索引 `report`
--
ALTER TABLE `report`
  ADD PRIMARY KEY (`reportID`),
  ADD KEY `doctorID` (`doctorID`);

--
-- 表的索引 `room`
--
ALTER TABLE `room`
  ADD PRIMARY KEY (`roomID`);

--
-- 表的索引 `user`
--
ALTER TABLE `user`
  ADD PRIMARY KEY (`userID`),
  ADD UNIQUE KEY `email` (`email`);

--
-- 表的索引 `workingtime`
--
ALTER TABLE `workingtime`
  ADD PRIMARY KEY (`workingID`),
  ADD KEY `doctorID` (`doctorID`);

--
-- 在导出的表使用AUTO_INCREMENT
--

--
-- 使用表AUTO_INCREMENT `appointment`
--
ALTER TABLE `appointment`
  MODIFY `appointmentID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- 使用表AUTO_INCREMENT `availabilityslot`
--
ALTER TABLE `availabilityslot`
  MODIFY `slotID` int(11) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `notification`
--
ALTER TABLE `notification`
  MODIFY `notificationID` int(11) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `notificationlog`
--
ALTER TABLE `notificationlog`
  MODIFY `logID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=48;

--
-- 使用表AUTO_INCREMENT `patientrecord`
--
ALTER TABLE `patientrecord`
  MODIFY `recordID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- 使用表AUTO_INCREMENT `report`
--
ALTER TABLE `report`
  MODIFY `reportID` int(11) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `room`
--
ALTER TABLE `room`
  MODIFY `roomID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- 使用表AUTO_INCREMENT `user`
--
ALTER TABLE `user`
  MODIFY `userID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- 使用表AUTO_INCREMENT `workingtime`
--
ALTER TABLE `workingtime`
  MODIFY `workingID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- 限制导出的表
--

--
-- 限制表 `appointment`
--
ALTER TABLE `appointment`
  ADD CONSTRAINT `appointment_ibfk_1` FOREIGN KEY (`patientID`) REFERENCES `user` (`userID`) ON DELETE CASCADE,
  ADD CONSTRAINT `appointment_ibfk_2` FOREIGN KEY (`doctorID`) REFERENCES `user` (`userID`) ON DELETE CASCADE,
  ADD CONSTRAINT `appointment_ibfk_3` FOREIGN KEY (`roomID`) REFERENCES `room` (`roomID`) ON DELETE SET NULL;

--
-- 限制表 `availabilityslot`
--
ALTER TABLE `availabilityslot`
  ADD CONSTRAINT `availabilityslot_ibfk_1` FOREIGN KEY (`doctorID`) REFERENCES `user` (`userID`) ON DELETE CASCADE;

--
-- 限制表 `notification`
--
ALTER TABLE `notification`
  ADD CONSTRAINT `notification_ibfk_1` FOREIGN KEY (`appointmentID`) REFERENCES `appointment` (`appointmentID`) ON DELETE CASCADE;

--
-- 限制表 `notificationlog`
--
ALTER TABLE `notificationlog`
  ADD CONSTRAINT `notificationlog_ibfk_1` FOREIGN KEY (`appointmentID`) REFERENCES `appointment` (`appointmentID`) ON DELETE CASCADE;

--
-- 限制表 `patientrecord`
--
ALTER TABLE `patientrecord`
  ADD CONSTRAINT `patientrecord_ibfk_1` FOREIGN KEY (`patientID`) REFERENCES `user` (`userID`) ON DELETE CASCADE,
  ADD CONSTRAINT `patientrecord_ibfk_2` FOREIGN KEY (`doctorID`) REFERENCES `user` (`userID`) ON DELETE SET NULL,
  ADD CONSTRAINT `patientrecord_ibfk_3` FOREIGN KEY (`parentFolderID`) REFERENCES `patientrecord` (`recordID`) ON DELETE CASCADE;

--
-- 限制表 `report`
--
ALTER TABLE `report`
  ADD CONSTRAINT `report_ibfk_1` FOREIGN KEY (`doctorID`) REFERENCES `user` (`userID`) ON DELETE CASCADE;

--
-- 限制表 `workingtime`
--
ALTER TABLE `workingtime`
  ADD CONSTRAINT `workingtime_ibfk_1` FOREIGN KEY (`doctorID`) REFERENCES `user` (`userID`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
<<<<<<< HEAD

SELECT 
    u.name AS Patient,
    a.type AS AppointmentType,
    a.status AS Status,
    a.dateTime AS VisitDate
FROM appointment a
JOIN user u ON a.patientID = u.userID
WHERE u.userID = @id;

SELECT 
    a.appointmentID,
    u.name AS Patient,
    a.type,
    a.status,
    a.dateTime
FROM appointment a
JOIN user u ON a.patientID = u.userID
WHERE a.dateTime BETWEEN @start AND @end;
=======
>>>>>>> 422f23b6b42e05d1657dada3ee19136d19b15070
