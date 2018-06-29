/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

--GO
--SET IDENTITY_INSERT [dbo].[IsoRequestStatus] ON 

--GO
--INSERT [dbo].[IsoRequestStatus] ([Id], [Name]) VALUES (1, N'Pending')
--GO
--INSERT [dbo].[IsoRequestStatus] ([Id], [Name]) VALUES (2, N'InProgress')
--GO
--INSERT [dbo].[IsoRequestStatus] ([Id], [Name]) VALUES (3, N'Completed')
--GO
--SET IDENTITY_INSERT [dbo].[IsoRequestStatus] OFF
--GO
