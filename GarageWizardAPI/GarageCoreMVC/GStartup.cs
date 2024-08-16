
using GarageCoreMVC.Services;
using GarageCoreMVC.Services.Interfaces;
using GarageCoreMVC.Data;
using GarageCoreMVC.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using GarageCoreMVC.Common.Configurations;
using System.Text;
using GarageCoreMVC.Middleware;
using System.Diagnostics.CodeAnalysis;


namespace GarageCoreMVC
{
    [ExcludeFromCodeCoverage]
    public class GStartup
    {
        // Register services using their dependent classes with IoC container
        // Service is simply a class with operations that are going to be used in other  classes 
        // IApplicationBuilder, IHostingEnvironment & ILoggerFactory are framework services
        // However, for custom operations, we need to create application services 
        public void ConfigureServices(IServiceCollection services)
        {
            // Every class needs to create a service descriptor to add them to collection of services 
            // Here, 'ILog' is a service type and Logger is an essential object instance of a certain life time
            // Singleton is a single instance shared throughout the app whereas Transient creates new instance every time!
            // Scoped creates an instance of service once per request and shared in that request !
            // Property injection is not supported in built-in IOC containers 
            services.AddControllersWithViews();

            services.AddDbContext<GarageDBContext>();
            services.AddIdentity<User, Role>()
                    .AddEntityFrameworkStores<GarageDBContext>();

            services.Add(new ServiceDescriptor(typeof(ILog), new Logger(), ServiceLifetime.Singleton));
            services.AddSingleton<IStorage<Vehicle>, Storage<Vehicle>>();
            services.AddSingleton<IStorage<Spot>, Storage<Spot>>();
            // services.AddScoped<IGarage, Garage> (); // For Garage XML retreival
            services.AddScoped<IGarage, GarageAccess>();
            // services.AddScoped<IToll, Toll>(); // For Toll XML retreival
            services.AddScoped<IToll, TollAccess>();
            services.AddScoped<IParking, ParkingAccess>();
            services.AddScoped<IRepair, RepairAccess>();
            services.AddAutoMapper(typeof(GStartup));
            services.AddCors(options =>
                { 
                    options.AddPolicy("MyPolicy", builder =>
                    {
                        builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                    });
                }
            );

            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = "/Account/Login"; // It is already the default route
            //});

            #region JWTAuthentication
            //Adding JWT Authentication Scheme
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = GlobalConfig.JWTIssuer,
                    ValidAudience = GlobalConfig.JWTAudience,
                    ClockSkew = TimeSpan.Zero, // Instant expiry after time
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalConfig.JWTSecretKey))
                };
                options.SaveToken = true;
                options.UseSecurityTokenValidators = true;
                options.Events = new()
                {
                    OnMessageReceived = context =>
                    {
                        if (context.HttpContext.Request.Cookies.TryGetValue(GlobalConfig.GeneratedTokenCookieName, out var token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            //{
            //    options.ExpireTimeSpan = TimeSpan.FromHours(GlobalConfig.JWTExpiryHours);
            //});
            #endregion

            //services.AddAuthentication(option =>
            //{
            //    option.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            //    option.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            //});

            services.AddAuthorization();
            //services.AddAuthorization(options => 
            //    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            //                                .RequireAuthenticatedUser().Build()
            //);

            // Enable in-memory caching
            services.AddMemoryCache();

            // Configuring session for SSO
            services.AddDistributedMemoryCache();
            services.AddSession(
                options =>
                {
                    options.IdleTimeout = TimeSpan.FromSeconds(5);
                    options.Cookie.Name = "Garage_Session";
                    options.Cookie.IsEssential = true;
                    options.Cookie.HttpOnly = true;
                    // Cookies default path is root 
                }
            );

            /*
                Another way to do this is to use generics:
                services.AddSingleton<ILog, Logger>(); or services.AddSingleton(typeof(ILog), typeof(Logger)) 
            */
        }

        private Task SayBye(HttpContext context)
        {
            return context.Response.WriteAsync("Bye World !");
        }

        // Services can be used in this method to configure middleware operations 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enabling Legacy DateTime behaviour for PostGreSQL
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //Console.WriteLine(env.EnvironmentName);
            // Check the environment if it is in staging, development or production
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Perform middleware operations for HTTP request pipeline

            //app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //            Path.Combine(Directory.GetCurrentDirectory(), "Images")),
            //    RequestPath = new PathString("/images")
            //});

            //Set up default root page options
            //DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            //defaultFilesOptions.DefaultFileNames.Clear();
            //defaultFilesOptions.DefaultFileNames.Add("index.html");
            //app.UseDefaultFiles(defaultFilesOptions);

            // Fileserver combines the functionality of both using default and static files
            app.UseFileServer();
            app.UseSession();

            app.UseRouting();
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            // Use custom middleware using class [We can also use: app.UseMiddleware<MiddlewareClass>()]
            //app.UseTokenMiddleware();
            //app.UseAuthMiddleware();

            //To invoke next middleware in the sequence, we can't use Run() anymore but, we should use Use()
            //app.Use(async (context, next) =>
            //    {
            //        await context.Response.WriteAsync($"Hello World\n");
            //        await next();
            //    }
            //);

            ////app.Run(async (context) =>
            ////{
            ////    await context.Response.WriteAsync("Hello World!");
            ////});
            //app.Run(SayBye);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}"
                );
                endpoints.MapControllers();
            });
        }
    }
}
