using MySql.Data.MySqlClient;
using System.Data;

namespace HealthTech.Services
{
    public class ReportDataService
    {
        private readonly string _connectionString;

        public ReportDataService(string? connectionString = null)
        {
            _connectionString = connectionString ?? "server=localhost;database=MedEaseDB;user=root;password=;";
        }

        // MEDICAL REPORT - Get patient records within date range
        public DataTable GetMedicalReport(int patientId, DateTime? startDate = null, DateTime? endDate = null)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT 
                    pr.recordID AS 'Record ID',
                    u.name AS 'Patient Name',
                    pr.recordType AS 'Type',
                    pr.title AS 'Title',
                    pr.details AS 'Details',
                    pr.createdDate AS 'Created Date',
                    pr.lastUpdated AS 'Last Updated',
                    d.name AS 'Doctor Name'
                FROM PatientRecord pr
                JOIN User u ON pr.patientID = u.userID
                LEFT JOIN User d ON pr.doctorID = d.userID
                WHERE pr.patientID = @patientId";

            if (startDate.HasValue && endDate.HasValue)
            {
                sql += " AND pr.createdDate BETWEEN @startDate AND @endDate";
            }

            sql += " ORDER BY pr.createdDate DESC";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@patientId", patientId);
            
            if (startDate.HasValue && endDate.HasValue)
            {
                cmd.Parameters.AddWithValue("@startDate", startDate.Value);
                cmd.Parameters.AddWithValue("@endDate", endDate.Value.AddDays(1).AddSeconds(-1));
            }

            var table = new DataTable("Medical Report");
            table.Load(cmd.ExecuteReader());
            return table;
        }

        // OPERATION REPORT - Get appointments within date range
        public DataTable GetOperationReport(DateTime startDate, DateTime endDate)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT 
                    a.appointmentID AS 'Appointment ID',
                    DATE_FORMAT(a.dateTime, '%Y-%m-%d') AS 'Date',
                    TIME_FORMAT(a.dateTime, '%H:%i') AS 'Time',
                    p.name AS 'Patient Name',
                    d.name AS 'Doctor Name',
                    a.type AS 'Type',
                    a.status AS 'Status',
                    a.remark AS 'Remark',
                    COALESCE(r.roomNumber, 'Online') AS 'Room/Link'
                FROM Appointment a
                JOIN User p ON a.patientID = p.userID
                JOIN User d ON a.doctorID = d.userID
                LEFT JOIN Room r ON a.roomID = r.roomID
                WHERE DATE(a.dateTime) BETWEEN @startDate AND @endDate
                ORDER BY a.dateTime DESC";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@startDate", startDate.Date);
            cmd.Parameters.AddWithValue("@endDate", endDate.Date.AddDays(1).AddSeconds(-1));

            var table = new DataTable("Operation Report");
            table.Load(cmd.ExecuteReader());
            return table;
        }

        // Get all patients for dropdown
        public DataTable GetAllPatients()
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT 
                    userID,
                    name,
                    email
                FROM User
                WHERE role = 'Patient'
                ORDER BY name";

            using var cmd = new MySqlCommand(sql, conn);

            var table = new DataTable();
            table.Load(cmd.ExecuteReader());
            return table;
        }
    }
}
