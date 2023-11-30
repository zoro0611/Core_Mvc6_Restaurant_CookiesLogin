using System.ComponentModel.DataAnnotations;

namespace AuthenticationProject.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "請輸入使用者名稱")]
        public string Username { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
