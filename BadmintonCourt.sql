USE [master]
GO
/****** Object:  Database [BadmintonCourt]    Script Date: 6/29/2024 3:52:45 AM ******/
CREATE DATABASE [BadmintonCourt]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BadmintonCourt', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\BadmintonCourt.mdf' , SIZE = 3136KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'BadmintonCourt_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\BadmintonCourt_log.ldf' , SIZE = 832KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
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
ALTER DATABASE [BadmintonCourt] SET AUTO_CREATE_STATISTICS ON 
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
USE [BadmintonCourt]
GO
/****** Object:  Table [dbo].[BookedSlot]    Script Date: 6/29/2024 3:52:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BookedSlot](
	[slotID] [varchar](30) NOT NULL,
	[startTime] [datetime] NOT NULL,
	[endTime] [datetime] NOT NULL,
	[courtID] [varchar](30) NOT NULL,
	[bookingID] [varchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[slotID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Booking]    Script Date: 6/29/2024 3:52:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Booking](
	[bookingID] [varchar](30) NOT NULL,
	[amount] [float] NOT NULL,
	[bookingType] [int] NOT NULL,
	[userID] [varchar](30) NOT NULL,
	[bookingDate] [datetime] NOT NULL,
	[changeLog] [int] not null,
PRIMARY KEY CLUSTERED 
(
	[bookingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Court]    Script Date: 6/29/2024 3:52:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CourtBranch]    Script Date: 6/29/2024 3:52:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CourtBranch](
	[branchID] [varchar](30) NOT NULL,
	[location] [varchar](50) NOT NULL,
	[branchName] [varchar](50) NOT NULL,
	[branchPhone] [varchar](10) NOT NULL,
	[branchImg] [varchar](3000) NULL,
	[branchStatus] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[branchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Discount]    Script Date: 6/29/2024 3:52:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Discount](
	[discountID] [varchar](30) NOT NULL,
	[amount] [float] NOT NULL,
	[proportion] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[discountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 6/29/2024 3:52:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Feedback](
	[feedbackID] [varchar](30) NOT NULL,
	[rating] [int] NOT NULL,
	[content] [nvarchar](500) NOT NULL,
	[userID] [varchar](30) NULL,
	[branchID] [varchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[feedbackID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 6/29/2024 3:52:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Role]    Script Date: 6/29/2024 3:52:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Role](
	[roleID] [varchar](30) NOT NULL,
	[role] [varchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[roleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[User]    Script Date: 6/29/2024 3:52:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[userID] [varchar](30) NOT NULL,
	[userName] [varchar](50) NULL,
	[password] [varchar](50) NULL,
	[branchID] [varchar](30) NULL,
	[roleID] [varchar](30) NOT NULL,
	[token] [varchar](1000) null,
	[actionPeriod] datetime null,
	[balance] [float] NULL,
	[accessFail] [int] NULL,
	[lastFail] [datetime] NULL,
	[activeStatus] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[userID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserDetail]    Script Date: 6/29/2024 3:52:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
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
