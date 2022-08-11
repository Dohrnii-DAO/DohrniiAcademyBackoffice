using DohrniiBackoffice.Domain.Abstract;
using DohrniiBackoffice.Domain.Entities;
using DohrniiBackoffice.DTO.Request;
using DohrniiBackoffice.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace DohrniiBackoffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LessonsController : DefaultController<ChaptersController>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ILessonClassRepository _lessonClassRepository;
        private readonly ILessonActivityRepository _lessonActivityRepository;
        private readonly ILessonClassActivityRepository _lessonClassActivityRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IQuizUnlockActivityRepository _quizUnlockActivityRepository;
        private readonly IChapterActivityRepository _chapterActivityRepository;
        private readonly IClassQuestionRepository _classQuestionRepository;
        private readonly IClassQuestionAnswerRepository _classQuestionAnswerRepository;
        private readonly IQuestionAttemptRepository _questionAttemptRepository;
        private readonly IEarningActivityRepository _earningActivityRepository;


        public LessonsController(ICategoryRepository categoryRepository, ILessonRepository lessonRepository, ILessonClassRepository lessonClassRepository, 
            ILessonActivityRepository lessonActivityRepository, ILessonClassActivityRepository lessonClassActivityRepository, IChapterRepository chapterRepository,
            IQuizUnlockActivityRepository quizUnlockActivityRepository, IChapterActivityRepository chapterActivityRepository, IClassQuestionRepository classQuestionRepository,
            IClassQuestionAnswerRepository classQuestionAnswerRepository, IQuestionAttemptRepository questionAttemptRepository, IEarningActivityRepository earningActivityRepository)
        {
            _categoryRepository = categoryRepository;
            _lessonRepository = lessonRepository;
            _lessonClassRepository = lessonClassRepository;
            _lessonActivityRepository = lessonActivityRepository;
            _lessonClassActivityRepository = lessonClassActivityRepository;
            _chapterRepository = chapterRepository;
            _quizUnlockActivityRepository = quizUnlockActivityRepository;
            _chapterActivityRepository = chapterActivityRepository;
            _classQuestionRepository = classQuestionRepository;
            _classQuestionAnswerRepository = classQuestionAnswerRepository;
            _questionAttemptRepository = questionAttemptRepository;
            _earningActivityRepository = earningActivityRepository;
        }

        [HttpPost("start")]
        [Produces(typeof(StartResponseDTO))]
        public async Task<IActionResult> StartLesson([FromBody] StartDTO dto)
        {
            try
            {
                var user = GetUser();
                if(user != null)
                {
                    var lessonActivity = _lessonActivityRepository.FindBy(c => c.LessonId == dto.Id && c.UserId == user.Id).FirstOrDefault();
                    if(lessonActivity == null)
                    {
                        var lesson = _lessonRepository.FindBy(c => c.Id == dto.Id).FirstOrDefault();
                        if (lesson != null)
                        {
                            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                            {
                                var chapter = _chapterActivityRepository.FindBy(c => c.Id == lesson.ChapterId && c.UserId == user.Id).FirstOrDefault();
                                if (chapter == null)
                                {
                                    var chapterActivity = new ChapterActivity
                                    {
                                        ChapterId = lesson.ChapterId,
                                        CategoryId = lesson.Chapter.CategoryId,
                                        UserId = user.Id,
                                        DateStarted = DateTime.UtcNow,
                                        DateCompleted = DateTime.UtcNow,
                                        IsCompleted = false
                                    };
                                    _chapterActivityRepository.Add(chapterActivity);
                                    await _chapterActivityRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                                }
                                var lessonActivityObj = new LessonActivity
                                {
                                    ChapterId = lesson.ChapterId,
                                    CategoryId = lesson.Chapter.CategoryId,
                                    LessonId = dto.Id,
                                    UserId = user.Id,
                                    DateStarted = DateTime.UtcNow,
                                    DateCompleted = DateTime.UtcNow,
                                    IsCompleted = false
                                };
                                _lessonActivityRepository.Add(lessonActivityObj);
                                await _lessonActivityRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());

                                if (lesson.LessonClasses.Count > 0)
                                {
                                    var mclass = lesson.LessonClasses.FirstOrDefault();
                                    if (mclass != null)
                                    {
                                        var classActivityObj = new LessonClassActivity
                                        {
                                            ChapterId = lesson.ChapterId,
                                            CategoryId = lesson.Chapter.CategoryId,
                                            LessonId = dto.Id,
                                            LessonClassId = mclass.Id,
                                            UserId = user.Id,
                                            DateStarted = DateTime.UtcNow,
                                            DateCompleted = DateTime.UtcNow,
                                            IsCompleted = false
                                        };
                                        _lessonClassActivityRepository.Add(classActivityObj);
                                        await _lessonClassActivityRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                                    }
                                }

                                scope.Complete();
                            }
                            
                            return Ok(new StartResponseDTO { IsStarted = true, Id = dto.Id, Name = lesson.Title });
                        }
                        return NotFound(new ErrorResponse { Details = "Record not found!" });
                    }
                    return Ok(new StartResponseDTO { IsStarted = true, Id = dto.Id });
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

        [HttpPut("complete")]
        [Produces(typeof(CompleteResponseDTO))]
        public async Task<IActionResult> CompleteLesson([FromBody] CompleteDTO dto)
        {
            try
            {
                var user = GetUser();
                if (user != null)
                {
                    var lessonActivity = _lessonActivityRepository.FindBy(c => c.LessonId == dto.Id && c.UserId == user.Id).FirstOrDefault();
                    if (lessonActivity != null)
                    {
                        lessonActivity.IsCompleted = true;
                        lessonActivity.DateCompleted = DateTime.UtcNow;
                        _lessonActivityRepository.Edit(lessonActivity);
                        await _lessonActivityRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());

                        var resp = new CompleteResponseDTO();
                        resp.IsCompleted = true;
                        resp.Id = dto.Id;
                        resp.Name = _lessonRepository.FindBy(c => c.Id == dto.Id).FirstOrDefault()?.Name;
                        var earnings = _earningActivityRepository.FindBy(c => c.UserId == user.Id && c.LessonId == dto.Id).ToList();
                        resp.TotalXpEarned = earnings.Sum(c => c.Xp);
                        resp.TotalJellyEarned = earnings.Sum(c => c.Jelly);
                        resp.TotalDhnEarned = earnings.Sum(c => c.Dhn);

                        var totalClasses = 0;
                        var lessons = _lessonRepository.FindBy(c => c.ChapterId == lessonActivity.ChapterId);
                        foreach (var item in lessons)
                        {
                            totalClasses += item.LessonClasses.Count;
                        }
                        var completedClasses = _lessonClassActivityRepository.FindBy(c => c.UserId == user.Id && c.IsCompleted == true && c.ChapterId==lessonActivity.ChapterId).ToList();
                        var percentage = (completedClasses.Count / totalClasses) * 100.0;
                        resp.PercentageComplete = Math.Round(percentage, MidpointRounding.AwayFromZero);


                        return Ok(resp);
                    }
                    return NotFound(new ErrorResponse { Details = "Record not found!" });
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

        [HttpGet("{Id:int}/progress")]
        [Produces(typeof(CompleteResponseDTO))]
        public IActionResult GetLessonProgress(int Id)
        {
            var user = GetUser();
            if(user != null)
            {
                var lesson = _lessonRepository.FindBy(c => c.Id == Id).FirstOrDefault();
                if (lesson != null)
                {
                    var lessonActivity = _lessonActivityRepository.FindBy(c => c.UserId == user.Id && c.LessonId == Id).FirstOrDefault();
                    var resp = new CompleteResponseDTO();
                    resp.IsCompleted = lessonActivity == null ? false : lessonActivity.IsCompleted;
                    resp.Id = Id;
                    resp.Name = lesson.Name;
                    var earnings = _earningActivityRepository.FindBy(c => c.UserId == user.Id && c.ClassId == Id).ToList();
                    resp.TotalXpEarned = earnings.Sum(c => c.Xp);
                    resp.TotalJellyEarned = earnings.Sum(c => c.Jelly);
                    resp.TotalDhnEarned = earnings.Sum(c => c.Dhn);

                    //var totalClasses = _lessonClassRepository.FindBy(c => c.LessonId == classActivity.LessonId).ToList();
                    //var completedClasses = _lessonClassActivityRepository.FindBy(c => c.UserId == user.Id && c.IsCompleted == true && c.LessonId == classActivity.LessonId).ToList();
                    //var percentage = (completedClasses.Count / totalClasses.Count) * 100.0;
                    //resp.PercentageComplete = Math.Round(percentage, MidpointRounding.AwayFromZero);


                    return Ok(resp);
                }
                return NotFound(new ErrorResponse { Details = "Record not found!" });
            }
            return NotFound(new ErrorResponse { Details = "We can't find your account!" });
        }
    }
}
