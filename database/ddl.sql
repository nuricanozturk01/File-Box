use FileBoxDb;

/*

USE master ;  
GO  
DROP DATABASE FileBoxDb;  
GO

*/



/*
 * Represent the users' root folder 
 *
*/
create table filebox_root_folder
(
	filebox_rootFolderId bigint primary key not null identity(1,1),
	filebox_rootFolderName varchar(80) unique not null,
	filebox_rootFolderPath varchar(80) unique not null
);


/*
 * Represent the user
 *
*/
create table filebox_user
(
	filebox_userId uniqueidentifier primary key not null,
	filebox_firstName varchar(80) not null,
	filebox_lastName varchar(80) not null,
	filebox_username varchar(45) unique not null,
	filebox_password varchar(80) not null,
	filebox_rootFolderId bigint not null

	foreign key (filebox_rootFolderId) references filebox_root_folder(filebox_rootFolderId) 
				on delete cascade on update cascade

);

/*
 * Represent folders.
 * parentFolderId represent the subfolders
 *
*/
create table filebox_folder
(
	filebox_folderId bigint primary key identity(1,1),
	filebox_parentFolderId bigint,
	filebox_userId uniqueidentifier  not null,
	filebox_folderName varchar(254) not null,
	filebox_folderPath varchar(259) not null

	foreign key (filebox_parentFolderId) references filebox_folder(filebox_folderId) on delete no action on update no action,
	foreign key (filebox_userId) references filebox_user(filebox_userId) on update cascade
);

/*
 * Represent the files.
 *
*/
create table filebox_file
(
	filebox_fileId int primary key not null identity(1,1),
	filebox_fileName varchar(254) not null,
	filebox_filePath varchar(259) not null,
	filebox_folderId bigint not null,

	foreign key (filebox_folderId) references filebox_folder(filebox_folderId)
);


		

-- ----------------------------------------------------- STORED PROCEDURES-----------------------------------------------------

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
 *
 * Create Root Folder
 * 
 *
*/
CREATE PROCEDURE filebox_insert_rootFolder @folderName_param varchar(254), @folderPath_param varchar(259)
AS 
BEGIN
	SET NOCOUNT ON;

	INSERT INTO filebox_root_folder(filebox_rootFolderName, filebox_rootFolderPath) 
				VALUES (@folderName_param, @folderPath_param);
END
GO











/*
 *
 * Insert User to database with given parameters
 * parameters (firstname, lastname, username, password) uuid generated automatically
 *
*/

CREATE PROCEDURE filebox_insert_user @firstname_param varchar(80), 
									 @lastname_param varchar(80), 
									 @username_param varchar(45),
									 @password_param varchar(80),
									 @rootFolder_param bigint
AS 
BEGIN
	SET NOCOUNT ON;

	INSERT INTO filebox_user (filebox_userId, filebox_firstName, filebox_lastName, filebox_username, filebox_password, filebox_rootFolderId) 
				VALUES (NEWID(), @firstname_param, @lastname_param, @username_param, @password_param, @rootFolder_param);
END
GO


/*
 *
 * Insert folder to database with given parameters
 * parameters (parentFolderId, userId, folderName, folderPath)
 *
*/
CREATE PROCEDURE filebox_insert_folder @parentFolder_param bigint, 
									   @userId_param uniqueidentifier, 
									   @folderName_param varchar(254),
									   @folderPath_param varchar(259)
AS 
BEGIN
	SET NOCOUNT ON;

	INSERT INTO filebox_folder (filebox_parentFolderId, filebox_userId, filebox_folderName, filebox_folderPath) 
				VALUES (@parentFolder_param, @userId_param, @folderName_param, @folderPath_param);
END
GO






/*
 *
 * Insert file to database with given parameters
 * parameters (folderId, fileName, filePath)
 *
*/
CREATE PROCEDURE filebox_insert_file 
						@folderId_param bigint, 
						@fileName_param varchar(254), 
						@filePath_param varchar(259)
					
AS 
BEGIN
	SET NOCOUNT ON;
	INSERT INTO filebox_file (filebox_fileName, filebox_filePath, filebox_folderId) VALUES (@fileName_param, @filePath_param, @folderId_param);
END
GO







/*
 *
 * delete files by given id parameters. 
 * parameters (fileId, folderId)
 *
*/

CREATE PROCEDURE filebox_file_deleteById @fileId_param bigint, @folderId_param bigint
as
begin
	set nocount on;
	delete from filebox_file where filebox_file.filebox_fileId = @fileId_param and 
								   filebox_file.filebox_folderId = @folderId_param;
end
go










/*
 *
 * delete folders with given folder and user id.
 * Deleted all files on folders.
 *
*/
CREATE PROCEDURE filebox_folder_deleteById @folderId_param bigint, @userId_param uniqueidentifier
as
begin
	set nocount on;
	delete from filebox_file where filebox_file.filebox_folderId = @folderId_param;

	delete from filebox_folder where filebox_folder.filebox_folderId = @folderId_param and
									 filebox_folder.filebox_userId = @userId_param
end
go







/*
 *
 * delete user with given uuid
 * 
 *
*/
CREATE PROCEDURE filebox_user_deleteById @userId_param uniqueidentifier
as
begin
	set nocount on;

	delete from filebox_user where filebox_user.filebox_userId = @userId_param;
end
go






/*
 *
 * delete user with given username
 * 
 *
*/
CREATE PROCEDURE filebox_user_deleteByUsername @username_param varchar(80)
as
begin
	set nocount on;

	delete from filebox_user where filebox_user.filebox_username = @username_param;
end
go
-- ----------------------------------------------------- STORED PROCEDURES-----------------------------------------------------




-- ----------------------------------------------------- TRIGGERS -----------------------------------------------------

-- ----------------------------------------------------- TRIGGERS -----------------------------------------------------









-- ----------------------------------------------------- VIEWS -----------------------------------------------------
CREATE VIEW viewUserFolders as 
	select usr.filebox_username, dir.filebox_folderName, dir.filebox_folderPath
			from filebox_user as usr, filebox_folder as dir where dir.filebox_userId = usr.filebox_userId;
go 

CREATE VIEW viewUserFiles as 
	select usr.filebox_userId, usr.filebox_username, fi.filebox_fileName, fi.filebox_filePath
			from filebox_user as usr, filebox_file as fi, filebox_folder as dir
			where fi.filebox_folderId = dir.filebox_folderId and dir.filebox_userId = usr.filebox_userId;
GO 
-- ----------------------------------------------------- VIEWS -----------------------------------------------------
















-- ----------------------------------------------------- FUNCTIONS -----------------------------------------------------


create function getFoldersByUsername(@username varchar(80)) 
returns table as
	RETURN (select * from viewUserFolders where viewUserFolders.filebox_username = @username);
go



create function getFilesByUsername(@username varchar(80)) 
returns table as
	RETURN (select * from viewUserFiles where viewUserFiles.filebox_username = @username);
go
-- ----------------------------------------------------- FUNCTIONS -----------------------------------------------------









-- ----------------------------------------------------- EXAMPLES -----------------------------------------------------
-- insert root folders
exec filebox_insert_rootFolder "Nuri Can", 'C:\Users\hp\Desktop\file_box\Nuri Can';
exec filebox_insert_rootFolder "Halil Can", 'C:\Users\hp\Desktop\file_box\Halil Can';

-- insert user
insert into filebox_user (filebox_userId, filebox_firstName, filebox_lastName, filebox_username, filebox_password, filebox_rootFolderId) values 
				('261F115D7-0324-41DC-928E-0025045F553E', 'Nuri Can', 'Ozturk', 'nuricanozturk','12345', 1);

-- declare @id_1 uniqueidentifier = CONVERT(uniqueidentifier,'261F115D7-0324-41DC-928E-0025045F553E');
DECLARE @id_1 uniqueidentifier = CONVERT(uniqueidentifier,'261F115D7-0324-41DC-928E-0025045F553E');

INSERT INTO filebox_user (filebox_userId, filebox_firstName, filebox_lastName, filebox_username, filebox_password, filebox_rootFolderId) 
				VALUES (@id_1, 'Nuri Can', 'Ozturk', 'nuricanozturk','12345', 1);


insert into filebox_user (filebox_userId, filebox_firstName, filebox_lastName, filebox_username, filebox_password, filebox_rootFolderId) values 
				('1F0ECEB5-AC39-4E08-A45B-E8B82A856264', 'Halil Can', 'Ozturk','halilozturk', '12312455', 2);


-- creating first folder to root path
exec filebox_insert_folder NULL, "A39BA087-FB27-469F-B199-101BF8CF1D4B", "FolderNuri", "Nuri Can\";
exec filebox_insert_folder NULL, "A39BA087-FB27-469F-B199-101BF8CF1D4B", "FolderNuri_Ders", "Nuri Can\";

-- Creating subfolders
exec filebox_insert_folder 3, "A39BA087-FB27-469F-B199-101BF8CF1D4B", "Design Patterns", "Nuri Can\FolderNuri_Ders";
exec filebox_insert_folder 2, "A39BA087-FB27-469F-B199-101BF8CF1D4B", "Creational Design Patterns",  "Nuri Can\FolderNuri_Ders\Creational Design Patterns";

-- creating first folder to root path
exec filebox_insert_folder NULL, "6FB05F7F-594C-46C1-98BD-FD54B026DA58", "FolderHalil", "Halil Can\"; 
exec filebox_insert_folder NULL, "6FB05F7F-594C-46C1-98BD-FD54B026DA58", "FolderHalil_Ders", "Halil Can\"; 


-- creating some files on nurican
exec filebox_insert_file 1, "first.txt", "/Nuri Can/Folder1/first.txt";
exec filebox_insert_file 3, "design_pattern_notes.pdf", "Nuri Can\FolderNuri_Ders";
exec filebox_insert_file 5, "creational_design_pattern_notes.pdf", "Nuri Can\FolderNuri_Ders\Creational Design Patterns";

-- creating some files on halil
exec filebox_insert_file 7, "first.txt", "/Halil Can/Folder1/first.txt";



-- delete some folders
-- exec filebox_folder_deleteById 5, "61F115D7-0324-41DC-928E-0025045F553E";
select * from viewUserFolders where viewUserFolders.filebox_username = 'nuricanozturk';



select * from  dbo.getFilesByUsername('nuricanozturk');

select * from filebox_root_folder;
select * from filebox_user;
select * from filebox_folder;
select * from filebox_file;
-- exec filebox_insert_folder NULL





/*

go
exec filebox_folder_deleteById 2, "A39BA087-FB27-469F-B199-101BF8CF1D4B";
go






go
exec filebox_file_deleteById 6, 6;
go


go
exec filebox_insert_file 6, "project2.txt", "Nuri Can/projects";
go

go
exec filebox_insert_folder NULL, "E14FCE8D-F89E-4287-8592-836F15B7D538", "Folder3" ,".../Folder1/Folder3";
go


go
exec filebox_insert_folder 5, "D0160896-75C6-4136-AABC-159A3B86A027", "projects" ,"Nuri Can/projects";
go

/*

GO  
EXEC filebox_insert_user "Nuri Can", "OZTURK", "nuricanozturk", "12345"  
GO  


select * from filebox_user;*/

select * from filebox_folder;
select * from filebox_file;

*/
-- ----------------------------------------------------- EXAMPLES -----------------------------------------------------