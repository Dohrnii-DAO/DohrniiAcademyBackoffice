namespace DohrniiBackoffice.DTO.Response
{
    public class CompleteResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsCompleted { get; set; }
        public int TotalXpEarned { get; set; }
        public int TotalJellyEarned { get; set; }
        public decimal TotalDhnEarned { get; set; }
        public double PercentageComplete { get; set; }
    }
}
