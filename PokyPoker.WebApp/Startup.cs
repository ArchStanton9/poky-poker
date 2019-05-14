using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokyPoker.Service;
using PokyPoker.WebApp.Auth;

namespace PokyPoker.WebApp
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR(opt => opt.EnableDetailedErrors = true);
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = "room.token";
                opt.DefaultChallengeScheme = "room.token";
            }).AddRoomTokenAuth(op => {});

            services.AddSingleton<IRoomsRepository, RoomsRepository>();
            services.AddSingleton<IGamesRepository, MemoryGameRepository>();
            services.AddSingleton<IGameMapper, DtoMapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseSignalR(routes => routes.MapHub<PokerGameHub>("/gameHub"));
            app.UseMvc();

        }
    }
}
