
// Program.cs
using HandlebarsDotNet;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<TimeTrackingReportFactory>();

// Register Handlebars helpers
Handlebars.RegisterHelper("formatDateFull", (context, arguments) =>
{
    if (arguments[0] is DateTime date)
    {
        return date.ToString("dd MMMM yyyy г.", new CultureInfo("ru-RU"));
    }
    return arguments[0]?.ToString() ?? "";
});

Handlebars.RegisterHelper("formatDateShort", (context, arguments) =>
{
    if (arguments[0] is DateTime date)
    {
        return date.ToString("dd.MM.yyyy", new CultureInfo("ru-RU"));
    }
    return arguments[0]?.ToString() ?? "";
});

Handlebars.RegisterHelper("formatTime", (context, arguments) =>
{
    if (arguments[0] is TimeSpan time)
    {
        return $"{(int)time.TotalHours}ч {time.Minutes:00}м {time.Seconds:00}с";
    }
    return arguments[0]?.ToString() ?? "";
});

Handlebars.RegisterHelper("formatPercentage", (context, arguments) =>
{
    if (arguments.Length >= 2 && arguments[0] is TimeSpan part && arguments[1] is TimeSpan total)
    {
        var percentage = total.TotalSeconds > 0 ? (part.TotalSeconds / total.TotalSeconds) * 100 : 0;
        return $"({percentage:F2}%)";
    }
    return "";
});

Handlebars.RegisterHelper("getActivityClass", (context, arguments) =>
{
    if (arguments[0] is string activity)
    {
        return activity switch
        {
            "Приложения для SIP-телефонии" or "Бухгалтерия" or "microsip.exe" or "1cv8c.exe" => "productive",
            _ => "unproductive"
        };
    }
    return "unproductive";
});

Handlebars.RegisterHelper("getCurrentDate", (context, arguments) =>
{
    return DateTime.Now.ToString("dd.MM.yyyy", new CultureInfo("ru-RU"));
});

Handlebars.RegisterHelper("eq", (context, arguments) =>
{
    return arguments.Length >= 2 && arguments[0]?.ToString() == arguments[1]?.ToString();
});

Handlebars.RegisterHelper("multiply", (context, arguments) =>
{
    if (arguments.Length >= 2 && arguments[0] is TimeSpan time && arguments[1] is int multiplier)
    {
        return time.TotalSeconds * multiplier;
    }
    return 0;
});

Handlebars.RegisterHelper("divide", (context, arguments) =>
{
    if (arguments.Length >= 2 && arguments[0] is double numerator && arguments[1] is double denominator && denominator != 0)
    {
        return numerator / denominator;
    }
    return 0;
});

Handlebars.RegisterHelper("calculatePercentage", (context, arguments) =>
{
    if (arguments.Length >= 2 && arguments[0] is TimeSpan part && arguments[1] is TimeSpan total)
    {
        var percentage = total.TotalSeconds > 0 ? (part.TotalSeconds / total.TotalSeconds) * 100 : 0;
        return percentage;
    }
    return 0;
});

Handlebars.RegisterHelper("each24", (writer, options, context, parameters) =>
{
    for (int i = 0; i < 24; i++)
    {
        options.Template(writer, i);
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("detailed-time-tracking-report", async (TimeTrackingReportFactory factory) =>
{
    var startDate = new DateTime(2024, 2, 16);
    var endDate = new DateTime(2024, 2, 25);
    var report = factory.CreateDetailedReport(startDate, endDate);

    var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "DetailedTimeTrackingReport.hbs");
    var templateContent = await File.ReadAllTextAsync(templatePath);

    var template = Handlebars.Compile(templateContent);
    var html = template(report);

    var browserFetcher = new BrowserFetcher();
    await browserFetcher.DownloadAsync();

    using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
    using var page = await browser.NewPageAsync();

    await page.SetContentAsync(html);
    await page.EvaluateExpressionHandleAsync("document.fonts.ready");

    var pdfData = await page.PdfDataAsync(new PdfOptions
    {
        Format = PaperFormat.A4,
        PrintBackground = true,
        MarginOptions = new MarginOptions
        {
            Top = "15px",
            Right = "15px",
            Bottom = "15px",
            Left = "15px"
        }
    });

    return Results.File(pdfData, "application/pdf", $"detailed-time-tracking-{startDate:yyyy-MM-dd}-{endDate:yyyy-MM-dd}.pdf");
});

app.UseHttpsRedirection();
app.Run();
