using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BethanysPieShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace BethanysPieShop
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<AppDbContext>();

            //register services here through Dependency Injection to achieve more loosely coupled architecture
            services.AddScoped<IPieRepository, PieRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ShoppingCart>(sp => ShoppingCart.GetCart(sp));
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddHttpContextAccessor();
            services.AddSession();

            //add transient and add singleton will often be used but not now
            //services.AddTransient
            //services.AddSingleton

            //register framework services - replaces add mvc and still supports mvc
            services.AddControllersWithViews();
            services.AddRazorPages();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //add middleware components here

            //enables the use of the developer exception page to help debug
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //require https
            app.UseHttpsRedirection();
            //enable site to enable the use of static files.
            app.UseStaticFiles();
            app.UseSession();

            app.UseStatusCodePages();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //responsible for mapping incoming requests with a controller
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
