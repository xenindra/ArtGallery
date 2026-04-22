-- ================================================================
--  ArtGalleryDB — скрипт создания и заполнения базы данных
--  Выполнить в SQL Server Management Studio или через Visual Studio
-- ================================================================

-- 1. Создаём базу данных
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ArtGalleryDB')
BEGIN
    CREATE DATABASE ArtGalleryDB;
END
GO

USE ArtGalleryDB;
GO

-- 2. Таблица картин
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Paintings')
BEGIN
CREATE TABLE Paintings (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Title       NVARCHAR(255)  NOT NULL,
    Author      NVARCHAR(255)  NOT NULL,
    Style       NVARCHAR(100)  NOT NULL,
    Year        INT            NOT NULL,
    Country     NVARCHAR(100)  NOT NULL,
    Materials   NVARCHAR(255)  NOT NULL DEFAULT '',
    Size        NVARCHAR(100)  NOT NULL DEFAULT '',
    ImageUrl    NVARCHAR(500)  NOT NULL DEFAULT '/images/paintings/placeholder.jpg',
    Description NVARCHAR(MAX)  NOT NULL DEFAULT ''
);
END
GO

-- 3. Таблица авторов
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Authors')
BEGIN
CREATE TABLE Authors (
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    Name     NVARCHAR(255) NOT NULL,
    Country  NVARCHAR(100) NOT NULL DEFAULT '',
    Years    NVARCHAR(50)  NOT NULL DEFAULT '',
    Style    NVARCHAR(100) NOT NULL DEFAULT '',
    Bio      NVARCHAR(MAX) NOT NULL DEFAULT '',
    PhotoUrl NVARCHAR(500) NOT NULL DEFAULT '/images/authors/placeholder.jpg'
);
END
GO

-- 4. Таблица избранного
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Favorites')
BEGIN
CREATE TABLE Favorites (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    PaintingId  INT           NOT NULL,
    SessionId   NVARCHAR(100) NOT NULL,
    CreatedAt   DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Favorites UNIQUE (PaintingId, SessionId),
    CONSTRAINT FK_Favorites_Paintings FOREIGN KEY (PaintingId) REFERENCES Paintings(Id)
);
END
GO

-- 5. Таблица для EF миграций (создаётся автоматически, но на всякий случай)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
CREATE TABLE __EFMigrationsHistory (
    MigrationId    NVARCHAR(150) NOT NULL PRIMARY KEY,
    ProductVersion NVARCHAR(32)  NOT NULL
);
END
GO

-- 6. Заполняем картины
IF NOT EXISTS (SELECT TOP 1 1 FROM Paintings)
BEGIN
INSERT INTO Paintings (Title, Author, Style, Year, Country, Materials, Size, ImageUrl, Description) VALUES
(N'Постоянство памяти',              N'Сальвадор Дали',       N'Сюрреализм',            1931, N'Испания',    N'Масло, холст',           N'24 × 33 см',       N'/images/paintings/collection1.jpg',      N'"Постоянство памяти" — одна из самых известных работ Дали. Плавящиеся часы — символ сюрреализма и размышление о гибкости времени.'),
(N'Утро в сосновом лесу',            N'Иван Шишкин',          N'Реализм',               1889, N'Россия',     N'Масло, холст',           N'139 × 213 см',     N'/images/paintings/collection2.jpg',      N'Одно из самых известных произведений Шишкина. Художник изобразил утренний лес. Медведи дописаны Константином Савицким.'),
(N'Звёздная ночь',                   N'Винсент Ван Гог',      N'Постимпрессионизм',     1889, N'Нидерланды', N'Масло, холст',           N'73.7 × 92.1 см',   N'/images/paintings/painting3.jpg',        N'Одна из самых узнаваемых картин Ван Гога. Ночной пейзаж с динамичными облаками и яркими звёздами.'),
(N'Девочка с персиками',             N'Валентин Серов',       N'Реализм',               1887, N'Россия',     N'Масло, холст',           N'91 × 85 см',       N'/images/paintings/painting4.jpg',        N'Знаменитая картина Серова, написанная в усадьбе Абрамцево. На полотне — Вера Мамонтова. Эталон русского реализма.'),
(N'Герника',                         N'Пабло Пикассо',        N'Кубизм',                1937, N'Испания',    N'Масло, холст',           N'349 × 776 см',     N'/images/paintings/painting5.jpg',        N'Монументальное полотно Пикассо, созданное в ответ на бомбардировку Герники. Символ протеста против войны.'),
(N'Впечатление. Восходящее солнце',  N'Клод Моне',            N'Импрессионизм',         1872, N'Франция',    N'Масло, холст',           N'48 × 63 см',       N'/images/paintings/painting6.jpg',        N'Картина Моне, давшая название импрессионизму. Художник изобразил гавань Гавра в утреннем тумане.'),
(N'Чёрный квадрат',                  N'Казимир Малевич',      N'Супрематизм',           1915, N'Россия',     N'Масло, холст',           N'79.5 × 79.5 см',   N'/images/paintings/painting7.jpg',        N'Самая известная работа Малевича, икона супрематизма и русского авангарда.'),
(N'Крик',                            N'Эдвард Мунк',          N'Экспрессионизм',        1893, N'Норвегия',   N'Масло, темпера, картон', N'91 × 73.5 см',     N'/images/paintings/painting8.jpg',        N'Знаменитая картина Мунка, выражающая экзистенциальный ужас современного человека.'),
(N'Мона Лиза',                       N'Леонардо да Винчи',    N'Возрождение',           1503, N'Италия',     N'Масло, тополь',          N'77 × 53 см',       N'/images/paintings/painting9.jpg',        N'Всемирно известный портрет Леонардо да Винчи, эталон портретной живописи эпохи Возрождения.'),
(N'Подсолнухи',                      N'Винсент Ван Гог',      N'Постимпрессионизм',     1888, N'Нидерланды', N'Масло, холст',           N'92 × 73 см',       N'/images/paintings/painting10.jpg',       N'Серия картин Ван Гога с подсолнухами в вазе. Яркие цвета и эмоциональность.'),
(N'Авиньонские девицы',              N'Пабло Пикассо',        N'Кубизм',                1907, N'Испания',    N'Масло, холст',           N'243.9 × 233.7 см', N'/images/paintings/painting11.jpg',       N'Новаторская работа Пикассо, предтеча кубизма. Радикальный разрыв с традиционной перспективой.'),
(N'Бурлаки на Волге',                N'Илья Репин',           N'Реализм',               1873, N'Россия',     N'Масло, холст',           N'131.5 × 281 см',   N'/images/paintings/painting12.jpg',       N'Знаменитая картина Репина. Символ социальной несправедливости.'),
(N'Буги-Вуги',                       N'Джордж Кондо',         N'Поп-арт',               2018, N'США',        N'Акрил, холст',           N'203 × 183 см',     N'/images/paintings/condo_boogie.jpg',     N'Работа Кондо в стиле искусственный реализм. Сочетает гротескные формы с поп-артом.'),
(N'Цветы 727-727',                   N'Такаси Мураками',      N'Поп-арт',               2022, N'Япония',     N'Акрил, холст',           N'100 × 100 см',     N'/images/paintings/murakami_flowers.jpg', N'Работа Мураками в стиле суперфлат. Японская эстетика и современная поп-культура.'),
(N'Венеция. Палаццо',                N'Валерий Кошляков',     N'Современное искусство', 2019, N'Россия',     N'Картон, темпера',        N'120 × 80 см',      N'/images/paintings/koshlyakov_venice.jpg',N'Работа Кошлякова. Нетрадиционные материалы для монументальных изображений архитектуры.');
END
GO

-- 7. Заполняем авторов
IF NOT EXISTS (SELECT TOP 1 1 FROM Authors)
BEGIN
INSERT INTO Authors (Name, Country, Years, Style, Bio, PhotoUrl) VALUES
(N'Сальвадор Дали',     N'Испания',    N'1904–1989', N'Сюрреализм',            N'Испанский художник-сюрреалист, один из самых известных представителей этого направления. Его работы отличаются фантастическими образами и виртуозной живописной техникой.',                              N'/images/authors/dali_author.jpg'),
(N'Иван Шишкин',        N'Россия',     N'1832–1898', N'Реализм',               N'Русский художник-пейзажист, академик. Один из крупнейших мастеров реалистической пейзажной живописи.',                                                                                                   N'/images/authors/shishkin_author.jpg'),
(N'Винсент Ван Гог',    N'Нидерланды', N'1853–1890', N'Постимпрессионизм',     N'Нидерландский художник-постимпрессионист. За свою жизнь создал более 2000 произведений. Его экспрессивная манера оказала огромное влияние на западное искусство.',                                      N'/images/authors/vangogh_author.jpg'),
(N'Валентин Серов',     N'Россия',     N'1865–1911', N'Реализм',               N'Русский живописец и график, мастер портрета. Один из крупнейших художников рубежа XIX–XX веков.',                                                                                                        N'/images/authors/serov_author.jpg'),
(N'Пабло Пикассо',      N'Испания',    N'1881–1973', N'Кубизм',                N'Испанский художник, скульптор, график. Основатель кубизма. Один из самых известных художников XX века. Создал более 20 000 произведений.',                                                              N'/images/authors/picasso_author.jpg'),
(N'Клод Моне',          N'Франция',    N'1840–1926', N'Импрессионизм',         N'Французский живописец, основоположник импрессионизма. Работал преимущественно на пленэре. Создал знаменитые серии «Стога сена», «Руанский собор», «Кувшинки».',                                          N'/images/authors/monet_author.jpg'),
(N'Казимир Малевич',    N'Россия',     N'1879–1935', N'Супрематизм',           N'Русский и советский художник, педагог, теоретик искусства. Основатель супрематизма — одного из наиболее ранних проявлений абстрактного искусства.',                                                     N'/images/authors/malevich_author.jpg'),
(N'Эдвард Мунк',        N'Норвегия',   N'1863–1944', N'Экспрессионизм',        N'Норвежский живописец и график. Один из первых представителей экспрессионизма. Его работы оказали значительное влияние на развитие этого направления.',                                                  N'/images/authors/munch_author.jpg'),
(N'Леонардо да Винчи',  N'Италия',     N'1452–1519', N'Возрождение',           N'Итальянский художник, учёный, изобретатель эпохи Высокого Возрождения. Гений, совместивший в себе художника и учёного.',                                                                                N'/images/authors/davinci_author.jpg'),
(N'Илья Репин',         N'Россия',     N'1844–1930', N'Реализм',               N'Русский живописец, педагог. Один из крупнейших мастеров русского реализма. Создал обширную галерею портретов современников.',                                                                            N'/images/authors/repin_author.jpg'),
(N'Джордж Кондо',       N'США',        N'1957–н.в.', N'Поп-арт',               N'Американский художник, известный стилем «искусственный реализм». Сочетает классические техники с гротескными образами.',                                                                                N'/images/authors/condo_avatar.jpg'),
(N'Такаси Мураками',    N'Япония',     N'1962–н.в.', N'Поп-арт',               N'Японский художник, создатель стиля «суперфлат». Работы сочетают японские мотивы с западной поп-культурой.',                                                                                             N'/images/authors/murakami_avatar.jpg'),
(N'Валерий Кошляков',   N'Россия',     N'1962–н.в.', N'Современное искусство', N'Российский художник, представитель постсоветского искусства. Работает с образами архитектурных памятников, используя нетрадиционные материалы.',                                                       N'/images/authors/koshlyakov_avatar.jpg');
END
GO

PRINT 'База данных ArtGalleryDB успешно создана и заполнена!';
GO
