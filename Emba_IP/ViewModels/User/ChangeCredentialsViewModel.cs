using System.ComponentModel.DataAnnotations;

namespace Emba_IP.ViewModels.User
{
    public class ChangeCredentialsViewModel
    {
        [Required(ErrorMessage = "Cari şifrə tələb olunur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Cari Şifrə")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifrə tələb olunur.")]
        [StringLength(100, ErrorMessage = "Şifrə minimum {2} və maksimum {1} simvol uzunluğunda olmalıdır.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifrə")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifrəni Təsdiqləyin")]
        [Compare("NewPassword", ErrorMessage = "Yeni şifrə və təsdiq şifrəsi uyğun gəlmir.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Yeni e-poçt tələb olunur.")]
        [EmailAddress(ErrorMessage = "Düzgün e-poçt ünvanı daxil edin.")]
        [Display(Name = "Yeni E-poçt")]
        public string NewEmail { get; set; }
    }
}
