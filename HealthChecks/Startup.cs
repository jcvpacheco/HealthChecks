using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthChecks
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
            //
            //  This project configure health checks for asp.net core project and UI
            //  in the same project. Typically health checks and UI are on different projects 
            //  UI exist also as container image
            //

            services.AddDbContext<MyDbContext>(options => options.UseInMemoryDatabase("DbContextHealthCheck"));

            services
                .AddHealthChecksUI()
                .AddHealthChecks()
                .AddCheck<RandomHealthCheck>("random")
                .AddCheck<MyDbContextHealthCheck>("DbContextHealthCheck")
                .Services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();


            //Option 1
            //app.UseHealthChecks("/health");

            //Option 2
            //app.UseHealthChecks("/health", new HealthCheckOptions
            //{
            //    ResponseWriter = async (context, report) =>
            //    {
            //        context.Response.ContentType = "application/json; charset=utf-8";
            //        var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(report));
            //        await context.Response.Body.WriteAsync(bytes);
            //    },

            //    ResultStatusCodes =
            //    {
            //        [HealthStatus.Healthy] = StatusCodes.Status200OK,
            //        [HealthStatus.Degraded] = StatusCodes.Status200OK,
            //        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            //    }
            //});

            //Option 3
            //Use / healthchecks-ui
            app.UseHealthChecks("/healthz", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            })
           .UseHealthChecksUI()
           .UseMvc();
        }
    }


}
