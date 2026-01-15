using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;

namespace HealthTech.Services
{
    /// <summary>
    /// Real notification service that sends email and WhatsApp messages
    /// Uses SMTP for email and Twilio for WhatsApp/SMS
    /// </summary>
    public class NotificationService
    {
        private readonly IConfiguration _configuration;

        public NotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeTwilio();
        }

        private void InitializeTwilio()
        {
            var accountSid = _configuration["TwilioSettings:AccountSid"];
            var authToken = _configuration["TwilioSettings:AuthToken"];
            TwilioClient.Init(accountSid, authToken);
        }

        /// <summary>
        /// Send email notification using SMTP (Gmail)
        /// </summary>
        public async Task<bool> SendEmailAsync(string recipientEmail, string recipientName, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["EmailSettings:SenderName"],
                    _configuration["EmailSettings:SenderEmail"]
                ));
                message.To.Add(new MailboxAddress(recipientName, recipientEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <h2 style='color: #007bff;'>HealthTech Medical</h2>
                            <p>{body}</p>
                            <hr>
                            <p style='color: #666; font-size: 12px;'>
                                This is an automated message from HealthTech Medical System.
                            </p>
                        </body>
                        </html>"
                };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(
                        _configuration["EmailSettings:SmtpServer"],
                        int.Parse(_configuration["EmailSettings:SmtpPort"]),
                        SecureSocketOptions.StartTls
                    );

                    await client.AuthenticateAsync(
                        _configuration["EmailSettings:SenderEmail"],
                        _configuration["EmailSettings:Password"]
                    );

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                Console.WriteLine($"[Email Service] Email sent to {recipientEmail}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Service] Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send WhatsApp message using Twilio API
        /// </summary>
        public async Task<bool> SendWhatsAppAsync(string recipientPhone, string messageBody)
        {
            try
            {
                // Ensure phone number is in E.164 format (e.g., +60123456789)
                if (!recipientPhone.StartsWith("+"))
                {
                    Console.WriteLine($"[WhatsApp Service] Invalid phone format: {recipientPhone}");
                    return false;
                }

                var message = await MessageResource.CreateAsync(
                    body: messageBody,
                    from: new PhoneNumber(_configuration["TwilioSettings:WhatsAppFrom"]),
                    to: new PhoneNumber($"whatsapp:{recipientPhone}")
                );

                Console.WriteLine($"[WhatsApp Service] Message sent to {recipientPhone}, SID: {message.Sid}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WhatsApp Service] Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send SMS message using Twilio (fallback option)
        /// </summary>
        public async Task<bool> SendSmsAsync(string recipientPhone, string messageBody)
        {
            try
            {
                // Ensure phone number is in E.164 format
                if (!recipientPhone.StartsWith("+"))
                {
                    Console.WriteLine($"[SMS Service] Invalid phone format: {recipientPhone}");
                    return false;
                }

                var message = await MessageResource.CreateAsync(
                    body: messageBody,
                    from: new PhoneNumber(_configuration["TwilioSettings:SmsFrom"]),
                    to: new PhoneNumber(recipientPhone)
                );

                Console.WriteLine($"[SMS Service] SMS sent to {recipientPhone}, SID: {message.Sid}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SMS Service] Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send appointment confirmation notification (Email + WhatsApp)
        /// </summary>
        public async Task SendAppointmentConfirmationAsync(
            string patientName,
            string patientEmail,
            string patientPhone,
            string doctorName,
            DateTime appointmentDate,
            string appointmentType)
        {
            string subject = "Appointment Confirmation - HealthTech Medical";
            string emailBody = $@"
                <p>Dear {patientName},</p>
                <p>Your appointment has been confirmed:</p>
                <ul>
                    <li><strong>Doctor:</strong> {doctorName}</li>
                    <li><strong>Date & Time:</strong> {appointmentDate:dd MMMM yyyy, hh:mm tt}</li>
                    <li><strong>Type:</strong> {appointmentType}</li>
                </ul>
                <p>Thank you for choosing HealthTech Medical.</p>";

            string whatsappBody = $"HealthTech Medical\n" +
                                $"✅ Appointment Confirmed\n\n" +
                                $"Patient: {patientName}\n" +
                                $"Doctor: {doctorName}\n" +
                                $"Date: {appointmentDate:dd MMM yyyy, hh:mm tt}\n" +
                                $"Type: {appointmentType}";

            // Send both email and WhatsApp
            await SendEmailAsync(patientEmail, patientName, subject, emailBody);
            await SendWhatsAppAsync(patientPhone, whatsappBody);
        }

        /// <summary>
        /// Send appointment reminder notification (Email + WhatsApp)
        /// </summary>
        public async Task SendAppointmentReminderAsync(
            string patientName,
            string patientEmail,
            string patientPhone,
            string doctorName,
            DateTime appointmentDate,
            string appointmentType,
            int hoursBeforeAppointment)
        {
            string subject = "Appointment Reminder - HealthTech Medical";
            string emailBody = $@"
                <p>Dear {patientName},</p>
                <p><strong>Reminder:</strong> You have an upcoming appointment in {hoursBeforeAppointment} hours:</p>
                <ul>
                    <li><strong>Doctor:</strong> {doctorName}</li>
                    <li><strong>Date & Time:</strong> {appointmentDate:dd MMMM yyyy, hh:mm tt}</li>
                    <li><strong>Type:</strong> {appointmentType}</li>
                </ul>
                <p>Please arrive 10 minutes early for check-in.</p>";

            string whatsappBody = $"HealthTech Medical\n" +
                                $"⏰ REMINDER: Appointment in {hoursBeforeAppointment}h\n\n" +
                                $"Patient: {patientName}\n" +
                                $"Doctor: {doctorName}\n" +
                                $"Date: {appointmentDate:dd MMM yyyy, hh:mm tt}\n" +
                                $"Type: {appointmentType}\n\n" +
                                $"Please arrive 10 min early.";

            // Send both email and WhatsApp
            await SendEmailAsync(patientEmail, patientName, subject, emailBody);
            await SendWhatsAppAsync(patientPhone, whatsappBody);
        }
    }
}
