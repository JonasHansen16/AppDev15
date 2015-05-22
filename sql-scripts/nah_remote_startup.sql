--
-- Database nah_remote STARTUP SCRIPT
-- WILL ERASE ANY AND ALL DATA PRESENT
--

-- --------------------------------------------------------

--
-- Use our database
--

USE nah_remote;

--
-- Drop tables
--

DROP TABLE questionlist;

DROP TABLE answer;

DROP TABLE client;

DROP TABLE form;

DROP TABLE usr;

DROP TABLE questionnaire;

DROP TABLE question;

-- --------------------------------------------------------

--
-- Table question
--

CREATE TABLE question (
  id int IDENTITY(1,1) PRIMARY KEY,
  theme varchar(255) NOT NULL,
  title varchar(255) NOT NULL,
  txt text NOT NULL,
  img image DEFAULT NULL
);


-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel questionnaire
--

CREATE TABLE questionnaire (
  id int IDENTITY(1,1) PRIMARY KEY,
  title varchar(255) NOT NULL,
  descr text NOT NULL,
  intro text NOT NULL,
  active bit DEFAULT NULL
);


-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel user
--

CREATE TABLE usr (
  id int IDENTITY(1,1) PRIMARY KEY,
  name varchar(50) NOT NULL,
  lastname varchar(50) NOT NULL,
  email varchar(255) NOT NULL,
  username varchar(20) NOT NULL,
  pass varchar(255) NOT NULL,
  occupation varchar(255) DEFAULT NULL,
  adm bit,
  active bit,
  denied bit
);


-- --------------------------------------------------------

--
-- Table form
--

CREATE TABLE form (
  id int IDENTITY(1,1) PRIMARY KEY,
  airid int FOREIGN KEY REFERENCES questionnaire(id),
  userid int FOREIGN KEY REFERENCES usr(id),
  memo varchar(100) DEFAULT NULL,
  category varchar(100) DEFAULT NULL,
  relation varchar(100) DEFAULT NULL,
  completed bit,
  checkedreport bit,
  repeats int DEFAULT 0
);


-- --------------------------------------------------------

--
-- Table client
--

CREATE TABLE client (
  id int IDENTITY(1,1) PRIMARY KEY,
  formid int FOREIGN KEY REFERENCES form(id),
  hash varchar(20),
  age int,
  start bit,
  done bit,
  func varchar(255) DEFAULT NULL
);


-- --------------------------------------------------------

--
-- Table answer
--

CREATE TABLE answer (
  id int IDENTITY(1,1) PRIMARY KEY,
  qid int FOREIGN KEY REFERENCES question(id),
  clientid int FOREIGN KEY REFERENCES client(id),
  score int,
  help bit,
  final bit
);


-- --------------------------------------------------------

--
-- table questionlist
--

CREATE TABLE questionlist (
  id int IDENTITY(1,1) PRIMARY KEY,
  airid int FOREIGN KEY REFERENCES questionnaire(id),
  qid int FOREIGN KEY REFERENCES question(id),
  active bit,
);


-- --------------------------------------------------------

--
-- Data for table question
--

INSERT INTO question (theme, title, txt, img) VALUES
('Leren en toepassen van kennis', 'title', 'Iets nieuws leren\r\n(zoals het leren\r\nomgaan m\r\net bijv. een nieuwe\r\nGSM, vaatwasmachine of\r\nafstands', NULL),
('Leren en toepassen van kennis', 'title', 'Zich kunnen concentreren zonder\r\nte worden afgeleid (zoals het\r\nvolgen van een gesprek in een\r\ndrukk', NULL),
('Algemene taken en activiteiten', 'title', 'Uitvoeren van dagelijkse\r\nroutinehandelingen (zoals zich\r\nwassen, ontbijten', NULL);


-- --------------------------------------------------------

--
-- Data for table questionnaire
--

INSERT INTO questionnaire (title, descr, intro, active) VALUES
('Functioneringsinstrument', 'Meetinstrument voor het functioneren van personen met NAH op vlak van activiteiten en participatie\r\n', 'introduction', 1);


-- --------------------------------------------------------