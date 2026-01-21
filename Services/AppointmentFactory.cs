namespace HealthTech.Services
{
    // 1. Product Interface
    public interface IAppointment
    {
        string GetBookingDetails();
        void ConfirmBooking(string patientName);
    }

    // 2. Concrete Product A: In-Person
    public class InPersonAppointment : Subject, IAppointment
    {
        public string GetBookingDetails() => "Room 101 Assigned.";

        public void ConfirmBooking(string patientName)
        {
            // This triggers the notification defined in the Parent 'Subject' class
            Notify($"Appointment Confirmed for {patientName} (In-Person).");
        }
    }

    // UPDATE: Inherit from 'Subject'
    public class TelemedicineAppointment : Subject, IAppointment
    {
        public string GetBookingDetails() => "Zoom Link Generated.";

        public void ConfirmBooking(string patientName)
        {
            // This triggers the notification
            Notify($"Appointment Confirmed for {patientName} (Online).");
        }
    }

    // 4. Creator (Factory) Class
    public abstract class AppointmentFactory
    {
        public abstract IAppointment CreateAppointment(string type);
    }

    // 5. Concrete Creator
    public class MedicalAppointmentFactory : AppointmentFactory
    {
        public override IAppointment CreateAppointment(string type)
        {
            // Logic from PDF Page 21
            switch (type.ToLower())
            {
                case "online": return new TelemedicineAppointment();
                case "physical": return new InPersonAppointment();
                default: throw new ArgumentException("Invalid appointment type");
            }
        }
    }
}