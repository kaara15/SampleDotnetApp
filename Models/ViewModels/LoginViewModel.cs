namespace SampleDotnetApp.Models.ViewModels
{
    public class LoginViewModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? HashedPassword { get; set; }
    }
}