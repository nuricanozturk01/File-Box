use FileBoxDb;

/*

USE master ;  
GO  
DROP DATABASE FileBoxDb;  
GO

*/

-- Create the filebox_user Table
CREATE TABLE filebox_user
(
    user_id uniqueidentifier PRIMARY KEY NOT NULL DEFAULT NEWID(), -- generated automatically
    first_name varchar(80) NOT NULL,
    last_name varchar(80) NOT NULL,
    email varchar(100) NOT NULL UNIQUE, -- unique
    username varchar(45) NOT NULL UNIQUE, -- unique
    password varchar(80) NOT NULL, -- encrypted on api
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

    FOREIGN KEY (parent_folder_id) REFERENCES filebox_folder(folder_id),
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








/*
-- ----------------------------------------------------- VIEWS -----------------------------------------------------

-- ----------------------------------------------------- VIEWS -----------------------------------------------------
*/















-- ----------------------------------------------------- FUNCTIONS -----------------------------------------------------

-- ----------------------------------------------------- FUNCTIONS -----------------------------------------------------









-- ----------------------------------------------------- EXAMPLES -----------------------------------------------------

-- FAKE DATAS GENERATED BY CHAT-GPT

exec filebox_insert_user  'Nuri Can', 'OZTURK', 'nuricanozturk', 'canozturk309@gmail.com', '12345'
exec filebox_insert_user  'Yagmur', 'Dolu', 'yagmur', 'yagmur@example.com', '12345'
exec filebox_insert_user  'ahmet', 'koc', 'ahmetkoc', 'ahmetkoc@example.com', '12345'
exec filebox_insert_user  'dogan', 'arik', 'dogan', 'dogan@example.com', '12345'
select * from filebox_user;

exec filebox_insert_folder NULL, '812EEE16-72AE-4D3E-A201-1010FC7DD172', 'johndoe', 'C:\Users\hp\Desktop\file_box\johndoe';
exec filebox_insert_folder 1, '5C69A30A-0B56-4A4D-A777-6B3656B14264', 'folder1', 'C:\Users\hp\Desktop\file_box\johndoe\folder1';
exec filebox_insert_folder 1, '5C69A30A-0B56-4A4D-A777-6B3656B14264', 'folder2', 'C:\Users\hp\Desktop\file_box\johndoe\folder2';

exec filebox_insert_folder NULL, 'B2D40BDD-2360-4A40-930F-F0A8348771C5', 'Root Folder', 'C:\Users\hp\Desktop\file_box\janesmith';
exec filebox_insert_folder 4, 'B2D40BDD-2360-4A40-930F-F0A8348771C5', 'subfolder', 'C:\Users\hp\Desktop\file_box\janesmith\subfolder';


exec filebox_insert_file 'document1.txt', 'Text', 1024, 'root/user_id_1/document1.txt', 1;
exec filebox_insert_file 'image1.jpg', 'Image', 2048, 'root/user_id_1/image1.jpg', 1;
exec filebox_insert_file 'document2.txt', 'Text', 512, 'root/user_id_1/Subfolder1/document2.txt', 2;
exec filebox_insert_file 'document3.txt', 'Text', 768, 'root/user_id_1/Subfolder1/document3.txt', 2;
exec filebox_insert_file 'image2.jpg', 'Image', 3072, 'root/user_id_2/image2.jpg', 4;


delete filebox_user from filebox_user where filebox_user.username = 'johndoe';

exec filebox_folder_deleteById 6, "5C69A30A-0B56-4A4D-A777-6B3656B14264";
exec filebox_user_deleteByUsername 'janesmith';
go
-- ----------------------------------------------------- EXAMPLES -----------------------------------------------------
select * from filebox_file;
select * from filebox_folder;
select * from filebox_user;

delete filebox_file from filebox_file where filebox_file.file_name = 'image1.jpg';
delete filebox_folder from filebox_folder where filebox_folder.folder_id = 2;


update filebox_folder set folder_path = 'ahmetkoc\dir1\ders' where folder_path = 'C:\Users\hp\Desktop\file_box\ahmetkoc\dir1\ders';
update filebox_folder set folder_name = 'ders' where folder_name = 'derss';


delete filebox_folder where folder_id = 2;
/*



use FileBoxDb;
go

drop table filebox_file;
drop table filebox_folder;
drop table filebox_user;
go

*/

