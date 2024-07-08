USE [master]
GO
/****** Object:  Database [BadmintonCourt]    Script Date: 7/7/2024 11:18:24 PM ******/
CREATE DATABASE [BadmintonCourt]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BadmintonCourt', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MINHDUC\MSSQL\DATA\BadmintonCourt.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'BadmintonCourt_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MINHDUC\MSSQL\DATA\BadmintonCourt_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [BadmintonCourt] SET COMPATIBILITY_LEVEL = 120
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
/****** Object:  Table [dbo].[BookedSlot]    Script Date: 7/7/2024 11:18:24 PM ******/
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
/****** Object:  Table [dbo].[Booking]    Script Date: 7/7/2024 11:18:24 PM ******/
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
/****** Object:  Table [dbo].[Court]    Script Date: 7/7/2024 11:18:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Court](
	[courtID] [varchar](30) NOT NULL,
	[courtImg] [varchar](3000) NULL,
	[branchID] [varchar](30) NOT NULL,
	[price] [real] NOT NULL,
	[description] [nvarchar](500) NOT NULL,
	[courtName] [nvarchar](30) NOT NULL,
	[courtStatus] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[courtID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CourtBranch]    Script Date: 7/7/2024 11:18:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourtBranch](
	[branchID] [varchar](30) NOT NULL,
	[location] [nvarchar](50) NOT NULL,
	[branchName] [nvarchar](50) NOT NULL,
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
/****** Object:  Table [dbo].[Discount]    Script Date: 7/7/2024 11:18:24 PM ******/
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
/****** Object:  Table [dbo].[Feedback]    Script Date: 7/7/2024 11:18:24 PM ******/
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
/****** Object:  Table [dbo].[Payment]    Script Date: 7/7/2024 11:18:24 PM ******/
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
/****** Object:  Table [dbo].[Role]    Script Date: 7/7/2024 11:18:24 PM ******/
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
/****** Object:  Table [dbo].[User]    Script Date: 7/7/2024 11:18:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[userID] [varchar](30) NOT NULL,
	[userName] [nvarchar](50) NULL,
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
/****** Object:  Table [dbo].[UserDetail]    Script Date: 7/7/2024 11:18:24 PM ******/
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
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000002', CAST(N'2024-06-07T17:00:00.000' AS DateTime), CAST(N'2024-06-07T19:00:00.000' AS DateTime), N'C014', N'BK0000002', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000003', CAST(N'2024-06-07T17:00:00.000' AS DateTime), CAST(N'2024-06-07T19:00:00.000' AS DateTime), N'C001', N'BK0000003', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000004', CAST(N'2024-06-14T17:00:00.000' AS DateTime), CAST(N'2024-06-14T19:00:00.000' AS DateTime), N'C001', N'BK0000003', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000005', CAST(N'2024-06-21T17:00:00.000' AS DateTime), CAST(N'2024-06-21T19:00:00.000' AS DateTime), N'C001', N'BK0000003', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000006', CAST(N'2024-06-28T17:00:00.000' AS DateTime), CAST(N'2024-06-28T19:00:00.000' AS DateTime), N'C001', N'BK0000003', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000007', CAST(N'2024-07-05T17:00:00.000' AS DateTime), CAST(N'2024-07-05T19:00:00.000' AS DateTime), N'C001', N'BK0000003', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000008', CAST(N'2024-07-12T17:00:00.000' AS DateTime), CAST(N'2024-07-12T19:00:00.000' AS DateTime), N'C001', N'BK0000003', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000009', CAST(N'2024-07-19T17:00:00.000' AS DateTime), CAST(N'2024-07-19T19:00:00.000' AS DateTime), N'C001', N'BK0000003', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000010', CAST(N'2024-07-26T17:00:00.000' AS DateTime), CAST(N'2024-07-26T19:00:00.000' AS DateTime), N'C001', N'BK0000003', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000011', CAST(N'2024-08-02T17:00:00.000' AS DateTime), CAST(N'2024-08-02T19:00:00.000' AS DateTime), N'C001', N'BK0000003', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000012', CAST(N'2024-06-03T13:00:00.000' AS DateTime), CAST(N'2024-06-03T15:00:00.000' AS DateTime), N'C001', N'BK0000004', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000013', CAST(N'2024-06-08T09:00:00.000' AS DateTime), CAST(N'2024-06-08T12:00:00.000' AS DateTime), N'C014', N'BK0000005', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000014', CAST(N'2024-07-09T09:00:00.000' AS DateTime), CAST(N'2024-07-09T12:00:00.000' AS DateTime), N'C014', N'BK0000006', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000015', CAST(N'2024-07-07T10:00:00.000' AS DateTime), CAST(N'2024-07-07T12:00:00.000' AS DateTime), N'C003', N'BK0000007', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000016', CAST(N'2024-07-05T12:00:00.000' AS DateTime), CAST(N'2024-07-05T14:00:00.000' AS DateTime), N'C003', N'BK0000008', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000017', CAST(N'2024-07-12T12:00:00.000' AS DateTime), CAST(N'2024-07-12T14:00:00.000' AS DateTime), N'C003', N'BK0000008', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000018', CAST(N'2024-07-19T12:00:00.000' AS DateTime), CAST(N'2024-07-19T14:00:00.000' AS DateTime), N'C003', N'BK0000008', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000019', CAST(N'2024-07-26T12:00:00.000' AS DateTime), CAST(N'2024-07-26T14:00:00.000' AS DateTime), N'C003', N'BK0000008', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000020', CAST(N'2024-08-02T12:00:00.000' AS DateTime), CAST(N'2024-08-02T14:00:00.000' AS DateTime), N'C003', N'BK0000008', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000021', CAST(N'2024-08-09T12:00:00.000' AS DateTime), CAST(N'2024-08-09T14:00:00.000' AS DateTime), N'C003', N'BK0000008', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000022', CAST(N'2024-08-16T12:00:00.000' AS DateTime), CAST(N'2024-08-16T14:00:00.000' AS DateTime), N'C003', N'BK0000008', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000023', CAST(N'2024-08-23T12:00:00.000' AS DateTime), CAST(N'2024-08-23T14:00:00.000' AS DateTime), N'C003', N'BK0000008', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S0000024', CAST(N'2024-08-30T12:00:00.000' AS DateTime), CAST(N'2024-08-30T14:00:00.000' AS DateTime), N'C003', N'BK0000008', NULL)
INSERT [dbo].[BookedSlot] ([slotID], [startTime], [endTime], [courtID], [bookingID], [isDelete]) VALUES (N'S1', CAST(N'2024-07-02T07:00:00.000' AS DateTime), CAST(N'2024-07-02T23:00:00.000' AS DateTime), N'C001', N'B1', NULL)
GO
INSERT [dbo].[Booking] ([bookingID], [amount], [bookingType], [userID], [bookingDate], [changeLog], [isDelete]) VALUES (N'B1', 0, 0, N'U0000027', CAST(N'1900-01-01T00:00:00.000' AS DateTime), 0, NULL)
INSERT [dbo].[Booking] ([bookingID], [amount], [bookingType], [userID], [bookingDate], [changeLog], [isDelete]) VALUES (N'BK0000002', 2640000, 1, N'U0000008', CAST(N'2024-07-02T01:40:30.667' AS DateTime), 0, NULL)
INSERT [dbo].[Booking] ([bookingID], [amount], [bookingType], [userID], [bookingDate], [changeLog], [isDelete]) VALUES (N'BK0000003', 560000, 2, N'U0000008', CAST(N'2024-07-02T01:42:39.990' AS DateTime), 0, NULL)
INSERT [dbo].[Booking] ([bookingID], [amount], [bookingType], [userID], [bookingDate], [changeLog], [isDelete]) VALUES (N'BK0000004', 560000, 1, N'U0000002', CAST(N'2024-07-02T01:44:36.433' AS DateTime), 0, NULL)
INSERT [dbo].[Booking] ([bookingID], [amount], [bookingType], [userID], [bookingDate], [changeLog], [isDelete]) VALUES (N'BK0000005', 1320000, 1, N'U0000008', CAST(N'2024-07-02T07:32:18.117' AS DateTime), 0, NULL)
INSERT [dbo].[Booking] ([bookingID], [amount], [bookingType], [userID], [bookingDate], [changeLog], [isDelete]) VALUES (N'BK0000006', 1320000, 1, N'U0000008', CAST(N'2024-07-02T07:48:55.467' AS DateTime), 0, NULL)
INSERT [dbo].[Booking] ([bookingID], [amount], [bookingType], [userID], [bookingDate], [changeLog], [isDelete]) VALUES (N'BK0000007', 60000, 1, N'U0000006', CAST(N'2024-07-02T08:06:09.583' AS DateTime), 0, NULL)
INSERT [dbo].[Booking] ([bookingID], [amount], [bookingType], [userID], [bookingDate], [changeLog], [isDelete]) VALUES (N'BK0000008', 480000, 2, N'U0000008', CAST(N'2024-07-02T08:08:22.980' AS DateTime), 0, NULL)
GO
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C001', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fc1ebce0c-f151-49ad-84a1-6ff6f001d77f?alt=media&token=67c6a16c-00ed-46ad-a196-4a90bddd1c17|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F27994f5c-ac6b-4783-b967-4d43df64ce03?alt=media&token=d82d15e2-8aef-4c78-894c-fb317913afd8|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F2d0c65a7-c8e3-41dc-87a8-73736f17f03c?alt=media&token=28fe4e3b-3fd3-41ff-8391-1608981e9228|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F035f22fb-bc07-40c8-91f7-ebac164ceb46?alt=media&token=ca73906b-3153-451d-9fc3-30ce2d8bcaa2', N'B001', 35000, N'Sân c?u lông ph? sáng ti?n nghi, ph?c v? t?t cho ngu?i choi.', N'Sân C?u Lông A', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C002', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F072a5e0e-d860-4abd-9ca3-9ef3364a3444?alt=media&token=c6e99c4d-8650-4762-9bdf-96db817fa1b9|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F7fd7d94a-5258-4b8e-a664-0c49abcdc20a?alt=media&token=d914487b-d61b-4ac9-a841-dc3f47c49164|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F10dd356d-22ef-4ffe-b85f-e8bfd5aa467a?alt=media&token=c84e99b1-a3dc-4a69-beae-55b5b00eeb13|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fbe2bce88-839d-493a-86ae-8bc4138f6c51?alt=media&token=8c63ab38-73a0-4d2b-b1a3-7af77fae5abe', N'B001', 40000, N'Sân c?u lông thi d?u chuyên nghi?p, d?y d? trang thi?t b?.', N'Sân C?u Lông B', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C003', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F2f7f29e3-f3b9-454d-bd6d-fd26b6804dd3?alt=media&token=6119aa48-58b4-48c5-a481-aa1285960096|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Feffa2a49-2b86-47ac-8044-38053dff3e3d?alt=media&token=14f39967-f7a0-4769-a58b-37a7f9223e40|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fe22c5bb9-d380-4b7e-82b1-7d98b71d0447?alt=media&token=961feb76-b506-4060-a15f-239b4c8b436c|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F7dc02ee6-c100-4613-ab47-64f0dd263d6f?alt=media&token=c917132e-dc11-4f0a-9993-8a0570e76d89', N'B001', 30000, N'Sân c?u lông r?ng rãi, không gian thoáng mát, thích h?p cho gia dình.', N'Sân C?u Lông C', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C004', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F36fea2a0-e0af-4393-a517-17116dc26afd?alt=media&token=e60fb50b-93a0-4a25-895a-956213bd8e1a|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F0ac47897-09a9-45c8-9801-d4a2af98cbbe?alt=media&token=3b785549-0882-40f4-95b7-c00d1640ccd8|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fdd81cf46-485f-4e27-af8c-ed7e0371b67a?alt=media&token=acee8ba3-a971-4010-835c-7d98da13d6d1|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F6aa8538a-9827-4af4-85b9-b0f7abec3dcf?alt=media&token=e1fbd9bf-00a5-41a7-b9a7-3c94072c9657', N'B001', 45000, N'Sân c?u lông du?c trang b? d?y d? ánh sáng và h? th?ng thông gió t?t.', N'Sân C?u Lông D', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C005', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F8097a422-4ce8-46e9-b22c-a95281439b2f?alt=media&token=b78fd0d2-f272-47be-99cf-06a767f24b00|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F1a91e267-adb3-4bff-9c8c-d8c4427d6a32?alt=media&token=b7dec73f-2b15-45c0-b195-fc460c305750|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fd48e180d-5ef3-4c7c-a669-7c873263ef49?alt=media&token=8ede2cac-f2f3-40b6-8d18-b3dae24581f6|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fb33d13ff-857d-482a-8b39-291943851000?alt=media&token=a16fe6ed-5b11-4935-b638-45c95a59d0a5', N'B002', 60000, N'Sân c?u lông d?ng c?p, dành cho các v?n d?ng viên chuyên nghi?p.', N'Sân C?u Lông A', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C006', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F0d679e8d-2eb8-4afc-9bfe-eb0d0181f1ab?alt=media&token=d7448304-f18f-469a-8d77-5f172c4c6c24|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fb455fe8c-f248-4b62-8e84-a73dcab1b996?alt=media&token=bcf9b0ee-017e-4336-a833-a0c283e8e5be|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fd9f2800c-50bc-4933-b7d8-f6fad09b1f7e?alt=media&token=e1ad1e61-fe39-4f64-8a8b-7ff6f2c559bd|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5516e7ea-f32f-4e37-b712-5df376871d15?alt=media&token=73dd8365-49ab-46d5-858c-56a7f0bc64a5', N'B002', 55000, N'Sân c?u lông ph?c v? nhu c?u thu giãn và rèn luy?n s?c kh?e.', N'Sân C?u Lông B', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C007', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F0ec87dda-cfcb-43d2-a953-96ee5fb8730f?alt=media&token=558cf25d-74c2-4b02-b666-055da9071b84|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fb4cf395e-a302-4b5f-9288-6ba9190c52ed?alt=media&token=ddd5ab40-c3e1-4fa6-b224-129860b1348b|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F31cd1bab-f884-4d70-b5f1-aebac78e0a06?alt=media&token=16b9dd43-dd9e-44a8-9ef6-2bad4cf30490|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fbd010a2e-97bb-44d5-a984-9ef26a1a1600?alt=media&token=5550fa7e-e901-47af-b822-b8239d9a628f', N'B002', 70000, N'Sân c?u lông cao c?p v?i d?y d? d?ch v? và ti?n nghi.', N'Sân C?u Lông C', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C008', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fca3ff73d-0cf0-4785-860e-c149af9b9c77?alt=media&token=ea322526-d140-4d70-9abf-1ca35f4623ac|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F9978c96c-544d-49a9-9811-95930e57f036?alt=media&token=01cded09-c0ea-4311-a6c3-c026bdd14ae6|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fccac74a2-a2cd-4d0d-9935-af3b378ae7b4?alt=media&token=2355df67-d4c9-4948-b530-183609ec8fd5|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F6dc7d6fa-5fff-4ab9-81a7-40e38a312e82?alt=media&token=2b436d62-c686-406a-97ae-ace7884db0aa', N'B003', 50000, N'Sân c?u lông phù h?p cho c? nh?ng ngu?i m?i b?t d?u và nh?ng v?n d?ng viên chuyên nghi?p.', N'Sân C?u Lông A', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C009', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F129a10c5-dfef-4efb-9598-7ddc154a7d7c?alt=media&token=296442f3-5101-4e57-b483-aa479a624a2d|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fb800d776-f473-4c86-88e3-959645c1e427?alt=media&token=c4bf0c97-92fd-493a-991d-df21c0e086de|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fbd8a723d-d573-49c9-8fe0-86be968c256e?alt=media&token=39713ac1-467c-48ab-979e-d745d05a4fbf|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F9989ec5c-e2eb-40cd-8c6c-6aee17cde9cd?alt=media&token=509aa685-df71-4aba-b6d2-d75629236a8b', N'B003', 35000, N'Sân c?u lông v?i không gian thoáng dãng và ti?n nghi.', N'Sân C?u Lông B', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C010', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fb728e454-dd53-41f0-89e1-63c7c8b6a750?alt=media&token=32dd9764-1c7e-4863-8d1c-5c75ed384965|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F7274ad9e-135c-4363-b4fb-560996144914?alt=media&token=a525ff84-e92c-4f6a-a435-7fb79a2adc77|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F05b582fe-bb40-472e-a47e-76dcc622eeb4?alt=media&token=659b0514-0f50-433b-ab09-7d35cfb2c90f|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F4da041e5-b9a0-481a-be6e-69346602bd6f?alt=media&token=a3cc3929-a639-4613-88aa-ecf309f832c6', N'B003', 40000, N'Sân c?u lông dáp ?ng m?i nhu c?u c?a ngu?i choi, t? gi?i trí d?n thi d?u.', N'Sân C?u Lông C', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C011', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F57e45e6c-dab6-4387-826f-adf722822489?alt=media&token=85666459-dfec-4cb2-9ddc-5291ccfc3267|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F1baba48a-d788-499a-808e-b130376df2fc?alt=media&token=a8a560a6-dd24-4490-b3fa-ff9a744ff819|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fe0a7f619-b04a-4458-b1af-5a87ad964257?alt=media&token=e82df3b6-c832-4f0a-844f-f26d87825110|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F4250603b-1f9e-475d-8144-319cba3a1609?alt=media&token=697b94f2-4922-4632-beb1-93d7fbd4c511', N'B003', 45000, N'Sân c?u lông ph?c v? cho m?i d?i tu?ng, t? tr? em d?n ngu?i già.', N'Sân C?u Lông D', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C012', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F670c8e7e-3b96-4751-b4d9-b34258451acc?alt=media&token=e34f3e31-4cc0-4d05-aa7d-4d677d16590c|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fb3eb1a18-f336-4d61-81dd-ab21c311a367?alt=media&token=ea1a09aa-41b9-48b3-8282-dc5154fadb99|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F9b07bb3e-7758-4d3f-bb30-4d4e8cab8ea5?alt=media&token=c2a3af31-619b-42b3-a3d8-2cbbf141c2fb|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F2a8bed62-1743-41a9-8b9a-43f4cef59c0a?alt=media&token=c880d1ff-0354-49f4-9183-f955d9d91c9e', N'B003', 38000, N'Sân c?u lông phù h?p cho các b?n tr? yêu thích th? thao.', N'Sân C?u Lông E', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C013', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F0126941f-7e40-47b1-8bd0-a1ad50eca76e?alt=media&token=5d63e35f-257c-4509-81af-44706bdc9e62|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fb8fb8669-4f60-4b31-a0a8-76dfe6060def?alt=media&token=a6700539-031c-4b48-b813-be0cd5288486|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fc6daf2b7-1468-44cc-b76a-93af5a15a2a5?alt=media&token=919fb7aa-af9a-4215-a05e-4137ff0e54c1|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fa00fce59-5ce2-4958-8948-d94f7f7e8a9b?alt=media&token=1230c159-9c2a-44b4-97de-1967725cd8c4', N'B003', 42000, N'Sân c?u lông có không gian m?, phù h?p cho gia dình và b?n bè.', N'Sân C?u Lông F', 0)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C014', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F4a1094b2-e0a1-4abf-94df-5d86ff83681a?alt=media&token=524f396f-fae2-4fb1-a7c3-b520bfbcc5e1|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F12115add-27a2-40d2-bde6-c4a43f963100?alt=media&token=92986946-2229-4ff6-b316-33b999bb38be|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fdb6b9b4f-4cc7-46f5-9879-b48e90f330f6?alt=media&token=021ae4c9-8762-4ab4-91a7-96db6f67a24b|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fbce0fc91-5c6b-4aeb-a6f8-3b988fd48df7?alt=media&token=aa1f0879-3717-4334-a3bd-b9999a684301', N'B004', 55000, N'Sân c?u lông r?ng rãi v?i d?y d? thi?t b? và ánh sáng t? nhiên.', N'Sân C?u Lông A', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C015', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fccbf1558-ac08-4119-8d87-26c43b28ce36?alt=media&token=b0e210e6-d8cd-4602-9a14-d6cadf4f54d6|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fbee9c932-3692-4177-b926-8021edebcdbf?alt=media&token=5efead18-9030-4812-947c-04ac7fd12bee|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Ffb69e56a-b887-4f6f-9f80-6c72814bafe3?alt=media&token=203377f3-f7fd-46d9-ba6e-3d682ee78c1d|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F6ac14cc2-101f-414c-8736-13b86e19a7fe?alt=media&token=848cd917-012f-4153-b22d-86f127e72fd0', N'B004', 60000, N'Sân c?u lông dành cho các cu?c thi chuyên nghi?p và hu?n luy?n viên.', N'Sân C?u Lông B', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C016', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F0da06a47-a5e8-4f54-88a2-45f9e7d752d6?alt=media&token=bbc8983d-9cb2-4fec-b43d-f2583c9eda17|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F45131afc-5202-4c30-b879-c30502a50388?alt=media&token=0ae3f17c-2912-4a3a-bea6-5fbc68e8e6a3|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F07b13b66-a4b2-46c2-8153-3ba98659e2fc?alt=media&token=41c79802-bc21-419f-a5a1-7ee964883c26|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F90b87f27-73a2-4776-93cc-0d1921b9e285?alt=media&token=10609efe-4ac4-485d-8a45-7afae815aadc', N'B004', 45000, N'Sân c?u lông ti?n nghi v?i không gian lý tu?ng d? rèn luy?n s?c kh?e.', N'Sân C?u Lông C', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C017', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F09193b3c-038e-43be-8c43-6fe0d149f8ff?alt=media&token=73d02267-fb6e-4b2a-bac3-3fa357255c94|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F0f585551-e354-4c5a-be3b-1da2dff0676a?alt=media&token=8b2b9252-fb91-43e7-84ad-97b207076a74|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F5090b4fb-3cfe-4f3d-81af-8785787894e0?alt=media&token=c4fdbb26-1c56-4b74-ad13-ca74f5eba64e|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fbb0e5e62-67ad-411c-a8d6-f6d585a594be?alt=media&token=a594da06-f94f-49c7-9850-87679fdc6055', N'B005', 40000, N'Sân c?u lông ph?c v? cho m?i nhu c?u, t? gia dình d?n b?n bè.', N'Sân C?u Lông A', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C018', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F4912d799-a5c5-4bd5-94f6-f8ff6d0da620?alt=media&token=064312c0-6aac-4958-9696-79a3f82d4005|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F456850a3-fc4d-4f4e-bca0-348d7d89ff84?alt=media&token=f35515ed-69e4-465a-b511-9173586bd334|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F2c9847fa-c635-4a87-952d-a81c5c6db6dd?alt=media&token=a3109131-e3a0-45ff-abe4-b2007259b6e9|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F87f03d8e-a368-43cc-a53a-2b787ee2bc82?alt=media&token=9a95096a-a1f4-4fb7-9406-043c66491df2', N'B005', 45000, N'Sân c?u lông dáp ?ng m?i yêu c?u v? ch?t lu?ng và d?ch v?.', N'Sân C?u Lông B', 1)
INSERT [dbo].[Court] ([courtID], [courtImg], [branchID], [price], [description], [courtName], [courtStatus]) VALUES (N'C019', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fdd66abed-901b-473f-ae30-f57f614ba906?alt=media&token=187c826d-61c1-4db6-85be-e3f24cb835d6|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F4959a73b-0d6e-4063-b4d2-9717659fc3ae?alt=media&token=0d0cbae8-ff00-40e1-9ac1-faadb9037ccd|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F39f99d35-8eb1-47d5-ac34-f36a05ecbaff?alt=media&token=180b9303-eeb2-4dc4-81ba-efd491808a8f|https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F2836816f-0c47-4982-a6af-ced3d9f494da?alt=media&token=e628141c-3bd3-4cb9-8902-a716e8c308d4', N'B005', 35000, N'Sân c?u lông phù h?p cho m?i d?i tu?ng t? ngu?i m?i choi d?n ngu?i chuyên nghi?p.', N'Sân C?u Lông C', 1)
GO
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'B001', N'Bà R?a - Vung Tàu', N'Sân C?u Lông Vung Tàu', N'0987654321', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F9d640055-c2bb-4f06-aa6a-bdb65c32f18a?alt=media&token=b0535ef9-be14-4ae1-9a2c-0f4fb53a4c73', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3916.9443900204574!2d106.87432764457839!3d10.96757020000001!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3174ddf13a55ec25:0x662ef71599c3d17!2zU8OibiBj4bqndSBsw7RuZyBOaOG6rXQgVGjDoG5o!5e0!3m2!1sen!2s!4v1720368371037!5m2!1sen!2s')
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'B002', N'H? Chí Minh', N'Sân C?u Lông Qu?n 1', N'0976543210', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fb0b99800-e368-45a4-8149-71c22cecc06a?alt=media&token=30a7b0cc-8bef-4d69-a238-60ffcc4465bc', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3916.8815686864264!2d106.89844363488771!3d10.972311000000007!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3174dda1621b4d5f:0x22bfdc5082afbeb6!2sDuc Thinh Badminton Club!5e0!3m2!1sen!2s!4v1720368396794!5m2!1sen!2s')
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'B003', N'Hà N?i', N'Sân C?u Lông Hoàn Ki?m', N'0912345678', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F1fd6a274-af67-4fa7-b945-a78c6438ac1c?alt=media&token=95306da9-170f-4707-8dfc-6dafd624a62a', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3917.9602806219896!2d106.90020523488771!3d10.890623200000002!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3174df61447a8da1:0x184f54e5efde4168!2zU8OibiBj4bqndSBsw7RuZyBQaMaw4bubYyBUw6Ju!5e0!3m2!1sen!2s!4v1720368420820!5m2!1sen!2s')
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'B004', N'Ðà N?ng', N'Sân C?u Lông Ðà N?ng', N'0901234567', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2Fed4976c4-1759-49d5-b329-52bb1ae0c078?alt=media&token=62882de4-9b79-437d-a27c-99c5d100acf6', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3917.1275711519697!2d106.85337103488773!3d10.953734899999994!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3174df1214344147:0x59ef30d1ff880c96!2zU8OibiBD4bqndSDEkOG7iW5oIENhbw!5e0!3m2!1sen!2s!4v1720368478338!5m2!1sen!2s')
INSERT [dbo].[CourtBranch] ([branchID], [location], [branchName], [branchPhone], [branchImg], [branchStatus], [mapUrl]) VALUES (N'B005', N'C?n Tho', N'Sân C?u Lông C?n Tho', N'0934567890', N'https://firebasestorage.googleapis.com/v0/b/badmintoncourtbooking-183b2.appspot.com/o/files%2F257b3ab7-b39f-4aa1-8ef5-8b2a9f4d8aad?alt=media&token=dbfc66fc-2832-4e19-9e94-bba9aac273a7', 1, N'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3917.1275711519697!2d106.85337103488773!3d10.953734899999994!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3174dd808bc444ed:0x81e9374266d93fe!2zU8OibiBD4bqndSBMw7RuZyBBbmggSGnhur91!5e0!3m2!1sen!2s!4v1720368515134!5m2!1sen!2s')
GO
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000001', 5, N'Sân bóng rộng rãi và đầy đủ tiện nghi, phục vụ tốt cho các trận đấu.', N'U0000001', N'B001', CAST(N'2024-06-11T01:31:27.513' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000002', 4, N'Giá cả hợp lý, không gian sân bóng rộng và thoáng mát.', N'U0000002', N'B002', CAST(N'2024-06-22T01:31:27.513' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000003', 3, N'Sân bóng không được bảo trì tốt, gây khó chịu khi chơi.', N'U0000003', N'B003', CAST(N'2024-06-23T01:31:27.513' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000004', 2, N'Nhân viên phục vụ không nhiệt tình, thiếu chuyên nghiệp.', N'U0000004', N'B004', CAST(N'2024-06-08T01:31:27.513' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000005', 5, N'Vị trí thuận lợi, dễ dàng di chuyển vào cuối tuần.', N'U0000005', N'B005', CAST(N'2024-06-19T01:31:27.513' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000006', 4, N'Không gian sân bóng rộng và sạch sẽ, phục vụ tốt cho các đội chơi.', N'U0000006', N'B001', CAST(N'2024-06-17T01:31:27.513' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000007', 3, N'Giá cả hơi cao so với chất lượng sân bóng.', N'U0000007', N'B002', CAST(N'2024-06-11T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000008', 1, N'Sân bóng quá đông vào các buổi chiều cuối tuần, không thoải mái cho việc chơi.', N'U0000008', N'B003', CAST(N'2024-06-10T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000009', 2, N'Không gian sân bóng hẹp và không có ánh sáng đủ vào ban đêm.', N'U0000009', N'B004', CAST(N'2024-06-15T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000010', 4, N'Sân bóng đẹp và rộng rãi, phù hợp cho các trận đấu.', N'U0000010', N'B005', CAST(N'2024-07-02T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000011', 3, N'Nhân viên không thân thiện và không giải quyết thắc mắc của khách hàng nhanh chóng.', N'U0000011', N'B001', CAST(N'2024-06-25T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000012', 5, N'Dịch vụ tốt, giá cả hợp lý và sân bóng luôn sạch sẽ.', N'U0000012', N'B002', CAST(N'2024-06-23T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000013', 2, N'Nhân viên phục vụ không nhiệt tình và không chuyên nghiệp.', N'U0000013', N'B003', CAST(N'2024-07-02T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000014', 4, N'Sân bóng sạch sẽ và thoáng mát vào ban đêm, phục vụ tốt cho các trận đấu.', N'U0000014', N'B004', CAST(N'2024-06-29T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000015', 3, N'Không gian chơi hẹp và ồn ào quá mức, không thoải mái cho các trận đấu lớn.', N'U0000015', N'B005', CAST(N'2024-06-18T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000016', 1, N'Không có chỗ đậu xe gần sân, rất bất tiện khi đi chơi bóng.', N'U0000001', N'B001', CAST(N'2024-06-29T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000017', 5, N'Trung tâm có nhiều sân chơi, phù hợp cho nhiều nhóm lớn.', N'U0000002', N'B002', CAST(N'2024-06-10T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000018', 4, N'Giá cả hợp lý, nhân viên nhiệt tình và sân bóng luôn được bảo trì tốt.', N'U0000003', N'B003', CAST(N'2024-06-23T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000019', 3, N'Sân bóng không được phẳng lặng, gây khó chơi cho các trận đấu.', N'U0000004', N'B004', CAST(N'2024-06-12T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000020', 2, N'Không có dịch vụ hỗ trợ cho người mới chơi, thiếu tiện nghi.', N'U0000005', N'B005', CAST(N'2024-06-18T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000021', 5, N'Địa điểm gần nhà, tiện lợi cho việc luyện tập hàng ngày.', N'U0000006', N'B001', CAST(N'2024-06-28T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000022', 3, N'Không gian sân bóng hơi chật và không phù hợp cho các trận đấu lớn.', N'U0000007', N'B002', CAST(N'2024-06-17T01:31:27.517' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000023', 4, N'Giá cả hợp lý và nhân viên phục vụ nhanh chóng, hiệu quả.', N'U0000008', N'B003', CAST(N'2024-06-06T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000024', 2, N'Nhân viên không thân thiện và không nhiệt tình trong phục vụ.', N'U0000009', N'B004', CAST(N'2024-06-22T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000025', 5, N'Vị trí thuận lợi và sân bóng luôn được bảo trì sạch sẽ.', N'U0000010', N'B005', CAST(N'2024-06-22T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000026', 4, N'Sân bóng rộng và đáp ứng đủ yêu cầu cho các trận đấu.', N'U0000011', N'B001', CAST(N'2024-06-06T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000027', 3, N'Giá cả hơi cao so với chất lượng sân bóng.', N'U0000012', N'B002', CAST(N'2024-06-15T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000028', 1, N'Sân bóng quá đông vào các buổi chiều cuối tuần, không thoải mái cho việc chơi.', N'U0000013', N'B003', CAST(N'2024-06-12T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000029', 2, N'Không gian sân bóng hẹp và không có ánh sáng đủ vào ban đêm.', N'U0000014', N'B004', CAST(N'2024-06-06T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000030', 4, N'Sân bóng đẹp và rộng rãi, phù hợp cho các trận đấu.', N'U0000015', N'B005', CAST(N'2024-06-22T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000031', 3, N'Nhân viên không thân thiện và không giải quyết thắc mắc của khách hàng nhanh chóng.', N'U0000001', N'B001', CAST(N'2024-06-29T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000032', 5, N'Dịch vụ tốt, giá cả hợp lý và sân bóng luôn sạch sẽ.', N'U0000002', N'B002', CAST(N'2024-06-30T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000033', 2, N'Nhân viên phục vụ không nhiệt tình và không chuyên nghiệp.', N'U0000003', N'B003', CAST(N'2024-06-16T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000034', 4, N'Sân bóng sạch sẽ và thoáng mát vào ban đêm, phục vụ tốt cho các trận đấu.', N'U0000004', N'B004', CAST(N'2024-06-04T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000035', 3, N'Không gian chơi hẹp và ồn ào quá mức, không thoải mái cho các trận đấu lớn.', N'U0000005', N'B005', CAST(N'2024-06-14T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000036', 1, N'Không có chỗ đậu xe gần sân, rất bất tiện khi đi chơi bóng.', N'U0000006', N'B001', CAST(N'2024-06-06T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000037', 5, N'Trung tâm có nhiều sân chơi, phù hợp cho nhiều nhóm lớn.', N'U0000007', N'B002', CAST(N'2024-06-19T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000038', 4, N'Giá cả hợp lý, nhân viên nhiệt tình và sân bóng luôn được bảo trì tốt.', N'U0000008', N'B003', CAST(N'2024-06-25T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000039', 3, N'Sân bóng không được phẳng lặng, gây khó chơi cho các trận đấu.', N'U0000009', N'B004', CAST(N'2024-06-26T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000040', 2, N'Không có dịch vụ hỗ trợ cho người mới chơi, thiếu tiện nghi.', N'U0000010', N'B005', CAST(N'2024-06-21T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000041', 5, N'Địa điểm gần nhà, tiện lợi cho việc luyện tập hàng ngày.', N'U0000011', N'B001', CAST(N'2024-06-08T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000042', 3, N'Không gian sân bóng hơi chật và không phù hợp cho các trận đấu lớn.', N'U0000012', N'B002', CAST(N'2024-06-20T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000043', 4, N'Giá cả hợp lý và nhân viên phục vụ nhanh chóng, hiệu quả.', N'U0000013', N'B003', CAST(N'2024-06-04T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000044', 2, N'Nhân viên không thân thiện và không nhiệt tình trong phục vụ.', N'U0000014', N'B004', CAST(N'2024-06-08T01:31:27.520' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000045', 5, N'Vị trí thuận lợi và sân bóng luôn được bảo trì sạch sẽ.', N'U0000015', N'B005', CAST(N'2024-06-15T01:31:27.523' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000046', 4, N'Sân bóng rộng và đáp ứng đủ yêu cầu cho các trận đấu.', N'U0000001', N'B001', CAST(N'2024-06-17T01:31:27.523' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000047', 3, N'Giá cả hơi cao so với chất lượng sân bóng.', N'U0000002', N'B002', CAST(N'2024-07-02T01:31:27.523' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000048', 1, N'Sân bóng quá đông vào các buổi chiều cuối tuần, không thoải mái cho việc chơi.', N'U0000003', N'B003', CAST(N'2024-06-14T01:31:27.523' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000049', 2, N'Không gian sân bóng hẹp và không có ánh sáng đủ vào ban đêm.', N'U0000004', N'B004', CAST(N'2024-06-15T01:31:27.523' AS DateTime))
INSERT [dbo].[Feedback] ([feedbackID], [rating], [content], [userID], [branchID], [period]) VALUES (N'F0000050', 4, N'Sân bóng đẹp và rộng rãi, phù hợp cho các trận đấu.', N'U0000005', N'B005', CAST(N'2024-06-13T01:31:27.523' AS DateTime))
GO
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000001', N'U0000008', CAST(N'2024-07-02T01:40:30.623' AS DateTime), N'BK0000002', 0, 2640000, N'14488493')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000002', N'U0000008', CAST(N'2024-07-02T01:42:39.980' AS DateTime), N'BK0000003', 0, 560000, N'14488494')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000003', N'U0000002', CAST(N'2024-07-02T01:44:36.423' AS DateTime), N'BK0000004', 0, 560000, N'14488496')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000004', N'U0000008', CAST(N'2024-07-02T07:32:18.083' AS DateTime), N'BK0000005', 0, 1320000, N'14488563')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000005', N'U0000008', CAST(N'2024-07-02T07:48:55.440' AS DateTime), N'BK0000006', 0, 1320000, N'14488565')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000006', N'U0000006', CAST(N'2024-07-02T08:06:09.573' AS DateTime), N'BK0000007', 0, 60000, N'14488568')
INSERT [dbo].[Payment] ([paymentID], [userID], [date], [bookingID], [method], [amount], [transactionId]) VALUES (N'P0000007', N'U0000008', CAST(N'2024-07-02T08:08:22.970' AS DateTime), N'BK0000008', 0, 480000, N'14488570')
GO
INSERT [dbo].[Role] ([roleID], [role]) VALUES (N'R001', N'Admin')
INSERT [dbo].[Role] ([roleID], [role]) VALUES (N'R002', N'Staff')
INSERT [dbo].[Role] ([roleID], [role]) VALUES (N'R003', N'Customer')
GO
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000001', N'NguyenPTT16', N'463489af-de9', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAwMSIsIlVzZXJuYW1lIjoiTmd1eWVuUFRUMTYiLCJTdGF0dXMiOiJGYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlIwMDMiLCJSb2xlIjoiUjAwMyIsImV4cCI6MTcxOTg1MjUwOSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8ifQ.PVjxtN0L7yob6Z2Mhj4BXc3bZ-QZsIg8VpJ5KsOxuv4', CAST(N'2024-07-01T23:33:29.613' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000002', N'ThuPM87', N'60524134-add', NULL, N'R003', NULL, NULL, 100000, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000003', N'ThaiDV35', N'567db4c7-599', NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000004', N'PhatPT98', N'64964cc5-12f', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAwNCIsIlVzZXJuYW1lIjoiUGhhdFBUOTgiLCJTdGF0dXMiOiJGYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlIwMDMiLCJSb2xlIjoiUjAwMyIsImV4cCI6MTcxOTg1MjUyMCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8ifQ.AmGmcjYX9I80zCLIqDDsXpn0bRunO508MiRuv7YJZx4', CAST(N'2024-07-01T23:33:40.050' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000005', N'ThanhDH81', N'92a214c2-8a5', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAwNSIsIlVzZXJuYW1lIjoiVGhhbmhESDgxIiwiU3RhdHVzIjoiRmFsc2UiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJSMDAzIiwiUm9sZSI6IlIwMDMiLCJleHAiOjE3MTk4NTI1MjQsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8iLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUyNjYvIn0.oEm1X-HuY-k5hmHenJUb8_oXc8x2pDVJxMQnHcAQC-4', CAST(N'2024-07-01T23:33:44.010' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000006', N'LuatLP95', N'e6791e2e-784', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAwNiIsIlVzZXJuYW1lIjoiTHVhdExQOTUiLCJTdGF0dXMiOiJGYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlIwMDMiLCJSb2xlIjoiUjAwMyIsImV4cCI6MTcxOTg1MjUyNywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8ifQ.XhE60D0KyAtLWiiSKANVTTRUR5YSP-IEliIbdR1oZ7o', CAST(N'2024-07-01T23:33:47.380' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000007', N'NhungPT59', N'5aaaf365-a33', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAwNyIsIlVzZXJuYW1lIjoiTmh1bmdQVDU5IiwiU3RhdHVzIjoiRmFsc2UiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJSMDAzIiwiUm9sZSI6IlIwMDMiLCJleHAiOjE3MTk4NTI1MzEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8iLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUyNjYvIn0.GmyIqtbw7xFdAJr4_cAG92ci15BF3S1pmYPAmfT_b5I', CAST(N'2024-07-01T23:33:51.400' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000008', N'TriTT68', N'aad62cd2-8e2', NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000009', N'HoangNV21', N'835ccf5c-4d4', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAwOSIsIlVzZXJuYW1lIjoiSG9hbmdOVjIxIiwiU3RhdHVzIjoiRmFsc2UiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJSMDAzIiwiUm9sZSI6IlIwMDMiLCJleHAiOjE3MTk4NTI1MzgsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8iLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUyNjYvIn0.vxd9Woy6FzCVW1SxSTiZx02ugtZwDNlHawlCSrSvEd0', CAST(N'2024-07-01T23:33:58.037' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000010', N'TriTT66', N'f7e872b9-237', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAxMCIsIlVzZXJuYW1lIjoiVHJpVFQ2NiIsIlN0YXR1cyI6IkZhbHNlIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiUjAwMyIsIlJvbGUiOiJSMDAzIiwiZXhwIjoxNzE5ODUyNTQxLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUyNjYvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyJ9.Q7Oa7zGUUCUKr7mL8jX-i-a0gl65gGnX7F2t929cHCs', CAST(N'2024-07-01T23:34:01.410' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000011', N'HueTTT25', N'5e85ba00-d4b', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAxMSIsIlVzZXJuYW1lIjoiSHVlVFRUMjUiLCJTdGF0dXMiOiJGYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlIwMDMiLCJSb2xlIjoiUjAwMyIsImV4cCI6MTcxOTg1MjU0NCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8ifQ.EfZj3wFR4XpmPzaNbJwXpVzYuTBsgE8N5kkkKOe7ESc', CAST(N'2024-07-01T23:34:04.793' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000012', N'NguyenPTT63', N'339b6545-275', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAxMiIsIlVzZXJuYW1lIjoiTmd1eWVuUFRUNjMiLCJTdGF0dXMiOiJGYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlIwMDMiLCJSb2xlIjoiUjAwMyIsImV4cCI6MTcxOTg1MjU3OSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8ifQ.fAEievjlYC-JREQWhUaLj87Y00YzYeBhEUWIvdy0oQQ', CAST(N'2024-07-01T23:34:39.500' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000013', N'ThuPM03', N'48b7cd7b-995', NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000014', N'ThaiDV88', N'c0072294-ad5', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAxNCIsIlVzZXJuYW1lIjoiVGhhaURWODgiLCJTdGF0dXMiOiJGYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlIwMDMiLCJSb2xlIjoiUjAwMyIsImV4cCI6MTcxOTg1MjU4OCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8ifQ.e8qosnefybiIPNA960ldricei9wPSIdsiCx2T_5PzDs', CAST(N'2024-07-01T23:34:48.010' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000015', N'PhatPT77', N'817cfad7-59b', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAxNSIsIlVzZXJuYW1lIjoiUGhhdFBUNzciLCJTdGF0dXMiOiJGYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlIwMDMiLCJSb2xlIjoiUjAwMyIsImV4cCI6MTcxOTg1MjU5MSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8ifQ.6n8M_ZcV6vS96-6Wm3HmaZJic1zOxNkTrvG0AYsh4tY', CAST(N'2024-07-01T23:34:51.373' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000016', N'ThanhDH39', N'51ecca94-e5c', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAxNiIsIlVzZXJuYW1lIjoiVGhhbmhESDM5IiwiU3RhdHVzIjoiRmFsc2UiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJSMDAzIiwiUm9sZSI6IlIwMDMiLCJleHAiOjE3MTk4NTI1OTQsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8iLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUyNjYvIn0.axBEwEziBUXkW256zOrbEPM_4RUvRHoBhBzYHqZzQyc', CAST(N'2024-07-01T23:34:54.673' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000017', N'LuatLP83', N'a5c9ea43-790', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAxNyIsIlVzZXJuYW1lIjoiTHVhdExQODMiLCJTdGF0dXMiOiJGYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlIwMDMiLCJSb2xlIjoiUjAwMyIsImV4cCI6MTcxOTg1MjU5OCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8ifQ.kjnUkCf9X595ShXuiG27k0tjL7GlsTNrKkipoH93hFM', CAST(N'2024-07-01T23:34:58.887' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000018', N'NhungPT99', N'97368401-40a', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAxOCIsIlVzZXJuYW1lIjoiTmh1bmdQVDk5IiwiU3RhdHVzIjoiRmFsc2UiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJSMDAzIiwiUm9sZSI6IlIwMDMiLCJleHAiOjE3MTk4NTI2MDIsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8iLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUyNjYvIn0.1vIZHl_2BFGEfXXwqGvM67RHKg7OCubD8UP1J4sBfW8', CAST(N'2024-07-01T23:35:02.770' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000019', N'TriTT94', N'c7405a5f-89e', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAxOSIsIlVzZXJuYW1lIjoiVHJpVFQ5NCIsIlN0YXR1cyI6IkZhbHNlIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiUjAwMyIsIlJvbGUiOiJSMDAzIiwiZXhwIjoxNzE5ODUyNjA2LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUyNjYvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyJ9.64hRr2K7AlHAnhmo3SzKATLNuN_Q3wHN0xcr0JkoW6g', CAST(N'2024-07-01T23:35:06.927' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000020', N'HoangNV55', N'96a4ec8f-d6f', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAyMCIsIlVzZXJuYW1lIjoiSG9hbmdOVjU1IiwiU3RhdHVzIjoiRmFsc2UiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJSMDAzIiwiUm9sZSI6IlIwMDMiLCJleHAiOjE3MTk4NTI2MTAsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8iLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUyNjYvIn0.1RsXAFeF55UCp2tILMHZztC0o61V9PJA5QsozrfySJw', CAST(N'2024-07-01T23:35:10.243' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000021', N'TriTT53', N'90cae1b8-cf5', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAyMSIsIlVzZXJuYW1lIjoiVHJpVFQ1MyIsIlN0YXR1cyI6IkZhbHNlIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiUjAwMyIsIlJvbGUiOiJSMDAzIiwiZXhwIjoxNzE5ODUyNjEzLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUyNjYvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyJ9.hirdIRnndx5hlez8rijS_A3vttts0AhgyTKNFTPvXzY', CAST(N'2024-07-01T23:35:13.577' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000022', N'HueTTT90', N'4dfccedb-0ce', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAyMiIsIlVzZXJuYW1lIjoiSHVlVFRUOTAiLCJTdGF0dXMiOiJGYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlIwMDMiLCJSb2xlIjoiUjAwMyIsImV4cCI6MTcxOTg1MjYxNywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8ifQ.6cxv-M0902dJ2-LJ_JnzeqPsPRdSyP5dFDaK-wfSKhI', CAST(N'2024-07-01T23:35:17.910' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000023', N'TienNQ40', N'6d6fd403-77c', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAyMyIsIlVzZXJuYW1lIjoiVGllbk5RNDAiLCJTdGF0dXMiOiJGYWxzZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlIwMDMiLCJSb2xlIjoiUjAwMyIsImV4cCI6MTcxOTg1MjYyMSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI2Ni8ifQ.uQ1OY_KaXBvAC5p9TxHkDArEMhZpd4XVLfLR0gzQcKw', CAST(N'2024-07-01T23:35:21.897' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000024', N'DatLN26', N'829b3fd5-ec3', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAyNCIsIlVzZXJuYW1lIjoiRGF0TE4yNiIsIlN0YXR1cyI6IkZhbHNlIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiUjAwMyIsIlJvbGUiOiJSMDAzIiwiZXhwIjoxNzE5ODUyNjI1LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUyNjYvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyJ9.QTob5PEEyRUuPbIPEWnnS19d69vJNXCFhnfggaataPU', CAST(N'2024-07-01T23:35:25.687' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000025', N'DatPT33', N'c5e3bb8f-223', NULL, N'R003', N'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJVMDAwMDAyNSIsIlVzZXJuYW1lIjoiRGF0UFQzMyIsIlN0YXR1cyI6IkZhbHNlIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiUjAwMyIsIlJvbGUiOiJSMDAzIiwiZXhwIjoxNzE5ODUyNjI4LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUyNjYvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MjY2LyJ9.kIe8X7S6QECF9ZQG2mi2G4MG9i0NnDfEP0OAQXEnwhU', CAST(N'2024-07-01T23:35:28.980' AS DateTime), 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000026', N'NhatVV11', N'7f0736f1-995', NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000027', N'admin', N'123', NULL, N'R001', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[User] ([userID], [userName], [password], [branchID], [roleID], [token], [actionPeriod], [balance], [accessFail], [lastFail], [activeStatus]) VALUES (N'U0000028', N'duccoi', N'123', NULL, N'R003', NULL, NULL, 0, 0, CAST(N'1900-01-01T00:00:00.000' AS DateTime), 1)
GO
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000001', N'Phan Thị Thảo', N'Nguyên', N'NguyenPTT16804@yahoo.com', N'0201060944', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000002', N'Phạm Minh', N'Thư', N'ThuPM87572@yahoo.com', N'0293881846', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000003', N'Đặng Văn', N'Thái', N'ThaiDV35610@gmail.com', N'0903157204', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000004', N'Phan Trung', N'Phát', N'PhatPT98769@outlook.com', N'0367038081', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000005', N'Đặng Hồng', N'Thanh', N'ThanhDH81781@gmail.com', N'0961082840', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000006', N'Lê Pháp', N'Luật', N'LuatLP95050@outlook.com', N'0824708851', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000007', N'Phạm Thùy', N'Nhung', N'NhungPT59368@outlook.com', N'0820866631', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000008', N'Trần Tài', N'Trí', N'TriTT68811@yahoo.com', N'0376086631', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000009', N'Nguyễn Việt', N'Hoàng', N'HoangNV21446@outlook.com', N'0306258019', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000010', N'Trần Tài', N'Trí', N'TriTT66874@yahoo.com', N'0262919590', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000011', N'Trần Thị Thu', N'Huệ', N'HueTTT25223@outlook.com', N'0311496483', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000012', N'Phan Thị Thảo', N'Nguyên', N'NguyenPTT63019@gmail.com', N'0202807589', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000013', N'Phạm Minh', N'Thư', N'ThuPM03718@gmail.com', N'0363168967', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000014', N'Đặng Văn', N'Thái', N'ThaiDV88409@yahoo.com', N'0966449982', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000015', N'Phan Trung', N'Phát', N'PhatPT77034@outlook.com', N'0263887251', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000016', N'Đặng Hồng', N'Thanh', N'ThanhDH39827@gmail.com', N'0916361684', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000017', N'Lê Pháp', N'Luật', N'LuatLP83800@yahoo.com', N'0303258783', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000018', N'Phạm Thùy', N'Nhung', N'NhungPT99431@outlook.com', N'0359367625', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000019', N'Trần Tài', N'Trí', N'TriTT94800@yahoo.com', N'0202846787', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000020', N'Nguyễn Việt', N'Hoàng', N'HoangNV55646@yahoo.com', N'0866987219', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000021', N'Trần Tài', N'Trí', N'TriTT53739@yahoo.com', N'0353559652', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000022', N'Trần Thị Thu', N'Huệ', N'HueTTT90532@outlook.com', N'0243905446', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000023', N'Nguyễn Quyết', N'Tiến', N'TienNQ40735@outlook.com', N'0388666093', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000024', N'Lê Ngọc', N'Đạt', N'DatLN26400@outlook.com', N'0817614855', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000025', N'Phạm Tiến', N'Đạt', N'DatPT33487@gmail.com', N'0365045156', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000026', N'Võ Văn', N'Nhất', N'NhatVV11428@outlook.com', N'0891631388', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000027', N'Admin', N'Admin', N'ducdmse183990@fpt.edu.vn', N'0977300916', NULL, NULL)
INSERT [dbo].[UserDetail] ([userID], [firstName], [lastName], [email], [phone], [facebook], [img]) VALUES (N'U0000028', N'Đặng Minh', N'Đức', N'duccoi16082004@gmail.com', N'0891631388', NULL, NULL)
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
