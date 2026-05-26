using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Repositories.Auth;
using QLNhaTro.Repositories.BaiDang;
using QLNhaTro.Repositories.ChuTro;
using QLNhaTro.Repositories.NhaTro;
using QLNhaTro.Repositories.PhongTro;
using QLNhaTro.Repositories.TienNghi;

namespace QLNhaTro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<PhongTroDaNangContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IPhongTroRepository, PhongTroRepository>();
            builder.Services.AddScoped<IPhongTroManagementRepository, PhongTroManagementRepository>();
            builder.Services.AddScoped<INhaTroRepository, NhaTroRepository>();
            builder.Services.AddScoped<IBaiDangRepository, BaiDangRepository>();
            builder.Services.AddScoped<IChuTroTaiKhoanRepository, ChuTroTaiKhoanRepository>();
            builder.Services.AddScoped<ITienNghiRepository, TienNghiRepository>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(2);
                    options.SlidingExpiration = true;
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
