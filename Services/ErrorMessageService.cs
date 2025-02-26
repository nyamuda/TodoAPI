namespace TodoAPI.Services
{
    public class ErrorMessageService
    {


        public ErrorMessageService() { }

        public string UnexpectedErrorMessage()
        {
            return "The server encountered an unexpected issue. Please try again later.";
        }
    }
}
