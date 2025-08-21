namespace HtmlToPdf.Contracts;

public class TimeTrackingEntry
{
    public DateTime Date { get; set; }
    public Dictionary<int, ActivityType> HourlyActivities { get; set; } = new();
    public TimeSpan TotalTime { get; set; }
    public Dictionary<string, TimeSpan> ActivityBreakdown { get; set; } = new();
}
