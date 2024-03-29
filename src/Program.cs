using Dapper;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NHibernate;
using theforum.BusinessLogic;
using theforum.DataAccess;
using theforum.Filters;
using theforum.HealthChecks;
using theforum.Resources;
using theforum.Settings;
using ISession = NHibernate.ISession;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON options
builder.Services.AddMvc();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationExceptionFilter>();
    options.Filters.Add<ValidateModelAttribute>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.MaxDepth = int.MaxValue;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// Suppress default model state validation
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Swagger and API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Data protection with Azure Blob Storage
var dataProtectionKeysBlobConnectionString = builder.Configuration["Storage:DataProtectionKeysConnectionString"];
var dataProtectionKeysBlobContainer = builder.Configuration["Storage:DataProtectionKeysContainer"];

if (!string.IsNullOrEmpty(dataProtectionKeysBlobConnectionString) &&
    !string.IsNullOrEmpty(dataProtectionKeysBlobContainer))
{
    builder.Services.AddDataProtection()
        .PersistKeysToAzureBlobStorage(
            dataProtectionKeysBlobConnectionString,
            dataProtectionKeysBlobContainer,
            "keys.xml");
}

// Authentication scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

// Register configurations
builder.Configuration.AddJsonFile("appsettings.resources.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile("appsettings.styles.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<ForumResources>(builder.Configuration.GetSection("Resources").GetSection("Forum"));
builder.Services.Configure<StyleSettings>(builder.Configuration.GetSection("Themes").GetSection("Forum"));

// Database connections
var masterConnectionString = builder.Configuration.GetConnectionString("MasterDatabase");
var applicationConnectionString = builder.Configuration.GetConnectionString("ApplicationDatabase");
if (string.IsNullOrEmpty(masterConnectionString) || string.IsNullOrEmpty(applicationConnectionString))
{
    // TODO: Log out an error
    return;
}
CheckAndCreateAppDatabase(masterConnectionString, applicationConnectionString);

// NHibernate Setup
NHibernateHelper.Initialize(applicationConnectionString);
builder.Services.AddSingleton(NHibernateHelper.GetSessionFactory());
builder.Services.AddScoped<ISession>(provider =>
{
    var sessionFactory = provider.GetRequiredService<ISessionFactory>();
    return sessionFactory.OpenSession();
});

// Repository and service registration
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IThreadRepository, ThreadRepository>();
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddScoped<UserAccountService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<ThreadService>();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");


// WebOptimizer for bundling and minification
builder.Services.AddWebOptimizer(pipeline =>
{
    // Minification doesn't actually work. I don't have the time or energy to work out why right now.
    // Will be swapping out the front-end pipeline for something new soon anyway so not work investigating.
    pipeline.AddCssBundle("/css/bundle.css", "/lib/**/*/*.css", "/css/**/*.css");
    pipeline.AddJavaScriptBundle("/js/bundle.js",
        "/lib/jquery/jquery-3.7.1.js",
        "/lib/linkifyjs/linkify.js",
        "/lib/linkifyjs/linkify-jquery.js",
        "/lib/momentjs/moment-with-locales.js",
        "/lib/knockout/knockout-3.5.1.debug.js",
        "/lib/format/format.js",
        "/js/setup-moment.js",
        "/js/setup-linkify.js",
        "/js/namespace.js",
        "/js/knockout-extensions.js",
        "/js/constants.js",
        "/js/reply.js",
        "/js/thread.js",
        "/js/forumService.js",
        "/js/forumViewModel.js"
    );
});

// Migration configuration
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSqlServer()
        .WithGlobalConnectionString(applicationConnectionString)
        .ScanIn(typeof(Program).Assembly).For.Migrations());

var app = builder.Build();

// Migrations execution
using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseWebOptimizer();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.UseNhSessionMiddleware();
app.Run();
return;

void CheckAndCreateAppDatabase(string master, string application)
{
    using var connection = new SqlConnection(master);
    connection.Open();
    var databaseName = new SqlConnectionStringBuilder(application).InitialCatalog;
    if (string.IsNullOrEmpty(databaseName))
    {
        return;
    }
    var checkDatabaseExistsSql = $"SELECT 1 FROM sys.databases WHERE name = N'{databaseName}'";
    var result = connection.QuerySingleOrDefault<int>(checkDatabaseExistsSql);
    if (result == 1)
    {
        return;   
    }
    var createDatabaseSql = $"CREATE DATABASE {databaseName}";
    connection.Execute(createDatabaseSql);
}
