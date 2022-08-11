using DohrniiBackoffice.Domain.Abstract;
using DohrniiBackoffice.Domain.Entities;
using DohrniiBackoffice.Helpers;
using DohrniiBackoffice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DohrniiBackoffice.Controllers
{
    [Authorize(AuthenticationSchemes = AuthConstants.AuthSchemes, Roles = AuthConstants.AdminRole)]
    public class LessonController : DefaultController<LessonController>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ILessonClassRepository _lessonClassRepository;
        private readonly IClassQuestionRepository _classQuestionRepository;
        private readonly IClassQuestionAnswerRepository _classQuestionAnswerRepository;

        public LessonController(ICategoryRepository categoryRepository, IChapterRepository chapterRepository, ILessonRepository lessonRepository, ILessonClassRepository lessonClassRepository, IClassQuestionRepository lessonClassQuestionRepository, IClassQuestionAnswerRepository classQuestionAnswerRepository)
        {
            _categoryRepository = categoryRepository;
            _chapterRepository = chapterRepository;
            _lessonRepository = lessonRepository;
            _lessonClassRepository = lessonClassRepository;
            _classQuestionRepository = lessonClassQuestionRepository;
            _classQuestionAnswerRepository = classQuestionAnswerRepository;
        }

        #region Categories
        public IActionResult Categories()
        {
            var dt = _categoryRepository.GetAll().ToList();
            ViewBag.Msg = TempData["msg"];
            return View(dt);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    category.DateAdded = DateTime.UtcNow;
                    _categoryRepository.Add(category);
                    await _categoryRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                    TempData["msg"] = _app.GetMsg(alert.success.ToString(), "Category created successfully!");
                    return RedirectToAction(nameof(Categories));
                }
                else
                {
                    TempData["msg"] = _app.GetMsg(alert.warning.ToString(), ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage);
                    return RedirectToAction(nameof(Categories));
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(Categories));
            }

        }
        public IActionResult CategoryDetails(int id)
        {
            var dt = _categoryRepository.FindBy(c => c.Id == id).FirstOrDefault();
            ViewBag.Msg = TempData["msg"];
            return View(dt);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(Category model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var category = _categoryRepository.FindBy(c => c.Id == model.Id).FirstOrDefault();
                    if (category != null)
                    {
                        category.Name = model.Name;
                        category.Description = model.Description;

                        _categoryRepository.Edit(category);
                        await _categoryRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                        TempData["msg"] = _app.GetMsg(alert.success.ToString(), "Updated successfully!");
                        return RedirectToAction(nameof(CategoryDetails), new { id = model.Id });
                    }
                    else
                    {
                        TempData["msg"] = _app.GetMsg(alert.warning.ToString(), "This record does not exist or has been deleted");
                        return RedirectToAction(nameof(CategoryDetails), new { id = model.Id });
                    }
                }
                else
                {
                    TempData["msg"] = _app.GetMsg(alert.warning.ToString(), ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage);
                    return RedirectToAction(nameof(CategoryDetails), new { id = model.Id });
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(CategoryDetails), new { id = model.Id });
            }

        }
        #endregion

        #region Chapters
        public IActionResult Chapters()
        {
            var dt = _chapterRepository.GetAll().ToList();
            ViewBag.Categories = _categoryRepository.GetAll().ToList();
            ViewBag.Msg = TempData["msg"];
            return View(dt);
        }

        [HttpPost]
        public async Task<IActionResult> AddChapter(MChapter model, bool FromDetailPage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var chapter = _mapper.Map<Chapter>(model);
                    chapter.DateAdded = DateTime.UtcNow;
                    _chapterRepository.Add(chapter);
                    await _chapterRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                    TempData["msg"] = _app.GetMsg(alert.success.ToString(), "Chapter created successfully!");
                    if (FromDetailPage)
                        return RedirectToAction(nameof(CategoryDetails), new { id = model.CategoryId });
                    else
                        return RedirectToAction(nameof(Chapters));
                }
                else
                {
                    TempData["msg"] = _app.GetMsg(alert.warning.ToString(), ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage);
                    if (FromDetailPage)
                        return RedirectToAction(nameof(CategoryDetails), new { id = model.CategoryId });
                    else
                        return RedirectToAction(nameof(Chapters));
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                if (FromDetailPage)
                    return RedirectToAction(nameof(CategoryDetails), new { id = model.CategoryId });
                else
                    return RedirectToAction(nameof(Chapters));
            }

        }

        public IActionResult ChapterDetails(int id)
        {
            var dt = _chapterRepository.FindBy(c => c.Id == id).FirstOrDefault();
            ViewBag.Categories = _categoryRepository.GetAll().ToList();
            ViewBag.Msg = TempData["msg"];
            return View(dt);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateChapter(MChapter model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var chapter = _chapterRepository.FindBy(c=>c.Id == model.ChapterId).FirstOrDefault();
                    if(chapter != null)
                    {
                        chapter.RequiredJelly = model.RequiredJelly;
                        chapter.Title = model.Title;
                        chapter.QuestionLimit = model.QuestionLimit;
                        chapter.RewardEighty = model.RewardEighty;
                        chapter.RewardNinety = model.RewardNinety;
                        chapter.RewardHundred = model.RewardHundred;
                        chapter.CategoryId = model.CategoryId;
                        chapter.TimeLimit = model.TimeLimit;
                        chapter.Name = model.Name;

                        _chapterRepository.Edit(chapter);
                        await _chapterRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                        TempData["msg"] = _app.GetMsg(alert.success.ToString(), "Updated successfully!");
                        return RedirectToAction(nameof(ChapterDetails), new { id = model.ChapterId });
                    }
                    else
                    {
                        TempData["msg"] = _app.GetMsg(alert.warning.ToString(), "This record does not exist or has been deleted");
                        return RedirectToAction(nameof(ChapterDetails), new { id = model.ChapterId });
                    }
                }
                else
                {
                    TempData["msg"] = _app.GetMsg(alert.warning.ToString(), ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage);
                    return RedirectToAction(nameof(ChapterDetails), new { id = model.ChapterId });
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(ChapterDetails), new { id = model.ChapterId });
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateChapterSequence(int CategoryId, int[] Id)
        {
            try
            {
                int sequence = 1;
                foreach (int id in Id)
                {
                    var chapter = _chapterRepository.FindBy(c => c.Id == id).FirstOrDefault();
                    if (chapter != null)
                    {
                        chapter.Sequence = sequence;
                        _chapterRepository.Edit(chapter);
                        sequence += 1;
                    }
                }
                await _chapterRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                TempData["msg"] = _app.GetMsg(alert.success.ToString(), "Updated!");
                return RedirectToAction(nameof(CategoryDetails), new { id = CategoryId });
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(CategoryDetails), new { id = CategoryId });
            }

        }

        #endregion

        #region Lessons
        public IActionResult Lessons()
        {
            var dt = _lessonRepository.GetAll().ToList();
            return View(dt);
        }

        [HttpPost]
        public async Task<IActionResult> AddLesson(MLesson model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var lesson = _mapper.Map<Lesson>(model);
                    lesson.DateAdded = DateTime.UtcNow;
                    _lessonRepository.Add(lesson);
                    await _lessonRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                    TempData["msg"] = _app.GetMsg(alert.success.ToString(), "lesson created successfully!");
                    return RedirectToAction(nameof(ChapterDetails), new { id = lesson.ChapterId});
                }
                else
                {
                    TempData["msg"] = _app.GetMsg(alert.warning.ToString(), ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage);
                    return RedirectToAction(nameof(ChapterDetails), new { id = model.ChapterId });
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(ChapterDetails), new { id = model.ChapterId });
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateLessonSequence(int ChapterId, int[] Id)
        {
            try
            {
                int sequence = 1;
                foreach (int id in Id)
                {
                    var lesson = _lessonRepository.FindBy(c=>c.Id == id).FirstOrDefault();
                    if(lesson != null)
                    {
                        lesson.Sequence = sequence;
                        _lessonRepository.Edit(lesson);
                        sequence += 1;
                    }
                }
                await _lessonRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                TempData["msg"] = _app.GetMsg(alert.success.ToString(), "Updated!");
                return RedirectToAction(nameof(ChapterDetails), new { id = ChapterId });
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(ChapterDetails), new { id = ChapterId });
            }

        }

        public IActionResult LessonDetails(int id)
        {
            var dt = _lessonRepository.FindBy(c => c.Id == id).FirstOrDefault();
            ViewBag.Chapters = _chapterRepository.GetAll().ToList();
            ViewBag.Msg = TempData["msg"];
            return View(dt);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLesson(MLesson model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var lesson = _lessonRepository.FindBy(c => c.Id == model.LessonId).FirstOrDefault();
                    if (lesson != null)
                    {
                        lesson.Title = model.Title;
                        lesson.Name = model.Name;
                        lesson.ChapterId = model.ChapterId;

                        _lessonRepository.Edit(lesson);
                        await _lessonRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                        TempData["msg"] = _app.GetMsg(alert.success.ToString(), "Updated successfully!");
                        return RedirectToAction(nameof(LessonDetails), new { id = model.LessonId });
                    }
                    else
                    {
                        TempData["msg"] = _app.GetMsg(alert.warning.ToString(), "This record does not exist or has been deleted");
                        return RedirectToAction(nameof(LessonDetails), new { id = model.LessonId });
                    }
                }
                else
                {
                    TempData["msg"] = _app.GetMsg(alert.warning.ToString(), ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage);
                    return RedirectToAction(nameof(LessonDetails), new { id = model.LessonId });
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(LessonDetails), new { id = model.LessonId });
            }

        }
        #endregion

        #region Classes
        public IActionResult Classes()
        {
            var dt = _lessonClassRepository.GetAll().ToList();
            return View(dt);
        }

        public IActionResult ClassDetails(int id)
        {
            var dt = _lessonClassRepository.FindBy(c => c.Id == id).FirstOrDefault();
            ViewBag.Lessons = _lessonRepository.GetAll().ToList();
            ViewBag.Msg = TempData["msg"];
            return View(dt);
        }

        [HttpPost]
        public async Task<IActionResult> AddClass(MClass model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mclass = _mapper.Map<LessonClass>(model);
                    mclass.DateAdded = DateTime.UtcNow;
                    _lessonClassRepository.Add(mclass);
                    await _lessonClassRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                    TempData["msg"] = _app.GetMsg(alert.success.ToString(), "lesson created successfully!");
                    return RedirectToAction(nameof(LessonDetails), new { id = model.LessonId });
                }
                else
                {
                    TempData["msg"] = _app.GetMsg(alert.warning.ToString(), ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage);
                    return RedirectToAction(nameof(LessonDetails), new { id = model.LessonId });
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(LessonDetails), new { id = model.LessonId });
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateClass(MClass model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mclass = _lessonClassRepository.FindBy(c => c.Id == model.ClassId).FirstOrDefault();
                    if (mclass != null)
                    {
                        mclass.Title = model.Title;
                        mclass.Name = model.Name;
                        mclass.LessonId = model.LessonId;
                        mclass.QuestionLimit = model.QuestionLimit;
                        mclass.XpPerQuestion = model.XpPerQuestion;
                        mclass.HtmlContent=model.HtmlContent;

                        _lessonClassRepository.Edit(mclass);
                        await _lessonClassRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                        TempData["msg"] = _app.GetMsg(alert.success.ToString(), "Updated successfully!");
                        return RedirectToAction(nameof(ClassDetails), new { id = model.ClassId });
                    }
                    else
                    {
                        TempData["msg"] = _app.GetMsg(alert.warning.ToString(), "This record does not exist or has been deleted");
                        return RedirectToAction(nameof(ClassDetails), new { id = model.ClassId });
                    }
                }
                else
                {
                    TempData["msg"] = _app.GetMsg(alert.warning.ToString(), ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage);
                    return RedirectToAction(nameof(ClassDetails), new { id = model.ClassId });
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(ClassDetails), new { id = model.ClassId });
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateClassSequence(int LessonId, int[] Id)
        {
            try
            {
                int sequence = 1;
                foreach (int id in Id)
                {
                    var lessonClass = _lessonClassRepository.FindBy(c => c.Id == id).FirstOrDefault();
                    if (lessonClass != null)
                    {
                        lessonClass.Sequence = sequence;
                        _lessonClassRepository.Edit(lessonClass);
                        sequence += 1;
                    }
                }
                await _lessonClassRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                TempData["msg"] = _app.GetMsg(alert.success.ToString(), "Updated!");
                return RedirectToAction(nameof(LessonDetails), new { id = LessonId });
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(LessonDetails), new { id = LessonId });
            }

        }
        #endregion

        #region Questions
        //public IActionResult Questions()
        //{
        //    var dt = _classQuestionRepository.GetAll().ToList();
        //    return View(dt);
        //}

        public IActionResult ClassQuestionDetails(int id)
        {
            var dt = _classQuestionRepository.FindBy(c => c.Id == id).FirstOrDefault();
            ViewBag.Classes = _lessonClassRepository.GetAll().ToList();
            ViewBag.Msg = TempData["msg"];
            return View(dt);
        }

        [HttpPost]
        public async Task<IActionResult> AddClassQuestion(mQuestion model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var classQuestion = _mapper.Map<ClassQuestion>(model);
                    classQuestion.DateAdded = DateTime.UtcNow;

                    _classQuestionRepository.Add(classQuestion);
                    await _classQuestionRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                    TempData["msg"] = _app.GetMsg(alert.success.ToString(), "Question created successfully!");
                    return RedirectToAction(nameof(ClassDetails), new { id = model.LessonClassId });
                }
                else
                {
                    TempData["msg"] = _app.GetMsg(alert.warning.ToString(), ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage);
                    return RedirectToAction(nameof(ClassDetails), new { id = model.LessonClassId });
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(ClassDetails), new { id = model.LessonClassId });
            }

        }

        public async Task<IActionResult> UpdateClassQuestion(mQuestion model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mQtn = _classQuestionRepository.FindBy(c => c.Id == model.QuestionId).FirstOrDefault();
                    if (mQtn != null)
                    {
                        mQtn.Question = model.Question;
                        mQtn.QuestionType = model.QuestionType;

                        _classQuestionRepository.Edit(mQtn);
                        await _classQuestionRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                        TempData["msg"] = _app.GetMsg(alert.success.ToString(), "Updated successfully!");
                        return RedirectToAction(nameof(ClassQuestionDetails), new { id = model.QuestionId });
                    }
                    else
                    {
                        TempData["msg"] = _app.GetMsg(alert.warning.ToString(), "This record does not exist or has been deleted");
                        return RedirectToAction(nameof(ClassQuestionDetails), new { id = model.QuestionId });
                    }
                }
                else
                {
                    TempData["msg"] = _app.GetMsg(alert.warning.ToString(), ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage);
                    return RedirectToAction(nameof(ClassQuestionDetails), new { id = model.QuestionId });
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(ClassQuestionDetails), new { id = model.QuestionId });
            }

        }

        #endregion

        #region Answers

        public IActionResult ClassAnswerDetails(int id)
        {
            var dt = _classQuestionAnswerRepository.FindBy(c => c.Id == id).FirstOrDefault();
            ViewBag.Msg = TempData["msg"];
            return View(dt);
        }

        [HttpPost]
        public async Task<IActionResult> AddClassQuestionAnswer(mAnswer model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var classQuestionAnswer = _mapper.Map<ClassQuestionAnswer>(model);

                    _classQuestionAnswerRepository.Add(classQuestionAnswer);
                    await _classQuestionAnswerRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                    TempData["msg"] = _app.GetMsg(alert.success.ToString(), "Option created successfully!");
                    return RedirectToAction(nameof(ClassQuestionDetails), new { id = model.ClassQuestionId });
                }
                else
                {
                    TempData["msg"] = _app.GetMsg(alert.warning.ToString(), ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage);
                    return RedirectToAction(nameof(ClassQuestionDetails), new { id = model.ClassQuestionId });
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(ClassQuestionDetails), new { id = model.ClassQuestionId });
            }

        }

        public async Task<IActionResult> UpdateClassQuestionAnswer(mAnswer model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mAns = _classQuestionAnswerRepository.FindBy(c => c.Id == model.ClassQuestionAnswerId).FirstOrDefault();
                    if (mAns != null)
                    {
                        mAns.AnswerOption = model.AnswerOption;
                        mAns.IsAnswer = model.IsAnswer;

                        _classQuestionAnswerRepository.Edit(mAns);
                        await _classQuestionAnswerRepository.Save(User.Identity.Name, _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
                        TempData["msg"] = _app.GetMsg(alert.success.ToString(), "Updated successfully!");
                        return RedirectToAction(nameof(ClassAnswerDetails), new { id = model.ClassQuestionAnswerId });
                    }
                    else
                    {
                        TempData["msg"] = _app.GetMsg(alert.warning.ToString(), "This record does not exist or has been deleted");
                        return RedirectToAction(nameof(ClassAnswerDetails), new { id = model.ClassQuestionAnswerId });
                    }
                }
                else
                {
                    TempData["msg"] = _app.GetMsg(alert.warning.ToString(), ModelState.FirstOrDefault().Value.Errors[0].ErrorMessage);
                    return RedirectToAction(nameof(ClassAnswerDetails), new { id = model.ClassQuestionAnswerId });
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = _app.GetMsg(alert.danger.ToString(), ex.Message);
                return RedirectToAction(nameof(ClassAnswerDetails), new { id = model.ClassQuestionAnswerId });
            }

        }

        #endregion
    }
}
