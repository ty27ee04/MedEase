public class CsvReport : IReportStrategy
{
    public void GenerateReport(string content)
    {
        File.WriteAllText("Report.csv", content);
        Console.WriteLine("CSV report saved as Report.csv");
    }
}
