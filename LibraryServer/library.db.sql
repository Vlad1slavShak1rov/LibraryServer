BEGIN TRANSACTION;

-- Удаляем таблицы (PostgreSQL синтаксис)
DROP TABLE IF EXISTS "AssignedTest" CASCADE;
DROP TABLE IF EXISTS "Authors" CASCADE;
DROP TABLE IF EXISTS "BookReservations" CASCADE;
DROP TABLE IF EXISTS "Books" CASCADE;
DROP TABLE IF EXISTS "EventPhoto" CASCADE;
DROP TABLE IF EXISTS "Events" CASCADE;
DROP TABLE IF EXISTS "ForumMessages" CASCADE;
DROP TABLE IF EXISTS "Forums" CASCADE;
DROP TABLE IF EXISTS "Materials" CASCADE;
DROP TABLE IF EXISTS "QuestionOptions" CASCADE;
DROP TABLE IF EXISTS "QuestionTests" CASCADE;
DROP TABLE IF EXISTS "QuotesBooks" CASCADE;
DROP TABLE IF EXISTS "ResultTests" CASCADE;
DROP TABLE IF EXISTS "Results" CASCADE;
DROP TABLE IF EXISTS "ReviewBooks" CASCADE;
DROP TABLE IF EXISTS "Students" CASCADE;
DROP TABLE IF EXISTS "Teachers" CASCADE;
DROP TABLE IF EXISTS "Tests" CASCADE;
DROP TABLE IF EXISTS "UserBooks" CASCADE;
DROP TABLE IF EXISTS "Users" CASCADE;
DROP TABLE IF EXISTS "__EFMigrationsHistory" CASCADE;
DROP TABLE IF EXISTS "__EFMigrationsLock" CASCADE;

-- Создаем таблицы (PostgreSQL синтаксис - SERIAL вместо AUTOINCREMENT)
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
    "DateOfBirth" TEXT NOT NULL,
    "DateOfDeath" TEXT
);

CREATE TABLE "Books" (
    "Id" SERIAL PRIMARY KEY,
    "AuthorID" INTEGER NOT NULL REFERENCES "Authors"("Id") ON DELETE CASCADE,
    "Genre" TEXT NOT NULL,
    "Title" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "InStock" INTEGER NOT NULL,
    "TotalRate" REAL NOT NULL,
    "Count" INTEGER NOT NULL,
    "ImagePath" TEXT
);

CREATE TABLE "Tests" (
    "Id" SERIAL PRIMARY KEY,
    "BookId" INTEGER NOT NULL REFERENCES "Books"("Id") ON DELETE CASCADE,
    "TestName" TEXT NOT NULL,
    "UserId" INTEGER REFERENCES "Users"("Id"),
    "TestDescription" TEXT NOT NULL DEFAULT ''
);

CREATE TABLE "AssignedTest" (
    "Id" SERIAL PRIMARY KEY,
    "AssignedAt" TEXT NOT NULL,
    "DueDate" TEXT,
    "IsCompleted" INTEGER NOT NULL,
    "StudentId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "TeacherId" INTEGER REFERENCES "Users"("Id"),
    "TestId" INTEGER NOT NULL REFERENCES "Tests"("Id") ON DELETE CASCADE
);

CREATE TABLE "Students" (
    "StudentId" SERIAL PRIMARY KEY,
    "UserID" INTEGER NOT NULL UNIQUE REFERENCES "Users"("Id") ON DELETE CASCADE,
    "FirstName" TEXT NOT NULL,
    "SecondName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "ClassNum" TEXT NOT NULL,
    "IsProfileComplete" INTEGER NOT NULL
);

CREATE TABLE "Teachers" (
    "TeacherId" SERIAL PRIMARY KEY,
    "UserID" INTEGER NOT NULL UNIQUE REFERENCES "Users"("Id") ON DELETE CASCADE,
    "FirstName" TEXT NOT NULL,
    "SecondName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "Contact" TEXT NOT NULL,
    "IsProfileComplete" INTEGER NOT NULL
);

CREATE TABLE "BookReservations" (
    "Id" SERIAL PRIMARY KEY,
    "BookId" INTEGER NOT NULL REFERENCES "Books"("Id") ON DELETE CASCADE,
    "BookingStatus" INTEGER NOT NULL,
    "EndReservation" TEXT NOT NULL,
    "RentStatus" INTEGER,
    "StartReservation" TEXT NOT NULL,
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE
);

CREATE TABLE "Events" (
    "Id" SERIAL PRIMARY KEY,
    "CreaterID" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "StartDate" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "NameEvent" TEXT NOT NULL DEFAULT ''
);

CREATE TABLE "EventPhoto" (
    "Id" SERIAL PRIMARY KEY,
    "EventId" INTEGER NOT NULL REFERENCES "Events"("Id") ON DELETE CASCADE,
    "Path" TEXT NOT NULL
);

CREATE TABLE "Forums" (
    "Id" SERIAL PRIMARY KEY,
    "AdditionalInfo" TEXT NOT NULL,
    "BookId" INTEGER REFERENCES "Books"("Id"),
    "CreaterID" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "DateCreated" TEXT NOT NULL,
    "Title" TEXT NOT NULL
);

CREATE TABLE "ForumMessages" (
    "Id" SERIAL PRIMARY KEY,
    "ForumId" INTEGER NOT NULL REFERENCES "Forums"("Id") ON DELETE CASCADE,
    "SenderId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "Message" TEXT NOT NULL,
    "ApplicationPath" TEXT NOT NULL,
    "DateSend" TEXT NOT NULL
);

CREATE TABLE "Materials" (
    "Id" SERIAL PRIMARY KEY,
    "SenderID" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "Name" TEXT NOT NULL,
    "Subject" INTEGER NOT NULL,
    "Path" TEXT NOT NULL
);

CREATE TABLE "QuestionTests" (
    "Id" SERIAL PRIMARY KEY,
    "CorrectAnswer" INTEGER NOT NULL,
    "Explanation" TEXT,
    "Number" INTEGER NOT NULL,
    "TestId" INTEGER NOT NULL REFERENCES "Tests"("Id") ON DELETE CASCADE,
    "Text" TEXT NOT NULL
);

CREATE TABLE "QuestionOptions" (
    "Id" SERIAL PRIMARY KEY,
    "QuestionTestId" INTEGER NOT NULL REFERENCES "QuestionTests"("Id") ON DELETE CASCADE,
    "Text" TEXT NOT NULL,
    "Order" INTEGER NOT NULL
);

CREATE TABLE "QuotesBooks" (
    "Id" SERIAL PRIMARY KEY,
    "BookId" INTEGER NOT NULL REFERENCES "Books"("Id") ON DELETE CASCADE,
    "Quotes" TEXT NOT NULL
);

CREATE TABLE "Results" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "TestId" INTEGER NOT NULL REFERENCES "Tests"("Id") ON DELETE CASCADE,
    "PercentSuccess" REAL NOT NULL,
    "CreatedAt" TEXT NOT NULL
);

CREATE TABLE "ResultTests" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "TestId" INTEGER NOT NULL REFERENCES "Tests"("Id") ON DELETE CASCADE,
    "Description" TEXT NOT NULL,
    "Score" INTEGER,
    "IsSuccess" INTEGER NOT NULL,
    "TotalQuest" INTEGER NOT NULL,
    "CorrectAnswers" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL
);

CREATE TABLE "ReviewBooks" (
    "Id" SERIAL PRIMARY KEY,
    "BookId" INTEGER NOT NULL REFERENCES "Books"("Id") ON DELETE CASCADE,
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "Message" TEXT NOT NULL,
    "Rate" REAL NOT NULL,
    "DateSend" TEXT NOT NULL
);

CREATE TABLE "UserBooks" (
    "Id" SERIAL PRIMARY KEY,
    "BookId" INTEGER NOT NULL REFERENCES "Books"("Id") ON DELETE CASCADE,
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "IsFavorite" INTEGER NOT NULL
);

CREATE TABLE "__EFMigrationsHistory" (
    "MigrationId" TEXT PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

CREATE TABLE "__EFMigrationsLock" (
    "Id" SERIAL PRIMARY KEY,
    "Timestamp" TEXT NOT NULL
);

-- Создаем индексы
CREATE INDEX "IX_AssignedTest_StudentId" ON "AssignedTest" ("StudentId");
CREATE INDEX "IX_AssignedTest_TeacherId" ON "AssignedTest" ("TeacherId");
CREATE INDEX "IX_AssignedTest_TestId" ON "AssignedTest" ("TestId");
CREATE INDEX "IX_Books_AuthorID" ON "Books" ("AuthorID");
CREATE INDEX "IX_Tests_BookId" ON "Tests" ("BookId");
CREATE INDEX "IX_Students_UserID" ON "Students" ("UserID");
CREATE INDEX "IX_Teachers_UserID" ON "Teachers" ("UserID");
CREATE INDEX "IX_BookReservations_BookId" ON "BookReservations" ("BookId");
CREATE INDEX "IX_BookReservations_UserId" ON "BookReservations" ("UserId");
CREATE INDEX "IX_Events_CreaterID" ON "Events" ("CreaterID");
CREATE INDEX "IX_EventPhoto_EventId" ON "EventPhoto" ("EventId");
CREATE INDEX "IX_Forums_BookId" ON "Forums" ("BookId");
CREATE INDEX "IX_Forums_CreaterID" ON "Forums" ("CreaterID");
CREATE INDEX "IX_ForumMessages_ForumId" ON "ForumMessages" ("ForumId");
CREATE INDEX "IX_ForumMessages_SenderId" ON "ForumMessages" ("SenderId");
CREATE INDEX "IX_Materials_SenderID" ON "Materials" ("SenderID");
CREATE INDEX "IX_QuestionOptions_QuestionTestId" ON "QuestionOptions" ("QuestionTestId");
CREATE INDEX "IX_QuestionTests_TestId" ON "QuestionTests" ("TestId");
CREATE INDEX "IX_QuotesBooks_BookId" ON "QuotesBooks" ("BookId");
CREATE INDEX "IX_Results_TestId" ON "Results" ("TestId");
CREATE INDEX "IX_Results_UserId" ON "Results" ("UserId");
CREATE INDEX "IX_ResultTests_TestId" ON "ResultTests" ("TestId");
CREATE INDEX "IX_ResultTests_UserId" ON "ResultTests" ("UserId");
CREATE INDEX "IX_ReviewBooks_BookId" ON "ReviewBooks" ("BookId");
CREATE INDEX "IX_ReviewBooks_UserId" ON "ReviewBooks" ("UserId");
CREATE INDEX "IX_Tests_UserId" ON "Tests" ("UserId");
CREATE INDEX "IX_UserBooks_BookId" ON "UserBooks" ("BookId");
CREATE INDEX "IX_UserBooks_UserId" ON "UserBooks" ("UserId");

COMMIT;