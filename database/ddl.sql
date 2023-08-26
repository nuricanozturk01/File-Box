use FileBoxDb;

/*

USE master ;  
GO  
DROP DATABASE FileBoxDb;  
GO

*/
/*
use FileBoxDb;
go

drop table filebox_file;
drop table filebox_folder;
drop table filebox_user;
go

*/



use FileBoxDb;
go

drop table filebox_file;
drop table filebox_folder;
drop table filebox_user;
go



-- Create the filebox_user Table
CREATE TABLE filebox_user
(
    user_id uniqueidentifier PRIMARY KEY NOT NULL DEFAULT NEWID(), -- generated automatically
    first_name varchar(80) NOT NULL,
    last_name varchar(80) NOT NULL,
    email varchar(100) NOT NULL UNIQUE, -- unique
    username varchar(45) NOT NULL UNIQUE, -- unique
    password varchar(128) NOT NULL, -- encrypted on api
	last_token varchar(256),
	reset_password_token varchar(256)
);

-- Create the filebox_folder Table
CREATE TABLE filebox_folder
(
    folder_id bigint PRIMARY KEY IDENTITY(1, 1), -- primary key folder id. if it is a root path, folder id is NULL
    parent_folder_id bigint, -- parent folder id used antipattern (Always depends on one's parent)
    user_id uniqueidentifier NOT NULL, -- user id for detect folder owner.
    folder_name varchar(255) NOT NULL, -- only name of folder
    creation_date datetime DEFAULT GETDATE(), -- creation date of folder
    updated_date datetime DEFAULT GETDATE(),
    folder_path varchar(255) NOT NULL,

    FOREIGN KEY (parent_folder_id) REFERENCES filebox_folder(folder_id) , -- on update cascade
    FOREIGN KEY (user_id) REFERENCES filebox_user(user_id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Create the filebox_file Table
CREATE TABLE filebox_file
(
    file_id bigint PRIMARY KEY NOT NULL IDENTITY(1, 1), -- primary key file id
    file_name varchar(255) NOT NULL, -- only file name
    file_type varchar(45) NOT NULL DEFAULT 'N/A', -- if not entered the file type, default N/A
    file_size bigint NOT NULL, -- stored byte
    file_path varchar(255) NOT NULL, -- file fullpath 
    created_date datetime DEFAULT GETDATE(), -- created file date
    updated_date datetime DEFAULT GETDATE(), -- updated file date

    folder_id bigint NOT NULL, -- foreign key. if null, root folder

    FOREIGN KEY (folder_id) REFERENCES filebox_folder(folder_id) ON DELETE CASCADE on update cascade
);

exec filebox_insert_user  'Nuri Can', 'OZTURK', 'nuricanozturk', 'canozturk309@gmail.com', '12345'
exec filebox_insert_user  'Ahmet', 'KOÇ', 'ahmetkoc', 'nuricanozturk01@gmail.com', '12345'
exec filebox_insert_user  'Halil Can', 'OZTURK', 'halilcanozturk', 'nuricanozturk02@gmail.com', '12345'
exec filebox_insert_user  'Burak', 'Karaduman', 'burak', 'lkjj93670@gmail.com', '12345'
select * from filebox_file;
select * from filebox_folder;
select * from filebox_user;

update filebox_folder set folder_path = 'nuricanozturk\ABC' where folder_path = 'nuricanozturk\ABCC';
delete from filebox_file where file_id = 18;
-- ----------------------------------------------------- STORED PROCEDURES-----------------------------------------------------

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO










/*
 *
 * Insert User to database with given parameters
 * parameters (firstname, lastname, username, email, password) uuid generated automatically
 *
*/
drop procedure if exists dbo.filebox_insert_user ;
go

create procedure filebox_insert_user @firstname varchar(80), 
									 @lastname varchar(80), 
									 @username varchar(45),
									 @email varchar(100),
									 @password varchar(80)
as 
begin
	set nocount on;
	insert into filebox_user (user_id, first_name, last_name, username, email, password) values (newid(), @firstname, @lastname, @username, @email, @password);
end
go

















/*
 *
 * Insert folder to database with given parameters
 * parameters (parentFolderId, userId, folderName, folderPath)
 *
*/
drop procedure if exists dbo.filebox_insert_folder;
go

create procedure filebox_insert_folder @parent_folder bigint, 
									   @user_id uniqueidentifier, 
									   @folder_name varchar(254),
									   @folder_path varchar(259)
as 
begin
	set nocount on;

	insert into filebox_folder (parent_folder_id, user_id, folder_name, folder_path) values (@parent_folder, @user_id, @folder_name, @folder_path);
end
go











/*
 *
 * Insert file to database with given parameters
 * parameters (folderId, fileName, filePath)
 *
*/
drop procedure if exists dbo.filebox_insert_file;
go

CREATE PROCEDURE filebox_insert_file 
						@file_name varchar(254), 
						@file_type varchar(45),
						@file_size bigint,
						@file_path varchar(259),
						@folder_id bigint
as 
begin
	set nocount on;
	insert into filebox_file (file_name, file_type, file_size, file_path, folder_id) values (@file_name, @file_type, @file_size, @file_path, @folder_id);
end
go











/*
 *
 * delete files by given id parameters. 
 * parameters (fileId, folderId)
 *
*/
drop procedure if exists dbo.filebox_file_deleteById;
go

CREATE PROCEDURE filebox_file_deleteById @file_id bigint, @folder_id bigint
as
begin
	set nocount on;
	delete from filebox_file where filebox_file.file_id = @folder_id and 
								   filebox_file.folder_id = @folder_id;
end
go










/*
 *
 * delete folders with given folder and user id.
 * Deleted all files on folders.
 *
*/

drop procedure if exists dbo.filebox_folder_deleteById;

go
CREATE PROCEDURE filebox_folder_deleteById @folder_id bigint, @user_id uniqueidentifier
as
begin
	set nocount on;
	delete from filebox_file where filebox_file.folder_id = @folder_id;

	delete from filebox_folder where filebox_folder.folder_id = @folder_id and
									 filebox_folder.user_id = @user_id
end
go







/*
 *
 * delete user with given uuid
 * 
 *
*/
drop procedure if exists dbo.filebox_user_deleteById;
go

CREATE PROCEDURE filebox_user_deleteById @user_id uniqueidentifier
as
begin
	set nocount on;

	delete from filebox_user where filebox_user.user_id = @user_id;
end
go






/*
 *
 * delete user with given username
 * 
 *
*/

drop procedure if exists dbo.filebox_user_deleteByUsername;
go

CREATE PROCEDURE filebox_user_deleteByUsername @username varchar(80)
as
begin
	set nocount on;

	delete from filebox_user where filebox_user.username = @username;
end
go
-- ----------------------------------------------------- STORED PROCEDURES-----------------------------------------------------




-- ----------------------------------------------------- TRIGGERS -----------------------------------------------------

-- delete user informations when user removed
drop trigger if exists dbo.RemoveFilesWhenDeletedNonParentFolder;
go

create trigger RemoveFilesWhenDeletedNonParentFolder on filebox_folder
after delete
as
begin
    set nocount on;
    
    delete from filebox_file where folder_id in (select folder_id from deleted);
end;
-- ----------------------------------------------------- TRIGGERS -----------------------------------------------------



-- ----------------------------------------------------- EXAMPLES -----------------------------------------------------