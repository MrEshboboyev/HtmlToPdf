// Models
using HandlebarsDotNet;
using PuppeteerSharp;
using PuppeteerSharp.Media;

public class TimeEntry
{
    public int Hour { get; set; }
    public ActivityType Activity { get; set; }
    public TimeSpan Duration { get; set; }
}

public class DayReport
{
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan ScheduledStart { get; set; } = TimeSpan.FromHours(9);
    public TimeSpan EndTime { get; set; }
    public TimeSpan ScheduledEnd { get; set; } = TimeSpan.FromHours(18);
    public TimeSpan TotalTime { get; set; }
    public TimeSpan PlannedTime { get; set; } = TimeSpan.FromHours(9);
    public TimeSpan OvertimeTime { get; set; }
    public TimeSpan LateTime { get; set; }
    public TimeSpan ActiveTime { get; set; }
    public TimeSpan InactiveTime { get; set; }
    public TimeSpan ProductiveTime { get; set; }
    public TimeSpan UnproductiveTime { get; set; }
    public TimeSpan NeutralTime { get; set; }

    public Dictionary<int, ActivityType> HourlyActivities { get; set; } = new();
    public Dictionary<string, TimeSpan> CategoryTotals { get; set; } = new();
    public Dictionary<string, TimeSpan> ApplicationTotals { get; set; } = new();
    public Dictionary<string, TimeSpan> WebsiteTotals { get; set; } = new();
}

public class UserReport
{
    public string UserName { get; set; }
    public List<DayReport> Days { get; set; } = new();
}

public enum ActivityType
{
    SipTelephony,
    Accounting,
    SystemInterface,
    Other,
    OfficeApps,
    SearchPortals,
    SystemAdministration,

    // Applications
    MicrosipExe,
    OneСExe,
    ExplorerExe,
    BrowserExe,
    MsiExecExe,
    TaskmgrExe,
    SearchappExe,
    MmcExe,
    StartmenuExe,
    YMusicExe,
    MsedgeExe,
    ShellExperienceExe,
    SearchuiExe,
    CalculatorExe,
    SystemctlExe,

    // Websites
    YandexRu,
    BingCom
}

public class TimeTrackingReport
{
    public string Title { get; set; } = "Учёт рабочего времени за период";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<UserReport> Users { get; set; } = new();
    public string ReportUrl { get; set; } = "http://localhost/analytics/report/aggregate/index/?hash=301d4f1a3713e1932ae34b2ecc2b15e1";
}

// Factory
internal sealed class TimeTrackingReportFactory
{
    private readonly Random _random = new();

    public TimeTrackingReport CreateDetailedReport(DateTime startDate, DateTime endDate)
    {
        var report = new TimeTrackingReport
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var users = new[] { "CallCenter2", "CallCenter4", "WIN-GI3MV2K1JQ3" };

        foreach (var userName in users)
        {
            var userReport = new UserReport { UserName = userName };

            // Generate only specific dates as shown in PDF
            var workDates = new[] {
                new DateTime(2024, 2, 21),
                new DateTime(2024, 2, 22),
                new DateTime(2024, 2, 23)
            };

            foreach (var date in workDates)
            {
                var dayReport = GenerateRealisticDayReport(date, userName);
                userReport.Days.Add(dayReport);
            }

            report.Users.Add(userReport);
        }

        return report;
    }

    private DayReport GenerateRealisticDayReport(DateTime date, string userName)
    {
        var day = new DayReport { Date = date };

        // Generate realistic start/end times with variations
        var startHour = _random.Next(8, 10);
        var startMinute = _random.Next(0, 60);
        day.StartTime = TimeSpan.FromHours(startHour).Add(TimeSpan.FromMinutes(startMinute));

        var endHour = _random.Next(17, 19);
        var endMinute = _random.Next(0, 60);
        day.EndTime = TimeSpan.FromHours(endHour).Add(TimeSpan.FromMinutes(endMinute));

        day.TotalTime = day.EndTime - day.StartTime;

        // Calculate overtime and late time
        day.OvertimeTime = day.TotalTime > day.PlannedTime ? day.TotalTime - day.PlannedTime : TimeSpan.Zero;
        day.LateTime = day.StartTime > day.ScheduledStart ? day.StartTime - day.ScheduledStart : TimeSpan.Zero;

        // Generate hourly activities for the timeline
        GenerateHourlyActivities(day, userName);

        // Calculate activity breakdowns
        CalculateDayBreakdowns(day, userName);

        return day;
    }

    private void GenerateHourlyActivities(DayReport day, string userName)
    {
        var workStart = (int)day.StartTime.TotalHours;
        var workEnd = (int)day.EndTime.TotalHours;

        for (int hour = 0; hour < 24; hour++)
        {
            if (hour >= workStart && hour <= workEnd)
            {
                // During work hours - assign productive activities
                day.HourlyActivities[hour] = GetPrimaryActivityForUser(userName);
            }
            else
            {
                // Outside work hours - inactive
                day.HourlyActivities[hour] = ActivityType.Other;
            }
        }
    }

    private ActivityType GetPrimaryActivityForUser(string userName)
    {
        return userName switch
        {
            "CallCenter2" => _random.NextDouble() > 0.2 ? ActivityType.SipTelephony : ActivityType.Accounting,
            "CallCenter4" => _random.NextDouble() > 0.1 ? ActivityType.SipTelephony : ActivityType.Accounting,
            "WIN-GI3MV2K1JQ3" => _random.NextDouble() > 0.15 ? ActivityType.SipTelephony : ActivityType.Accounting,
            _ => ActivityType.SipTelephony
        };
    }

    private void CalculateDayBreakdowns(DayReport day, string userName)
    {
        // Generate realistic category breakdowns based on the PDF data
        switch (userName)
        {
            case "CallCenter2":
                GenerateCallCenter2Breakdown(day);
                break;
            case "CallCenter4":
                GenerateCallCenter4Breakdown(day);
                break;
            case "WIN-GI3MV2K1JQ3":
                GenerateWinGi3Breakdown(day);
                break;
        }
    }

    private void GenerateCallCenter2Breakdown(DayReport day)
    {
        var totalMinutes = (int)day.TotalTime.TotalMinutes;

        // Productivity categories (as shown in PDF for CallCenter2)
        day.CategoryTotals["Приложения для SIP-телефонии"] = TimeSpan.FromMinutes(totalMinutes * 0.77); // 76.83%
        day.CategoryTotals["Бухгалтерия"] = TimeSpan.FromMinutes(totalMinutes * 0.20); // 20.41%
        day.CategoryTotals["Интерфейс системы"] = TimeSpan.FromMinutes(totalMinutes * 0.02); // 2.37%
        day.CategoryTotals["Всё остальное"] = TimeSpan.FromMinutes(totalMinutes * 0.01); // 0.26%

        // Applications
        day.ApplicationTotals["microsip.exe"] = day.CategoryTotals["Приложения для SIP-телефонии"];
        day.ApplicationTotals["1cv8c.exe"] = day.CategoryTotals["Бухгалтерия"];
        day.ApplicationTotals["explorer.exe"] = day.CategoryTotals["Интерфейс системы"];
        day.ApplicationTotals["browser.exe"] = TimeSpan.FromMinutes(2);
        day.ApplicationTotals["msiexec.exe"] = TimeSpan.FromMinutes(2);

        // Websites
        day.WebsiteTotals["yandex.ru"] = TimeSpan.FromMinutes(1);

        // Calculate productive vs unproductive time
        day.ProductiveTime = day.CategoryTotals["Приложения для SIP-телефонии"] + day.CategoryTotals["Бухгалтерия"];
        day.UnproductiveTime = day.CategoryTotals["Интерфейс системы"] + day.CategoryTotals["Всё остальное"];
        day.ActiveTime = day.ProductiveTime + day.UnproductiveTime;
        day.InactiveTime = day.TotalTime - day.ActiveTime;
    }

    private void GenerateCallCenter4Breakdown(DayReport day)
    {
        var totalMinutes = (int)day.TotalTime.TotalMinutes;

        // Higher productivity for CallCenter4
        day.CategoryTotals["Приложения для SIP-телефонии"] = TimeSpan.FromMinutes(totalMinutes * 0.90);
        day.CategoryTotals["Бухгалтерия"] = TimeSpan.FromMinutes(totalMinutes * 0.08);
        day.CategoryTotals["Интерфейс системы"] = TimeSpan.FromMinutes(totalMinutes * 0.02);

        day.ApplicationTotals["microsip.exe"] = day.CategoryTotals["Приложения для SIP-телефонии"];
        day.ApplicationTotals["1cv8c.exe"] = day.CategoryTotals["Бухгалтерия"];
        day.ApplicationTotals["explorer.exe"] = day.CategoryTotals["Интерфейс системы"];

        day.ProductiveTime = day.CategoryTotals["Приложения для SIP-телефонии"] + day.CategoryTotals["Бухгалтерия"];
        day.UnproductiveTime = day.CategoryTotals["Интерфейс системы"];
        day.ActiveTime = day.ProductiveTime + day.UnproductiveTime;
        day.InactiveTime = day.TotalTime - day.ActiveTime;
    }

    private void GenerateWinGi3Breakdown(DayReport day)
    {
        var totalMinutes = (int)day.TotalTime.TotalMinutes;

        day.CategoryTotals["Приложения для SIP-телефонии"] = TimeSpan.FromMinutes(totalMinutes * 0.89);
        day.CategoryTotals["Бухгалтерия"] = TimeSpan.FromMinutes(totalMinutes * 0.09);
        day.CategoryTotals["Офисные приложения"] = TimeSpan.FromMinutes(totalMinutes * 0.01);
        day.CategoryTotals["Интерфейс системы"] = TimeSpan.FromMinutes(totalMinutes * 0.01);

        day.ApplicationTotals["microsip.exe"] = day.CategoryTotals["Приложения для SIP-телефонии"];
        day.ApplicationTotals["1cv8c.exe"] = day.CategoryTotals["Бухгалтерия"];
        day.ApplicationTotals["calculator.exe"] = day.CategoryTotals["Офисные приложения"];
        day.ApplicationTotals["explorer.exe"] = day.CategoryTotals["Интерфейс системы"];

        day.ProductiveTime = day.CategoryTotals["Приложения для SIP-телефонии"] + day.CategoryTotals["Бухгалтерия"];
        day.UnproductiveTime = day.CategoryTotals["Офисные приложения"] + day.CategoryTotals["Интерфейс системы"];
        day.ActiveTime = day.ProductiveTime + day.UnproductiveTime;
        day.InactiveTime = day.TotalTime - day.ActiveTime;
    }
}
