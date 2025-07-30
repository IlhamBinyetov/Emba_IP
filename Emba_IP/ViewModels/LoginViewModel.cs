using System.ComponentModel.DataAnnotations;

namespace Emba_IP.ViewModels
{
   
        public class LoginViewModel
        {
            //[Required(ErrorMessage = "İstifadəçi adı boş ola bilməz.")]
            //public string Username { get; set; }

            [Required(ErrorMessage = "Email boş ola bilməz.")]
            [EmailAddress(ErrorMessage = "Düzgün email formatı deyil.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Şifrə boş ola bilməz.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }
    
}
