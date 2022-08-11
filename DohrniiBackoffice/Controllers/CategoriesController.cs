using DohrniiBackoffice.Domain.Abstract;
using DohrniiBackoffice.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DohrniiBackoffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : DefaultController<AuthController>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ILessonClassRepository _lessonClassRepository;
        private readonly ILessonActivityRepository _lessonActivityRepository;
        private readonly ILessonClassActivityRepository _lessonClassActivityRepository;
        private readonly IChapterRepository _chapterRepository;

        public CategoriesController(ICategoryRepository categoryRepository, ILessonRepository lessonRepository, ILessonClassRepository lessonClassRepository, ILessonActivityRepository lessonActivityRepository, ILessonClassActivityRepository lessonClassActivityRepository, IChapterRepository chapterRepository)
        {
            _categoryRepository = categoryRepository;
            _lessonRepository = lessonRepository;
            _lessonClassRepository = lessonClassRepository;
            _lessonActivityRepository = lessonActivityRepository;
            _lessonClassActivityRepository = lessonClassActivityRepository;
            _chapterRepository = chapterRepository;
        }

        [HttpGet()]
        [Produces(typeof(List<CategoryDTO>))]
        public IActionResult GetCategories()
        {
            try
            {
                var rst = _categoryRepository.GetAll().ToList();
                var results = _mapper.Map<List<CategoryDTO>>(rst);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                return InternalServerError(new ErrorResponse { Details = ex.Message });
            }
        }

        [HttpGet("{Id:int}/chapters")]
        [Produces(typeof(List<ChapterDTO>))]
        public IActionResult GetChapter(int Id)
        {
            try
            {
                List<ChapterDTO> chapters = new List<ChapterDTO>();
                var mChapters = _chapterRepository.FindBy(c => c.CategoryId == Id).ToList();
                foreach (var item in mChapters)
                {
                    var mc = _mapper.Map<ChapterDTO>(item);
                    mc.CategoryName = item.Category.Name;
                    mc.Lessons = _mapper.Map<List<LessonDTO>>(item.Lessons);
                    chapters.Add(mc);
                }
                return Ok(chapters);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                return InternalServerError(new ErrorResponse { Details = ex.Message });
            }
        }

        [HttpGet("{Id:int}/progress")]
        [Produces(typeof(CategoryProgressDTO))]
        public IActionResult GetProgress(int Id)
        {
            try
            {
                var user = GetUser();
                if(user != null)
                {
                    var progress = new CategoryProgressDTO();
                    var category = _categoryRepository.FindBy(c => c.Id == Id).FirstOrDefault();
                    if (category != null)
                    {
                        progress.CategoryId = category.Id;
                        progress.Name = category.Name;
                        foreach (var chapter in category.Chapters)
                        {
                            progress.TotalLesson += chapter.Lessons.Count();
                            foreach (var lesson in chapter.Lessons)
                            {
                                progress.TotalClass += lesson.LessonClasses.Count();
                            }
                        }
                        progress.CompletedLesson = _lessonActivityRepository.FindBy(c => c.CategoryId == Id && c.IsCompleted == true && c.UserId == user.Id).Count();
                        progress.CompletedClass = _lessonClassActivityRepository.FindBy(c => c.CategoryId == Id && c.IsCompleted == true && c.UserId == user.Id).Count();
                        return Ok(progress);
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
    }
}
