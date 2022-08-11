namespace DohrniiBackoffice.DTO.Response
{
    public class ClassQuestionOptionDTO
    {
        public int Id { get; set; }
        public int ClassQuestionId { get; set; }
        public string AnswerOption { get; set; }
        public bool IsAnswer { get; set; }
    }
}
