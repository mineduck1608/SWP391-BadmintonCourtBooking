USE [master]
GO
/****** Object:  Database [BadmintonCourt]    Script Date: 7/1/2024 10:58:42 PM ******/
CREATE DATABASE [BadmintonCourt]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BadmintonCourt', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MINHDUC\MSSQL\DATA\BadmintonCourt.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'BadmintonCourt_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MINHDUC\MSSQL\DATA\BadmintonCourt_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [BadmintonCourt] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BadmintonCourt].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BadmintonCourt] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BadmintonCourt] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BadmintonCourt] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BadmintonCourt] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BadmintonCourt] SET ARITHABORT OFF 
GO
ALTER DATABASE [BadmintonCourt] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BadmintonCourt] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BadmintonCourt] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BadmintonCourt] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BadmintonCourt] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BadmintonCourt] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BadmintonCourt] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BadmintonCourt] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BadmintonCourt] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BadmintonCourt] SET  DISABLE_BROKER 
GO
ALTER DATABASE [BadmintonCourt] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BadmintonCourt] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BadmintonCourt] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BadmintonCourt] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BadmintonCourt] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BadmintonCourt] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BadmintonCourt] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BadmintonCourt] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [BadmintonCourt] SET  MULTI_USER 
GO
ALTER DATABASE [BadmintonCourt] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BadmintonCourt] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BadmintonCourt] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BadmintonCourt] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [BadmintonCourt] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [BadmintonCourt] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [BadmintonCourt] SET QUERY_STORE = ON
GO
ALTER DATABASE [BadmintonCourt] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [BadmintonCourt]
GO
/****** Object:  Table [dbo].[BookedSlot]    Script Date: 7/1/2024 10:58:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookedSlot](
	[slotID] [varchar](30) NOT NULL,
	[startTime] [datetime] NOT NULL,
	[endTime] [datetime] NOT NULL,
	[courtID] [varchar](30) NOT NULL,
	[bookingID] [varchar](30) NOT NULL,
	[isDelete] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[slotID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Booking]    Script Date: 7/1/2024 10:58:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Booking](
	[bookingID] [varchar](30) NOT NULL,
	[amount] [float] NOT NULL,
	[bookingType] [int] NOT NULL,
	[userID] [varchar](30) NOT NULL,
	[bookingDate] [datetime] NOT NULL,
	[changeLog] [int] NOT NULL,
	[isDelete] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[bookingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Court]    Script Date: 7/1/2024 10:58:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Court](
	[courtID] [varchar](30) NOT NULL,
	[courtImg] [varchar](3000) NULL,
	[branchID] [varchar](30) NOT NULL,
	[price] [real] NOT NULL,
	[description] [varchar](500) NOT NULL,
	[courtName] [varchar](30) NOT NULL,
	[courtStatus] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[courtID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CourtBranch]    Script Date: 7/1/2024 10:58:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourtBranch](
	[branchID] [varchar](30) NOT NULL,
	[location] [varchar](50) NOT NULL,
	[branchName] [varchar](50) NOT NULL,
	[branchPhone] [varchar](10) NOT NULL,
	[branchImg] [varchar](3000) NULL,
	[branchStatus] [int] NOT NULL,
	[mapUrl] [varchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[branchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Discount]    Script Date: 7/1/2024 10:58:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Discount](
	[discountID] [varchar](30) NOT NULL,
	[amount] [float] NOT NULL,
	[proportion] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[discountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 7/1/2024 10:58:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[feedbackID] [varchar](30) NOT NULL,
	[rating] [int] NOT NULL,
	[content] [nvarchar](500) NOT NULL,
	[userID] [varchar](30) NULL,
	[branchID] [varchar](30) NOT NULL,
	[period] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[feedbackID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 7/1/2024 10:58:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[paymentID] [varchar](30) NOT NULL,
	[userID] [varchar](30) NOT NULL,
	[date] [datetime] NOT NULL,
	[bookingID] [varchar](30) NULL,
	[method] [int] NOT NULL,
	[amount] [float] NOT NULL,
	[transactionId] [varchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[paymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 7/1/2024 10:58:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[roleID] [varchar](30) NOT NULL,
	[role] [varchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[roleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 7/1/2024 10:58:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[userID] [varchar](30) NOT NULL,
	[userName] [varchar](50) NULL,
	[password] [varchar](50) NULL,
	[branchID] [varchar](30) NULL,
	[roleID] [varchar](30) NOT NULL,
	[token] [varchar](1000) NULL,
	[actionPeriod] [datetime] NULL,
	[balance] [float] NULL,
	[accessFail] [int] NULL,
	[lastFail] [datetime] NULL,
	[activeStatus] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[userID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserDetail]    Script Date: 7/1/2024 10:58:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserDetail](
	[userID] [varchar](30) NOT NULL,
	[firstName] [nvarchar](50) NULL,
	[lastName] [nvarchar](50) NULL,
	[email] [varchar](50) NOT NULL,
	[phone] [varchar](10) NULL,
	[facebook] [varchar](50) NULL,
	[img] [varchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[userID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-001', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5cf8e617-6a4f-470b-961d-6c8c33b8ee20?alt=media&token=5ced7887-b864-4b37-9940-b50618c1ecdd', N'BR-002', 40000, N'Come here', N'Sân 1', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-002', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fde67e132-63dc-478b-b5c6-ae49fc73bd76?alt=media&token=5f40ab0d-329c-44a8-b5f1-4a98e0c7481c', N'BR-003', 30000, N'Sân mát lém. Vô dê :)', N'Sân 1', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-003', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fc62f644e-86d1-4a51-b98c-50ce05f2b954?alt=media&token=de82da81-1aa1-4f2d-83ff-2d483e388f74', N'BR-001', 50000, N'...', N'Sân 1', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-004', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Febf5f5ff-f141-40d0-8ce8-ecf0d6280ac3?alt=media&token=01085ac8-aae4-4600-86d4-0a9c86220d9f', N'BR-002', 35000, N'... Hello', N'Sân 3', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-005', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F80827841-ae38-4d79-92c4-00253867b984?alt=media&token=55ea7826-6de2-417b-8fb4-40e918ab24bb', N'BR-001', 25000, N'R? nè', N'Sân 4', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-006', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fc98955fc-bb5d-460b-b393-657716a98622?alt=media&token=2ea91288-1e94-46cd-b54c-f1f1fe7aa21a', N'BR-003', 50000, N'...', N'Sân 2', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-007', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fc20c6db0-26fe-4358-a0ed-ab658d9ecb58?alt=media&token=37d97b2b-38f0-4975-8b83-8172a0196e81', N'BR-002', 20000, N':)', N'Sân 2', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-008', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F65391dd8-9386-403b-ba93-9789ed955af0?alt=media&token=ad99bec1-b515-4028-8dc1-72787d172b9c', N'BR-002', 45000, N':(', N'Sân 4', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-009', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5176c8e5-9bb0-49f9-8388-0ba2f4cfcc19?alt=media&token=6fd4c540-6026-4dfd-bd30-a57b1f9d69b7', N'BR-001', 65000, N':))', N'Sân 2', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-010', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F6384442c-c195-4566-ba1e-84308db0f4d0?alt=media&token=677d1b87-5d91-4815-b6bc-3518e3083c73', N'BR-004', 30000, N':>', N'Sân 2', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-011', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F444c925b-f15d-458e-8fb3-83a403ed5295?alt=media&token=5dcaea48-3237-4d40-a776-273c619bc9f3', N'BR-001', 70000, N'Never gonna give you up...', N'Sân 3', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-012', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5176c8e5-9bb0-49f9-8388-0ba2f4cfcc19?alt=media&token=6fd4c540-6026-4dfd-bd30-a57b1f9d69b7', N'BR-004', 65000, N'Never gonna let you down', N'Sân 0 :)', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-013', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5176c8e5-9bb0-49f9-8388-0ba2f4cfcc19?alt=media&token=6fd4c540-6026-4dfd-bd30-a57b1f9d69b7', N'BR-004', 50000, N'Never gonna run around and desert you', N'Sân 3', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-014', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5176c8e5-9bb0-49f9-8388-0ba2f4cfcc19?alt=media&token=6fd4c540-6026-4dfd-bd30-a57b1f9d69b7', N'BR-003', 40000, N'Never gonna make you cry', N'Sân 4', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-015', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5176c8e5-9bb0-49f9-8388-0ba2f4cfcc19?alt=media&token=6fd4c540-6026-4dfd-bd30-a57b1f9d69b7', N'BR-004', 40000, N'Never gonna say goodbye', N'Sân 3', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-016', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5176c8e5-9bb0-49f9-8388-0ba2f4cfcc19?alt=media&token=6fd4c540-6026-4dfd-bd30-a57b1f9d69b7', N'BR-003', 55000, N'Never gonna tell a lie and hurt you', N'Sân 3', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-017', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5176c8e5-9bb0-49f9-8388-0ba2f4cfcc19?alt=media&token=6fd4c540-6026-4dfd-bd30-a57b1f9d69b7', N'BR-005', 45000, N'.', N'Sân 1', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-018', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5176c8e5-9bb0-49f9-8388-0ba2f4cfcc19?alt=media&token=6fd4c540-6026-4dfd-bd30-a57b1f9d69b7', N'BR-005', 30000, N'..', N'Sân 2', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-019', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5176c8e5-9bb0-49f9-8388-0ba2f4cfcc19?alt=media&token=6fd4c540-6026-4dfd-bd30-a57b1f9d69b7', N'BR-005', 35000, N'...', N'Sân 4', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-020', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5176c8e5-9bb0-49f9-8388-0ba2f4cfcc19?alt=media&token=6fd4c540-6026-4dfd-bd30-a57b1f9d69b7', N'BR-005', 40000, N'....', N'Sân 3', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-021', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5176c8e5-9bb0-49f9-8388-0ba2f4cfcc19?alt=media&token=6fd4c540-6026-4dfd-bd30-a57b1f9d69b7', N'BR-005', 3000, N'1', N'Sân 5', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C022', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files/4027ce3c-9b71-40d0-b6a0-887d1ff129b8?alt=media', N'BR-005', 45000, N'Come hereww', N'Court 6', 0)
GO
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'BR-001', N'S? 101-103-105 Phan Van Trí, P.1, Qu?n 7', N'Hoàng Thiên1', N'0906246912', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fbbfeb04c-ea7b-4ba8-8966-cad3ce3075eb?alt=media&token=c4ccd0ad-2fe7-4fc6-8d1d-2c49d8335f73', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d31359.967418420183!2d106.83023359999999!3d10.7347968!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317523b926e6c675%3A0xa0bb598051ad0fa0!2sThe%20Hammock%20Glamping!5e0!3m2!1sen!2s!4v1719847889642!5m2!1sen!2s')
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'BR-002', N'277 – 279 An Duong Vuong, P.5, Qu?n 7', N'Hoàng Phi', N'0856209864', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F9e7c39b7-9c2a-4957-856b-31a1e0bf134d?alt=media&token=c99ab5c4-359d-4dab-ab09-4eda27a0844e', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d31359.967418420183!2d106.83023359999999!3d10.7347968!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3175239bfefbd913%3A0x6c75af3281d654d4!2sJeongsan%20country%20club!5e0!3m2!1sen!2s!4v1719848421086!5m2!1sen!2s')
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'BR-003', N'S? 3022 Ph?m Th? Hi?n, P.7, Qu?n 8', N'Hành Phi', N'0846412980', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F3d0ffe3b-45af-452b-9a19-087ef6945c3e?alt=media&token=aa6503fa-365b-4aa4-b1aa-60891714457e', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d31359.967418420183!2d106.83023359999999!3d10.7347968!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3175239bfefbd913%3A0x6c75af3281d654d4!2sJeongsan%20country%20club!5e0!3m2!1sen!2s!4v1719848421086!5m2!1sen!2s')
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'BR-004', N'S? 302 Châu Huong, P.5, Qu?n 2', N'Rebell', N'0907410332', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fd9e26227-5f4f-41af-afae-84907ba29788?alt=media&token=c585770b-5658-4104-bb56-e9da970fabab', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d31359.967418420183!2d106.83023359999999!3d10.7347968!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317523b926e6c675%3A0xa0bb598051ad0fa0!2sThe%20Hammock%20Glamping!5e0!3m2!1sen!2s!4v1719847889642!5m2!1sen!2s')
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'BR-005', N'Trung tâm TDTT Qu?n 10, du?ng Thành Thái, Qu?n 10', N'Nguy?n Tri Phuong', N'0935128603', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files/f65b6fba-8e3f-494e-828d-c9e0a4c938bf?alt=media', 0, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d31359.967418420183!2d106.83023359999999!3d10.7347968!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317523b926e6c675%3A0xa0bb598051ad0fa0!2sThe%20Hammock%20Glamping!5e0!3m2!1sen!2s!4v1719847889642!5m2!1sen!2s')
GO
INSERT [dbo].[Discount] ([discountID], [amount], [proportion]) VALUES (N'D-000001', 10000, 0.1)
INSERT [dbo].[Discount] ([discountID], [amount], [proportion]) VALUES (N'D-000002', 30000, 0.2)
INSERT [dbo].[Discount] ([discountID], [amount], [proportion]) VALUES (N'D-000003', 50000, 0.4)
GO
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000004', N'U0000012', CAST(N'2024-06-23T23:41:19.500' AS DateTime), NULL, 1, 5000000, N'14474525')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000005', N'U0000013', CAST(N'2024-06-24T00:23:50.000' AS DateTime), NULL, 1, 10000, N'4067513248')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000006', N'U0000013', CAST(N'2024-06-24T00:29:50.000' AS DateTime), NULL, 1, 5000, N'4067513269')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000007', N'U0000013', CAST(N'2024-06-24T00:49:49.000' AS DateTime), NULL, 1, 5000, N'4067482792')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000008', N'U0000013', CAST(N'2024-06-24T12:53:02.000' AS DateTime), NULL, 1, 5000, N'4067675475')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000009', N'U0000013', CAST(N'2024-06-24T12:54:16.000' AS DateTime), NULL, 1, 5000, N'4067675484')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000010', N'U0000013', CAST(N'2024-06-24T12:55:59.040' AS DateTime), NULL, 1, 400000, N'14475291')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000011', N'U0000013', CAST(N'2024-06-24T12:57:09.393' AS DateTime), NULL, 1, 500000, N'0')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000012', N'U0000013', CAST(N'2024-06-24T12:57:58.370' AS DateTime), NULL, 1, 500000, N'14475295')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000013', N'U0000013', CAST(N'2024-06-24T12:58:35.130' AS DateTime), NULL, 1, 1000000, N'14475296')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000014', N'U0000012', CAST(N'2024-06-24T13:28:19.463' AS DateTime), NULL, 1, 5000000, N'0')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000015', N'U0000013', CAST(N'2024-06-24T14:22:43.000' AS DateTime), NULL, 1, 5000, N'4067693503')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000016', N'U0000013', CAST(N'2024-06-24T14:23:32.000' AS DateTime), NULL, 1, 5000, N'4067693507')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000017', N'U0000013', CAST(N'2024-06-24T14:41:30.000' AS DateTime), NULL, 1, 5000, N'4067714689')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000018', N'U0000013', CAST(N'2024-06-24T22:12:41.000' AS DateTime), NULL, 1, 10000, N'4067798642')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000019', N'U0000013', CAST(N'2024-06-24T22:14:19.000' AS DateTime), NULL, 1, 10000, N'4067777866')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000020', N'U0000012', CAST(N'2024-06-24T22:38:49.707' AS DateTime), NULL, 1, 50000, N'14476473')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000021', N'U0000013', CAST(N'2024-06-24T22:49:59.000' AS DateTime), NULL, 1, 5000, N'4067800070')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000022', N'U0000013', CAST(N'2024-06-24T22:56:07.000' AS DateTime), NULL, 1, 50000, N'4067812368')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000023', N'U0000013', CAST(N'2024-06-24T22:59:03.000' AS DateTime), NULL, 1, 50000, N'4067812401')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000024', N'U0000013', CAST(N'2024-06-24T23:07:25.000' AS DateTime), NULL, 1, 5000, N'4067801069')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000025', N'U0000013', CAST(N'2024-06-24T23:39:28.757' AS DateTime), NULL, 1, 10000, N'14476617')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000026', N'U0000013', CAST(N'2024-06-24T23:45:40.810' AS DateTime), NULL, 1, 50000, N'14476625')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000027', N'U0000013', CAST(N'2024-06-24T23:54:33.383' AS DateTime), NULL, 1, 500000, N'14476639')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000028', N'U0000013', CAST(N'2024-06-25T00:02:33.183' AS DateTime), NULL, 1, 50000, N'14476643')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000029', N'U0000013', CAST(N'2024-06-25T00:14:03.773' AS DateTime), NULL, 1, 50000, N'14476661')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000030', N'U0000013', CAST(N'2024-06-25T00:24:19.090' AS DateTime), NULL, 1, 5000000, N'14476681')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000031', N'U0000013', CAST(N'2024-06-25T00:25:11.000' AS DateTime), NULL, 1, 50000, N'4067804859')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000032', N'U0000013', CAST(N'2024-06-25T23:07:03.137' AS DateTime), NULL, 1, 1000000, N'14478502')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000033', N'U0000013', CAST(N'2024-06-25T23:12:01.470' AS DateTime), NULL, 1, 1000000, N'14478510')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000034', N'U0000013', CAST(N'2024-06-25T23:14:25.293' AS DateTime), NULL, 1, 10000, N'14478515')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000035', N'U0000013', CAST(N'2024-06-25T23:15:16.000' AS DateTime), NULL, 1, 10000, N'4070586701')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000036', N'U0000013', CAST(N'2024-06-26T20:11:05.830' AS DateTime), NULL, 0, 1000000, N'14480265')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000037', N'U0000013', CAST(N'2024-06-26T00:00:00.000' AS DateTime), NULL, 0, 10000, N'4071355403')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000038', N'U0000013', CAST(N'2024-06-26T20:14:19.363' AS DateTime), NULL, 0, 10000, N'14480272')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000039', N'U0000013', CAST(N'2024-06-26T20:20:53.250' AS DateTime), NULL, 0, 10000, N'14480282')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000040', N'U0000013', CAST(N'2024-06-26T21:26:39.043' AS DateTime), NULL, 0, 10000, N'14480357')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000041', N'U0000013', CAST(N'2024-06-26T21:32:43.487' AS DateTime), NULL, 0, 10000, N'14480368')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000042', N'U0000013', CAST(N'2024-06-26T21:36:33.480' AS DateTime), NULL, 0, 10000, N'14480378')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000043', N'U0000013', CAST(N'2024-06-26T00:00:00.000' AS DateTime), NULL, 0, 10000, N'4071361623')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000044', N'U0000013', CAST(N'2024-06-26T21:39:38.060' AS DateTime), NULL, 0, 10000, N'14480383')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000048', N'U0000013', CAST(N'2024-06-30T00:36:33.460' AS DateTime), NULL, 0, 10000, N'14485700')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000049', N'U0000013', CAST(N'2024-06-30T11:30:32.757' AS DateTime), NULL, 0, 10000, N'14485908')
GO
INSERT [dbo].[Role] ([roleID], [role]) VALUES (N'R001', N'Admin')
INSERT [dbo].[Role] ([roleID], [role]) VALUES (N'R002', N'Staff')
INSERT [dbo].[Role] ([roleID], [role]) VALUES (N'R003', N'Customer')
GO
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'S0000020', NULL, NULL, NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'S0000021', NULL, NULL, NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'S0000022', NULL, NULL, NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'S0000023', NULL, NULL, NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'S0000024', NULL, NULL, NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000012', N'minhduccoi', N'123', NULL, N'R003', NULL, NULL, 3915220, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000013', N'minhduc', N'123', NULL, N'R003', NULL, NULL, 10010, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000014', N'duchuycoi', N'123', NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000015', NULL, NULL, NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000016', N'admin1', N'123', N'BR-001', N'R001', NULL, NULL, 1, 0, CAST(N'2024-06-23T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000017', N'testadmin', N'123', NULL, N'R001', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000018', N'teststaff', N'123', N'BR-001', N'R002', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000019', N'duccoi1234', N'123', NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000025', N'duc', N'Duc0977300916@', NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U1', N'Admin', N'123', NULL, N'R001', NULL, NULL, 1, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U10', N'Ngoc', N'helloWorld345@345!', NULL, N'R003', NULL, NULL, 125, 3, CAST(N'2024-06-19T22:56:09.910' AS DateTime), 0)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U11', N'Quynh', N'aAAAAAA@134AAA!', NULL, N'R003', NULL, NULL, 120, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U2', N'minhduc123', N'123', NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U3', N'Username', N'strongPassword@321u', NULL, N'R001', NULL, NULL, NULL, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U4', N'Staff', N'Passta123@#$aaa', NULL, N'R002', NULL, NULL, 0, 2, CAST(N'2024-06-18T12:31:02.030' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U5', N'Staff2', N'sigh762^&a!', N'BR-002', N'R002', NULL, NULL, NULL, 3, CAST(N'2024-06-19T17:01:03.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U6', N'A fan', N'password123()123@a', N'BR-004', N'R002', NULL, NULL, NULL, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U7', N'Duc', N'queryA@56ab10', NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 0)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U8', N'Phuc', N'beeEEE#780Q', NULL, N'R003', NULL, NULL, 125, 1, CAST(N'2024-06-19T12:00:03.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U9', N'Linh', N'motHaiBa123<>@sbc', NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 0)
GO
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'S0000024', N'minh2', N'duchuy2222u', N'ducdmse183990@fpt.edu.vn', N'0977300234', NULL, N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F7fb08916-ea2d-4937-ab5e-0c039985aefb?alt=media&token=2ca659eb-ea23-4b37-8f27-0fc1d8cff722')
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000012', N'duccoi', N'duccoi', N'duchhh@gmail.com', N'0977300915', NULL, N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F3c30bcdb-fedf-4342-a018-dd4c8d06b2a3?alt=media&token=56ff6e37-dd9b-4de4-add5-eda8aec09eb5')
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000013', N'minh', N'duc12221', N'duccc@gmail.com', N'0977300912', NULL, N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F44c5df87-febb-4867-90f2-97575b5d0f5c?alt=media')
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000014', NULL, NULL, N'du232c@gmail.com', N'0977300917', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000015', NULL, NULL, N'duc2222@gmail.com', N'0977300981', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000016', N'Ad', N'minh', N'admin@gmail.com', N'123456789', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000017', NULL, NULL, N'ducadmin@gmail.com', N'0977300981', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000018', N'staff', N'oke', N'staff@gmail.com', N'0985598217', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000019', N'minh', N'duc', N'duccoi213132@gmail.com', N'0123456783', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000025', N'123', N'123', N'duc232coi@gmail.com', N'0977300916', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U1', N'admin', N'admin', N'duccc223oi@gmail.com', N'0977300666', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U10', N'DT', N'Ngoc', N'ngocdt@outlook.com', N'0912840048', N'fb4', N'img9')
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U11', N'TLS', N'Quynh', N'quynhtls@domain.com', N'0826498759', N'fb6', N'')
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U2', N'minh', N'duccoi', N'du22c@gmail.com', N'1234567892', NULL, N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5fe08c02-bddf-42a5-97d5-23e794014fdd?alt=media&token=83a2bd78-b258-4486-aeaa-d75b3d7387bf')
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U3', N'User', N'Name', N'user123name@gmail.com', N'082481258', NULL, N'img2')
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U4', N'null', N'null', N'staff1@gmail.com', N'098027293', N'fbu4', NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U5', N'Cute', N'Phomaique', N'abcxyz@gmail.com', N'0841025832', N'fbu5', N'img5')
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U6', N'Nhu', N'Nao', N'fxy@outlook.com', N'0912846331', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U7', N'DM', N'Duc', N'ducdt@gmail.com', N'0890136720', N'fb1', N'img3')
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U8', N'HTQ', N'Phuc', N'phuchtq@gmail.com', N'0879283749', N'fb3', N'img4')
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U9', N'NHP', N'Linh', N'linhnhp@abc.edu.vn', N'0823060180', N'fb2', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F8e3c917c-b10a-4a19-9d9b-238305e02b4d?alt=media&token=8d5dbc08-25f7-47c8-8a58-8e33dae3ae0f')
GO
ALTER TABLE [dbo].[BookedSlot]  WITH CHECK ADD  CONSTRAINT [FKBookedSlot690847] FOREIGN KEY([bookingID])
REFERENCES [dbo].[Booking] ([bookingID])
GO
ALTER TABLE [dbo].[BookedSlot] CHECK CONSTRAINT [FKBookedSlot690847]
GO
ALTER TABLE [dbo].[BookedSlot]  WITH CHECK ADD  CONSTRAINT [FKBookedSlot778580] FOREIGN KEY([courtID])
REFERENCES [dbo].[Court] ([courtID])
GO
ALTER TABLE [dbo].[BookedSlot] CHECK CONSTRAINT [FKBookedSlot778580]
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FKBooking923627] FOREIGN KEY([userID])
REFERENCES [dbo].[User] ([userID])
GO
ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FKBooking923627]
GO
ALTER TABLE [dbo].[Court]  WITH CHECK ADD  CONSTRAINT [FKCourt788847] FOREIGN KEY([branchID])
REFERENCES [dbo].[CourtBranch] ([branchID])
GO
ALTER TABLE [dbo].[Court] CHECK CONSTRAINT [FKCourt788847]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FKFeedback274984] FOREIGN KEY([userID])
REFERENCES [dbo].[User] ([userID])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FKFeedback274984]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FKFeedback632553] FOREIGN KEY([branchID])
REFERENCES [dbo].[CourtBranch] ([branchID])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FKFeedback632553]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FKPayment444730] FOREIGN KEY([userID])
REFERENCES [dbo].[User] ([userID])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FKPayment444730]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FKPayment887923] FOREIGN KEY([bookingID])
REFERENCES [dbo].[Booking] ([bookingID])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FKPayment887923]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FKUser135985] FOREIGN KEY([branchID])
REFERENCES [dbo].[CourtBranch] ([branchID])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FKUser135985]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FKUser635730] FOREIGN KEY([roleID])
REFERENCES [dbo].[Role] ([roleID])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FKUser635730]
GO
ALTER TABLE [dbo].[UserDetail]  WITH CHECK ADD  CONSTRAINT [FKUserDetail940563] FOREIGN KEY([userID])
REFERENCES [dbo].[User] ([userID])
GO
ALTER TABLE [dbo].[UserDetail] CHECK CONSTRAINT [FKUserDetail940563]
GO
USE [master]
GO
ALTER DATABASE [BadmintonCourt] SET  READ_WRITE 
GO
