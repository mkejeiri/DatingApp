using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API
{
    public class Startup
    {
       public Startup(IConfiguration configuration) 
        {            
            this.Configuration = configuration;            
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //everything as a service get injected into the app
            services.AddDbContext<DataContext>(x=>x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors();
            //services.AddSingleton(IAuthRepository); //not good for concurrent request
            //services.AddTransient(IAuthRepository); //an object per request is created and lighw ight for services
            services.AddScoped<IAuthRepository, AuthResposity>();//created per request within the scope, a singleton within a scope itself

            //add Authorization service
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer =false /*So far we use localhost*/,
                        ValidateAudience = false /*So far we use localhost*/
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //Order matter here and not in ConfigureServices
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // app.UseHsts();
            }

            // app.UseHttpsRedirection();

            /* I N F O : we define a Cors policy because of cross call!!!
            Failed to load http://localhost:5000/api/values: 
            No 'Access-Control-Allow-Origin' header is present on the requested resource.
            Origin 'http://localhost:4200' is therefore not allowed access.
            */
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseMvc(); //Middleware: route the request the appropriate controller 
        }
    }
}
