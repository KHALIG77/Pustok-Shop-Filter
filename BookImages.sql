/*
   Monday, May 8, 202311:48:46 AM
   User: 
   Server: DESKTOP-497OP8B\SQLEXPRESS
   Database: Pustok
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.BookImages
	DROP COLUMN SliderImageUrl
GO
ALTER TABLE dbo.BookImages SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.BookImages', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.BookImages', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.BookImages', 'Object', 'CONTROL') as Contr_Per 