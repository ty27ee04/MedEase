public class PdfReport : IReportStrategy
{
    public void GenerateReport(string content)
    {
        File.WriteAllText("Report.pdf.txt", content);
        Console.WriteLine("PDF report saved as Report.pdf.txt");
    }
}
