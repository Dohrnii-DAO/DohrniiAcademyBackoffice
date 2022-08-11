namespace DohrniiBackoffice.DTO.Response
{
    public class UserStausDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TotalXP { get; set; }
        public int TotalCryptoJelly { get; set; }
        public decimal TotalDHN { get; set; }
        public int XpPerCryptojelly { get; set; }
        public List<LessonInprogress> LessonsInprogress { get; set; }
    }
}
