using RepositoryLib.Dal;
using RepositoryLib.Models;

namespace Service.Services.ScanService
{
    // Add reverse scan, if file or folder does not exists on db but exists on directory. Remove from db.
    public class ScanService : IScanService
    {
        private readonly UserRepositoryDal m_userRepositoryDal;
        private readonly FolderRepositoryDal m_folderRepositoryDal;
        private readonly FileRepositoryDal m_fileRepositoryDal;


        public ScanService(UserRepositoryDal userRepositoryDal, FolderRepositoryDal folderRepositoryDal, FileRepositoryDal fileRepositoryDal)
        {
            m_userRepositoryDal = userRepositoryDal;
            m_folderRepositoryDal = folderRepositoryDal;
            m_fileRepositoryDal = fileRepositoryDal;
        }

        public async Task<string> ScanUserAllFolders(Guid guid)
        {
            var user = await m_userRepositoryDal.FindByIdAsyncUser(guid); // find user by id

            var result = await scanFolders(user);

            return result;
        }
        private async Task<long[]> ReverseScan(FileboxUser user)
        {
            var affectedFileCount = 0;
            var affectedFolderCount = 0;

            // Find folders from db
            var folders = m_folderRepositoryDal.FindAllAsync().Result.Where(folder => folder.UserId == user.UserId);

            foreach (var folder in folders)
            {
                // folder in real directory
                var folderInfo = new DirectoryInfo(Util.DIRECTORY_BASE + folder.FolderPath);

                // if folder does not exists remove it
                if (folderInfo is null || !folderInfo.Exists)
                {
                    m_folderRepositoryDal.Delete(folder);
                    m_folderRepositoryDal.SaveChanges();
                    affectedFolderCount++;
                }
                else
                {
                    // if folder is not empty on real directory control the files
                    var files = await m_fileRepositoryDal.FindByFilterAsync(f => f.FolderId == folder.FolderId);

                    foreach (var file in files)
                    {
                        // Find real directory file.
                        var fi = new FileInfo(Util.DIRECTORY_BASE + file.FilePath);

                        //if file not exists on real directory remove it.
                        if (fi is null || !fi.Exists)
                        {
                            m_fileRepositoryDal.Delete(file);
                            await m_fileRepositoryDal.SaveChangesAsync();
                            affectedFileCount++;
                        }
                    }
                }


            }

            return new long[] { affectedFileCount, affectedFolderCount };
        }
        private async Task<string> scanFolders(FileboxUser user)
        {
            long affectedFolderCount = 0;
            long affectedFileCount = 0;

            var userDir = Util.DIRECTORY_BASE + user.Username; // file_box\\nuricanozturk etc

            var rootDir = new DirectoryInfo(userDir); // root directory specific user(nuricanozturk)

            var allDirs = rootDir.GetDirectories("*.*", SearchOption.AllDirectories).ToList(); // all directories with subdirectories
            //allDirs.Add(userDir)

            foreach (var dir in allDirs)
            {
                FileboxFolder? folder = null;

                if (!IsExistsOnDb(dir.Name, user.UserId).Result)
                {
                    folder = await addDirectoryToDb(dir, user);
                    affectedFolderCount++;
                }

                if (folder is null)
                    folder = m_folderRepositoryDal.FindByFilterAsync(f => f.FolderName == dir.Name && f.UserId == user.UserId).Result.FirstOrDefault();

                var files = dir.GetFiles();

                foreach (var file in files)
                {
                    if (!IsExistFileOnDirectory(folder, file, user.UserId).Result)
                    {
                        AddFileToDirectory(folder, file);
                        affectedFileCount++;
                    }
                }
            }
            var reverseScan = await ReverseScan(user);
            affectedFileCount += reverseScan[0];
            affectedFolderCount += reverseScan[1];
            return "affected folder count: " + affectedFolderCount + ", affected file count: " + affectedFileCount;
        }

        private void AddFileToDirectory(FileboxFolder folder, FileInfo file)
        {
            var fileBox = new FileboxFile(folder.FolderId, file.Name, folder.FolderPath + "\\" + file.Name, file.Extension, file.Length);


            folder.FileboxFiles.Add(fileBox);

            m_folderRepositoryDal.Update(folder);
            m_folderRepositoryDal.SaveChanges();
        }

        private async Task<bool> IsExistFileOnDirectory(FileboxFolder dir, FileInfo file, Guid userId)
        {
            var result = await m_fileRepositoryDal.FindByFilterAsync(f => f.FileName == file.Name &&
                                                                          f.FolderId == dir.FolderId &&
                                                                          dir.UserId == userId);

            return result.FirstOrDefault() != null;
        }

        private async Task<FileboxFolder> addDirectoryToDb(DirectoryInfo dir, FileboxUser user)
        {
            string[] pathParts = dir.FullName.Split('\\');

            string? parent = string.Join("\\", pathParts.Take(pathParts.Length - 1)); // find parent name


            var folder = m_folderRepositoryDal.FindByFilterAsync(f => Util.DIRECTORY_BASE + f.FolderPath == parent).Result.FirstOrDefault(); // parentFolder on db

            var folderBox = new FileboxFolder(folder.FolderId, user.UserId, dir.Name, folder.FolderPath + "\\" + dir.Name);

            var savedFolder = await m_folderRepositoryDal.Save(folderBox);
            await m_folderRepositoryDal.SaveChangesAsync();
            return savedFolder;
        }

        private async Task<bool> IsExistsOnDb(string folderName, Guid userId)
        {
            var result = await m_folderRepositoryDal.FindByFilterAsync(f => f.FolderName == folderName && f.UserId == userId);

            return result.FirstOrDefault() != null;
        }
    }
}
