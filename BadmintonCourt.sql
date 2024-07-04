USE [master]
GO
/****** Object:  Database [BadmintonCourt]    Script Date: 7/4/2024 10:14:54 PM ******/
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
/****** Object:  Table [dbo].[BookedSlot]    Script Date: 7/4/2024 10:14:54 PM ******/
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
/****** Object:  Table [dbo].[Booking]    Script Date: 7/4/2024 10:14:54 PM ******/
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
/****** Object:  Table [dbo].[Court]    Script Date: 7/4/2024 10:14:54 PM ******/
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
/****** Object:  Table [dbo].[CourtBranch]    Script Date: 7/4/2024 10:14:54 PM ******/
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
/****** Object:  Table [dbo].[Discount]    Script Date: 7/4/2024 10:14:54 PM ******/
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
/****** Object:  Table [dbo].[Feedback]    Script Date: 7/4/2024 10:14:54 PM ******/
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
/****** Object:  Table [dbo].[Payment]    Script Date: 7/4/2024 10:14:54 PM ******/
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
/****** Object:  Table [dbo].[Role]    Script Date: 7/4/2024 10:14:54 PM ******/
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
/****** Object:  Table [dbo].[User]    Script Date: 7/4/2024 10:14:54 PM ******/
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
/****** Object:  Table [dbo].[UserDetail]    Script Date: 7/4/2024 10:14:54 PM ******/
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
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000002', CAST(N'2024-07-05T08:00:00.000' AS DateTime), CAST(N'2024-07-05T09:00:00.000' AS DateTime), N'C-001', N'BK0000003', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S1', CAST(N'1900-01-01T06:00:00.000' AS DateTime), CAST(N'1900-01-01T22:00:00.000' AS DateTime), N'C-001', N'BK1', NULL)
GO
INSERT [dbo].[Booking] ([bookingID], [amount], [bookingType], [userID], [bookingDate], [changeLog], [isDelete]) VALUES (N'BK0000002', 65000, 1, N'U0000002', CAST(N'2024-07-04T15:41:16.733' AS DateTime), 0, NULL)
INSERT [dbo].[Booking] ([bookingID], [amount], [bookingType], [userID], [bookingDate], [changeLog], [isDelete]) VALUES (N'BK0000003', 40000, 1, N'U0000002', CAST(N'2024-07-04T21:57:47.613' AS DateTime), 0, NULL)
INSERT [dbo].[Booking] ([bookingID], [amount], [bookingType], [userID], [bookingDate], [changeLog], [isDelete]) VALUES (N'BK1', 10000, 1, N'U0000001', CAST(N'2024-07-02T00:00:00.000' AS DateTime), 0, NULL)
GO
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C-001', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F58931397-739c-4cc2-903e-0bec28d0cd66?alt=media&token=6feaf8a0-0d0e-431a-9382-af143e258183|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F842cd6e5-1152-41e6-b118-97619200476c?alt=media&token=d4b590bc-e436-481e-9348-bda8bbe946b2|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F423271bd-1657-435c-996d-385c4cb17c35?alt=media&token=0e9c5f80-b091-4070-8705-3d6a02301379', N'BR-002', 40000, N'Come here ok', N'Sân 1', 1)
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
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'BR-001', N'S? 101-103-105 Phan Van Trí, P.1, Qu?n 7', N'Hoàng Thiên1', N'0906246912', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fbbfeb04c-ea7b-4ba8-8966-cad3ce3075eb?alt=media&token=c4ccd0ad-2fe7-4fc6-8d1d-2c49d8335f73|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F9e7c39b7-9c2a-4957-856b-31a1e0bf134d?alt=media&token=c99ab5c4-359d-4dab-ab09-4eda27a0844e', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d31359.967418420183!2d106.83023359999999!3d10.7347968!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317523b926e6c675%3A0xa0bb598051ad0fa0!2sThe%20Hammock%20Glamping!5e0!3m2!1sen!2s!4v1719847889642!5m2!1sen!2s')
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'BR-002', N'277 – 279 An Duong Vuong, P.5, Qu?n 7', N'Hoàng Phi', N'0856209864', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F9e7c39b7-9c2a-4957-856b-31a1e0bf134d?alt=media&token=c99ab5c4-359d-4dab-ab09-4eda27a0844e', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d31359.967418420183!2d106.83023359999999!3d10.7347968!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3175239bfefbd913%3A0x6c75af3281d654d4!2sJeongsan%20country%20club!5e0!3m2!1sen!2s!4v1719848421086!5m2!1sen!2s')
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'BR-003', N'S? 3022 Ph?m Th? Hi?n, P.7, Qu?n 8', N'Hành Phi', N'0846412980', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F3d0ffe3b-45af-452b-9a19-087ef6945c3e?alt=media&token=aa6503fa-365b-4aa4-b1aa-60891714457e', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d31359.967418420183!2d106.83023359999999!3d10.7347968!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3175239bfefbd913%3A0x6c75af3281d654d4!2sJeongsan%20country%20club!5e0!3m2!1sen!2s!4v1719848421086!5m2!1sen!2s')
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'BR-004', N'S? 302 Châu Huong, P.5, Qu?n 2', N'Rebell', N'0907410332', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fd9e26227-5f4f-41af-afae-84907ba29788?alt=media&token=c585770b-5658-4104-bb56-e9da970fabab', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d31359.967418420183!2d106.83023359999999!3d10.7347968!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317523b926e6c675%3A0xa0bb598051ad0fa0!2sThe%20Hammock%20Glamping!5e0!3m2!1sen!2s!4v1719847889642!5m2!1sen!2s')
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'BR-005', N'Trung tâm TDTT Qu?n 10, du?ng Thành Thái, Qu?n 10', N'Nguy?n Tri Phuong', N'0935128603', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Ff1f7a449-54af-4327-9cf3-dca42ae7dbc7?alt=media&token=fdab006b-d816-4261-9ff0-f00a2b3f1654', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d31359.967418420183!2d106.83023359999999!3d10.7347968!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317523b926e6c675%3A0xa0bb598051ad0fa0!2sThe%20Hammock%20Glamping!5e0!3m2!1sen!2s!4v1719847889642!5m2!1sen!2s')
GO
INSERT [dbo].[Discount] ([discountID], [amount], [proportion]) VALUES (N'D-000001', 10000, 0.1)
INSERT [dbo].[Discount] ([discountID], [amount], [proportion]) VALUES (N'D-000002', 30000, 0.2)
INSERT [dbo].[Discount] ([discountID], [amount], [proportion]) VALUES (N'D-000003', 50000, 0.4)
GO
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000001', N'U0000002', CAST(N'2024-07-03T18:13:17.120' AS DateTime), NULL, 0, 10000, N'14492031')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000002', N'U0000002', CAST(N'2024-07-04T15:41:16.720' AS DateTime), N'BK0000002', 0, 65000, N'14493571')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000003', N'U0000002', CAST(N'2024-07-04T21:56:07.903' AS DateTime), NULL, 0, 100000, N'14494190')
GO
INSERT [dbo].[Role] ([roleID], [role]) VALUES (N'R001', N'Admin')
INSERT [dbo].[Role] ([roleID], [role]) VALUES (N'R002', N'Staff')
INSERT [dbo].[Role] ([roleID], [role]) VALUES (N'R003', N'Customer')
GO
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000001', N'admin', N'123', NULL, N'R001', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000002', N'minhduc', N'Duc0977300916@', NULL, N'R003', NULL, NULL, 70670, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
GO
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000001', N'minh', N'duc2', N'duc@gmail.com', N'0977300916', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000002', N'minh', N'duc', N'duccoi16082004@gmail.com', N'0977300915', NULL, N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F9426d9ee-1b77-4d33-89c9-0d2e8248f4eb?alt=media&token=7220e0eb-6d57-4bc5-9fda-ba717adc4727')
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
