using System.IO.Compression;

void ZipFolders(string[] sourceFolderPaths, string destinationZipPath)
{
    try
    {
        using (var zipArchive = ZipFile.Open(destinationZipPath, ZipArchiveMode.Create))
        {
            foreach (string folderPath in sourceFolderPaths)
            {
                AddFolderToZip(zipArchive, folderPath, "");
            }
        }

        Console.WriteLine("Folders successfully zipped!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error while zipping folders: {ex.Message}");
    }
}

void AddFolderToZip(ZipArchive zipArchive, string sourceFolderPath, string parentFolderName)
{
    string folderName = Path.GetFileName(sourceFolderPath);

    // Create an entry for the folder in the zip archive
    if (!string.IsNullOrEmpty(parentFolderName))
    {
        folderName = Path.Combine(parentFolderName, folderName);
    }
    zipArchive.CreateEntry(folderName + "/");

    // Add all files in the folder to the zip archive
    foreach (string filePath in Directory.GetFiles(sourceFolderPath))
    {
        string fileName = Path.GetFileName(filePath);
        zipArchive.CreateEntryFromFile(filePath, Path.Combine(folderName, fileName));
    }

    // Recursively add subfolders
    foreach (string subfolderPath in Directory.GetDirectories(sourceFolderPath))
    {
        AddFolderToZip(zipArchive, subfolderPath, folderName);
    }
}


void Main()
{
    string[] sourceFolderPaths = new string[]
    {
        @"C:\Users\hp\Desktop\file_box\nuricanozturk\copy_multiple_2",
        @"C:\Users\hp\Desktop\copy_multiple_3"
    };

    string destinationZipPath = @"C:\Users\hp\Desktop\FoldersArchive.zip"; // Replace with your desired destination zip file path

    ZipFolders(sourceFolderPaths, destinationZipPath);
}

Main();