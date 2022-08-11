using AutoMapper;
using DohrniiBackoffice.Domain.Entities;
using DohrniiBackoffice.DTO.Response;
using DohrniiBackoffice.Models;

namespace DohrniiBackoffice.ObjectMapper
{
    public class ApplicationProfile: Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Chapter, MChapter>().ReverseMap();
            CreateMap<Lesson, MLesson>().ReverseMap();
            CreateMap<LessonClass, MClass>().ReverseMap();
            CreateMap<User, UserResp>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Chapter, ChapterDTO>().ReverseMap();
            CreateMap<Lesson, LessonDTO>().ReverseMap();
            CreateMap<LessonClass, ClassDTO>().ReverseMap();
            CreateMap<ClassQuestion, mQuestion>().ReverseMap();
            CreateMap<ClassQuestionAnswer, mAnswer>().ReverseMap();
            CreateMap<ClassQuestion, ClassQuestionDTO>().ReverseMap();
            CreateMap<ClassQuestionAnswer, ClassQuestionOptionDTO>().ReverseMap();
            CreateMap<QuestionAttempt, ClassQuestionAttemptDTO>().ReverseMap();
            CreateMap<VQuestion, ChapterQuestionDTO>().ReverseMap();
            CreateMap<QuizAttempt, ClassQuestionAttemptDTO>().ReverseMap();
        }
    }
}
