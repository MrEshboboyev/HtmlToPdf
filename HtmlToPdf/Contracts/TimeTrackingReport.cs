namespace HtmlToPdf.Contracts;

public class TimeTrackingReport
{
    public string Title { get; set; } = "Учёт рабочего времени за период";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string UserName { get; set; } = "CallCenter2";
    public List<TimeTrackingEntry> Entries { get; set; } = new();
    public Dictionary<string, TimeSpan> TotalsByCategory { get; set; } = new();
    public Dictionary<string, double> ProductivityPercentages { get; set; } = new();
}
