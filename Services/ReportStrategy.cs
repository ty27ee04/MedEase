using System;
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
