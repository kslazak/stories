using System.Reflection;
using Microsoft.OpenApi.Models;
using StoriesAPI;

var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var apiInfo = new OpenApiInfo
{
    Version = "v1",
    Title = "Stories API",
    Description = "An ASP.NET Core Web API for retrieving Hacker News stories."
};
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IStoryProvider, StoryProvider>();
builder.Services.AddSingleton<IStoryCache, StoryCache>();
builder.Services.AddSingleton<IStoryService, StoryService>();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        var factory = options.InvalidModelStateResponseFactory;

        options.InvalidModelStateResponseFactory = context =>
        {
            if (!context.ModelState.IsValid)
            {
                var request = context.HttpContext.Request;
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                foreach (var modelState in context.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                        logger.LogAsError($"Invalid request: {request.Path}{request.QueryString} {error.ErrorMessage}", context.HttpContext);
                }
            }

            return factory(context);
        };
    });

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", apiInfo);
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
