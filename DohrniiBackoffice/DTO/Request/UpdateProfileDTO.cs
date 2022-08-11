namespace DohrniiBackoffice.DTO.Request
{
    public class UpdateProfileDTO
    {
        public string UserName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? WalletAddress { get; set; }
    }
}
