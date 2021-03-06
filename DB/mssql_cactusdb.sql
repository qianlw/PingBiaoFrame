USE [CactusDB]
GO
/****** Object:  Table [dbo].[sys_role]    Script Date: 02/25/2016 10:29:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_role]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[sys_role](
	[Role_Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](20) NULL,
	[ActionIds] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.sys_role] PRIMARY KEY CLUSTERED 
(
	[Role_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET IDENTITY_INSERT [dbo].[sys_role] ON
INSERT [dbo].[sys_role] ([Role_Id], [RoleName], [ActionIds]) VALUES (1, N'超级管理员', N'1,2')
INSERT [dbo].[sys_role] ([Role_Id], [RoleName], [ActionIds]) VALUES (2, N'普通管理员', N'9,10,11,12,1,4,5')
INSERT [dbo].[sys_role] ([Role_Id], [RoleName], [ActionIds]) VALUES (3, N'普通管理员2', N'3,1')
SET IDENTITY_INSERT [dbo].[sys_role] OFF
/****** Object:  Table [dbo].[sys_user]    Script Date: 02/25/2016 10:29:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_user]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[sys_user](
	[User_Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Password] [nvarchar](255) NULL,
	[NickName] [nvarchar](32) NULL,
	[Avatar] [nvarchar](128) NULL,
	[Email] [nvarchar](32) NULL,
	[Phone] [nvarchar](32) NULL,
	[Qq] [nvarchar](16) NULL,
	[AddTime] [datetime] NOT NULL,
	[LastLoginTime] [datetime] NOT NULL,
	[LastLoginIp] [nvarchar](32) NULL,
	[IsSuperUser] [bit] NOT NULL,
	[IsLock] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.sys_user] PRIMARY KEY CLUSTERED 
(
	[User_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET IDENTITY_INSERT [dbo].[sys_user] ON
INSERT [dbo].[sys_user] ([User_Id], [RoleId], [Name], [Password], [NickName], [Avatar], [Email], [Phone], [Qq], [AddTime], [LastLoginTime], [LastLoginIp], [IsSuperUser], [IsLock]) VALUES (1, 1, N'admin', N'19a2854144b63a8f7617a6f22519b12', N'漫漫洒洒', N'/Upload/Avatars/1.png', N'702295399@qq.com', N'18714253698', N'702295399', CAST(0x0000A4B201736F94 AS DateTime), CAST(0x0000A4D20100F79D AS DateTime), N'127.0.0.1', 1, 0)
INSERT [dbo].[sys_user] ([User_Id], [RoleId], [Name], [Password], [NickName], [Avatar], [Email], [Phone], [Qq], [AddTime], [LastLoginTime], [LastLoginIp], [IsSuperUser], [IsLock]) VALUES (6, 2, N'lww01', N'cebfd1559b68d67688884d7a3d3e8c', N'测试用户1', N'/Images/avatar.jpg', N'702295399@qq.com', N'18768547946', N'702295399', CAST(0x0000A4BA015C6E3F AS DateTime), CAST(0x0000A4C400F4472C AS DateTime), N'127.0.0.1', 0, 0)
SET IDENTITY_INSERT [dbo].[sys_user] OFF
/****** Object:  ForeignKey [FK_dbo.sys_user_dbo.sys_role_RoleId]    Script Date: 02/25/2016 10:29:31 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.sys_user_dbo.sys_role_RoleId]') AND parent_object_id = OBJECT_ID(N'[dbo].[sys_user]'))
ALTER TABLE [dbo].[sys_user]  WITH CHECK ADD  CONSTRAINT [FK_dbo.sys_user_dbo.sys_role_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[sys_role] ([Role_Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.sys_user_dbo.sys_role_RoleId]') AND parent_object_id = OBJECT_ID(N'[dbo].[sys_user]'))
ALTER TABLE [dbo].[sys_user] CHECK CONSTRAINT [FK_dbo.sys_user_dbo.sys_role_RoleId]
GO
