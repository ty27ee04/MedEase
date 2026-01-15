public class ReportContext
{
    private IReportStrategy _strategy;

    public void SetStrategy(IReportStrategy strategy)
    {
        _strategy = strategy;
    }

    public void Generate(string content)
    {
        _strategy.GenerateReport(content);
    }
}
