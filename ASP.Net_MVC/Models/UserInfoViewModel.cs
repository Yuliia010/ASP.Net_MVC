namespace ASP.Net_MVC.Models
{
    public class UserInfoViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
