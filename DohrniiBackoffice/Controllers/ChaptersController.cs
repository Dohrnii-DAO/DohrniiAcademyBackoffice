using DohrniiBackoffice.Domain.Abstract;
using DohrniiBackoffice.Domain.Entities;
using DohrniiBackoffice.DTO.Request;
using DohrniiBackoffice.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

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
        private readonly IWithdrawActivityRepository _withdrawActivityRepository;
        private readonly IAppSettingsRepository _appSettingsRepository;


        public ChaptersController(ICategoryRepository categoryRepository, ILessonRepository lessonRepository, 
            ILessonClassRepository lessonClassRepository, ILessonActivityRepository lessonActivityRepository,
            ILessonClassActivityRepository lessonClassActivityRepository, IChapterRepository chapterRepository,
            IQuizUnlockActivityRepository quizUnlockActivityRepository, IChapterActivityRepository chapterActivityRepository,
            IEarningActivityRepository earningActivityRepository, IvQuestionRepository vQuestionRepository,
            IClassQuestionAnswerRepository classQuestionAnswerRepository, IQuizAttemptRepository quizAttemptRepository, 
            IWithdrawActivityRepository withdrawActivityRepository, IAppSettingsRepository appSettingsRepository)
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
            _withdrawActivityRepository = withdrawActivityRepository;
            _appSettingsRepository = appSettingsRepository;
        }

        /// <summary>
        /// Get chapter details with clesson and classes
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
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
                        chapter.IsQuizDone = mChapter.QuizAttempts.FirstOrDefault(c => c.UserId == user.Id) != null;
                        
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


        /// <summary>
        /// Get Chapter quiz questions with anwser options
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
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
                            item.Attempts = _mapper.Map<List<ClassQuestionAttemptDTO>>(_quizAttemptRepository.FindBy(c => c.QuestionId == item.Id && c.UserId == user.Id && c.ChapterId == Id).ToList());
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

        /// <summary>
        /// Unlock chapter quiz with the required jelly set in the chapter detail
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("unlockquiz")]
        [Produces(typeof(UserResp))]
        public async Task<IActionResult> ConvertXPtoJelly([FromBody] UnlockChapterDTO dto)
        {
            try
            {
                var user = GetUser();
                if (user != null)
                {
                    var chapter = _chapterRepository.FindBy(c=>c.Id == dto.ChapterId).FirstOrDefault();
                    if (chapter != null)
                    {
                        if (user.TotalJelly >= chapter.RequiredJelly)
                        {
                            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                            {
                                var withdraw = new WithdrawActivity
                                {
                                    DateAdded = DateTime.UtcNow,
                                    Dhn = 0,
                                    Jelly = chapter.RequiredJelly,
                                    UserId = user.Id
                                };
                                _withdrawActivityRepository.Add(withdraw);
                                await _withdrawActivityRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());

                                var unluckActivity = new QuizUnlockActivity
                                {
                                    Jelly = chapter.RequiredJelly,
                                    UserId = user.Id,
                                    ChapterId = chapter.Id,
                                    DateUnlocked = DateTime.UtcNow
                                };
                                _quizUnlockActivityRepository.Add(unluckActivity);
                                await _quizUnlockActivityRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());

                                user.TotalJelly -= chapter.RequiredJelly;
                                _userRepository.Edit(user);
                                await _userRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());

                                scope.Complete();
                            }
                            var userResp = _mapper.Map<UserResp>(user);
                            var settings = _appSettingsRepository.GetAll().FirstOrDefault();
                            if(settings != null)
                            {
                                userResp.XpPerCryptojelly = settings.XpToJelly;
                            }

                            return Ok(userResp);
                        }
                        return Conflict(new ErrorResponse { Details = "You don't have enough Jelly to unlock this quiz!" });

                    }
                    return NotFound(new ErrorResponse { Details = "No record found!" });
                }
                else
                {
                    return NotFound(new ErrorResponse { Details = "User not found!" });
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                return InternalServerError(new ErrorResponse { Details = ex.Message });
            }
        }

        /// <summary>
        /// Attempt a quiz question
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("quizattempt")]
        [Produces(typeof(QuestionAttemptRespDTO))]
        public async Task<IActionResult> QuestionAttempt([FromBody] QuizAttemptDTO dto)
        {
            try
            {
                var user = GetUser();
                if (user != null)
                {
                    var chapter = _chapterRepository.FindBy(c => c.Id == dto.ChapterId).FirstOrDefault();
                    if (chapter != null)
                    {
                        var attempt = new QuizAttempt
                        {
                            DateAttempt = DateTime.UtcNow,
                            QuestionId = dto.QuestionId,
                            SelectedAnswerId = dto.SelectedAnswerId,
                            UserId = user.Id,
                            Xpcollected = dto.Xpcollected,
                            IsCorrect = dto.IsCorrect,
                            ChapterId = dto.ChapterId
                        };
                        _quizAttemptRepository.Add(attempt);
                        await _quizAttemptRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());

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

        /// <summary>
        /// Award user dhn
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("awarddhn")]
        [Produces(typeof(UserResp))]
        public async Task<IActionResult> AwardDhn([FromBody] AwardDhnDTO dto)
        {
            try
            {
                var user = GetUser();
                if (user != null)
                {
                    var chapter = _chapterRepository.FindBy(c=>c.Id == dto.ChapterId).FirstOrDefault();
                    if (chapter != null)
                    {
                        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            var earn = new EarningActivity
                            {
                                DateAdded = DateTime.UtcNow,
                                Dhn = dto.AwardedDhn,
                                UserId = user.Id,
                                ChapterId = chapter.Id,
                                CategoryId = chapter.CategoryId
                            };
                            _earningActivityRepository.Add(earn);
                            await _earningActivityRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());

                            user.TotalDhn += dto.AwardedDhn;
                            _userRepository.Edit(user);
                            await _userRepository.Save(user.Email, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());

                            scope.Complete();
                        }
                        var userResp = _mapper.Map<UserResp>(user);
                        var settings = _appSettingsRepository.GetAll().FirstOrDefault();
                        if (settings != null)
                        {
                            userResp.XpPerCryptojelly = settings.XpToJelly;
                        }
                        return Ok(userResp);

                    }
                    return NotFound(new ErrorResponse { Details = "Record not found!" });
                }
                else
                {
                    return NotFound(new ErrorResponse { Details = "User not found!" });
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                return InternalServerError(new ErrorResponse { Details = ex.Message });
            }
        }
    }
}
