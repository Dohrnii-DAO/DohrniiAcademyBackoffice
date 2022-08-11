using DohrniiBackoffice.Domain.Abstract;
using DohrniiBackoffice.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChaptersController : DefaultController<ChaptersController>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ILessonClassRepository _lessonClassRepository;
        private readonly ILessonActivityRepository _lessonActivityRepository;
        private readonly ILessonClassActivityRepository _lessonClassActivityRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IQuizUnlockActivityRepository _quizUnlockActivityRepository;
        private readonly IChapterActivityRepository _chapterActivityRepository;
        private readonly IEarningActivityRepository _earningActivityRepository;
        private readonly IvQuestionRepository _vQuestionRepository;
        private readonly IClassQuestionAnswerRepository _classQuestionAnswerRepository;
        private readonly IQuizAttemptRepository _quizAttemptRepository;


        public ChaptersController(ICategoryRepository categoryRepository, ILessonRepository lessonRepository, 
            ILessonClassRepository lessonClassRepository, ILessonActivityRepository lessonActivityRepository,
            ILessonClassActivityRepository lessonClassActivityRepository, IChapterRepository chapterRepository,
            IQuizUnlockActivityRepository quizUnlockActivityRepository, IChapterActivityRepository chapterActivityRepository,
            IEarningActivityRepository earningActivityRepository, IvQuestionRepository vQuestionRepository,
            IClassQuestionAnswerRepository classQuestionAnswerRepository, IQuizAttemptRepository quizAttemptRepository)
        {
            _categoryRepository = categoryRepository;
            _lessonRepository = lessonRepository;
            _lessonClassRepository = lessonClassRepository;
            _lessonActivityRepository = lessonActivityRepository;
            _lessonClassActivityRepository = lessonClassActivityRepository;
            _chapterRepository = chapterRepository;
            _quizUnlockActivityRepository = quizUnlockActivityRepository;
            _chapterActivityRepository = chapterActivityRepository;
            _earningActivityRepository = earningActivityRepository;
            _vQuestionRepository = vQuestionRepository;
            _classQuestionAnswerRepository = classQuestionAnswerRepository;
            _quizAttemptRepository = quizAttemptRepository;
        }


        [HttpGet("{Id:int}")]
        [Produces(typeof(ChapterDTO))]
        public IActionResult GetChapter(int Id)
        {
            try
            {
                var user = GetUser();
                if (user != null)
                {
                    var mChapter = _chapterRepository.FindBy(c => c.Id == Id).FirstOrDefault();
                    if (mChapter != null)
                    {
                        var chapter = _mapper.Map<ChapterDTO>(mChapter);
                        chapter.CategoryName = mChapter.Category.Name;
                        chapter.CompletedClass = mChapter.LessonClassActivities.Where(c => c.IsCompleted == true && c.UserId == user.Id).Count();
                        foreach (var item in mChapter.Lessons)
                        {
                            chapter.TotalClass += item.LessonClasses.Count;
                        }
                        chapter.IsQuizUnlocked = mChapter.QuizUnlockActivities.FirstOrDefault(c => c.UserId == user.Id) != null;
                        chapter.IsStarted = mChapter.ChapterActivities.FirstOrDefault(c => c.UserId == user.Id) != null;
                        chapter.IsCompleted = mChapter.ChapterActivities.FirstOrDefault(c => c.IsCompleted == true && c.UserId == user.Id) != null;
                        
                        chapter.Lessons = new List<LessonDTO>();
                        foreach (var item in mChapter.Lessons)
                        {
                            var lesson = _mapper.Map<LessonDTO>(item);
                            lesson.CompletedClass = item.LessonClassActivities.Where(c => c.IsCompleted == true && c.UserId == user.Id).Count();
                            lesson.TotalClass = item.LessonClasses.Count;
                            lesson.IsStarted = item.LessonActivities.FirstOrDefault(c => c.UserId == user.Id) != null;
                            lesson.IsCompleted = item.LessonActivities.FirstOrDefault(c => c.IsCompleted == true && c.UserId == user.Id) != null;
                            var earnings = _earningActivityRepository.FindBy(c => c.UserId == user.Id && c.LessonId == item.Id).ToList();
                            lesson.TotalXPEarned = earnings.Sum(c=>c.Xp);
                            lesson.TotalJellyEarned = earnings.Sum(c => c.Jelly);


                            lesson.Classes = new List<ClassDTO>();
                            foreach (var mItem in item.LessonClasses)
                            {
                                var mClass = _mapper.Map<ClassDTO>(mItem);
                                mClass.IsStarted = mItem.LessonClassActivities.FirstOrDefault(c => c.UserId == user.Id) != null;
                                mClass.IsCompleted = mItem.LessonClassActivities.FirstOrDefault(c => c.IsCompleted == true && c.UserId == user.Id) != null;
                                mClass.TotalXP = mItem.ClassQuestions.Count * mItem.XpPerQuestion;
                                lesson.TotalXP += mClass.TotalXP;
                                lesson.Classes.Add(mClass);
                            }

                            chapter.Lessons.Add(lesson);
                        }
                        return Ok(chapter);
                    }
                    else
                    {
                        return NotFound(new ErrorResponse { Details = "Record not found!" });
                    }
                }
                else
                {
                    return NotFound(new ErrorResponse { Details = "We can't find your account!" });
                }

            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                return InternalServerError(new ErrorResponse { Details = ex.Message });
            }
        }

        [HttpGet("{Id:int}/quiz")]
        [Produces(typeof(List<ChapterQuestionDTO>))]
        public IActionResult GetQuestions(int Id)
        {
            try
            {
                var user = GetUser();
                if (user != null)
                {
                    var chapter = _chapterRepository.FindBy(c=>c.Id == Id).FirstOrDefault();
                    if(chapter != null)
                    {
                        var options = new List<ChapterQuestionDTO>();
                        var qtns = _vQuestionRepository.FindBy(c => c.ChapterId == Id).OrderBy(r => Guid.NewGuid()).Take(chapter.QuestionLimit);
                        options = _mapper.Map<List<ChapterQuestionDTO>>(qtns);
                        foreach (var item in options)
                        {
                            item.Options = _mapper.Map<List<ClassQuestionOptionDTO>>(_classQuestionAnswerRepository.FindBy(c => c.ClassQuestionId == item.Id).ToList());
                            item.Attempts = _mapper.Map<List<ClassQuestionAttemptDTO>>(_quizAttemptRepository.FindBy(c => c.QuestionId == item.Id && c.UserId == user.Id).ToList());
                            item.IsAttempted = item.Attempts.Count > 0;
                        }

                        return Ok(options);
                    }
                    return NotFound(new ErrorResponse { Details = "Chapter not found!" });
                }
                return NotFound(new ErrorResponse { Details = "User not found!" });
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                return InternalServerError(new ErrorResponse { Details = ex.Message });
            }
        }
    }
}
