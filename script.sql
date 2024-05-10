﻿/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2016 (13.0.4001)
    Source Database Engine Edition : Microsoft SQL Server Express Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Standard Edition
    Target Database Engine Type : Standalone SQL Server
*/
USE []
/****** Object:  Table [dbo].[ApiKeys]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApiKeys](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApiKey] [nvarchar](64) NOT NULL,
	[ClientId] [int] NOT NULL,
	[AmountOfRequests] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Limit] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ApiKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApiLogs]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApiLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [int] NOT NULL,
	[Method] [varchar](10) NOT NULL,
	[Route] [varchar](250) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Clients]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clients](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](320) NOT NULL,
	[Password] [nvarchar](64) NOT NULL,
	[CountryCode] [nvarchar](5) NOT NULL,
	[ContactNumber] [nvarchar](10) NOT NULL,
	[Country] [nvarchar](50) NOT NULL,
	[State] [nvarchar](100) NULL,
	[City] [nvarchar](100) NULL,
	[Address] [nvarchar](150) NOT NULL,
	[DateAdded] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VendorId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[Category] [int] NOT NULL,
	[Price] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vendors]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](320) NOT NULL,
	[Password] [nvarchar](64) NOT NULL,
	[CountryCode] [nvarchar](5) NOT NULL,
	[ContactNumber] [nvarchar](10) NOT NULL,
	[Country] [nvarchar](50) NOT NULL,
	[State] [nvarchar](100) NULL,
	[City] [nvarchar](100) NULL,
	[Address] [nvarchar](150) NOT NULL,
	[DateAdded] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ApiKeys] ADD  DEFAULT ((0)) FOR [AmountOfRequests]
GO
ALTER TABLE [dbo].[ApiKeys] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[ApiKeys] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ApiKeys] ADD  DEFAULT ((10000)) FOR [Limit]
GO
ALTER TABLE [dbo].[ApiLogs] ADD  DEFAULT (getdate()) FOR [Timestamp]
GO
ALTER TABLE [dbo].[Clients] ADD  DEFAULT (getdate()) FOR [DateAdded]
GO
ALTER TABLE [dbo].[Vendors] ADD  DEFAULT (getdate()) FOR [DateAdded]
GO
ALTER TABLE [dbo].[ApiKeys]  WITH CHECK ADD FOREIGN KEY([ClientId])
REFERENCES [dbo].[Clients] ([Id])
GO
ALTER TABLE [dbo].[ApiLogs]  WITH CHECK ADD FOREIGN KEY([ClientId])
REFERENCES [dbo].[Clients] ([Id])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([VendorId])
REFERENCES [dbo].[Vendors] ([Id])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD CHECK  (([Category]='Video_Games' OR [Category]='Movies' OR [Category]='Books' OR [Category]='Furniture' OR [Category]='Electronics'))
GO
/****** Object:  StoredProcedure [dbo].[AddApiKey]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddApiKey]
(
	@ApiKey nvarchar(64),
	@ClientId int
)
AS
BEGIN
	INSERT INTO ApiKeys (ApiKey, ClientId)
	VALUES (
		@ApiKey,
		@ClientId
	)
END
GO
/****** Object:  StoredProcedure [dbo].[AddApiRequest]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddApiRequest]
	@ApiKey nvarchar(64),
	@Method varchar(10),
	@Route varchar(250)
AS
BEGIN
	INSERT INTO ApiLogs (ClientId, Method, Route)
	VALUES (
		(SELECT ClientId FROM ApiKeys WHERE ApiKey = @ApiKey),
		@Method,
		@Route
	)
END
GO
/****** Object:  StoredProcedure [dbo].[AddClient]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddClient]
(
	@Name nvarchar(100),
	@Email nvarchar(320),
	@Password nvarchar(64),
	@CountryCode nvarchar(5),
	@ContactNumber nvarchar(10),
	@Address nvarchar(150),
	@Country nvarchar(50),
	@State nvarchar(100),
	@City nvarchar(100)
)
AS
BEGIN
	INSERT INTO Clients (Name, Email, Password, CountryCode, ContactNumber, Address, Country, State, City)
	VALUES 
		(
			@Name, 
			@Email,
			@Password,
			@CountryCode,
			@ContactNumber,
			@Address,
			@Country,
			@State,
			@City
		)
END
GO
/****** Object:  StoredProcedure [dbo].[AddVendor]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddVendor]
(
	@Name nvarchar(100),
	@Email nvarchar(320),
	@Password nvarchar(64),
	@CountryCode nvarchar(5),
	@ContactNumber nvarchar(10),
	@Address nvarchar(150),
	@Country nvarchar(50),
	@State nvarchar(100),
	@City nvarchar(100)
)
AS
BEGIN
	INSERT INTO Vendors (Name, Email, Password, CountryCode, ContactNumber, Address, Country, State, City)
	VALUES 
		(
			@Name, 
			@Email,
			@Password,
			@CountryCode,
			@ContactNumber,
			@Address,
			@Country,
			@State,
			@City
		)
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllVendors]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllVendors]
AS
BEGIN
	SELECT * FROM Vendors 
END
GO
/****** Object:  StoredProcedure [dbo].[GetApiKey]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetApiKey]
(
	@ApiKey NVARCHAR(64),
	@KeyCount int OUTPUT
)
AS
BEGIN
	SELECT @KeyCount = COUNT(ApiKey) FROM ApiKeys WHERE ApiKey = @ApiKey
END
GO
/****** Object:  StoredProcedure [dbo].[GetApiKeyInfo]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetApiKeyInfo]
(
	@ClientId int
)
AS
BEGIN
	SELECT * FROM ApiKeys WHERE ClientId = @ClientId
END
GO
/****** Object:  StoredProcedure [dbo].[GetApiLogsByClientEmail]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetApiLogsByClientEmail]
	@Email nvarchar(320)
AS
BEGIN
	SELECT TOP 100 * FROM ApiLogs WHERE ClientId = (SELECT Id FROM Clients WHERE Email = @Email) ORDER BY Id DESC
END
GO
/****** Object:  StoredProcedure [dbo].[GetClientByEmail]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetClientByEmail]
(
	@Email nvarchar(320)
)
AS
BEGIN
	SELECT * FROM Clients WHERE Email = @Email
END
GO
/****** Object:  StoredProcedure [dbo].[GetClientPasswordByEmail]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetClientPasswordByEmail]
(
	@Email nvarchar(320),
	@Password nvarchar(64) OUTPUT
)
AS
BEGIN
	SELECT @Password = Password FROM Clients WHERE Email = @Email
END
GO
/****** Object:  StoredProcedure [dbo].[GetDistinctCountriesCount]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetDistinctCountriesCount]
	@TotalDistinctCountries int OUTPUT
AS
BEGIN
	SELECT @TotalDistinctCountries = COUNT(DISTINCT(Country)) FROM Clients
END
GO
/****** Object:  StoredProcedure [dbo].[GetTotalApiRequests]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetTotalApiRequests]
	@TotalRequests int OUTPUT
AS
BEGIN
	SELECT @TotalRequests = SUM(AmountOfRequests) FROM ApiKeys
END
GO
/****** Object:  StoredProcedure [dbo].[GetTotalClients]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetTotalClients]
	@TotalClients int OUTPUT
AS
BEGIN
	SELECT @TotalClients = COUNT(Id) FROM Clients
END
GO
/****** Object:  StoredProcedure [dbo].[GetVendorByEmail]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetVendorByEmail]
(
	@Email nvarchar(320),
	@IsExactMatch bit
)
AS
BEGIN
	IF @IsExactMatch = 1
		SELECT * FROM Vendors WHERE Email = @Email
	ELSE
		SELECT * FROM Vendors WHERE Email LIKE @Email
END
GO
/****** Object:  StoredProcedure [dbo].[GetVendorByName]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetVendorByName]
(
	@Name nvarchar(100),
	@IsExactMatch bit
)
AS
BEGIN
	IF @IsExactMatch = 1
		SELECT * FROM Vendors WHERE Name = @Name
	ELSE
		SELECT * FROM Vendors WHERE Name LIKE @Name
END
GO
/****** Object:  StoredProcedure [dbo].[IncrementRequestCount]    Script Date: 05/04/2024 5:22:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[IncrementRequestCount]
	@ApiKey nvarchar(64)
AS
BEGIN
	UPDATE ApiKeys SET AmountOfRequests = AmountOfRequests + 1 WHERE ApiKey = @ApiKey
END
GO
