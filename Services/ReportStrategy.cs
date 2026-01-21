using System;
<<<<<<< HEAD
using System.Collections.Generic;

namespace HealthTech.Services
{
    // 1. REPORT DATA MODELS
    public class MedicalReportData
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = "";
        public string Diagnosis { get; set; } = "";
        public string Prescription { get; set; } = "";
        public DateTime VisitDate { get; set; }
    }

    public class OperationReportData
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalAppointments { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
    }

    // 2. REPORT STRATEGY (TYPE)
    public interface IReportTypeStrategy
    {
        object GenerateReport(object requestData);
    }

    public class MedicalReportStrategy : IReportTypeStrategy
    {
        public object GenerateReport(object requestData)
        {
            int patientId = (int)requestData;

            var service = new ReportDataService();
            return service.GetMedicalReport(patientId);   // returns DataTable
        }
    }


    public class OperationReportStrategy : IReportTypeStrategy
    {
        public object GenerateReport(object requestData)
        {
            var dates = (Tuple<DateTime, DateTime>)requestData;

            var service = new ReportDataService();
            return service.GetOperationReport(dates.Item1, dates.Item2);
        }
    }


    // 3. EXPORT STRATEGY (FORMAT)
    public interface IExportStrategy
    {
        string Export(object reportData);
    }

    public class PdfExportStrategy : IExportStrategy
    {
        public string Export(object reportData)
        {
            return $"[PDF]\n{System.Text.Json.JsonSerializer.Serialize(reportData, null, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}";
        }
    }

    public class CsvExportStrategy : IExportStrategy
{
    public string Export(object reportData)
    {
        var table = (DataTable)reportData;

        var sb = new System.Text.StringBuilder();

        foreach (DataColumn col in table.Columns)
            sb.Append(col.ColumnName + ",");

        sb.AppendLine();

        foreach (DataRow row in table.Rows)
        {
            foreach (var item in row.ItemArray)
                sb.Append(item + ",");

            sb.AppendLine();
        }

        return sb.ToString();
    }
}


    public class JsonExportStrategy : IExportStrategy
{
    public string Export(object reportData)
    {
        var table = (DataTable)reportData;
        return System.Text.Json.JsonSerializer.Serialize(table);
    }
}

public class HtmlPreviewStrategy : IExportStrategy
{
    public string Export(object reportData)
    {
        var table = (DataTable)reportData;

        if (table.Rows.Count == 0)
            return "<p><strong>No records found.</strong></p>";

        string html = "<table class='table table-bordered'><tr>";

        foreach (DataColumn col in table.Columns)
            html += $"<th>{col.ColumnName}</th>";

        html += "</tr>";

        foreach (DataRow row in table.Rows)
        {
            html += "<tr>";
            foreach (var item in row.ItemArray)
                html += $"<td>{item}</td>";
            html += "</tr>";
        }

        html += "</table>";
        return html;
    }
}



    // 4. CONTEXT (BRAIN)
    public class ReportContext
    {
        private IReportTypeStrategy? _reportType;
        private IExportStrategy? _exportType;

        public void SetReportType(IReportTypeStrategy strategy)
        {
            _reportType = strategy;
        }

        public void SetExportType(IExportStrategy strategy)
        {
            _exportType = strategy;
        }

        public string Generate(object requestData)
        {
            if (_reportType == null || _exportType == null)
                return "Error: Report type or export format not selected.";

            var reportData = _reportType.GenerateReport(requestData);
            return _exportType.Export(reportData);
        }
    }
}
=======

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
>>>>>>> 422f23b6b42e05d1657dada3ee19136d19b15070
