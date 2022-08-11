namespace DohrniiBackoffice.DTO.Response
{
    public class UserResp
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? WalletAddress { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime DateAdded { get; set; }
        public int TotalXp { get; set; }
        public int TotalJelly { get; set; }
        public decimal TotalDhn { get; set; }
        public int XpPerCryptojelly { get; set; }
    }
}
