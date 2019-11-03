using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Intranet.Data;
using Intranet.Data.Services;
using Intranet.Model.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Intranet.Web.Data;
using Microsoft.AspNetCore.Localization;
using Zek.Extensions.Security.Claims;
using Zek.Model.Config;
using Zek.Model.Identity;
using Zek.Services;
using IEmailSender = Intranet.Web.Services.IEmailSender;
using User = Intranet.Web.Models.User;

namespace Intranet.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("TestConnection")))
                .AddDbContext<IntranetDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<Zek.Model.Identity.User, Role>()
                .AddEntityFrameworkStores<IntranetDbContext>()
                .AddDefaultTokenProviders();

            //todo services.AddMvc(MvcOptionHelper.Config);
            services.AddMvc();
            services.AddMemoryCache();
            services.AddResponseCompression();

            services.AddTransient<Zek.Services.IEmailSender, EmailSender>();
            services.AddTransient<ISmsSender, GeocellSmsSender>();
            services.AddTransient<IApiClient, HttpApiClient>();
            services.AddTransient<IIntranetUnitOfWork, IntranetUnitOfWork>();
            services.AddTransient<IIntranetCacheService, IntranetCacheService>();
            //services.AddTransient<ILicenseService, LicenseService>();
            //services.AddTransient<Travel.Data.Services.ICacheService, Travel.Data.Services.CacheService>();
            //services.AddTransient<IPlanCacheService, PlanCacheService>();

            //services.AddTransient<ICustomerCareUnitOfWork, CustomerCareUnitOfWork>();
            //services.AddTransient<ICorporateUnitOfWork, CorporateUnitOfWork>();
            //services.AddTransient<ICorporateCacheService, CorporateCacheService>();


            services.AddOptions();
            services.Configure<SmsSenderOptions>(options => Configuration.GetSection(nameof(SmsSenderOptions)).Bind(options));
            services.Configure<EmailSenderOptions>(options => Configuration.GetSection(nameof(EmailSenderOptions)).Bind(options));
            services.Configure<IntranetOptions>(options => Configuration.GetSection(nameof(IntranetOptions)).Bind(options));
             services.Configure<ApiOptions>(options => Configuration.GetSection(nameof(ApiOptions)).Bind(options));

            services.Configure<IdentityOptions>(options =>
            {
                //options.Cookies.ApplicationCookie.AuthenticationScheme += ".Intranet.Web";
                //ClaimsPrincipalExtensions.AuthenticationType = options.Cookies.ApplicationCookie.AuthenticationScheme;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            var supportedCultures = new[]
            {
                new CultureInfo("ka-GE"),
                //new CultureInfo("en-US"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ka-GE"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            app.UseResponseCompression();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "api",
                    template: "api/{controller}/{action}/{id?}");
            });
        }
    }
}
