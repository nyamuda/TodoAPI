using TodoAPI.Data;
using TodoAPI.Dtos.Contact;

namespace TodoAPI.Services
{
    public class ContactService
    {
        private readonly ApplicationDbContext _readonly;
        private readonly TemplateService _templateService;
        private readonly EmailSender _emailSender;


        public ContactService(ApplicationDbContext context, TemplateService templateService, EmailSender emailSender)
        {
            _readonly = context;
            _templateService = templateService;
            _emailSender = emailSender;
        }


        //Send email with information submitted by cients from the contact us page
        public async Task SendContactUsEmail(ContactUsDto contactUsDto)
        {
            string emailSubject = $"Message from {contactUsDto.Name} via Your Website";

            //the body of the email
            //containing the email, name, and message of the client
            string htmlTemplate = _templateService.ContactUs(email: contactUsDto.Email, name: contactUsDto.Name, message: contactUsDto.Message);


            //send email to detail
            string sendToEmail = _emailSender.SenderEmail;
            string sendToName=_emailSender.SenderName;
            await _emailSender.SendEmail(name: sendToName, email: sendToEmail, subject: emailSubject, message: htmlTemplate);


        }
    }
}
