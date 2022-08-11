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
    public class ClassesController :  DefaultController<ClassesController>
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
        private readonly IvQuestionRepository _vQuestionRepository;

        public ClassesController(ICategoryRepository categoryRepository, ILessonRepository lessonRepository, 
            ILessonClassRepository lessonClassRepository, ILessonActivityRepository lessonActivityRepository, 
            ILessonClassActivityRepository lessonClassActivityRepository, IChapterRepository chapterRepository, 
            IQuizUnlockActivityRepository quizUnlockActivityRepository, IChapterActivityRepository chapterActivityRepository, 
            IClassQuestionRepository classQuestionRepository, IClassQuestionAnswerRepository classQuestionAnswerRepository, 
            IQuestionAttemptRepository questionAttemptRepository, IEarningActivityRepository earningActivityRepository, 
            IvQuestionRepository vQuestionRepository)
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
            _vQuestionRepository = vQuestionRepository;
        }

        [HttpPost("start")]
        [Produces(typeof(StartResponseDTO))]
        public async Task<IActionResult> StartClass([FromBody] StartDTO dto)
        {
            try
            {
                var user = GetUser();
                if (user != null)
                {
                    var mclass = _lessonClassRepository.FindBy(c => c.Id == dto.Id).FirstOrDefault();
                    if (mclass != null)
                    {
                        var classActivity = _lessonClassActivityRepository.FindBy(c => c.LessonClassId == dto.Id && c.UserId == user.Id).FirstOrDefault();
                        if (classActivity == null)
                        {
                            var classActivityObj = new LessonClassActivity
                            {
                                ChapterId = mclass.Lesson.ChapterId,
                                CategoryId = mclass.Lesson.Chapter.CategoryId,
                                LessonId = dto.Id,
                                LessonClassId = dto.Id,
                                UserId = user.Id,
                                DateStarted = DateTime.UtcNow,
                                DateCompleted = DateTime.UtcNow,
                                IsCompleted = false
                            };
                            _lessonClassActivityRepository.Add(classActivityObj);
                            await _lessonClassActivityRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                        }

                        return Ok(new StartResponseDTO { IsStarted = true, Id = dto.Id, Name = mclass.Name });
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

        [HttpPut("complete")]
        [Produces(typeof(CompleteResponseDTO))]
        public async Task<IActionResult> CompleteClass([FromBody] CompleteDTO dto)
        {
            try
            {
                var user = GetUser();
                if (user != null)
                {
                    var classActivity = _lessonClassActivityRepository.FindBy(c => c.LessonClassId == dto.Id && c.UserId==user.Id).FirstOrDefault();
                    if (classActivity != null)
                    {
                        classActivity.IsCompleted = true;
                        classActivity.DateCompleted = DateTime.UtcNow;
                        _lessonClassActivityRepository.Edit(classActivity);
                        await _lessonClassActivityRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());

                        var resp = new CompleteResponseDTO();
                        resp.IsCompleted = true;
                        resp.Id = dto.Id;
                        resp.Name = _lessonClassRepository.FindBy(c => c.Id == dto.Id).FirstOrDefault()?.Name;
                        var earnings = _earningActivityRepository.FindBy(c => c.UserId == user.Id && c.ClassId == dto.Id).ToList();
                        resp.TotalXpEarned = earnings.Sum(c => c.Xp);
                        resp.TotalJellyEarned = earnings.Sum(c => c.Jelly);
                        resp.TotalDhnEarned = earnings.Sum(c => c.Dhn);

                        var totalClasses = _lessonClassRepository.FindBy(c=>c.LessonId == classActivity.LessonId).ToList();
                        var completedClasses = _lessonClassActivityRepository.FindBy(c => c.UserId == user.Id && c.IsCompleted == true && c.LessonId == classActivity.LessonId).ToList();
                        var percentage = (Convert.ToDouble(completedClasses.Count) / Convert.ToDouble(totalClasses.Count)) * 100.0;
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

        [HttpPost("questionattempt")]
        [Produces(typeof(QuestionAttemptRespDTO))]
        public async Task<IActionResult> QuestionAttempt([FromBody] QuestionAttemptDTO dto)
        {
            try
            {
                var user = GetUser();
                if (user != null)
                {
                    var lesson = _lessonRepository.FindBy(c => c.Id == dto.LessonId).FirstOrDefault();
                    if (lesson != null)
                    {
                        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            var attempt = new QuestionAttempt
                            {
                                DateAttempt = DateTime.UtcNow,
                                QuestionId = dto.QuestionId,
                                SelectedAnswerId = dto.SelectedAnswerId,
                                UserId = user.Id,
                                Xpcollected = dto.Xpcollected,
                                IsCorrect = dto.IsCorrect
                            };
                            _questionAttemptRepository.Add(attempt);
                            await _questionAttemptRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());

                            var earning = new EarningActivity
                            {
                                UserId = user.Id,
                                CategoryId = lesson.Chapter.CategoryId,
                                ChapterId = lesson.ChapterId,
                                ClassId = dto.ClassId,
                                DateAdded = DateTime.UtcNow,
                                Dhn = 0,
                                Jelly = 0,
                                LessonId = dto.LessonId,
                                Xp = dto.Xpcollected
                            };
                            _earningActivityRepository.Add(earning);
                            await _earningActivityRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());

                            user.TotalXp += dto.Xpcollected;
                            _userRepository.Edit(user);
                            await _userRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());

                            scope.Complete();
                        }
                        return Ok(new QuestionAttemptRespDTO { IsCorrect = true, QuestionId = dto.QuestionId, AnwserId = dto.SelectedAnswerId });
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

        [HttpGet("{Id:int}/questions")]
        [Produces(typeof(List<ClassQuestionDTO>))]
        public IActionResult GetQuestions(int Id)
        {
            try
            {
                var user = GetUser();
                if (user != null)
                {
                    var mclass = _lessonClassRepository.FindBy(c=>c.Id == Id).FirstOrDefault();
                    if(mclass != null)
                    {
                        var options = new List<ClassQuestionDTO>();
                        var qtns = _classQuestionRepository.FindBy(c => c.LessonClassId == Id).OrderBy(r => Guid.NewGuid()).Take(mclass.QuestionLimit);
                        options = _mapper.Map<List<ClassQuestionDTO>>(qtns);
                        foreach (var item in options)
                        {
                            item.Options = _mapper.Map<List<ClassQuestionOptionDTO>>(_classQuestionAnswerRepository.FindBy(c => c.ClassQuestionId == item.Id).ToList());
                            item.Attempts = _mapper.Map<List<ClassQuestionAttemptDTO>>(_questionAttemptRepository.FindBy(c => c.QuestionId == item.Id && c.UserId == user.Id).ToList());
                            item.IsAttempted = item.Attempts.Count > 0;
                        }

                        return Ok(options);
                    }
                    return NotFound(new ErrorResponse { Details = "Class not found!" });
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
