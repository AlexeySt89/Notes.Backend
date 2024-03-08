using Notes.Application.Common.Mapping;
using System.Reflection;
using Notes.Persistence;
using Notes.Application.Interfaces;
using Notes.Application;
using Microsoft.Extensions.Options;
using Notes.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Notes.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            /*var serviseProvider = builder.Services.BuildServiceProvider();
            try
            {
                var context = serviseProvider.GetRequiredService<NotesDbContext>();
                DbInitialize.Initialize(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine("JIB<RF!!!!!!");
            }*/
            

            builder.Services.AddAutoMapper(config =>
            {
                config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
                config.AddProfile(new AssemblyMappingProfile(typeof(INotesDbContext).Assembly));
            });


            builder.Services.AddApplication();
            builder.Services.AddPersistence(builder.Configuration);
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });

            builder.Services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:7295";
                    options.Audience = "NotesWebAPI";
                    options.RequireHttpsMetadata = false;
                });

            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
            });
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            builder.Services.AddSwaggerGen();

            builder.Services.AddApiVersioning();

            var app = builder.Build();
            /*IApiVersionDescriptionProvider provider;*/

            using (var serviceScope = app.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<NotesDbContext>();
                    DbInitialize.Initialize(context);
                }catch (Exception ex)
                {
                    Console.WriteLine("ÎØÈÁÊÀ!!!!!!!");
                }
            }
            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                /*foreach(var desc in provider.ApiVersionDescriptions)
                {
                    config.SwaggerEndpoint(
                        $"/swagger/{desc.GroupName}/swagger.json",
                        desc.GroupName.ToUpperInvariant());
                }*/
                config.RoutePrefix = string.Empty;
                config.SwaggerEndpoint("swagger/v1/swagger.json", "Notes API");
            });
            app.UseCustomExceptionHandler();
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseApiVersioning();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            /*app.MapGet("/", () => "Hello World!");*/

            app.Run();
        }
    }
}
