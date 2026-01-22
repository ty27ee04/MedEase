using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HealthTech.Services
{
    // 1. REPORT REQUEST MODELS
    public class MedicalReportRequest
    {
        public int PatientId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool UseAllDates { get; set; } = true;
    }

    public class OperationReportRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
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
            var request = (MedicalReportRequest)requestData;
            var service = new ReportDataService();
            
            if (request.UseAllDates)
            {
                return service.GetMedicalReport(request.PatientId, null, null);
            }
            else
            {
                return service.GetMedicalReport(request.PatientId, request.StartDate, request.EndDate);
            }
        }
    }

    public class OperationReportStrategy : IReportTypeStrategy
    {
        public object GenerateReport(object requestData)
        {
            var request = (OperationReportRequest)requestData;
            var service = new ReportDataService();
            return service.GetOperationReport(request.StartDate, request.EndDate);
        }
    }


    // 3. EXPORT STRATEGY (FORMAT)
    public interface IExportStrategy
    {
        string Export(object reportData);
        string GetContentType();
        string GetFileExtension();
    }

    public class PdfExportStrategy : IExportStrategy
    {
        public string Export(object reportData)
        {
            var table = (DataTable)reportData;
            
            // Get report title from DataTable name, default to "Report"
            string reportTitle = string.IsNullOrEmpty(table.TableName) ? "Report" : table.TableName;
            
            // Generate HTML that looks like a PDF document
            var sb = new StringBuilder();
            
            // HTML structure for PDF-like document
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset='UTF-8'>");
            sb.AppendLine($"<title>{reportTitle}</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; margin: 40px; }");
            sb.AppendLine("h1 { color: #333; border-bottom: 3px solid #007bff; padding-bottom: 10px; }");
            sb.AppendLine(".metadata { color: #666; margin-bottom: 20px; }");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            sb.AppendLine("th { background-color: #007bff; color: white; padding: 12px; text-align: left; border: 1px solid #ddd; }");
            sb.AppendLine("td { padding: 10px; border: 1px solid #ddd; }");
            sb.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
            sb.AppendLine("tr:hover { background-color: #f0f0f0; }");
            sb.AppendLine(".footer { margin-top: 30px; text-align: center; color: #999; font-size: 12px; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            
            // Header
            sb.AppendLine($"<h1>{reportTitle}</h1>");
            sb.AppendLine($"<div class='metadata'>");
            sb.AppendLine($"<p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
            sb.AppendLine($"<p><strong>Total Records:</strong> {table.Rows.Count}</p>");
            sb.AppendLine("</div>");

            // Table
            sb.AppendLine("<table>");
            sb.AppendLine("<thead><tr>");
            foreach (DataColumn col in table.Columns)
            {
                sb.AppendLine($"<th>{col.ColumnName}</th>");
            }
            sb.AppendLine("</tr></thead>");
            sb.AppendLine("<tbody>");

            foreach (DataRow row in table.Rows)
            {
                sb.AppendLine("<tr>");
                foreach (var item in row.ItemArray)
                {
                    var value = item?.ToString() ?? "";
                    // Escape HTML special characters
                    value = value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
                    sb.AppendLine($"<td>{value}</td>");
                }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            
            // Footer
            sb.AppendLine("<div class='footer'>");
            sb.AppendLine($"<p>HealthTech Medical System | Page 1 of 1</p>");
            sb.AppendLine("</div>");
            
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        public string GetContentType() => "text/html";
        public string GetFileExtension() => ".html";
    }

    public class CsvExportStrategy : IExportStrategy
    {
        public string Export(object reportData)
        {
            var table = (DataTable)reportData;
            var sb = new StringBuilder();

            // Headers
            var headers = new List<string>();
            foreach (DataColumn col in table.Columns)
            {
                headers.Add(EscapeCsvField(col.ColumnName));
            }
            sb.AppendLine(string.Join(",", headers));

            // Rows
            foreach (DataRow row in table.Rows)
            {
                var fields = new List<string>();
                foreach (var item in row.ItemArray)
                {
                    fields.Add(EscapeCsvField(item?.ToString() ?? ""));
                }
                sb.AppendLine(string.Join(",", fields));
            }

            return sb.ToString();
        }

        private string EscapeCsvField(string field)
        {
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }
            return field;
        }

        public string GetContentType() => "text/csv";
        public string GetFileExtension() => ".csv";
    }

    public class JsonExportStrategy : IExportStrategy
    {
        public string Export(object reportData)
        {
            var table = (DataTable)reportData;
            var rows = new List<Dictionary<string, object>>();

            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }
                rows.Add(dict);
            }

            return System.Text.Json.JsonSerializer.Serialize(rows, new System.Text.Json.JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }

        public string GetContentType() => "application/json";
        public string GetFileExtension() => ".json";
    }

    public class HtmlPreviewStrategy : IExportStrategy
    {
        public string Export(object reportData)
        {
            var table = (DataTable)reportData;

            if (table.Rows.Count == 0)
                return "<p><strong>No records found.</strong></p>";

            string html = "<table class='table table-bordered table-striped'>";
            html += "<thead class='table-dark'><tr>";

            foreach (DataColumn col in table.Columns)
            {
                html += $"<th>{col.ColumnName}</th>";
            }

            html += "</tr></thead><tbody>";

            foreach (DataRow row in table.Rows)
            {
                html += "<tr>";
                foreach (var item in row.ItemArray)
                {
                    html += $"<td>{item}</td>";
                }
                html += "</tr>";
            }

            html += "</tbody></table>";
            return html;
        }

        public string GetContentType() => "text/html";
        public string GetFileExtension() => ".html";
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
