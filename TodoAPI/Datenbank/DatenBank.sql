-- 1. Datenbank anlegen (optional)
DROP DATABASE IF EXISTS TodoDB;
CREATE DATABASE IF NOT EXISTS TodoDB;
USE TodoDB;

-- 2. Tabelle Item anlegen
CREATE TABLE IF NOT EXISTS Item (
  ID INT PRIMARY KEY NOT NULL,
  Name VARCHAR(255),
  Beschreibung VARCHAR(1000),
  Status INT,
  StartDate DATETIME,
  DueDate   DATETIME
);

-- 3. Tabelle AppUser anlegen
CREATE TABLE IF NOT EXISTS AppUser(
    ID INT PRIMARY KEY NOT NULL,
    Name VARCHAR(255),
    Email VARCHAR(255),
    Klasse VARCHAR(255)
);

-- 4. Tabelle AppUserItems anlegen (Zwischentabelle)
CREATE TABLE IF NOT EXISTS AppUserItems(
    AppUserID INT NOT NULL,
    ItemID INT NOT NULL,
    PRIMARY KEY (AppUserID, ItemID), -- Zusammengesetzter Primärschlüssel
    FOREIGN KEY (AppUserID) REFERENCES AppUser(ID),
    FOREIGN KEY (ItemID) REFERENCES Item(ID)
);
INSERT INTO Appuser (ID, Name, Email, Klasse) VALUES
(1, 'Max Mustermann', 'max.mustermann@example.com', '10a'),
(2, 'Erika Musterfrau', 'erika.musterfrau@example.com', '10b'),
(3, 'John Doe', 'john.doe@example.com', '11a');

INSERT INTO Item (ID, Name, Beschreibung, StartDate, DueDate, Status) VALUES
(101, 'Hausaufgaben erledigen', 'Mathe-Hausaufgaben für den 25.10.2024', '2024-10-24 16:00:00', '2024-10-25 18:00:00', 0),
(102, 'Projektarbeit', 'Projektarbeit für das Fach Informatik', '2024-10-26 09:00:00', '2024-11-10 16:00:00', 1),
(103, 'Lesen', 'Kapitel 5 für Deutschunterricht lesen', '2024-10-27 14:00:00', '2024-10-28 10:00:00', 0),
(104, 'Vokabeln lernen', 'Englisch Vokabeln für Test am 30.10.2024', '2024-10-28 16:00:00', '2024-10-30 08:00:00', 0);

INSERT INTO AppUserItems (AppUserID, ItemID) VALUES
(1, 101), -- Max hat die Hausaufgaben
(1, 102), -- Max hat die Projektarbeit
(2, 102), -- Erika hat die Projektarbeit
(3, 103), -- John hat den Leseauftrag
(3, 104); -- John hat die Vokabeln

-- Scaffold-DbContext "Server=localhost;Database=TodoDB;Trusted_Connection=true;Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -o Models
-- Scaffold-DbContext "Server=localhost;User=root;Password=;Database=TodoDB" Pomelo.EntityFrameworkCore.MySql -OutputDir Models -f