using System.ComponentModel.DataAnnotations;

namespace Common.Models;

public class UserModel : BaseModel
{

    [Display(Name = "Korisničko ime*:")]
    [Required(ErrorMessage = "Korisničko ime je obavezno.")]
    public string Name { get; set; }
    [Display(Name = "Ime i prezime*:")]
    [Required(ErrorMessage = "Ime i prezime je obavezno.")]
    public string Description { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }
    [Display(Name = "Šifra*:")]
    [MinLength(8, ErrorMessage = "Vaša šifra mora sadržati minimum 8 znakova.")]
    [Required(ErrorMessage = "Vaša šifra mora sadržati minimum 8 znakova.")]
    public string Password { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
}
