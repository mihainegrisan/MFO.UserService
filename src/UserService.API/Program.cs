using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSwag;
using Serilog;
using UserService.Application.CommandsQueries.Queries;
using UserService.Application.Interfaces;
using UserService.Application.Mapping;
using UserService.Application.Validators;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // To preserve the default behavior, capture the original delegate to call later.
        var builtInFactory = options.InvalidModelStateResponseFactory;

        options.InvalidModelStateResponseFactory = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

            //logger.LogWarning("Invalid model state: {Errors}", context.ModelState.Values
            //    .SelectMany(v => v.Errors)
            //    .Select(e => e.ErrorMessage));

            var errors = context.ModelState
                .Where(kvp => kvp.Value?.Errors.Count > 0)
                .Select(kvp => new {
                    Field = kvp.Key,
                    Messages = kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                })
                .ToArray();

            logger.LogWarning(
                "ModelState invalid on {Path}. Errors: {@Errors}",
                context.HttpContext.Request.Path,
                errors);

            // Invoke the default behavior, which produces a ValidationProblemDetails response.
            // To produce a custom response, return a different implementation of IActionResult instead.
            return builtInFactory(context);
        };
    });

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options 
    => options.UseSqlServer(builder.Configuration.GetConnectionString("UserContext")));

builder.Services.AddScoped<IUserRepository, UserRepository>();

// This points to the assembly where the MediatR handlers are located.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetUserByIdHandler).Assembly));

builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new UserServiceProfile()));

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>(ServiceLifetime.Transient);
// Or you can register a specific validator like this:
// builder.Services.AddScoped<IValidator<User>, CreateUserDtoValidator>();

builder.Services.AddOpenApiDocument(options =>
{
    options.PostProcess = document =>
    {
        document.Info = new OpenApiInfo
        {
            Version = "v1",
            Title = "User Service API",
            Description = "An ASP.NET Core Web API for managing Users",
            //TermsOfService = "https://example.com/terms",
            Contact = new OpenApiContact
            {
                Name = "Mihai Negrisan",
                Url = "https://github.com/mihainegrisan?tab=overview"
            },
            //License = new OpenApiLicense
            //{
            //    Name = "Example License",
            //    Url = "https://example.com/license"
            //}
        };
    };
});

// Uncomment the following line to enable CORS for all origins, methods, and headers.
// builder.Services.AddCors(options => options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Uncomment the following line to use an in-memory database instead of SQL Server.
//builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("UserContext"));

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "UserService")
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Add OpenAPI 3.0 document serving middleware
    // Available at: http://localhost:<port>/swagger/v1/swagger.json
    app.UseOpenApi();

    // Add web UIs to interact with the document
    // Available at: http://localhost:<port>/swagger
    app.UseSwaggerUi(); // UseSwaggerUI Protected by if (env.IsDevelopment())
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    //context.Database.EnsureCreated(); // checks if the database exists and creates it with the current model if it doesn't — without using migrations. This creates all the tables directly.
    DbInitializer.Initialize(context);
}

app.MapControllers();

app.Run();
