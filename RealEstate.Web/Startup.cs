using CacheManager.Core;
using EFSecondLevelCache.Core;
using ElmahCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using RealEstate.Base;
using RealEstate.Configuration;
using RealEstate.Resources;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Extensions;
using RealEstate.Services.Tracker;
using System;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using WebMarkupMin.AspNetCore2;
using WebMarkupMin.Core;
using ConfigurationBuilder = CacheManager.Core.ConfigurationBuilder;
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
            var connectionStrings = Configuration.GetConnectionString("DefaultConnection");
            if (connectionStrings.Contains("{{CFG}}"))
            {
                // D:\\RSDB\\RSDB.mdf
                var config = Assembly.GetEntryAssembly().ReadConfiguration();
                if (config == null)
                    return;

                connectionStrings = connectionStrings.Replace("{{CFG}}", config.DbPath);
            }
            Console.WriteLine(connectionStrings);
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(connectionStrings,
                    optionsBuilder =>
                    {
                        optionsBuilder.MigrationsAssembly($"{nameof(RealEstate)}.{nameof(RealEstate.Web)}");
                        optionsBuilder.UseNetTopologySuite();
                        optionsBuilder.EnableRetryOnFailure();
                        optionsBuilder.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
                    });
                options.ConfigureWarnings(config =>
                {
                    config.Log(CoreEventId.IncludeIgnoredWarning);
                    config.Log(CoreEventId.NavigationIncluded);
                    config.Log(CoreEventId.NavigationLazyLoading);
                    config.Log(CoreEventId.DetachedLazyLoadingWarning);
                    config.Log(CoreEventId.LazyLoadOnDisposedContextWarning);
                    config.Log(RelationalEventId.QueryClientEvaluationWarning);
                });
                options.EnableSensitiveDataLogging();
            });
            //            .AddEntityFrameworkProxies();

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

            services
                .AddMvcCore();

            services.AddElmah(options =>
            {
                //                options.CheckPermissionAction = context => context.User.Identity.IsAuthenticated;
                options.ConnectionString = connectionStrings;
            });
            services.AddSingleton<LocalizationService>();
            services.AddLocalization(options => options.ResourcesPath = "");

            services
                .AddMvc()
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

            services.AddConnections();

            services.AddSignalR(options =>
            {
                // Faster pings for testing
                options.KeepAliveInterval = TimeSpan.FromSeconds(5);
            });
            //                .AddMessagePackProtocol();
            //.AddStackExchangeRedis();

            services
                .AddEFSecondLevelCache();
            services
                .AddSingleton(typeof(ICacheManager<>), typeof(BaseCacheManager<>));
            services
                .AddSingleton(typeof(ICacheManagerConfiguration),
                    new ConfigurationBuilder()
                    .WithJsonSerializer()
                    .WithMicrosoftMemoryCacheHandle()
                    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(10))
                    .Build());

            services.AddScoped<AuthenticationTracker>();
            services.AddScoped<IUnitOfWork, ApplicationDbContext>();
            services
                .AddDistributedMemoryCache();
            services
                .AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services
                .AddTransient<IActionContextAccessor, ActionContextAccessor>();

            services.AddRequiredServices();
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

            app.UseStatusCodePages(async context =>
            {
                var statusCode = context.HttpContext.Response.StatusCode;
                if (statusCode == 404)
                    context.HttpContext.Response.Redirect("/Error");
            });
            //app.UseRouting(routes =>
            //{
            //    routes.MapHub<DynamicChat>("/dynamic");
            //    routes.MapHub<Chat>("/default");
            //    routes.MapHub<Streaming>("/streaming");
            //    routes.MapHub<UploadHub>("/uploading");
            //    routes.MapHub<HubTChat>("/hubT");

            //    routes.MapConnectionHandler<MessagesConnectionHandler>("/chat");

            //    routes.MapGet("/deployment", context =>
            //    {
            //        var attributes = Assembly.GetAssembly(typeof(Startup)).GetCustomAttributes<AssemblyMetadataAttribute>();

            //        context.Response.ContentType = "application/json";
            //        using (var textWriter = new StreamWriter(context.Response.Body))
            //        using (var writer = new JsonTextWriter(textWriter))
            //        {
            //            var json = new JObject();
            //            var commitHash = string.Empty;

            //            foreach (var attribute in attributes)
            //            {
            //                json.Add(attribute.Key, attribute.Value);

            //                if (string.Equals(attribute.Key, "CommitHash"))
            //                {
            //                    commitHash = attribute.Value;
            //                }
            //            }

            //            if (!string.IsNullOrEmpty(commitHash))
            //            {
            //                json.Add("GitHubUrl", $"https://github.com/aspnet/SignalR/commit/{commitHash}");
            //            }

            //            json.WriteTo(writer);
            //        }

            //        return Task.CompletedTask;
            //    });
            //});

            app.UseEFSecondLevelCache();
            app.UseAuthentication();
            app.UseElmah();

            if (!env.IsDevelopment())
            {
                app.UseResponseCaching();
                app.UseResponseCompression();
                app.UseWebMarkupMin();
            }
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