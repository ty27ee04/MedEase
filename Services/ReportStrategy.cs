using System;

namespace HealthTech.Services
{
    // 1. The Strategy Interface (From PDF Page 17)
    // Defines a common interface for all export methods.
    public interface IReportStrategy
    {
        string Export(string data);
    }

    // 2. Concrete Strategy A: PDF
    // Implements the logic for PDF generation.
    public class PdfStrategy : IReportStrategy
    {
        public string Export(string data)
        {
            // In a real app, this would use a library like iTextSharp to build a PDF file.
            // For this assignment prototype, we simulate the logic.
            return $"[PDF GENERATOR] Header: CLINIC REPORT\nBody: {data}\nFooter: Page 1 of 1";
        }
    }

    // 3. Concrete Strategy B: CSV
    // Implements the logic for Excel/CSV generation.
    public class CsvStrategy : IReportStrategy
    {
        public string Export(string data)
        {
            // CSV format is just comma-separated values.
            return $"ID,Data,Date\n001,{data},{DateTime.Now.ToShortDateString()}";
        }
    }

    // 4. The Context Class (From PDF Page 17)
    // This class uses the strategy but doesn't care which one it is.
    public class ReportContext
    {
        private IReportStrategy? _strategy;

        // Allows us to switch the strategy at runtime (e.g., when user clicks a dropdown)
        public void SetStrategy(IReportStrategy strategy)
        {
            _strategy = strategy;
        }

        public string GenerateReport(string data)
        {
            if (_strategy == null)
            {
                return "Error: No export format selected.";
            }
            return _strategy.Export(data);
        }
    }
}