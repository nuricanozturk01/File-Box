namespace RepositoryLib.Models;

public partial class FileboxUser
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? LastToken { get; set; }
    public string? ResetPasswordToken { get; set; }

    public virtual ICollection<FileboxFolder> FileboxFolders { get; set; } = new List<FileboxFolder>();
}
