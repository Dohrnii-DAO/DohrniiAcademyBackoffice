using AutoMapper;
using DohrniiBackoffice.Configuration;
using DohrniiBackoffice.Domain.Abstract;
using DohrniiBackoffice.Domain.Contract;
using DohrniiBackoffice.Helpers;
using DohrniiBackoffice.Messages;
using DohrniiBackoffice.ObjectMapper;
using DohrniiBackoffice.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;

namespace DohrniiBackoffice.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void IntallServices(IServiceCollection services, IConfiguration Configuration)
        {
            services.AddScoped<IAspNetUserRepository, EFAspNetUserRepository>();
            services.AddScoped<ICategoryRepository, EFCategoryRepository>();
            services.AddScoped<IChapterRepository, EFChapterRepository>();
            services.AddScoped<IClassQuestionAnswerRepository, EFClassQuestionAnswerRepository>();
            services.AddScoped<IClassQuestionRepository, EFClassQuestionRepository>();
            services.AddScoped<IEarningActivityRepository, EFEarningActivityRepository>();
            services.AddScoped<IFriendRequestRepository, EFFriendRquestRepository>();
            services.AddScoped<ILessonActivityRepository, EFLessonActivityRepository>();
            services.AddScoped<ILessonClassActivityRepository, EFLessonClassActivityRepository>();
            services.AddScoped<ILessonClassRepository, EFLessonClassRepository>();
            services.AddScoped<ILessonRepository, EFLessonRepository>();
            services.AddScoped<IUserRepository, EFUserRepository>();
            services.AddScoped<IvEarningActivityRepository, EFvEarningActivityRepository>();
            services.AddScoped<IvFriendRequestRepository, EFvFriendRequestRepository>();
            services.AddScoped<IvWithdrawActivityRepository, EFvWithdrawActivityRepository>();
            services.AddScoped<IWithdrawActivityRepository, EFWithdrawActivityRepository>();
            services.AddScoped<IAppSettingsRepository, EFAppSettingsRepository>();
            services.AddScoped<IQuizAttemptRepository, EFQuizAttemptRepository>();
            services.AddScoped<IQuestionAttemptRepository, EFQuestionAttemptRepository>();
            services.AddScoped<IChapterActivityRepository, EFChapterActivityRepository>();
            services.AddScoped<IQuizUnlockActivityRepository, EFQuizUnlockActivityRepository>();
            services.AddScoped<IEmailVerificationRepository, EFEmailVerificationRepository>();
            services.AddScoped<IvQuestionRepository, EFvQuestionRepository>();

            var jwtSection = Configuration.GetSection(nameof(JwtSettings));
            services.Configure<JwtSettings>(jwtSection);
            services.Configure<MailSettings>(Configuration.GetSection(nameof(MailSettings)));

            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IImageHelper, ImageHelper>();
            services.AddTransient<IAppUtil, AppUtil>();
            services.AddTransient<IMailHelper, MailHelper>();

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ImageFiles")));
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-GB");
                //By default the below will be set to whatever the server culture is. 
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
            });

            #region Swagger
            var jwt = jwtSection.Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwt.Secret);

            services.AddAuthentication(c =>
            {
                c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                c.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(c => c.SlidingExpiration = true)
            .AddJwtBearer(c =>
            {
                c.RequireHttpsMetadata = true;
                c.SaveToken = true;
                c.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Dohrnii Academy API",
                    Version = "v1.0",
                    Contact = new OpenApiContact
                    {
                        Name = "Sylvester Chima",
                        Email = string.Empty,
                        Url = new Uri("https://twitter.com/SylvesterLimaco"),
                    },
                });

                x.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using bearer scheme",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                        {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                    }
                });
            });
            #endregion

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.User.RequireUniqueEmail = true;
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromDays(1);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            services.Configure<CookieTempDataProviderOptions>(options => {
                options.Cookie.IsEssential = true;
            });

            services.AddControllersWithViews().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddRazorPages();

            services.AddMvc();


            var config = new MapperConfiguration(c => c.AddProfile(new ApplicationProfile()));
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
