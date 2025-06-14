namespace SampleDotnetApp.Models.ViewModels
{
    public class RegisterViewModel
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? HashedPassword { get; set; }
    }
}