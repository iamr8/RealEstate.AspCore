using CacheManager.Core;
using EFSecondLevelCache.Core;
using ElmahCore.Mvc;
using HibernatingRhinos.Profiler.Appender.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Configuration;
using RealEstate.Resources;
using RealEstate.Services.Database;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.Tracker;
using System;
using System.Data.SqlClient;
using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using WebMarkupMin.AspNetCore2;
using WebMarkupMin.Core;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace RealEstate.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEFSecondLevelCache();

            // Add an in-memory cache service provider
            services.AddSingleton(typeof(ICacheManager<>), typeof(BaseCacheManager<>));
            services.AddSingleton(typeof(ICacheManagerConfiguration),
                new CacheManager.Core.ConfigurationBuilder()
                    .WithJsonSerializer(JsonExtensions.JsonNetSetting, JsonExtensions.JsonNetSetting)
                    .WithMicrosoftMemoryCacheHandle(instanceName: "MemoryCache1")
                    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(10))
                    .DisablePerformanceCounters()
                    .DisableStatistics()
                    .Build());

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (connectionString.Contains("{{CFG}}", StringComparison.CurrentCulture))
            {
                // D:\\RSDB\\RSDB.mdf
                var config = Assembly.GetEntryAssembly().ReadConfiguration();
                if (config == null)
                    return;

                connectionString = connectionString.Replace("{{CFG}}", config.DbPath, StringComparison.CurrentCulture);
            }
            Console.WriteLine(connectionString);
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseLazyLoadingProxies();

                options.UseSqlServer(connectionString,
                        optionsBuilder =>
                        {
                            optionsBuilder.MigrationsAssembly($"{nameof(RealEstate)}.{nameof(RealEstate.Web)}");
                            optionsBuilder.UseNetTopologySuite();
                            optionsBuilder.EnableRetryOnFailure();
                            optionsBuilder.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
                            optionsBuilder.UseRowNumberForPaging();
                        });
                options.ConfigureWarnings(config =>
                {
                    config.Log(CoreEventId.IncludeIgnoredWarning);
                    config.Log(CoreEventId.NavigationIncluded);
                    config.Log(CoreEventId.LazyLoadOnDisposedContextWarning);
                    config.Throw(RelationalEventId.QueryClientEvaluationWarning);
                    config.Throw(CoreEventId.LazyLoadOnDisposedContextWarning);
                    config.Throw(CoreEventId.NavigationLazyLoading);
                });
                options.EnableSensitiveDataLogging();
            });

            services.AddCors();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = AuthenticationScheme.Scheme;
                    options.DefaultSignInScheme = AuthenticationScheme.Scheme;
                    options.DefaultChallengeScheme = AuthenticationScheme.Scheme;
                })
                .AddCookie(AuthenticationScheme.Scheme,
                    options =>
                    {
                        options.SlidingExpiration = false;
                        options.LoginPath = "/Index";
                        options.LogoutPath = $"/{nameof(RealEstate.Web.Pages.Manage)}/Logout";
                        options.AccessDeniedPath = "/Forbidden";
                        options.ExpireTimeSpan = TimeSpan.FromDays(30);
                        options.EventsType = typeof(AuthenticationTracker);
                        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                        options.Cookie.SameSite = SameSiteMode.Lax;
                    });

            services.AddMvcCore();

            services.AddElmah(options =>
            {
                options.ConnectionString = connectionString;
            });
            services.AddSingleton<LocalizationService>();
            services.AddLocalization(options => options.ResourcesPath = "");

            services
                .AddMvc(options =>
                {
                    options.ModelBinderProviders.Insert(0, new StringModelBinderProvider());
                })
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder($"/{nameof(RealEstate.Web.Pages.Manage)}");
                    options.Conventions.AddPageRoute("/manage/owners/edit", "manage/owners/edit/{id}");
                })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                        return factory.Create(nameof(SharedResource), assemblyName.Name);
                    };
                })
                .AddJsonOptions(
                    options =>
                    {
                        var settings = JsonExtensions.JsonNetSetting;
                        options.SerializerSettings.Error = settings.Error;
                        options.SerializerSettings.DefaultValueHandling = settings.DefaultValueHandling;
                        options.SerializerSettings.ReferenceLoopHandling = settings.ReferenceLoopHandling;
                        options.SerializerSettings.ObjectCreationHandling = settings.ObjectCreationHandling;
                        options.SerializerSettings.ContractResolver = settings.ContractResolver;
                        options.SerializerSettings.Formatting = settings.Formatting;
                        options.SerializerSettings.PreserveReferencesHandling = settings.PreserveReferencesHandling;
                    }
                );

            EntityFrameworkProfiler.Initialize();
            services.AddHealthChecks()
                .AddCheck("sql-server", () =>
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                        }
                        catch (SqlException)
                        {
                            return HealthCheckResult.Unhealthy();
                        }
                    }
                    return HealthCheckResult.Healthy();
                });

            services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = 838860800);
            services.AddWebMarkupMin(
                    options =>
                    {
                        options.AllowMinificationInDevelopmentEnvironment = true;
                        options.AllowCompressionInDevelopmentEnvironment = true;
                        options.DisablePoweredByHttpHeaders = true;
                    })
                .AddHtmlMinification(
                    options =>
                    {
                        options.MinificationSettings.RemoveRedundantAttributes = true;
                        options.MinificationSettings.RemoveHttpProtocolFromAttributes = true;
                        options.MinificationSettings.RemoveHttpsProtocolFromAttributes = true;
                        options.MinificationSettings.MinifyEmbeddedJsCode = true;
                        options.GenerateStatistics = true;
                        options.MinificationSettings.RemoveHtmlComments = true;
                        options.MinificationSettings.MinifyInlineCssCode = true;
                        options.MinificationSettings.WhitespaceMinificationMode = WhitespaceMinificationMode.Aggressive;
                        options.MinificationSettings.RemoveJsTypeAttributes = true;
                    });
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });
            services
                .AddResponseCaching()
                .AddResponseCompression(options =>
                {
                    options.Providers.Add<BrotliCompressionProvider>();
                    options.Providers.Add<GzipCompressionProvider>();
                    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                            new[] { "image/svg+xml" });
                });
            services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

            services.AddScoped<AuthenticationTracker>();
            services.AddScoped<IUnitOfWork, ApplicationDbContext>();
            services.AddDistributedMemoryCache();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            services.AddRequiredServices();

            services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler";
                options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();
                options.ResultsAuthorize = request => !Program.DisableProfilingResults;
                options.TrackConnectionOpenClose = false;
            }).AddEntityFramework();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseFileServer();

            app.UseCors(options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonConvert.SerializeObject(new
                    {
                        status = report.Status.ToString(),
                        errors = report.Entries.Select(e =>
                            new
                            {
                                key = e.Key,
                                value = Enum.GetName(typeof(HealthStatus), e.Value.Status)
                            })
                    });
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            });

            if (!env.IsDevelopment())
            {
                app.UseStatusCodePages(async context =>
                {
                    var statusCode = context.HttpContext.Response.StatusCode;
                    if (statusCode == 404)
                        context.HttpContext.Response.Redirect("/Error");
                });
            }

            app.UseAuthentication();
            app.UseElmah();

            if (!env.IsDevelopment())
            {
                app.UseResponseCaching();
                app.UseResponseCompression();
                app.UseWebMarkupMin();
            }

            if (!env.IsDevelopment())
            {
                app.UseResponseCaching();
                app.UseResponseCompression();
                app.UseWebMarkupMin();
            }

            if (env.IsDevelopment())
                app.UseMiniProfiler();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers[HeaderNames.CacheControl] =
                        $"public,max-age={TimeSpan.FromDays(365).TotalMilliseconds}";
                }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Setting}/{action=Index}/{id?}");
            });
        }
    }
}