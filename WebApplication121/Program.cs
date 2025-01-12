using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebApplication121.BackgroundJobs;
using WebApplication121.ServiceInterfaces;
using WebApplication121.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<IWebDriver>(sp =>
{
    var options = new ChromeOptions();
    options.AddArgument("--headless");
    options.AddArgument("--disable-gpu");
    return new ChromeDriver(options);
});
builder.Services.AddScoped<ITranslateService, TranslateService>();
builder.Services.AddScoped<ITrendsService, TrendsService>();
builder.Services.AddHostedService<SchedulerJobService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
