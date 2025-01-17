namespace TodoAPI.Dtos.Account
{
    public class GoogleTokenResponseDto
    {
        public string Access_Token { get; set; }
        public int Expires_In { get; set; }
        public string Scope { get; set; }
        public string Token_Type { get; set; }
        public string Id_Token { get; set; }
    }

}
