using RepositoryLib.Models;

namespace FileBoxTest.MockTests
{
    public static class MockDataUtil
    {

        public static List<FileboxFolder> GetFileboxFolder()
        {
            return new List<FileboxFolder>
            {
                new FileboxFolder
                {
                    UserId = Guid.Parse(Util.USER_ID),
                    User = GetFileBoxUser().First(),
                    UpdatedDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    FolderId = 23L,
                    FileboxFiles = new List<FileboxFile>(),
                    FolderName = "ahmetkoc",
                    FolderPath = "ahmetkoc",
                    ParentFolder = null,
                    ParentFolderId = null,
                    InverseParentFolder = new List<FileboxFolder>()
                }
            };
        }

        public static List<FileboxUser> GetFileBoxUser()
        {
            return new List<FileboxUser>
            {
                new FileboxUser
                {
                    Username = "ahmetkoc",
                    Email = "nuricanozturk01@gmail.com",
                    FirstName = "Ahmet",
                    LastName = "KOC",
                    FileboxFolders = new List<FileboxFolder>(),
                    LastToken = "",
                    Password = "$2a$11$ry4X5CGhucEQhoi/SsX7K.IZeGDlZuAVCTWjPJwPN6zVYlHsSRUye",
                    UserId = Guid.Parse(Util.USER_ID),
                    ResetPasswordToken= ""
                }
            };
        }

        internal static List<FileboxFile> GetFakeFilteredFiles()
        {
            return new List<FileboxFile>
            {
                new FileboxFile
                {
                    CreatedDate = DateTime.Now,
                    FileId = 1,
                    FileName = "node.pdf",
                    FilePath = "ahmetkoc/deneme1/node.pdf",
                    FileSize = 1_234_543,
                    FileType = ".pdf",
                    Folder = GetFakeFolder(),
                    FolderId = 24L,
                    UpdatedDate = DateTime.Now
                },
                new FileboxFile
                {
                    CreatedDate = DateTime.Now,
                    FileId = 2,
                    FileName = "notes.docx",
                    FilePath = "ahmetkoc/deneme1/notes.docx",
                    FileSize = 1_234_543,
                    FileType = ".docx",
                    Folder = GetFakeFolder(),
                    FolderId = 24L,
                    UpdatedDate = DateTime.Now
                },
                new FileboxFile
                {
                    CreatedDate = DateTime.Now,
                    FileId = 3,
                    FileName = "react.pdf",
                    FilePath = "ahmetkoc/deneme1/react.pdf",
                    FileSize = 1_234_543,
                    FileType = ".pdf",
                    Folder = GetFakeFolder(),
                    FolderId = 24L,
                    UpdatedDate = DateTime.Now
                },
                new FileboxFile
                {
                    CreatedDate = DateTime.Now,
                    FileId = 4,
                    FileName = "presentation.pptx",
                    FilePath = "ahmetkoc/deneme1/presentation.pptx",
                    FileSize = 1_234_543,
                    FileType = ".pptx",
                    Folder = GetFakeFolder(),
                    FolderId = 24L,
                    UpdatedDate = DateTime.Now
                },
                new FileboxFile
                {
                    CreatedDate = DateTime.Now,
                    FileId = 5,
                    FileName = "license.txt",
                    FilePath = "ahmetkoc/deneme1/license.txt",
                    FileSize = 1_234_543,
                    FileType = ".txt",
                    Folder = GetFakeFolder(),
                    FolderId = 24L,
                    UpdatedDate = DateTime.Now
                },

            };
        }
        internal static FileboxFolder GetFakeFolder()
        {
            return new FileboxFolder
            {
                CreationDate = DateTime.Now,
                FileboxFiles = new List<FileboxFile>(),
                FolderId = 24,
                FolderName = "deneme1",
                ParentFolder = null,
                InverseParentFolder = new List<FileboxFolder>(),
                ParentFolderId = null,
                FolderPath = "ahmetkoc/deneme1",
                UpdatedDate = DateTime.Now,
                User = null,
                UserId = Guid.Parse(Util.USER_ID)
            };
        }

        internal static List<FileboxFolder> GetFakeFoldersByUserCorrectUserId()
        {
            return new List<FileboxFolder>
            {
                new FileboxFolder
                {
                    CreationDate = DateTime.Now,
                    FileboxFiles = new List<FileboxFile>(),
                    FolderId = 5,
                    FolderName = "one",
                    ParentFolder = null,
                    InverseParentFolder = new List<FileboxFolder>(),
                    ParentFolderId = null,
                    FolderPath = "ahmetkoc/one",
                    UpdatedDate = DateTime.Now,
                    User = null,
                    UserId = Guid.Parse(Util.USER_ID)
                },
                 new FileboxFolder
                {
                    CreationDate = DateTime.Now,
                    FileboxFiles = new List<FileboxFile>(),
                    FolderId = 6,
                    FolderName = "two",
                    ParentFolder = null,
                    InverseParentFolder = new List<FileboxFolder>(),
                    ParentFolderId = null,
                    FolderPath = "ahmetkoc/two",
                    UpdatedDate = DateTime.Now,
                    User = null,
                    UserId = Guid.Parse(Util.USER_ID)
                },
                  new FileboxFolder
                {
                    CreationDate = DateTime.Now,
                    FileboxFiles = new List<FileboxFile>(),
                    FolderId = 7,
                    FolderName = "three",
                    ParentFolder = null,
                    InverseParentFolder = new List<FileboxFolder>(),
                    ParentFolderId = 6,
                    FolderPath = "ahmetkoc/two/three",
                    UpdatedDate = DateTime.Now,
                    User = null,
                    UserId = Guid.Parse(Util.USER_ID)
                },
                   new FileboxFolder
                {
                    CreationDate = DateTime.Now,
                    FileboxFiles = new List<FileboxFile>(),
                    FolderId = 8,
                    FolderName = "four",
                    ParentFolder = null,
                    InverseParentFolder = new List<FileboxFolder>(),
                    ParentFolderId = 7,
                    FolderPath = "ahmetkoc/two/three/four",
                    UpdatedDate = DateTime.Now,
                    User = null,
                    UserId = Guid.Parse(Util.USER_ID)
                },
                new FileboxFolder
                {
                    CreationDate = DateTime.Now,
                    FileboxFiles = new List<FileboxFile>(),
                    FolderId = 24,
                    FolderName = "deneme1",
                    ParentFolder = null,
                    InverseParentFolder = new List<FileboxFolder>(),
                    ParentFolderId = null,
                    FolderPath = "ahmetkoc/deneme1",
                    UpdatedDate = DateTime.Now,
                    User = null,
                    UserId = Guid.Parse(Util.USER_ID)
                }
            };
        }


        internal static List<FileboxFolder> GetFakeFoldersByUserWrongUserId()
        {
            var userId = Guid.NewGuid();
            return new List<FileboxFolder>
            {
                new FileboxFolder
                {
                    CreationDate = DateTime.Now,
                    FileboxFiles = new List<FileboxFile>(),
                    FolderId = 5,
                    FolderName = "one",
                    ParentFolder = null,
                    InverseParentFolder = new List<FileboxFolder>(),
                    ParentFolderId = null,
                    FolderPath = "ahmetkoc/one",
                    UpdatedDate = DateTime.Now,
                    User = null,
                    UserId = userId
                },
                 new FileboxFolder
                {
                    CreationDate = DateTime.Now,
                    FileboxFiles = new List<FileboxFile>(),
                    FolderId = 6,
                    FolderName = "two",
                    ParentFolder = null,
                    InverseParentFolder = new List<FileboxFolder>(),
                    ParentFolderId = null,
                    FolderPath = "ahmetkoc/two",
                    UpdatedDate = DateTime.Now,
                    User = null,
                    UserId = userId
                },
                  new FileboxFolder
                {
                    CreationDate = DateTime.Now,
                    FileboxFiles = new List<FileboxFile>(),
                    FolderId = 7,
                    FolderName = "three",
                    ParentFolder = null,
                    InverseParentFolder = new List<FileboxFolder>(),
                    ParentFolderId = 6,
                    FolderPath = "ahmetkoc/two/three",
                    UpdatedDate = DateTime.Now,
                    User = null,
                    UserId = userId
                },
                   new FileboxFolder
                {
                    CreationDate = DateTime.Now,
                    FileboxFiles = new List<FileboxFile>(),
                    FolderId = 8,
                    FolderName = "four",
                    ParentFolder = null,
                    InverseParentFolder = new List<FileboxFolder>(),
                    ParentFolderId = 7,
                    FolderPath = "ahmetkoc/two/three/four",
                    UpdatedDate = DateTime.Now,
                    User = null,
                    UserId = userId
                }
            };
        }

        internal static FileboxFolder GetInvalidFakeFolder()
        {
            return new FileboxFolder
            {
                CreationDate = DateTime.Now,
                FileboxFiles = new List<FileboxFile>(),
                FolderId = -4,
                FolderName = "deneme1",
                ParentFolder = null,
                InverseParentFolder = new List<FileboxFolder>(),
                ParentFolderId = null,
                FolderPath = "ahmetkoc/deneme1",
                UpdatedDate = DateTime.Now,
                User = null,
                UserId = Guid.Parse(Util.USER_ID)
            };
        }


        internal static List<FileboxUser> GetValidUsers()
        {
            return new List<FileboxUser>
            {
                new FileboxUser
                {
                    Username = "ahmetkoc",
                    Email = "nuricanozturk01@gmail.com",
                    FirstName = "Ahmet",
                    LastName = "KOC",
                    FileboxFolders = new List<FileboxFolder>(),
                    LastToken = "",
                    Password = "$2a$11$ry4X5CGhucEQhoi/SsX7K.IZeGDlZuAVCTWjPJwPN6zVYlHsSRUye",
                    UserId = Guid.Parse(Util.USER_ID),
                    ResetPasswordToken= ""
                }
            };
        }
    }
}
