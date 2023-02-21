-- Copyright Forged Wow LLC
-- Licensed under GPL v3.0 https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE 
-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               8.0.31 - MySQL Community Server - GPL
-- Server OS:                    Win64
-- HeidiSQL Version:             12.3.0.6589
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- Dumping structure for table hotfix_bk.char_base_info
DROP TABLE IF EXISTS `char_base_info`;
CREATE TABLE IF NOT EXISTS `char_base_info` (
  `ID` int unsigned NOT NULL DEFAULT '0',
  `RaceID` tinyint NOT NULL DEFAULT '0',
  `ClassID` tinyint NOT NULL DEFAULT '0',
  `FactionXferId` int NOT NULL DEFAULT '0',
  `VerifiedBuild` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`ID`,`VerifiedBuild`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Dumping data for table hotfix_bk.char_base_info: ~283 rows (approximately)
INSERT INTO `char_base_info` (`ID`, `RaceID`, `ClassID`, `FactionXferId`, `VerifiedBuild`) VALUES
	(1, 1, 1, 5, 45745),
	(2, 1, 2, 5, 45745),
	(3, 1, 3, 5, 45745),
	(4, 1, 4, 5, 45745),
	(5, 1, 5, 5, 45745),
	(6, 1, 6, 5, 45745),
	(7, 1, 7, 5, 45745),
	(8, 1, 8, 5, 45745),
	(9, 1, 9, 5, 45745),
	(10, 1, 10, 5, 45745),
	(11, 1, 11, 5, 45745),
	(12, 2, 1, 3, 45745),
	(13, 2, 2, 3, 45745),
	(14, 2, 3, 3, 45745),
	(15, 2, 4, 3, 45745),
	(16, 2, 5, 3, 45745),
	(17, 2, 6, 3, 45745),
	(18, 2, 7, 3, 45745),
	(19, 2, 8, 3, 45745),
	(20, 2, 9, 3, 45745),
	(21, 2, 10, 3, 45745),
	(22, 2, 11, 3, 45745),
	(23, 3, 1, 2, 45745),
	(24, 3, 2, 2, 45745),
	(25, 3, 3, 2, 45745),
	(26, 3, 4, 2, 45745),
	(27, 3, 5, 2, 45745),
	(28, 3, 6, 2, 45745),
	(29, 3, 7, 2, 45745),
	(30, 3, 8, 2, 45745),
	(31, 3, 9, 2, 45745),
	(32, 3, 10, 2, 45745),
	(33, 3, 11, 2, 45745),
	(34, 4, 1, 8, 45745),
	(35, 4, 2, 8, 45745),
	(36, 4, 3, 8, 45745),
	(37, 4, 4, 8, 45745),
	(38, 4, 5, 8, 45745),
	(39, 4, 6, 8, 45745),
	(40, 4, 7, 8, 45745),
	(41, 4, 8, 8, 45745),
	(42, 4, 9, 8, 45745),
	(43, 4, 10, 8, 45745),
	(44, 4, 11, 8, 45745),
	(45, 4, 12, 8, 45745),
	(46, 5, 1, 1, 45745),
	(47, 5, 2, 1, 45745),
	(48, 5, 3, 1, 45745),
	(49, 5, 4, 1, 45745),
	(50, 5, 5, 1, 45745),
	(51, 5, 6, 1, 45745),
	(52, 5, 7, 1, 45745),
	(53, 5, 8, 1, 45745),
	(54, 5, 9, 1, 45745),
	(55, 5, 10, 1, 45745),
	(56, 5, 11, 1, 45745),
	(57, 6, 1, 11, 45745),
	(58, 6, 2, 11, 45745),
	(59, 6, 3, 11, 45745),
	(60, 6, 4, 11, 45745),
	(61, 6, 5, 11, 45745),
	(62, 6, 6, 11, 45745),
	(63, 6, 7, 11, 45745),
	(64, 6, 8, 11, 45745),
	(65, 6, 9, 11, 45745),
	(66, 6, 10, 11, 45745),
	(67, 6, 11, 11, 45745),
	(68, 7, 1, 9, 45745),
	(69, 7, 2, 9, 45745),
	(70, 7, 3, 9, 45745),
	(71, 7, 4, 9, 45745),
	(72, 7, 5, 9, 45745),
	(73, 7, 6, 9, 45745),
	(74, 7, 7, 9, 45745),
	(75, 7, 8, 9, 45745),
	(76, 7, 9, 9, 45745),
	(77, 7, 10, 9, 45745),
	(78, 7, 11, 9, 45745),
	(79, 8, 1, 4, 45745),
	(80, 8, 2, 4, 45745),
	(81, 8, 3, 4, 45745),
	(82, 8, 4, 4, 45745),
	(83, 8, 5, 4, 45745),
	(84, 8, 6, 4, 45745),
	(85, 8, 7, 4, 45745),
	(86, 8, 8, 4, 45745),
	(87, 8, 9, 4, 45745),
	(88, 8, 10, 4, 45745),
	(89, 8, 11, 4, 45745),
	(90, 9, 1, 7, 45745),
	(91, 9, 2, 7, 45745),
	(92, 9, 3, 7, 45745),
	(93, 9, 4, 7, 45745),
	(94, 9, 5, 7, 45745),
	(95, 9, 6, 7, 45745),
	(96, 9, 7, 7, 45745),
	(97, 9, 8, 7, 45745),
	(98, 9, 9, 7, 45745),
	(99, 9, 10, 7, 45745),
	(100, 9, 11, 7, 45745),
	(101, 10, 1, 1, 45745),
	(102, 10, 2, 1, 45745),
	(103, 10, 3, 1, 45745),
	(104, 10, 4, 1, 45745),
	(105, 10, 5, 1, 45745),
	(106, 10, 6, 1, 45745),
	(107, 10, 7, 1, 45745),
	(108, 10, 8, 1, 45745),
	(109, 10, 9, 1, 45745),
	(110, 10, 10, 1, 45745),
	(111, 10, 11, 1, 45745),
	(112, 10, 12, 1, 45745),
	(113, 11, 1, 6, 45745),
	(114, 11, 2, 6, 45745),
	(115, 11, 3, 6, 45745),
	(116, 11, 4, 6, 45745),
	(117, 11, 5, 6, 45745),
	(118, 11, 6, 6, 45745),
	(119, 11, 7, 6, 45745),
	(120, 11, 8, 6, 45745),
	(121, 11, 9, 6, 45745),
	(122, 11, 10, 6, 45745),
	(123, 11, 11, 6, 45745),
	(124, 22, 1, 8, 45745),
	(125, 22, 2, 8, 45745),
	(126, 22, 3, 8, 45745),
	(127, 22, 4, 8, 45745),
	(128, 22, 5, 8, 45745),
	(129, 22, 6, 8, 45745),
	(130, 22, 7, 8, 45745),
	(131, 22, 8, 8, 45745),
	(132, 22, 9, 8, 45745),
	(133, 22, 10, 8, 45745),
	(134, 22, 11, 8, 45745),
	(135, 24, 1, 24, 45745),
	(136, 24, 2, 24, 45745),
	(137, 24, 3, 24, 45745),
	(138, 24, 4, 24, 45745),
	(139, 24, 5, 24, 45745),
	(140, 24, 6, 24, 45745),
	(141, 24, 7, 24, 45745),
	(142, 24, 8, 24, 45745),
	(143, 24, 9, 24, 45745),
	(144, 24, 10, 24, 45745),
	(145, 24, 11, 24, 45745),
	(146, 25, 1, 26, 45745),
	(147, 25, 2, 26, 45745),
	(148, 25, 3, 26, 45745),
	(149, 25, 4, 26, 45745),
	(150, 25, 5, 26, 45745),
	(151, 25, 6, 26, 45745),
	(152, 25, 7, 26, 45745),
	(153, 25, 8, 26, 45745),
	(154, 25, 9, 26, 45745),
	(155, 25, 10, 26, 45745),
	(156, 25, 11, 26, 45745),
	(157, 26, 1, 25, 45745),
	(158, 26, 2, 25, 45745),
	(159, 26, 3, 25, 45745),
	(160, 26, 4, 25, 45745),
	(161, 26, 5, 25, 45745),
	(162, 26, 6, 25, 45745),
	(163, 26, 7, 25, 45745),
	(164, 26, 8, 25, 45745),
	(165, 26, 9, 25, 45745),
	(166, 26, 10, 25, 45745),
	(167, 26, 11, 25, 45745),
	(168, 27, 1, 4, 45745),
	(169, 27, 2, 4, 45745),
	(170, 27, 3, 4, 45745),
	(171, 27, 4, 4, 45745),
	(172, 27, 5, 4, 45745),
	(173, 27, 6, 4, 45745),
	(174, 27, 7, 4, 45745),
	(175, 27, 8, 4, 45745),
	(176, 27, 9, 4, 45745),
	(177, 27, 10, 4, 45745),
	(178, 27, 11, 4, 45745),
	(179, 28, 1, 30, 45745),
	(180, 28, 2, 30, 45745),
	(181, 28, 3, 30, 45745),
	(182, 28, 4, 30, 45745),
	(183, 28, 5, 30, 45745),
	(184, 28, 6, 30, 45745),
	(185, 28, 7, 30, 45745),
	(186, 28, 8, 30, 45745),
	(187, 28, 9, 30, 45745),
	(188, 28, 10, 30, 45745),
	(189, 28, 11, 30, 45745),
	(190, 29, 1, 10, 45745),
	(191, 29, 2, 10, 45745),
	(192, 29, 3, 10, 45745),
	(193, 29, 4, 10, 45745),
	(194, 29, 5, 10, 45745),
	(195, 29, 6, 10, 45745),
	(196, 29, 7, 10, 45745),
	(197, 29, 8, 10, 45745),
	(198, 29, 9, 10, 45745),
	(199, 29, 10, 10, 45745),
	(200, 29, 11, 10, 45745),
	(201, 30, 1, 6, 45745),
	(202, 30, 2, 6, 45745),
	(203, 30, 3, 6, 45745),
	(204, 30, 4, 6, 45745),
	(205, 30, 5, 6, 45745),
	(206, 30, 6, 6, 45745),
	(207, 30, 7, 6, 45745),
	(208, 30, 8, 6, 45745),
	(209, 30, 9, 6, 45745),
	(210, 30, 10, 6, 45745),
	(211, 30, 11, 6, 45745),
	(212, 31, 1, 32, 45745),
	(213, 31, 2, 32, 45745),
	(214, 31, 3, 32, 45745),
	(215, 31, 4, 32, 45745),
	(216, 31, 5, 32, 45745),
	(217, 31, 6, 32, 45745),
	(218, 31, 7, 32, 45745),
	(219, 31, 8, 32, 45745),
	(220, 31, 9, 32, 45745),
	(221, 31, 10, 32, 45745),
	(222, 31, 11, 32, 45745),
	(223, 32, 1, 31, 45745),
	(224, 32, 2, 31, 45745),
	(225, 32, 3, 31, 45745),
	(226, 32, 4, 31, 45745),
	(227, 32, 5, 31, 45745),
	(228, 32, 6, 31, 45745),
	(229, 32, 7, 31, 45745),
	(230, 32, 8, 31, 45745),
	(231, 32, 9, 31, 45745),
	(232, 32, 10, 31, 45745),
	(233, 32, 11, 31, 45745),
	(234, 34, 1, 36, 45745),
	(235, 34, 2, 36, 45745),
	(236, 34, 3, 36, 45745),
	(237, 34, 4, 36, 45745),
	(238, 34, 5, 36, 45745),
	(239, 34, 6, 36, 45745),
	(240, 34, 7, 36, 45745),
	(241, 34, 8, 36, 45745),
	(242, 34, 9, 36, 45745),
	(243, 34, 10, 36, 45745),
	(244, 34, 11, 36, 45745),
	(245, 35, 1, 37, 45745),
	(246, 35, 2, 37, 45745),
	(247, 35, 3, 37, 45745),
	(248, 35, 4, 37, 45745),
	(249, 35, 5, 37, 45745),
	(250, 35, 6, 37, 45745),
	(251, 35, 7, 37, 45745),
	(252, 35, 8, 37, 45745),
	(253, 35, 9, 37, 45745),
	(254, 35, 10, 37, 45745),
	(255, 35, 11, 37, 45745),
	(256, 36, 1, 34, 45745),
	(257, 36, 2, 34, 45745),
	(258, 36, 3, 34, 45745),
	(259, 36, 4, 34, 45745),
	(260, 36, 5, 34, 45745),
	(261, 36, 6, 34, 45745),
	(262, 36, 7, 34, 45745),
	(263, 36, 8, 34, 45745),
	(264, 36, 9, 34, 45745),
	(265, 36, 10, 34, 45745),
	(266, 36, 11, 34, 45745),
	(267, 37, 1, 35, 45745),
	(268, 37, 2, 35, 45745),
	(269, 37, 3, 35, 45745),
	(270, 37, 4, 35, 45745),
	(271, 37, 5, 35, 45745),
	(272, 37, 6, 35, 45745),
	(273, 37, 7, 35, 45745),
	(274, 37, 8, 35, 45745),
	(275, 37, 9, 35, 45745),
	(276, 37, 10, 35, 45745),
	(277, 37, 11, 35, 45745),
	(278, 52, 13, 70, 45745),
	(279, 52, 14, 70, 45745),
	(280, 70, 13, 52, 45745),
	(281, 70, 14, 52, 45745),
	(282, 75, 14, 76, 45745),
	(283, 76, 14, 75, 45745);

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=1;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 1, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=2;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 2, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=3;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 3, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=4;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 4, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=5;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 5, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=6;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 6, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=7;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 7, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=8;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 8, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=9;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 9, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=10;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 10, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=11;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 11, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=12;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 12, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=13;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 13, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=14;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 14, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=15;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 15, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=16;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 16, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=17;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 17, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=18;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 18, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=19;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 19, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=20;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 20, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=21;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 21, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=22;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 22, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=23;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 23, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=24;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 24, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=25;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 25, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=26;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 26, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=27;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 27, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=28;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 28, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=29;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 29, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=30;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 30, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=31;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 31, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=32;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 32, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=33;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 33, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=34;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 34, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=35;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 35, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=36;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 36, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=37;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 37, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=38;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 38, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=39;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 39, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=40;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 40, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=41;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 41, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=42;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 42, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=43;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 43, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=44;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 44, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=45;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 45, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=46;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 46, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=47;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 47, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=48;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 48, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=49;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 49, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=50;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 50, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=51;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 51, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=52;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 52, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=53;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 53, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=54;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 54, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=55;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 55, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=56;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 56, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=57;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 57, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=58;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 58, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=59;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 59, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=60;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 60, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=61;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 61, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=62;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 62, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=63;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 63, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=64;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 64, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=65;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 65, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=66;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 66, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=67;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 67, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=68;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 68, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=69;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 69, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=70;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 70, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=71;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 71, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=72;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 72, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=73;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 73, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=74;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 74, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=75;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 75, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=76;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 76, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=77;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 77, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=78;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 78, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=79;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 79, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=80;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 80, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=81;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 81, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=82;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 82, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=83;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 83, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=84;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 84, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=85;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 85, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=86;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 86, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=87;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 87, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=88;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 88, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=89;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 89, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=90;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 90, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=91;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 91, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=92;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 92, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=93;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 93, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=94;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 94, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=95;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 95, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=96;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 96, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=97;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 97, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=98;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 98, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=99;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 99, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=100;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 100, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=101;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 101, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=102;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 102, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=103;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 103, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=104;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 104, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=105;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 105, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=106;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 106, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=107;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 107, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=108;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 108, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=109;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 109, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=110;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 110, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=111;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 111, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=112;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 112, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=113;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 113, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=114;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 114, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=115;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 115, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=116;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 116, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=117;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 117, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=118;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 118, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=119;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 119, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=120;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 120, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=121;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 121, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=122;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 122, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=123;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 123, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=124;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 124, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=125;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 125, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=126;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 126, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=127;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 127, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=128;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 128, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=129;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 129, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=130;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 130, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=131;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 131, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=132;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 132, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=133;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 133, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=134;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 134, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=135;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 135, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=136;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 136, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=137;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 137, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=138;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 138, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=139;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 139, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=140;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 140, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=141;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 141, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=142;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 142, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=143;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 143, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=144;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 144, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=145;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 145, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=146;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 146, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=147;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 147, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=148;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 148, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=149;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 149, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=150;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 150, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=151;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 151, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=152;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 152, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=153;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 153, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=154;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 154, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=155;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 155, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=156;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 156, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=157;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 157, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=158;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 158, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=159;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 159, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=160;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 160, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=161;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 161, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=162;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 162, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=163;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 163, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=164;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 164, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=165;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 165, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=166;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 166, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=167;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 167, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=168;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 168, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=169;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 169, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=170;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 170, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=171;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 171, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=172;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 172, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=173;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 173, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=174;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 174, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=175;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 175, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=176;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 176, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=177;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 177, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=178;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 178, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=179;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 179, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=180;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 180, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=181;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 181, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=182;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 182, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=183;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 183, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=184;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 184, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=185;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 185, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=186;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 186, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=187;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 187, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=188;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 188, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=189;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 189, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=190;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 190, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=191;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 191, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=192;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 192, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=193;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 193, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=194;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 194, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=195;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 195, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=196;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 196, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=197;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 197, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=198;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 198, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=199;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 199, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=200;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 200, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=201;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 201, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=202;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 202, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=203;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 203, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=204;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 204, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=205;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 205, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=206;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 206, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=207;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 207, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=208;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 208, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=209;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 209, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=210;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 210, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=211;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 211, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=212;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 212, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=213;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 213, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=214;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 214, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=215;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 215, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=216;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 216, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=217;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 217, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=218;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 218, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=219;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 219, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=220;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 220, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=221;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 221, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=222;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 222, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=223;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 223, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=224;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 224, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=225;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 225, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=226;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 226, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=227;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 227, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=228;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 228, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=229;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 229, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=230;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 230, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=231;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 231, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=232;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 232, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=233;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 233, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=234;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 234, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=235;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 235, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=236;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 236, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=237;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 237, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=238;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 238, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=239;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 239, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=240;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 240, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=241;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 241, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=242;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 242, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=243;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 243, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=244;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 244, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=245;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 245, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=246;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 246, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=247;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 247, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=248;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 248, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=249;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 249, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=250;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 250, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=251;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 251, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=252;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 252, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=253;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 253, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=254;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 254, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=255;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 255, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=256;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 256, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=257;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 257, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=258;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 258, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=259;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 259, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=260;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 260, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=261;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 261, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=262;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 262, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=263;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 263, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=264;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 264, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=265;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 265, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=266;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 266, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=267;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 267, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=268;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 268, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=269;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 269, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=270;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 270, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=271;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 271, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=272;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 272, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=273;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 273, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=274;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 274, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=275;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 275, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=276;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 276, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=277;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 277, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=278;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 278, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=279;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 279, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=280;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 280, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=281;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 281, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=282;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 282, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=68943 AND `TableHash`=812099832 AND `RecordId`=283;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (68943, 3734412021, 812099832, 283, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=283;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 283, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=282;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 282, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=281;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 281, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=280;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 280, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=279;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 279, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=278;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 278, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=277;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 277, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=276;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 276, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=275;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 275, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=274;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 274, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=273;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 273, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=272;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 272, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=271;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 271, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=270;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 270, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=269;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 269, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=268;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 268, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=267;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 267, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=266;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 266, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=265;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 265, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=264;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 264, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=263;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 263, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=262;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 262, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=261;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 261, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=260;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 260, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=259;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 259, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=258;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 258, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=257;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 257, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=256;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 256, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=255;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 255, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=254;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 254, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=253;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 253, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=252;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 252, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=251;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 251, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=250;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 250, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=249;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 249, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=248;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 248, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=247;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 247, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=246;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 246, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=245;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 245, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=244;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 244, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=243;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 243, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=242;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 242, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=241;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 241, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=240;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 240, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=239;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 239, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=238;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 238, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=237;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 237, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=236;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 236, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=235;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 235, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=234;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 234, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=233;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 233, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=232;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 232, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=231;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 231, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=230;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 230, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=229;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 229, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=228;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 228, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=227;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 227, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=226;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 226, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=225;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 225, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=224;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 224, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=223;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 223, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=222;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 222, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=221;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 221, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=220;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 220, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=219;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 219, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=218;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 218, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=217;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 217, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=216;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 216, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=215;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 215, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=214;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 214, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=213;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 213, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=212;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 212, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=211;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 211, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=210;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 210, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=209;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 209, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=208;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 208, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=207;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 207, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=206;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 206, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=205;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 205, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=204;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 204, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=203;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 203, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=202;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 202, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=201;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 201, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=200;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 200, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=199;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 199, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=198;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 198, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=197;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 197, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=196;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 196, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=195;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 195, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=194;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 194, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=193;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 193, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=192;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 192, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=191;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 191, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=190;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 190, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=189;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 189, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=188;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 188, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=187;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 187, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=186;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 186, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=185;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 185, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=184;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 184, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=183;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 183, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=182;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 182, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=181;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 181, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=180;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 180, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=179;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 179, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=178;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 178, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=177;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 177, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=176;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 176, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=175;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 175, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=174;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 174, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=173;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 173, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=172;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 172, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=171;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 171, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=170;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 170, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=169;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 169, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=168;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 168, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=167;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 167, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=166;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 166, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=165;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 165, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=164;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 164, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=163;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 163, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=162;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 162, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=161;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 161, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=160;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 160, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=159;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 159, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=158;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 158, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=157;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 157, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=156;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 156, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=155;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 155, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=154;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 154, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=153;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 153, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=152;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 152, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=151;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 151, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=150;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 150, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=149;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 149, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=148;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 148, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=147;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 147, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=146;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 146, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=145;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 145, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=144;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 144, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=143;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 143, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=142;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 142, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=141;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 141, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=140;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 140, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=139;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 139, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=138;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 138, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=137;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 137, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=136;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 136, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=135;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 135, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=134;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 134, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=133;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 133, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=132;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 132, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=131;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 131, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=130;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 130, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=129;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 129, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=128;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 128, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=127;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 127, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=126;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 126, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=125;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 125, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=124;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 124, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=123;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 123, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=122;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 122, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=121;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 121, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=120;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 120, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=119;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 119, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=118;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 118, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=117;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 117, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=116;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 116, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=115;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 115, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=114;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 114, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=113;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 113, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=112;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 112, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=111;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 111, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=110;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 110, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=109;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 109, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=108;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 108, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=107;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 107, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=106;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 106, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=105;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 105, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=104;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 104, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=103;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 103, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=102;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 102, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=101;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 101, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=100;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 100, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=99;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 99, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=98;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 98, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=97;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 97, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=96;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 96, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=95;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 95, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=94;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 94, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=93;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 93, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=92;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 92, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=91;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 91, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=90;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 90, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=89;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 89, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=88;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 88, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=87;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 87, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=86;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 86, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=85;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 85, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=84;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 84, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=83;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 83, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=82;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 82, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=81;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 81, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=80;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 80, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=79;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 79, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=78;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 78, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=77;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 77, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=76;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 76, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=75;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 75, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=74;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 74, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=73;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 73, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=72;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 72, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=71;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 71, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=70;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 70, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=69;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 69, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=68;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 68, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=67;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 67, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=66;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 66, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=65;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 65, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=64;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 64, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=63;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 63, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=62;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 62, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=61;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 61, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=60;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 60, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=59;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 59, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=58;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 58, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=57;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 57, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=56;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 56, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=55;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 55, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=54;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 54, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=53;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 53, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=52;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 52, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=51;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 51, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=50;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 50, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=49;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 49, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=48;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 48, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=47;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 47, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=46;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 46, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=45;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 45, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=44;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 44, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=43;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 43, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=42;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 42, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=41;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 41, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=40;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 40, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=39;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 39, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=38;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 38, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=37;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 37, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=36;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 36, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=35;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 35, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=34;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 34, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=33;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 33, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=32;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 32, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=31;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 31, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=30;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 30, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=29;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 29, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=28;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 28, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=27;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 27, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=26;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 26, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=25;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 25, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=24;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 24, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=23;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 23, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=22;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 22, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=21;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 21, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=20;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 20, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=19;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 19, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=18;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 18, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=17;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 17, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=16;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 16, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=15;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 15, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=14;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 14, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=13;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 13, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=12;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 12, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=11;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 11, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=10;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 10, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=9;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 9, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=8;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 8, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=7;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 7, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=6;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 6, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=5;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 5, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=4;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 4, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=3;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 3, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=2;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 2, 1, 45745);
DELETE FROM `hotfix_data` WHERE `Id`=80000 AND `TableHash`=812099832 AND `RecordId`=1;
INSERT INTO `hotfix_data` (`Id`, `UniqueId`, `TableHash`, `RecordId`, `Status`, `VerifiedBuild`) VALUES (80000, 123456789, 812099832, 1, 1, 45745);
