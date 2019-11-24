
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/24/2019 15:14:01
-- Generated from EDMX file: C:\Users\danie\Desktop\Development\Projects\Shareable-data-generator\Shareable data generator\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ShareableData];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[MainTable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MainTable];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'MainTable'
CREATE TABLE [dbo].[MainTable] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CustomerName] nvarchar(max)  NULL,
    [ExcelName] nvarchar(max)  NULL,
    [ColumnsWidth] nvarchar(max)  NULL,
    [ColumnsName] nvarchar(max)  NULL,
    [SQLstring] nvarchar(max)  NULL,
    [ISYSview] nvarchar(max)  NULL,
    [FolderPath] nvarchar(max)  NULL,
    [FilePath] nvarchar(max)  NULL,
    [ShareableLink] nvarchar(max)  NULL,
    [LastQuery] datetime  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'MainTable'
ALTER TABLE [dbo].[MainTable]
ADD CONSTRAINT [PK_MainTable]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------