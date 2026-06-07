DROP TABLE IF EXISTS "AssignedTest" CASCADE;
DROP TABLE IF EXISTS "Results" CASCADE;
DROP TABLE IF EXISTS "QuestionOptions" CASCADE;
DROP TABLE IF EXISTS "QuestionTests" CASCADE;
DROP TABLE IF EXISTS "Tests" CASCADE;
DROP TABLE IF EXISTS "Books" CASCADE;
DROP TABLE IF EXISTS "Authors" CASCADE;
DROP TABLE IF EXISTS "Students" CASCADE;
DROP TABLE IF EXISTS "Teachers" CASCADE;
DROP TABLE IF EXISTS "Users" CASCADE;
DROP TABLE IF EXISTS "__EFMigrationsHistory" CASCADE;

CREATE TABLE "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Login" TEXT NOT NULL,
    "Password" TEXT NOT NULL,
    "Role" INTEGER NOT NULL
);

CREATE TABLE "Authors" (
    "Id" SERIAL PRIMARY KEY,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "SecondName" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "ImagePath" TEXT,
    "DateOfBirth" TIMESTAMP  NOT NULL,
    "DateOfDeath" TIMESTAMP 
);

CREATE TABLE "Books" (
    "Id" SERIAL PRIMARY KEY,
    "AuthorID" INTEGER NOT NULL REFERENCES "Authors"("Id") ON DELETE CASCADE,
    "Genre" TEXT NOT NULL,
    "Title" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "InStock" BOOLEAN NOT NULL,
    "TotalRate" REAL NOT NULL,
    "Count" INTEGER NOT NULL,
    "ImagePath" TEXT
);

CREATE TABLE "Tests" (
    "Id" SERIAL PRIMARY KEY,
    "BookId" INTEGER NOT NULL REFERENCES "Books"("Id") ON DELETE CASCADE,
    "TestName" TEXT NOT NULL,
    "TestDescription" TEXT NOT NULL,
    "UserId" INTEGER REFERENCES "Users"("Id")
);

CREATE TABLE "AssignedTest" (
    "Id" SERIAL PRIMARY KEY,
    "StudentId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "TeacherId" INTEGER REFERENCES "Users"("Id"),
    "TestId" INTEGER NOT NULL REFERENCES "Tests"("Id") ON DELETE CASCADE,
    "AssignedAt" TIMESTAMP  NOT NULL,
    "DueDate" TIMESTAMP,
    "IsCompleted" BOOLEAN NOT NULL
);

CREATE TABLE "Students" (
    "StudentId" SERIAL PRIMARY KEY,
    "UserID" INTEGER NOT NULL UNIQUE REFERENCES "Users"("Id") ON DELETE CASCADE,
    "FirstName" TEXT NOT NULL,
    "SecondName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "ClassNum" TEXT NOT NULL,
    "IsProfileComplete" BOOLEAN NOT NULL
);

CREATE TABLE "Teachers" (
    "TeacherId" SERIAL PRIMARY KEY,
    "UserID" INTEGER NOT NULL UNIQUE REFERENCES "Users"("Id") ON DELETE CASCADE,
    "FirstName" TEXT NOT NULL,
    "SecondName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "Contact" TEXT NOT NULL,
    "IsProfileComplete" BOOLEAN NOT NULL
);

CREATE TABLE "QuestionTests" (
    "Id" SERIAL PRIMARY KEY,
    "TestId" INTEGER NOT NULL REFERENCES "Tests"("Id") ON DELETE CASCADE,
    "Number" INTEGER NOT NULL,
    "Text" TEXT NOT NULL,
    "CorrectAnswer" INTEGER NOT NULL,
    "Explanation" TEXT
);

CREATE TABLE "QuestionOptions" (
    "Id" SERIAL PRIMARY KEY,
    "QuestionTestId" INTEGER NOT NULL REFERENCES "QuestionTests"("Id") ON DELETE CASCADE,
    "Text" TEXT NOT NULL,
    "Order" INTEGER NOT NULL
);

CREATE TABLE "Results" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "TestId" INTEGER NOT NULL REFERENCES "Tests"("Id") ON DELETE CASCADE,
    "PercentSuccess" REAL NOT NULL,
    "CreatedAt" TIMESTAMP  NOT NULL
);

CREATE TABLE "__EFMigrationsHistory" (
    "MigrationId" TEXT PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

-- Индексы
CREATE INDEX "IX_AssignedTest_StudentId" ON "AssignedTest" ("StudentId");
CREATE INDEX "IX_AssignedTest_TeacherId" ON "AssignedTest" ("TeacherId");
CREATE INDEX "IX_AssignedTest_TestId" ON "AssignedTest" ("TestId");
CREATE INDEX "IX_Books_AuthorID" ON "Books" ("AuthorID");
CREATE INDEX "IX_Tests_BookId" ON "Tests" ("BookId");
CREATE INDEX "IX_QuestionTests_TestId" ON "QuestionTests" ("TestId");
CREATE INDEX "IX_QuestionOptions_QuestionTestId" ON "QuestionOptions" ("QuestionTestId");
CREATE INDEX "IX_Results_UserId" ON "Results" ("UserId");
CREATE INDEX "IX_Results_TestId" ON "Results" ("TestId");
CREATE INDEX "IX_Students_UserID" ON "Students" ("UserID");
CREATE INDEX "IX_Teachers_UserID" ON "Teachers" ("UserID");

BEGIN TRANSACTION;


INSERT INTO "Users" ("Login", "Password", "Role") VALUES 
('admin', '$2a$11$EXxLswf6BTgsBId69GCHIOiL2Vch3KRg/aZbNn6s1oT9WiHKa1uqG', 0),
('student', '$2a$11$4UL3DgY9UbaC24YVlTkcVe9UTzLzOygAKajN8qwJrw7hX3LwZIVdC', 2),
('teacher', '$2a$11$vZEkSyctYA3FIv5wvvRwwOChtj1Mv4b.L2gOF.yJAykzZ9rJaAVh.', 1),
('teacher1', '$2a$11$yPmPzsSxuUVTWSKq3KCeOuW8rDmDaqQUgmMcIusjtA8i20HlGe5Jy', 1),
('TEACHER1', '$2a$11$MHqb1fSJwjfpH4.INbU3uOt1Ox4pWpjJOINTwuAzP5bra7buuBXTi', 1),
('student123', '$2a$11$L/sz9zhiS3NHQXQc01C0JOK3jNpGSMPDrJARcYfee6Ziy.bV0CWFu', 2),
('student2', '$2a$11$yFgnDPbp12J85qXj2CuuseP0of7pM44L0fb.tU.b7TTAiBp5sLMcq', 2);

INSERT INTO "Authors" ("FirstName", "LastName", "SecondName", "Description", "ImagePath", "DateOfBirth", "DateOfDeath") VALUES 
('Александр', 'Пушкин', 'Сергеевич', 'Русский поэт, драматург и прозаик, основоположник современного русского литературного языка.', '/resources/author/1/41eabb35-110e-4a23-aee1-23552a746f96.jpg', '1799-06-06', '1837-02-10'),
('Лев', 'Толстой', 'Николаевич', 'Русский писатель, автор романов "Война и мир" и "Анна Каренина".', '/resources/author/2/5d24464a-e3f3-469b-ae75-ed63f69a08ab.jpg', '1828-09-09', '1910-11-20'),
('Фёдор', 'Достоевский', 'Михайлович', 'Русский писатель, автор романов "Преступление и наказание" и "Братья Карамазовы".', '/resources/author/3/cc5e0639-f4aa-41e6-a59c-f58a245c3349.jpg', '1821-11-11', '1881-02-09'),
('Антон', 'Чехов', 'Павлович', 'Русский писатель, драматург, автор рассказов и пьес.', '/resources/author/4/6142e2c1-7c9b-4b94-bc63-de7d02ef32e0.jpg', '1860-01-29', '1904-07-15'),
('Николай', 'Гоголь', 'Васильевич', 'Русский писатель украинского происхождения, автор "Мёртвых душ".', '/resources/author/5/701d7936-1625-4abb-b38e-920ae923d883.jpg', '1809-04-01', '1852-03-04'),
('Иван', 'Тургенев', 'Сергеевич', 'Русский писатель, автор романа "Отцы и дети".', NULL, '1818-11-09', '1883-09-03'),
('Михаил', 'Лермонтов', 'Юрьевич', 'Русский поэт и писатель, автор романа "Герой нашего времени".', NULL, '1814-10-15', '1841-07-27'),
('Иван', 'Бунин', 'Алексеевич', 'Русский писатель и поэт, лауреат Нобелевской премии по литературе.', NULL, '1870-10-22', '1953-11-08'),
('Максим', 'Горький', 'Алексеевич', 'Русский писатель, драматург и общественный деятель.', NULL, '1868-03-28', '1936-06-18'),
('Владимир', 'Набоков', 'Владимирович', 'Русский и американский писатель, автор романа "Лолита".', NULL, '1899-04-22', '1977-07-02');

-- 3. Books
INSERT INTO "Books" ("AuthorID", "Genre", "Title", "Description", "InStock", "TotalRate", "Count", "ImagePath") VALUES 
(1, 'Поэзия', 'Евгений Онегин', 'Роман в стихах, одно из самых известных произведений Александра Пушкина.', true, 5, 10, '/resources/book/1/image.jpg'),
(1, 'Сказка', 'Капитанская дочка', 'Исторический роман о событиях восстания Пугачёва.', true, 4, 9, '/resources/book/2/image.jpg'),
(2, 'Роман', 'Война и мир', 'Эпический роман о судьбах людей на фоне Отечественной войны 1812 года.', true, 5, 12, '/resources/book/3/image.jpg'),
(2, 'Роман', 'Анна Каренина', 'Психологический роман о любви, семье и обществе.', true, 5, 11, '/resources/book/4/image.jpg'),
(3, 'Роман', 'Преступление и наказание', 'Философский роман о нравственном выборе и раскаянии.', true, 5, 14, '/resources/book/5/image.jpg'),
(3, 'Роман', 'Идиот', 'Роман о князе Мышкине и столкновении добра с жестоким миром.', true, 4, 8, NULL),
(4, 'Пьеса', 'Вишнёвый сад', 'Одна из самых известных пьес Чехова.', true, 5, 7, NULL),
(4, 'Повесть', 'Палата № 6', 'Повесть о человеческом страдании и равнодушии.', true, 4, 6, NULL),
(5, 'Поэма', 'Мёртвые души', 'Сатирическое произведение о российском обществе XIX века.', true, 5, 10, NULL),
(5, 'Повесть', 'Шинель', 'Классическая повесть о судьбе маленького человека.', true, 4, 9, NULL),
(6, 'Роман', 'Отцы и дети', 'Роман о конфликте поколений.', true, 5, 10, NULL),
(6, 'Повесть', 'Ася', 'Лирическая повесть о любви.', true, 4, 5, NULL),
(7, 'Роман', 'Герой нашего времени', 'Первый психологический роман в русской литературе.', true, 5, 11, NULL),
(7, 'Поэма', 'Мцыри', 'Романтическая поэма о стремлении к свободе.', true, 4, 7, NULL),
(8, 'Сборник рассказов', 'Тёмные аллеи', 'Сборник рассказов о любви.', true, 5, 8, NULL),
(8, 'Повесть', 'Деревня', 'Повесть о русской деревне начала XX века.', true, 4, 6, NULL),
(9, 'Роман', 'Мать', 'Роман о революционном движении.', true, 4, 7, NULL),
(9, 'Пьеса', 'На дне', 'Драма о жизни обитателей ночлежки.', true, 5, 9, NULL),
(10, 'Роман', 'Лолита', 'Самый известный роман Владимира Набокова.', true, 5, 13, NULL),
(10, 'Роман', 'Дар', 'Автобиографический роман о русском писателе-эмигранте.', true, 4, 8, NULL);

-- 4. Tests (без Id)
INSERT INTO "Tests" ("BookId", "TestName", "TestDescription", "UserId") VALUES 
(1, 'Тест по произведению Евгений Онегин', 'Тест по произведению "Евгений Онегин"', NULL),
(4, 'Тест по произведению Анна Каренина', 'Тест по произведению "Анна Каренина"', NULL),
(8, 'Тест по произведению Палата № 6', 'Тест по произведению "Палата № 6"', NULL),
(7, 'Тест по произведению Вишнёвый сад', 'Тест по произведению "Вишнёвый сад"', NULL);

-- 5. Students
INSERT INTO "Students" ("UserID", "FirstName", "SecondName", "LastName", "ClassNum", "IsProfileComplete") VALUES 
(2, 'Виктория', 'Сивурова', 'Владимировна', '11А', true),
(6, 'fdsfsf', 'fdsfds', 'fdsfsdf', '9А', true),
(7, 'fdsfsd', 'fsdfds', 'fsdfsd', '111', true);

-- 6. Teachers
INSERT INTO "Teachers" ("UserID", "FirstName", "SecondName", "LastName", "Contact", "IsProfileComplete") VALUES 
(3, 'fdsfsd', 'fsdfds', 'fsdfsd', 'fsdfsdfsd', true),
(5, 'GDFGDF', 'GFGDF', 'GFDGDGD', 'FSDFSDFSD', true);

-- 7. QuestionTests (связываем через реальные ID тестов, они будут 1,2,3,4)
INSERT INTO "QuestionTests" ("TestId", "Number", "Text", "CorrectAnswer", "Explanation") VALUES 
(1, 1, 'Кто написал роман «Евгений Онегин»?', 0, '«Евгений Онегин» — знаменитый роман в стихах Александра Пушкина.'),
(1, 2, 'Какой жанр у произведения «Евгений Онегин»?', 1, '«Евгений Онегин» — это роман в стихах.'),
(1, 3, 'Как зовут главную героиню?', 1, 'Татьяна — главная героиня романа.'),
(1, 4, 'Кто был другом Онегина?', 1, 'Ленский — лучший друг Онегина.'),
(1, 5, 'Почему Онегин отверг письмо Татьяны?', 1, 'Онегин не верил в любовь.'),
(1, 6, 'Что произошло в дуэли?', 1, 'В дуэли Онегин убил Ленского.'),
(1, 7, 'Где начинается действие романа?', 2, 'Действие романа начинается в деревне.'),
(1, 8, 'Что сделал Онегин после убийства Ленского?', 1, 'Онегин уехал из города.'),
(1, 9, 'Как называется первая глава?', 1, 'Первая глава называется «Деревня».'),
(1, 10, 'Какой размер использовал Пушкин?', 2, 'Пушкин использовал онемейщий ямб.');

-- 8. QuestionOptions
INSERT INTO "QuestionOptions" ("QuestionTestId", "Text", "Order") VALUES 
(1, 'Александр Пушкин', 0),
(1, 'Лев Толстой', 1),
(1, 'Иван Тургенев', 2),
(1, 'Фёдор Достоевский', 3),
(2, 'Роман', 0),
(2, 'Роман в стихах', 1),
(2, 'Повесть', 2),
(2, 'Поэма', 3),
(3, 'Ольга', 0),
(3, 'Татьяна', 1),
(3, 'Наташа', 2),
(3, 'Мария', 3);

-- 9. AssignedTest
INSERT INTO "AssignedTest" ("StudentId", "TeacherId", "TestId", "AssignedAt", "DueDate", "IsCompleted") VALUES 
(1, NULL, 1, '2026-05-17 12:02:19.839751+00', NULL, true),
(1, 1, 1, '2026-05-17 12:04:10.228634+00', '2026-05-24 12:04:10.217+00', false),
(1, 1, 1, '2026-05-17 12:44:01.82487+00', '2026-05-24 12:44:01.796+00', false),
(2, 3, 1, '2026-05-17 12:47:34.863171+00', '2026-05-24 12:47:34.809+00', true),
(2, 5, 1, '2026-05-18 13:46:37.988272+00', '2026-05-25 13:46:37.977+00', false),
(6, 3, 1, '2026-05-18 13:52:58.277163+00', '2026-05-25 13:52:58.268+00', false),
(2, 3, 2, '2026-05-18 14:14:45.526308+00', '2026-05-25 14:14:45.514+00', true),
(2, 3, 3, '2026-05-18 14:20:57.237368+00', '2026-05-25 14:20:57.228+00', true),
(2, 3, 4, '2026-05-18 14:26:59.393358+00', '2026-05-25 14:26:59.353+00', true),
(7, 3, 1, '2026-05-18 14:34:28.39021+00', '2026-05-25 14:34:28.344+00', true);

-- 10. Results
INSERT INTO "Results" ("UserId", "TestId", "PercentSuccess", "CreatedAt") VALUES 
(2, 1, 0, '2026-05-17 12:02:19.797052+00'),
(2, 1, 0, '2026-05-18 14:00:04.216266+00'),
(2, 1, 0, '2026-05-18 14:09:33.47597+00'),
(2, 2, 0, '2026-05-18 14:17:16.090033+00'),
(2, 1, 0, '2026-05-18 14:18:00.110012+00'),
(2, 3, 0, '2026-05-18 14:21:13.324589+00'),
(2, 4, 0, '2026-05-18 14:27:46.910099+00'),
(7, 1, 50, '2026-05-18 14:35:33.112651+00');

-- 11. __EFMigrationsHistory
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") VALUES ('20260531153418_InitialCreate', '10.0.8');

COMMIT;