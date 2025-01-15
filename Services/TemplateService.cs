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
    }
}
