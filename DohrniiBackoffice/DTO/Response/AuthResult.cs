namespace DohrniiBackoffice.DTO.Response
{
    public class AuthResult
    {
        public string Token { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Email { get; set; }
    }
}
