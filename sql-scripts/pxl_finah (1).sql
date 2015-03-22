-- phpMyAdmin SQL Dump
-- version 4.1.14
-- http://www.phpmyadmin.net
--
-- Machine: 127.0.0.1
-- Gegenereerd op: 22 mrt 2015 om 16:12
-- Serverversie: 5.6.17
-- PHP-versie: 5.5.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Databank: `pxl_finah`
--

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `answer`
--

CREATE TABLE IF NOT EXISTS `answer` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `qid` int(11) NOT NULL,
  `clientid` int(11) NOT NULL,
  `score` int(11) DEFAULT NULL,
  `help` tinyint(1) DEFAULT NULL,
  `final` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `qid` (`qid`),
  KEY `clientid` (`clientid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `client`
--

CREATE TABLE IF NOT EXISTS `client` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `start` tinyint(1) NOT NULL,
  `formid` int(11) NOT NULL,
  `age` int(11) DEFAULT NULL,
  `function` varchar(20) DEFAULT NULL,
  `hash` varchar(20) DEFAULT NULL,
  `done` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `formid` (`formid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `form`
--

CREATE TABLE IF NOT EXISTS `form` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `airid` int(11) NOT NULL,
  `userid` int(11) NOT NULL,
  `memo` varchar(100) DEFAULT NULL,
  `category` varchar(20) DEFAULT NULL,
  `relation` varchar(20) DEFAULT NULL,
  `completed` tinyint(1) DEFAULT NULL,
  `checkedreport` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `airid` (`airid`),
  KEY `userid` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `question`
--

CREATE TABLE IF NOT EXISTS `question` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `theme` varchar(50) DEFAULT NULL,
  `title` varchar(20) DEFAULT NULL,
  `text` varchar(100) DEFAULT NULL,
  `image` blob,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=4 ;

--
-- Gegevens worden geëxporteerd voor tabel `question`
--

INSERT INTO `question` (`id`, `theme`, `title`, `text`, `image`) VALUES
(1, 'Leren en toepassen van kennis', 'title', 'Iets nieuws leren\r\n(zoals het leren\r\nomgaan m\r\net bijv. een nieuwe\r\nGSM, vaatwasmachine of\r\nafstands', NULL),
(2, 'Leren en toepassen van kennis', 'title', 'Zich kunnen concentreren zonder\r\nte worden afgeleid (zoals het\r\nvolgen van een gesprek in een\r\ndrukk', NULL),
(3, 'Algemene taken en activiteiten', 'title', 'Uitvoeren van dagelijkse\r\nroutinehandelingen (zoals zich\r\nwassen, ontbijten', NULL);

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `questionlist`
--

CREATE TABLE IF NOT EXISTS `questionlist` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `airid` int(11) NOT NULL,
  `qid` int(11) NOT NULL,
  `enabled` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `airid` (`airid`),
  KEY `qid` (`qid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `questionnaire`
--

CREATE TABLE IF NOT EXISTS `questionnaire` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(30) NOT NULL,
  `description` varchar(100) NOT NULL,
  `intro` varchar(150) NOT NULL,
  `enabled` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=2 ;

--
-- Gegevens worden geëxporteerd voor tabel `questionnaire`
--

INSERT INTO `questionnaire` (`id`, `title`, `description`, `intro`, `enabled`) VALUES
(1, 'Functioneringsinstrument', 'Meetinstrument voor het functioneren van personen met NAH op vlak van activiteiten en participatie\r\n', 'introduction', 1);

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `user`
--

CREATE TABLE IF NOT EXISTS `user` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(20) NOT NULL,
  `lastname` varchar(20) NOT NULL,
  `email` varchar(35) DEFAULT NULL,
  `username` varchar(20) NOT NULL,
  `password` varchar(20) NOT NULL,
  `admin` tinyint(1) NOT NULL,
  `enabled` tinyint(1) NOT NULL,
  `beroep` varchar(25) DEFAULT NULL,
  `denied` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

--
-- Beperkingen voor geëxporteerde tabellen
--

--
-- Beperkingen voor tabel `answer`
--
ALTER TABLE `answer`
  ADD CONSTRAINT `answer_ibfk_1` FOREIGN KEY (`qid`) REFERENCES `question` (`id`),
  ADD CONSTRAINT `answer_ibfk_2` FOREIGN KEY (`clientid`) REFERENCES `client` (`id`);

--
-- Beperkingen voor tabel `client`
--
ALTER TABLE `client`
  ADD CONSTRAINT `client_ibfk_1` FOREIGN KEY (`formid`) REFERENCES `form` (`id`);

--
-- Beperkingen voor tabel `form`
--
ALTER TABLE `form`
  ADD CONSTRAINT `form_ibfk_1` FOREIGN KEY (`airid`) REFERENCES `questionnaire` (`id`),
  ADD CONSTRAINT `form_ibfk_2` FOREIGN KEY (`userid`) REFERENCES `user` (`id`);

--
-- Beperkingen voor tabel `questionlist`
--
ALTER TABLE `questionlist`
  ADD CONSTRAINT `questionlist_ibfk_1` FOREIGN KEY (`airid`) REFERENCES `questionnaire` (`id`),
  ADD CONSTRAINT `questionlist_ibfk_2` FOREIGN KEY (`qid`) REFERENCES `question` (`id`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
