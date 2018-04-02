﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using TriggMine.ChatBot.Core.Services;
using TriggMine.ChatBot.Core.Services.Interfaces;
using TriggMine.ChatBot.Repository.Context;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Repository.Repository;

namespace TriggMine.ChatBot.Core
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

            //var sqlConnectionString = Configuration.GetConnectionString("ChatBotConnectionStrings");

            //services.AddDbContext<ChatBotContext>(options => options.UseNpgsql(sqlConnectionString
            //    , x => x.MigrationsHistoryTable("__EFMigrationsHistory", "ChatBot")
            //    ).EnableSensitiveDataLogging());




            services.AddMvc();
            services.AddLogging();

            services.AddTransient<Func<IChatBotContext>>(s => () => new ChatBotContext());

            services.AddTransient<IChatBotRepository<User>, UserRepository>();
            services.AddTransient<IChatBotRepository<Message>, MessageRepository>();
            services.AddTransient<IChatBotRepository<ResolvedUrl>, ResolvedUrlRepository>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IResolverUrlService, ResolverUrlService>();
            services.AddTransient<ITelegramBotService, TelegramBotService>();

            //Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Notification API", Version = "v1" });
                c.CustomSchemaIds(x => x.FullName);
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ITelegramBotService telegramBotService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            telegramBotService.GetBot();
            //Connect Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification API V1");
            });

            //Connect SeriLog with appsetings.json 

            var configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .Build();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
