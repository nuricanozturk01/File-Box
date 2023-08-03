namespace Service.Services.ForgottenInformationService
{
    public interface IForgottenInformationService
    {
        Task<(string email, string username)> ChangePasswordAsync(string email);
    }
}
