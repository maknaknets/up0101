IF DB_ID(N'DB_Cargo') IS NULL
    CREATE DATABASE DB_Cargo;
GO

USE DB_Cargo;
GO

CREATE TABLE Users (
    ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Login NVARCHAR(30) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    Role NVARCHAR(10) NOT NULL CHECK (Role IN (N'User', N'Admin')),
    FIO NVARCHAR(100) NOT NULL,
    Photo NVARCHAR(200) NULL
);
GO

CREATE TABLE CargoType (
    ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE Shipment (
    ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CargoTypeID INT NOT NULL,
    UserID INT NOT NULL,
    ShipDate DATE NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Weight INT NOT NULL CHECK (Weight BETWEEN 1 AND 40000),
    Price INT NOT NULL CHECK (Price BETWEEN 100 AND 10000000),
    CONSTRAINT FK_Shipment_CargoType FOREIGN KEY (CargoTypeID) REFERENCES CargoType(ID) ON DELETE CASCADE,
    CONSTRAINT FK_Shipment_Users FOREIGN KEY (UserID) REFERENCES Users(ID) ON DELETE CASCADE
);
GO

INSERT INTO Users (Login, Password, Role, FIO, Photo) VALUES
(N'admin',   N'3B612C75A7B5048A435FB6EC81E52FF92D6D795A8B5A9C17070F6A63C97A53B2', N'Admin', N'Орлов Сергей Викторович', NULL),
(N'ivanov',  N'A61A8ADF60038792A2CB88E670B20540A9D6C2CA204AB754FC768950E79E7D36', N'User',  N'Иванов Алексей Петрович', NULL),
(N'petrova', N'A61A8ADF60038792A2CB88E670B20540A9D6C2CA204AB754FC768950E79E7D36', N'User',  N'Петрова Мария Сергеевна', NULL),
(N'sidorov', N'A61A8ADF60038792A2CB88E670B20540A9D6C2CA204AB754FC768950E79E7D36', N'User',  N'Сидоров Дмитрий Андреевич', NULL);
GO

INSERT INTO CargoType (Name) VALUES
(N'Продукты питания'),
(N'Строительные материалы'),
(N'Хрупкие грузы'),
(N'Опасные грузы'),
(N'Негабаритные грузы');
GO

INSERT INTO Shipment (CargoTypeID, UserID, ShipDate, Name, Weight, Price) VALUES
(1, 2, '2026-01-14', N'Молочная продукция', 3500, 18500),
(1, 2, '2026-02-03', N'Овощи и фрукты', 5200, 24000),
(2, 2, '2026-03-21', N'Цемент М500', 12000, 46000),
(3, 2, '2026-05-10', N'Стеклянная посуда', 800, 15500),
(2, 3, '2026-02-18', N'Кирпич облицовочный', 20000, 72000),
(2, 3, '2026-04-02', N'Пиломатериалы', 9500, 38000),
(4, 3, '2026-06-15', N'Лакокрасочные материалы', 2100, 29500),
(1, 3, '2026-08-09', N'Мясные полуфабрикаты', 4300, 31000),
(5, 4, '2026-03-30', N'Строительный кран (разобранный)', 18000, 125000),
(3, 4, '2026-07-12', N'Офисная техника', 1200, 22000),
(4, 4, '2026-09-05', N'Бытовая химия', 1700, 19800),
(1, 4, '2026-10-01', N'Замороженная рыба', 6000, 41500),
(3, 1, '2026-09-20', N'Витрины стеклянные', 2400, 33000),
(2, 1, '2026-10-10', N'Плитка керамическая', 7800, 27500);
GO
