using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;
using VmarmyshApp.EF;
using VmarmyshApp.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BinaryTreeNodeContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("TreesDb")));
builder.Services.AddControllers()
    .AddJsonOptions(jsonOptions => jsonOptions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddJsonOptions(jsonOptions => jsonOptions.JsonSerializerOptions.Converters.Add(new SecureExceptionSerializer()));
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
    loggerConfiguration.WriteTo.File("logs/log.log", Serilog.Events.LogEventLevel.Information);
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();