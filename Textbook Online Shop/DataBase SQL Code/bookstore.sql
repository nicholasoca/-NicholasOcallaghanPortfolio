-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Generation Time: Nov 22, 2023 at 12:00 PM
-- Server version: 8.0.31
-- PHP Version: 8.0.26

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `bookstore`
--

-- --------------------------------------------------------

--
-- Table structure for table `adminusers`
--

DROP TABLE IF EXISTS `adminusers`;
CREATE TABLE IF NOT EXISTS `adminusers` (
  `id` int NOT NULL AUTO_INCREMENT,
  `usernameAdmin` varchar(50) NOT NULL,
  `passwordAdmin` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `books`
--

DROP TABLE IF EXISTS `books`;
CREATE TABLE IF NOT EXISTS `books` (
  `id` int NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  `author` varchar(255) NOT NULL,
  `price` decimal(10,2) NOT NULL,
  `image_path` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `books`
--

INSERT INTO `books` (`id`, `title`, `author`, `price`, `image_path`) VALUES
(1, 'The great', 'F', '200.00', 'images/book1.jpg'),
(2, 'Mockingbird', 'Lee', '100.00', 'images/book2.jpg'),
(3, 'Pride and Prejudice', 'Jane Austen', '99.99', 'images/book3.jpg'),
(4, '1984', 'George Orwell', '450.99', 'images/book4.jpg'),
(5, 'The Catcher in the RYE', 'J.D. Salinger', '139.99', 'images/book5.jpg'),
(6, 'English For Everyone', 'DK', '144.99', 'images/book6.jpg'),
(7, 'Statistical methods & Data Analysis', 'R. Lyman Ott', '749.99', 'images/book7.jpg'),
(8, 'Accounting for all', 'Madri Schutte', '554.99', 'images/book8.jpg'),
(9, 'Essentials of Human Anatomy & Physioly', 'Elaine N. Marieb', '1259.99', 'images/book9.jpg'),
(10, 'Biology', 'Campbell', '1264.99', 'images/book10.jpg'),
(11, 'Engineering Mechanics Statics', 'R.C. Hibbeler', '859.99', 'images/book11.jpg'),
(12, 'Economics', 'Paul Horing', '259.99', 'images/book12.jpg'),
(13, 'Statistics fo management and economics', 'Gerald keller', '359.99', 'images/book13.jpg'),
(14, 'Basic Business Statistics', 'Mark L. Berenson', '459.99', 'images/book14.jpg'),
(15, 'BioChemistry', 'Campbell, Farrel, McDougal', '559.99', 'images/book15.jpg');

-- --------------------------------------------------------

--
-- Table structure for table `cart`
--

DROP TABLE IF EXISTS `cart`;
CREATE TABLE IF NOT EXISTS `cart` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `book_id` int NOT NULL,
  `quantity` int NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `processed` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `user_id` (`user_id`),
  KEY `book_id` (`book_id`)
) ENGINE=MyISAM AUTO_INCREMENT=33 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `cart`
--

INSERT INTO `cart` (`id`, `user_id`, `book_id`, `quantity`, `created_at`, `processed`) VALUES
(11, 2, 2, 1, '2023-11-21 01:17:54', 1),
(12, 2, 1, 1, '2023-11-21 01:26:11', 1),
(13, 2, 2, 1, '2023-11-21 01:26:15', 1),
(14, 2, 3, 1, '2023-11-21 01:26:21', 1),
(15, 2, 2, 1, '2023-11-21 01:42:41', 1),
(16, 1, 5, 1, '2023-11-21 02:55:47', 1),
(17, 1, 12, 1, '2023-11-21 02:55:58', 1),
(18, 1, 2, 1, '2023-11-21 15:50:04', 1),
(19, 1, 4, 1, '2023-11-21 15:50:15', 1),
(20, 1, 4, 1, '2023-11-21 15:50:33', 1),
(21, 1, 5, 1, '2023-11-21 15:50:37', 1),
(24, 1, 1, 1, '2023-11-22 10:49:37', 1),
(23, 1, 2, 1, '2023-11-22 10:49:00', 1),
(25, 1, 3, 1, '2023-11-22 10:49:41', 1),
(27, 4, 2, 1, '2023-11-22 11:05:32', 1),
(28, 4, 3, 1, '2023-11-22 11:05:35', 1),
(29, 2, 1, 1, '2023-11-22 11:07:31', 1),
(31, 5, 3, 1, '2023-11-22 11:36:15', 1),
(32, 5, 4, 1, '2023-11-22 11:36:19', 1);

-- --------------------------------------------------------

--
-- Table structure for table `contact`
--

DROP TABLE IF EXISTS `contact`;
CREATE TABLE IF NOT EXISTS `contact` (
  `id` int NOT NULL AUTO_INCREMENT,
  `email` varchar(255) NOT NULL,
  `submission_date` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `sales`
--

DROP TABLE IF EXISTS `sales`;
CREATE TABLE IF NOT EXISTS `sales` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int DEFAULT NULL,
  `book_id` int DEFAULT NULL,
  `quantity` int DEFAULT NULL,
  `status` varchar(20) DEFAULT 'pending',
  `customer_email` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `user_id` (`user_id`),
  KEY `book_id` (`book_id`)
) ENGINE=MyISAM AUTO_INCREMENT=38 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `sales`
--

INSERT INTO `sales` (`id`, `user_id`, `book_id`, `quantity`, `status`, `customer_email`) VALUES
(36, 5, 3, 1, 'pending', NULL),
(33, 2, 3, 1, 'pending', NULL),
(28, 4, 2, 1, 'pending', NULL),
(34, 2, 2, 1, 'pending', NULL),
(29, 4, 3, 1, 'pending', NULL),
(37, 5, 4, 1, 'pending', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `sellbook`
--

DROP TABLE IF EXISTS `sellbook`;
CREATE TABLE IF NOT EXISTS `sellbook` (
  `id` int NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  `author` varchar(255) NOT NULL,
  `price` decimal(10,2) NOT NULL,
  `book_condition` varchar(20) NOT NULL,
  `description` text,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `sellbook`
--

INSERT INTO `sellbook` (`id`, `title`, `author`, `price`, `book_condition`, `description`) VALUES
(3, '1', '1', '1.00', 'new', '1'),
(4, '1', '1', '1.00', 'like-new', '1');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
CREATE TABLE IF NOT EXISTS `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `email` varchar(255) NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `username`, `password`, `email`, `created_at`) VALUES
(1, 'a@a.co.za', '$2y$10$iTuDspbpyct.LQyfzrSiKuBJjYDSQ/Jyq6JG.7TcNfDozr/qS0cNS', 'a@a.co.za', '2023-11-21 00:37:42'),
(2, 'b@b', '$2y$10$74alYCDZX6bLZXajEKlvdOUIGf4hWNv9pO9zE1wp3Mr7aK5vvCsva', 'b@b', '2023-11-21 01:04:44'),
(4, '1@1', '$2y$10$CE/rAMSms3ifWU6Ehu6GleCDzB3snmYLTqzvpXYrHdRE38dcft.L6', '1@1', '2023-11-22 11:04:57'),
(5, 'd@d', '$2y$10$2rbI9oq2UDnt1Lk3dKaJnOgURsxyCRzJyaavuQMw..TQzLJwJQ8bi', 'd@d', '2023-11-22 11:35:06');
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
