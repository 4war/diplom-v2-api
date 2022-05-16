using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Api.Rtt.Models;
using Api.Rtt.Models.Seeds;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Api.Rtt
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();
      var authOptionsConfiguration = Configuration.GetSection("Auth");
      var authOptions = authOptionsConfiguration.Get<AuthOptions>();
      services.Configure<AuthOptions>(authOptionsConfiguration);

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          options.RequireHttpsMetadata = false;
          options.TokenValidationParameters = new TokenValidationParameters()
          {
            ValidateIssuer = true,
            ValidIssuer = authOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = authOptions.Audience,

            ValidateLifetime = true,

            IssuerSigningKey = authOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
          };
        });

      services.AddCors(options =>
      {
        options.AddPolicy("CorsPolicy",
          c => c
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
        );
      });

      services.AddDbContext<ApiContext>(options =>
      {
        options.EnableSensitiveDataLogging();
        options.UseMySQL(
          Configuration["Data:ConnectionString"]);
      });

      services.AddTransient<DataSeed>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataSeed seed)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseCors("CorsPolicy");
      }

      app.UseStaticFiles();
      app.UseHttpsRedirection();
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      SetupEndpoints(app);
      seed.SeedData();
    }

    private static void SetupEndpoints(IApplicationBuilder app)
    {
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "api/{controller}/{action}/{id?}");
      });
    }
  }
}
