-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- 主机： 127.0.0.1
-- 生成日期： 2026-01-23 10:36:45
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
  `link` varchar(255) DEFAULT NULL,
  `remark` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 转存表中的数据 `appointment`
--

INSERT INTO `appointment` (`appointmentID`, `patientID`, `doctorID`, `roomID`, `dateTime`, `status`, `type`, `link`, `remark`) VALUES
(1, 2, 1, NULL, '2026-01-17 09:00:00', 'Confirmed', 'Online', 'https://zoom.us/j/198817396', 'Body Check Up\n'),
(2, 2, 1, NULL, '2026-01-12 09:30:00', 'Confirmed', 'Physical', NULL, 'Fever'),
(3, 2, 1, NULL, '2026-01-14 18:35:00', 'ReminderSent', 'Physical', NULL, 'Throat Pain'),
(4, 2, 6, 1, '2026-01-23 16:30:00', 'Completed', 'Physical', NULL, 'Follow Up');

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
  `lastUpdated` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `InFolder` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 转存表中的数据 `patientrecord`
--

INSERT INTO `patientrecord` (`recordID`, `patientID`, `doctorID`, `parentFolderID`, `recordType`, `title`, `details`, `createdDate`, `lastUpdated`, `InFolder`) VALUES
(1, 2, NULL, NULL, 'File', 'Test Update', '3', '2026-01-19 00:17:54', '2026-01-19 11:34:51', NULL),
(3, 3, NULL, NULL, 'File', '123', '4', '2026-01-19 10:04:13', '2026-01-19 11:48:01', NULL),
(4, 2, NULL, NULL, 'File', 'Test', 'Test data', '2026-01-19 10:25:45', '2026-01-19 10:25:45', NULL),
(5, 3, NULL, NULL, 'Folder', 'Test Folder', '123', '2026-01-22 12:12:11', '2026-01-22 14:17:48', 9),
(6, 3, 1, NULL, 'File', 'Test File in Folder', 'Testing Data', '2026-01-22 12:12:34', '2026-01-23 02:11:57', 9),
(7, 3, NULL, NULL, 'File', 'jrjrjr', 'jrjrjr123', '2026-01-22 12:12:53', '2026-01-22 13:53:25', 5),
(8, 3, NULL, NULL, 'File', '123', '123', '2026-01-22 13:29:45', '2026-01-22 13:52:44', 5),
(9, 3, NULL, NULL, 'Folder', 'Second level of folder', 'Test', '2026-01-22 13:54:06', '2026-01-22 14:17:43', NULL),
(10, 3, 1, NULL, 'File', 'Test', '', '2026-01-23 01:53:45', '2026-01-23 01:53:45', NULL),
(11, 3, 6, NULL, 'File', '1', '1', '2026-01-23 02:12:40', '2026-01-23 02:12:40', NULL),
(12, 3, 4, NULL, 'File', '1', '1', '2026-01-23 02:13:11', '2026-01-23 02:13:11', NULL),
(13, 2, 6, NULL, 'File', 'Blood Test Result', 'Result all good', '2026-01-23 15:54:42', '2026-01-23 15:54:56', 14),
(14, 2, NULL, NULL, 'Folder', 'Test Result', '', '2026-01-23 15:54:50', '2026-01-23 15:54:50', NULL);

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
  `createdAt` datetime DEFAULT current_timestamp(),
  `age` int(11) DEFAULT NULL,
  `gender` varchar(10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 转存表中的数据 `user`
--

INSERT INTO `user` (`userID`, `name`, `email`, `password`, `phone`, `role`, `specialization`, `licenseNumber`, `createdAt`, `age`, `gender`) VALUES
(1, 'Yong Ye', 'yongyeyongye70@gmail.com', '12345', '+60167255620', 'Doctor', NULL, NULL, '2026-01-09 14:27:38', NULL, NULL),
(2, 'Patient', 'tyongye04@gmail.com', '12345', '+60167255620', 'Patient', NULL, NULL, '2026-01-09 14:28:22', NULL, NULL),
(3, 'mon', 'mon@gmail.com', 'DefaultPassword123', '01234567896', 'Patient', NULL, NULL, '2026-01-19 10:00:47', 100, 'Other'),
(4, 'tan', 'tan@test.com', '123', '0123456789', 'Doctor', NULL, NULL, '2026-01-22 15:47:17', NULL, NULL),
(5, 'Receptionist', 'receptionist@gmail.com', '123456', '0167255620', 'Receptionist', NULL, NULL, '2026-01-22 16:57:35', 21, 'Male'),
(6, 'Doctor', 'doctor@gmail.com', '123456', '0167255620', 'Doctor', 'Pediatrics', '123-567', '2026-01-22 21:57:42', 21, 'Male'),
(7, 'Tan Yong Ye', 'yongye@gmail.com', '123456', '0123456789', 'Receptionist', NULL, NULL, '2026-01-23 15:52:33', 21, 'Male');

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
(88, 1, '09:00:00', '17:00:00', NULL, 'Monday', 'working'),
(89, 1, '09:00:00', '17:00:00', NULL, 'Tuesday', 'working'),
(90, 1, '09:00:00', '17:00:00', NULL, 'Thursday', 'working'),
(91, 1, '09:00:00', '17:00:00', NULL, 'Friday', 'working'),
(92, 1, '09:00:00', '17:00:00', NULL, 'Saturday', 'working'),
(93, 1, '09:00:00', '17:00:00', NULL, 'Sunday', 'working'),
(94, 1, NULL, NULL, '2026-01-27', 'Tuesday', 'on leave'),
(95, 6, NULL, NULL, '2026-01-29', 'Thursday', 'on leave'),
(96, 6, NULL, NULL, '2026-01-30', 'Friday', 'on leave'),
(101, 6, '09:00:00', '17:00:00', NULL, 'Monday', 'working'),
(102, 6, '09:00:00', '17:00:00', NULL, 'Tuesday', 'working'),
(103, 6, '09:00:00', '17:00:00', NULL, 'Wednesday', 'working'),
(104, 6, '09:00:00', '17:00:00', NULL, 'Thursday', 'working'),
(105, 6, '09:00:00', '17:00:00', NULL, 'Friday', 'working'),
(106, 6, '09:00:00', '17:00:00', NULL, 'Saturday', 'working');

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
-- 表的索引 `patientrecord`
--
ALTER TABLE `patientrecord`
  ADD PRIMARY KEY (`recordID`),
  ADD KEY `patientID` (`patientID`),
  ADD KEY `doctorID` (`doctorID`),
  ADD KEY `parentFolderID` (`parentFolderID`),
  ADD KEY `FK_PatientRecord_InFolder` (`InFolder`);

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
  MODIFY `appointmentID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- 使用表AUTO_INCREMENT `patientrecord`
--
ALTER TABLE `patientrecord`
  MODIFY `recordID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- 使用表AUTO_INCREMENT `room`
--
ALTER TABLE `room`
  MODIFY `roomID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- 使用表AUTO_INCREMENT `user`
--
ALTER TABLE `user`
  MODIFY `userID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- 使用表AUTO_INCREMENT `workingtime`
--
ALTER TABLE `workingtime`
  MODIFY `workingID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=107;

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
-- 限制表 `patientrecord`
--
ALTER TABLE `patientrecord`
  ADD CONSTRAINT `FK_PatientRecord_InFolder` FOREIGN KEY (`InFolder`) REFERENCES `patientrecord` (`recordID`),
  ADD CONSTRAINT `patientrecord_ibfk_1` FOREIGN KEY (`patientID`) REFERENCES `user` (`userID`) ON DELETE CASCADE,
  ADD CONSTRAINT `patientrecord_ibfk_2` FOREIGN KEY (`doctorID`) REFERENCES `user` (`userID`) ON DELETE SET NULL,
  ADD CONSTRAINT `patientrecord_ibfk_3` FOREIGN KEY (`parentFolderID`) REFERENCES `patientrecord` (`recordID`) ON DELETE CASCADE;

--
-- 限制表 `workingtime`
--
ALTER TABLE `workingtime`
  ADD CONSTRAINT `workingtime_ibfk_1` FOREIGN KEY (`doctorID`) REFERENCES `user` (`userID`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
