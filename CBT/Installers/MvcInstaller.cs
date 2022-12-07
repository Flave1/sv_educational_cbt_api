using CBT.BLL.Services.Authentication;
using CBT.BLL.Services.CandidateAnswers;
using CBT.BLL.Services.Candidates;
using CBT.BLL.Services.Category;
using CBT.BLL.Services.Class;
using CBT.BLL.Services.Examinations;
using CBT.BLL.Services.FileUpload;
using CBT.BLL.Services.Questions;
using CBT.BLL.Services.Session;
using CBT.BLL.Services.Settings;
using CBT.BLL.Services.Subject;
using CBT.BLL.Services.WebRequests;
using CBT.Contracts.Options;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;

namespace CBT.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FwsConfigSettings>(
               configuration.GetSection("FwsConfigSettings"));

            services.AddScoped<IWebRequest, WebRequest>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ICandidateCategoryService, CandidateCategoryService>();
            services.AddScoped<ICandidateService, CandidateService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IExaminationService, ExaminationService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<ICandidateAnswerService, CandidateAnswerService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISettingService, SettingService>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddHttpClient();

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FLAVECONSOLE CBT APIS",
                    Version = "V1",
                    Description = "An API for computer base test.",
                    TermsOfService = new Uri("http://www.godp.co.uk/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Emmanuel Favour",
                        Email = "favouremmanuel433@gmail.com",
                        //Url = new Uri("https://twitter.com/FavourE65881201"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "FLAVE API LICX",
                        //Url = new Uri("www.flavetechs.com"),
                    },

                });

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[0] }
                };
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "FWS Authorization header using bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {new OpenApiSecurityScheme {Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    } }, new List<string>() }
                });
            });
        }
    }
}
