namespace TodoAPI.Dtos.Account
{
    public class FacebookTokenResponseDto
    {
        public string Access_Token { get; set; }
        
        public string Token_Type { get; set; }

        public int Expires_In { get; set; }

    }
}
