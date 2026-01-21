using MySql.Data.MySqlClient;
using System.Data;

namespace HealthTech.Services
{
    public class ReportDataService
    {
        private readonly string _connectionString =
            "server=localhost;database=medease;user=root;password=;";

        // MEDICAL REPORT
        public DataTable GetMedicalReport(int patientId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT 
                    p.PatientID,
                    p.FullName,
                    d.Diagnosis,
                    d.Treatment,
                    d.VisitDate
                FROM patients p
                JOIN diagnoses d ON p.PatientID = d.PatientID
                WHERE p.PatientID = @id;
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", patientId);

            var table = new DataTable();
            table.Load(cmd.ExecuteReader());
            return table;
        }

        // OPERATION REPORT
        public DataTable GetOperationReport(DateTime start, DateTime end)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT 
                    o.OperationID,
                    o.OperationDate,
                    p.FullName AS Patient,
                    d.FullName AS Doctor,
                    o.Procedure,
                    o.Status
                FROM operations o
                JOIN patients p ON o.PatientID = p.PatientID
                JOIN doctors d ON o.DoctorID = d.DoctorID
                WHERE o.OperationDate BETWEEN @start AND @end;
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@start", start);
            cmd.Parameters.AddWithValue("@end", end);

            var table = new DataTable();
            table.Load(cmd.ExecuteReader());
            return table;
        }
    }
}
