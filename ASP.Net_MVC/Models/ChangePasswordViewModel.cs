using System.ComponentModel.DataAnnotations;

namespace ASP.Net_MVC.Models
{
    public class ChangePasswordViewModel
    {
       
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
