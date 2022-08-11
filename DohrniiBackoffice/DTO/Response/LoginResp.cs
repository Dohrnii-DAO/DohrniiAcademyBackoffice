namespace DohrniiBackoffice.DTO.Response
{
    public class LoginResp
    {
        public UserResp User { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiringDate { get; set; }
    }
}
