using System.ComponentModel.DataAnnotations;

namespace Ab108Uniqlo.ViewModels.Auths
{
    public class ForgetPasswordOrUserNameVM
    {
        [Required]
        public string Email { get; set; }
    }
}
