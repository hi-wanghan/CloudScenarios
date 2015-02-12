
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/09/2014 10:59:10
-- Generated from EDMX file: C:\Users\hanwan\Documents\Visual Studio 2013\Projects\ArenaTest\ArenaTest\Models\ArenaTestModels.edmx
-- "SQL (40508): USE statement is not supported to switch between databases. Use a new connection to connect to a different Database"
-- Get CREATE TABLE permission denied in database 'master' if I comment off "Use" command
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
--USE [ArenaTest];
--GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_TestCases_0]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TestCases] DROP CONSTRAINT [FK_TestCases_0];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[TestCases]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TestCases];
GO
IF OBJECT_ID(N'[dbo].[TestRuns]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TestRuns];
GO
IF OBJECT_ID(N'[dbo].[Tests]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tests];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'TestCases'
CREATE TABLE [dbo].[TestCases] (
    [TestCaseId] int IDENTITY(1,1) NOT NULL,
    [Metrics] nvarchar(max)  NOT NULL,
    [Method] nvarchar(max)  NOT NULL,
    [TestId] int  NOT NULL
);
GO

-- Creating table 'TestRuns'
CREATE TABLE [dbo].[TestRuns] (
    [TestRunId] int IDENTITY(1,1) NOT NULL,
    [Platform] nvarchar(max)  NOT NULL,
    [TestId] int  NOT NULL
);
GO

-- Creating table 'Tests'
CREATE TABLE [dbo].[Tests] (
    [TestId] int IDENTITY(1,1) NOT NULL,
    [TestName] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [TestCaseId] in table 'TestCases'
ALTER TABLE [dbo].[TestCases]
ADD CONSTRAINT [PK_TestCases]
    PRIMARY KEY CLUSTERED ([TestCaseId] ASC);
GO

-- Creating primary key on [TestRunId] in table 'TestRuns'
ALTER TABLE [dbo].[TestRuns]
ADD CONSTRAINT [PK_TestRuns]
    PRIMARY KEY CLUSTERED ([TestRunId] ASC);
GO

-- Creating primary key on [TestId] in table 'Tests'
ALTER TABLE [dbo].[Tests]
ADD CONSTRAINT [PK_Tests]
    PRIMARY KEY CLUSTERED ([TestId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [TestId] in table 'TestCases'
ALTER TABLE [dbo].[TestCases]
ADD CONSTRAINT [FK_TestCases_0]
    FOREIGN KEY ([TestId])
    REFERENCES [dbo].[Tests]
        ([TestId])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TestCases_0'
CREATE INDEX [IX_FK_TestCases_0]
ON [dbo].[TestCases]
    ([TestId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------