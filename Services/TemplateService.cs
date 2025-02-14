using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class TemplateService
    {
        //Template for resetting password

        public string ResetPassword(string url, string name)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Password Reset Request</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}

        .email-container {{
           
            text-align: start;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 2rem;
            padding-top: 2rem;
           
        }}

        .email-header {{
            text-align: start;
            margin-bottom: 1rem; 
            
        }}

        .email-header h1 {{
            margin: 0;
            font-size: 24px;
            color: #333333;
        }}

        .email-body {{
            margin-bottom: 20px;
        }}

        .email-body p {{
            font-size: 16px;
            color: #555555;
            line-height: 1.5;
        }}

        .email-footer {{
            text-align: start;
        }}

        .reset-button {{
            display: inline-block;
            padding: 10px 20px;
            background-color: #28a745;
            color: #ffffff !important;
            text-decoration: none;
            border-radius: 5px;
            font-size: 16px;
        }}

        .reset-button:hover {{
            background-color: #218838;
        }}

        .footer-text {{
            font-size: 14px;
            color: #999999;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>Password reset request</h1>
        </div>
        <div class=""email-body"">
            <p>Hi {name},</p>
            <p>You recently requested to reset your password. Click the button below to reset it.</p>
           
            <p>Thank you,</p>
            <p>Prioritia</p>
        </div>
        <div class=""email-footer"">
            <a href=""{url}"" class=""reset-button"">Reset Password</a>
            <p class=""footer-text"">If you did not request a password reset, please ignore this email.</p>
            
        </div>
    </div>
</body>
</html>
";
        }
        //Template for confirming email
        public string ConfirmEmail(string url, string name)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Password Reset Request</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}

        .email-container {{
           
            text-align: start;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 2rem;
            padding-top: 2rem;
           
        }}

        .email-header {{
            text-align: start;
            margin-bottom: 1rem; 
            
        }}

        .email-header h1 {{
            margin: 0;
            font-size: 24px;
            color: #333333;
        }}

        .email-body {{
            margin-bottom: 20px;
        }}

        .email-body p {{
            font-size: 16px;
            color: #555555;
            line-height: 1.5;
        }}

        .email-footer {{
            text-align: start;
        }}

        .reset-button {{
            display: inline-block;
            padding: 10px 20px;
            background-color: #28a745;
            color: #ffffff !important;
            text-decoration: none;
            border-radius: 5px;
            font-size: 16px;
        }}

        .reset-button:hover {{
            background-color: #218838;
        }}

        .footer-text {{
            font-size: 14px;
            color: #999999;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>Confirm your email</h1>
        </div>
        <div class=""email-body"">
            <p>Hi {name},</p>
            <p>Please click the link below to confirm your email.</p>  
            <p>Thank you,</p>
            <p>Prioritia</p>
        </div>
        <div class=""email-footer"">
            <a href=""{url}"" class=""reset-button"">Confirm Email</a>
            <p class=""footer-text"">If you did not make this request, please ignore this email.</p>    
        </div>
    </div>
</body>
</html>
";
        }
        //Template for contact us
        public string ContactUs(string email, string name, string message)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>New Contact Us Message</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}

        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 2rem;
            border: 1px solid #dddddd;
            border-radius: 8px;
        }}

        .email-header {{
            text-align: center;
            margin-bottom: 1rem; 
        }}

        .email-header h1 {{
            margin: 0;
            font-size: 24px;
            color: #333333;
        }}

        .email-body p {{
            font-size: 16px;
            color: #555555;
            line-height: 1.5;
        }}

        .email-footer {{
            text-align: center;
            margin-top: 20px;
        }}

        .footer-text {{
            font-size: 14px;
            color: #999999;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>New Contact Us Message</h1>
        </div>
        <div class=""email-body"">
            <p><strong>Name:</strong> {name}</p>
            <p><strong>Email:</strong> {email}</p>
            <p><strong>Message:</strong></p>
            <p>{message}</p>  
        </div>
        <div class=""email-footer"">
            <p class=""footer-text"">You received this message from the contact us page.</p>    
        </div>
    </div>
</body>
</html>
";
        }

        // Template for booking creation notification
        public string BookingCreated(string name, string email, string phoneNumber,ServiceType serviceType, string vehicleType, string location, DateTime scheduledAt, string additionalNotes)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Booking Created</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}

        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 2rem;
            border: 1px solid #dddddd;
            border-radius: 8px;
        }}

        .email-header {{
            text-align: center;
            margin-bottom: 1rem; 
        }}

        .email-header h1 {{
            margin: 0;
            font-size: 24px;
            color: #333333;
        }}

        .email-body p {{
            font-size: 16px;
            color: #555555;
            line-height: 1.5;
        }}

        .highlight {{
            font-weight: bold;
            color: #333333;
        }}

        .email-footer {{
            text-align: center;
            margin-top: 20px;
        }}

        .footer-text {{
            font-size: 14px;
            color: #999999;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>New Booking Created</h1>
        </div>
        <div class=""email-body"">
            <p>A new booking has been created with the following details:</p>
            <p><span class=""highlight"">Name:</span> {name}</p>
            <p><span class=""highlight"">Email:</span> {email}</p>
            <p><span class=""highlight"">Phone Number:</span> {phoneNumber}</p>
            <p><span class=""highlight"">Location:</span> {location}</p>
             <p><span class=""highlight"">Vehicle Type:</span> {vehicleType}</p>
             <p><span class=""highlight"">Service Type:</span> {serviceType.Name} (R{serviceType.Price})</p>
            <p><span class=""highlight"">Scheduled At:</span> {scheduledAt:dddd, MMMM dd, yyyy h:mm tt}</p>
            <p><span class=""highlight"">Additional Notes:</span> {(string.IsNullOrWhiteSpace(additionalNotes) ? "No additional notes provided." : additionalNotes)}</p>
            <p><strong>Note:</strong> This booking is not yet confirmed. You will be notified once the booking is reviewed and confirmed by the admin.</p>
        </div>
        <div class=""email-footer"">
            <p class=""footer-text"">Thank you for using our car wash service. If you have any questions, feel free to reach out to us.</p>    
        </div>
    </div>
</body>
</html>
";
        }



        // Template for booking cancellation
        public string BookingCancellation(string name, string email, string phoneNumber, ServiceType serviceType, string vehicleType, string location, DateTime scheduledAt, string cancellationReason)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Car Wash Booking Cancellation</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}

        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 2rem;
            border: 1px solid #dddddd;
            border-radius: 8px;
        }}

        .email-header {{
            text-align: center;
            margin-bottom: 1rem; 
        }}

        .email-header h1 {{
            margin: 0;
            font-size: 24px;
            color: #333333;
        }}

        .email-body p {{
            font-size: 16px;
            color: #555555;
            line-height: 1.5;
        }}

        .highlight {{
            font-weight: bold;
            color: #333333;
        }}

        .email-footer {{
            text-align: center;
            margin-top: 20px;
        }}

        .footer-text {{
            font-size: 14px;
            color: #999999;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>Car Wash Booking Cancellation</h1>
        </div>
        <div class=""email-body"">
            <p>A car wash booking has been canceled with the following details:</p>
            <p><span class=""highlight"">Name:</span> {name}</p>
            <p><span class=""highlight"">Email:</span> {email}</p>
            <p><span class=""highlight"">Phone Number:</span> {phoneNumber}</p>
            <p><span class=""highlight"">Location:</span> {location}</p>
             <p><span class=""highlight"">Vehicle Type:</span> {vehicleType}</p>
             <p><span class=""highlight"">Service Type:</span> {serviceType.Name} (R{serviceType.Price})</p>
            <p><span class=""highlight"">Scheduled At:</span> {scheduledAt:dddd, MMMM dd, yyyy h:mm tt}</p>
            <p><span class=""highlight"">Cancellation Reason:</span> {cancellationReason}</p>
            
            <p>If you have any questions or concerns, please contact us for further assistance.</p>
        </div>
        <div class=""email-footer"">
            <p class=""footer-text"">This email is sent as a confirmation of a canceled car wash booking.</p>    
        </div>
    </div>
</body>
</html>
";
        }

        // Template for booking confirmation
        public string BookingConfirmation(string name, string email, string phoneNumber, ServiceType serviceType, string vehicleType, string location, DateTime scheduledAt, string additionalNotes)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Booking Confirmation</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}

        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 2rem;
            border: 1px solid #dddddd;
            border-radius: 8px;
        }}

        .email-header {{
            text-align: center;
            margin-bottom: 1rem; 
        }}

        .email-header h1 {{
            margin: 0;
            font-size: 24px;
            color: #333333;
        }}

        .email-body p {{
            font-size: 16px;
            color: #555555;
            line-height: 1.5;
        }}

        .highlight {{
            font-weight: bold;
            color: #333333;
        }}

        .email-footer {{
            text-align: center;
            margin-top: 20px;
        }}

        .footer-text {{
            font-size: 14px;
            color: #999999;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>Your Booking is Confirmed!</h1>
        </div>
        <div class=""email-body"">
            <p>Dear {name},</p>
            <p>We are pleased to inform you that your car wash booking has been officially confirmed. Here are the details of your booking:</p>
            <p><span class=""highlight"">Name:</span> {name}</p>
            <p><span class=""highlight"">Email:</span> {email}</p>
            <p><span class=""highlight"">Phone Number:</span> {phoneNumber}</p>
            <p><span class=""highlight"">Location:</span> {location}</p>
             <p><span class=""highlight"">Vehicle Type:</span> {vehicleType}</p>
             <p><span class=""highlight"">Service Type:</span> {serviceType.Name} (R{serviceType.Price})</p>
            <p><span class=""highlight"">Scheduled At:</span> {scheduledAt:dddd, MMMM dd, yyyy h:mm tt}</p>
            <p><span class=""highlight"">Additional Notes:</span> {(string.IsNullOrWhiteSpace(additionalNotes) ? "No additional notes provided." : additionalNotes)}</p>
            <p>We look forward to serving you! If you have any questions or need to make changes to your booking, please feel free to contact us.</p>
        </div>
        <div class=""email-footer"">
            <p class=""footer-text"">Thank you for choosing our car wash service.</p>    
        </div>
    </div>
</body>
</html>
";
        }

        // Template for "En Route" booking status notification
        public string BookingEnRoute(string name, DateTime scheduledAt, string location)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Booking En Route</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}

        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 2rem;
            border: 1px solid #dddddd;
            border-radius: 8px;
        }}

        .email-header {{
            text-align: center;
            margin-bottom: 1rem; 
        }}

        .email-header h1 {{
            margin: 0;
            font-size: 24px;
            color: #333333;
        }}

        .email-body p {{
            font-size: 16px;
            color: #555555;
            line-height: 1.5;
        }}

        .highlight {{
            font-weight: bold;
            color: #333333;
        }}

        .email-footer {{
            text-align: center;
            margin-top: 20px;
        }}

        .footer-text {{
            font-size: 14px;
            color: #999999;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>Your Car Wash is On the Way</h1>
        </div>
        <div class=""email-body"">
            <p>Hi {name},</p>
            <p>We're excited to let you know that our car wash team is currently en route to your location. Please review the details of your booking below:</p>
            <p><span class=""highlight"">Scheduled At:</span> {scheduledAt:dddd, MMMM dd, yyyy h:mm tt}</p>
            <p><span class=""highlight"">Location:</span> {location}</p>
            <p>We look forward to providing you with excellent service.</p>
        </div>
        <div class=""email-footer"">
            <p class=""footer-text"">This is an automated message. If you have any concerns, please contact our support team.</p>    
        </div>
    </div>
</body>
</html>
";
        }

        public string BookingCompletedEmail(string url, string name, string serviceType)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Booking Completed</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}

        .email-container {{
            text-align: start;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 2rem;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            max-width: 600px;
        }}

        .email-header {{
            text-align: start;
            margin-bottom: 1rem;
        }}

        .email-header h1 {{
            margin: 0;
            font-size: 24px;
            color: #333333;
        }}

        .email-body {{
            margin-bottom: 20px;
        }}

        .email-body p {{
            font-size: 16px;
            color: #555555;
            line-height: 1.5;
        }}

        .email-footer {{
            text-align: start;
        }}

        .feedback-button {{
            display: inline-block;
            padding: 10px 20px;
            background-color: #007bff;
            color: #ffffff !important;
            text-decoration: none;
            border-radius: 5px;
            font-size: 16px;
            margin-top: 10px;
        }}

        .feedback-button:hover {{
            background-color: #0056b3;
        }}

        .footer-text {{
            font-size: 14px;
            color: #999999;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>Booking Completed!</h1>
        </div>
        <div class=""email-body"">
            <p>Hi {name},</p>
            <p>We’re happy to inform you that your <strong>{serviceType}</strong> service has been successfully completed.</p>
            <p>We’d love to hear about your experience! Click the button below to provide feedback and rate your service.</p>
            
            <p>Thank you for choosing us!</p>
            <p>Best regards,</p>
            <p>Your Car Wash Team</p>
        </div>
        <div class=""email-footer"">
            <a href=""{url}"" class=""feedback-button"">Give Feedback</a>
            <p class=""footer-text"">If you have any questions, feel free to contact our support team.</p>
        </div>
    </div>
</body>
</html>
";
        }



    }


}
