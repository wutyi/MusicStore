using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Security;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using MusicStore.Apis;
using MusicStore.Models;
using MusicStore.Spa.Infrastructure;

namespace MusicStore.Spa
{
    public class Startup
    {
        public Startup()
        {
            Configuration = new Configuration()
                        .AddJsonFile("Config.json")
                        .AddEnvironmentVariables();
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SiteSettings>(settings =>
            {
                settings.DefaultAdminUsername = Configuration.Get("DefaultAdminUsername");
                settings.DefaultAdminPassword = Configuration.Get("DefaultAdminPassword");
            });

            // Add MVC services to the service container
            services.AddMvc();

            // Add EF services to the service container
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<MusicStoreContext>(options =>
                {
                    options.UseSqlServer(Configuration.Get("Data:DefaultConnection:ConnectionString"));
                });

            // Add Identity services to the services container
            services.AddIdentity<ApplicationUser, IdentityRole>(Configuration)
                    .AddEntityFrameworkStores<MusicStoreContext>()
                    .AddDefaultTokenProviders()
                    .AddMessageProvider<EmailMessageProvider>()
                    .AddMessageProvider<SmsMessageProvider>();

            // Add application services to the service container
            //services.AddTransient<IModelMetadataProvider, BuddyModelMetadataProvider>();

            // Configure Auth
            services.Configure<AuthorizationOptions>(options =>
            {
                options.AddPolicy("app-ManageStore", new AuthorizationPolicyBuilder().RequiresClaim("app-ManageStore", "Allowed").Build());
            });

#if ASPNET50
            AutoMapper.Mapper.CreateMap<AlbumChangeDto, Album>();
            AutoMapper.Mapper.CreateMap<Album, AlbumChangeDto>();
            AutoMapper.Mapper.CreateMap<Album, AlbumResultDto>();
            AutoMapper.Mapper.CreateMap<AlbumResultDto,Album>();
            AutoMapper.Mapper.CreateMap<Artist, ArtistResultDto>();
            AutoMapper.Mapper.CreateMap<ArtistResultDto, Artist>();
            AutoMapper.Mapper.CreateMap<Genre, GenreResultDto>();
            AutoMapper.Mapper.CreateMap<GenreResultDto, Genre>();
#endif
        }

        public void Configure(IApplicationBuilder app)
        {
            // Initialize the sample data
            SampleData.InitializeMusicStoreDatabaseAsync(app.ApplicationServices).Wait();

            // Configure the HTTP request pipeline

            // Add cookie auth
            app.UseIdentity();

            // Add static files
            app.UseStaticFiles();

            // Add MVC
            app.UseMvc();
        }
    }
}
