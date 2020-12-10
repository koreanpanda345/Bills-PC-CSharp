using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BillsPC.Bot;
using BillsPC.Pokemon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BillsPC.Web_Dashboard.Data;
using Discord.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Syncfusion.Blazor;

namespace BillsPC.Web_Dashboard
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            var bot = new Bot.Bot(Configuration["bot:token"], Configuration["bot:prefix"], Configuration["Airtable:api_key"], Configuration["Airtable:base_id"]);
            services.AddSingleton(bot);
            services.AddScoped<ToastService>();
            services.AddTransient<BannerTimer>();
            services.AddSyncfusionBlazor();
            services.AddAuthentication(opt =>
                {
                    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = DiscordDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddDiscord(x =>
                {
                    x.AppId = Configuration["Discord:AppId"];
                    x.AppSecret = Configuration["Discord:AppSecret"];
                    x.Scope.Add("guilds");
                    x.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async (context)=>
                        {
                            var guildClaim = await DiscordExtensions.GetGuildClaims(context);
                            DiscordExtensions.GuildClaim = guildClaim;
                        }
                    };
                    x.SaveTokens = true;
                });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}