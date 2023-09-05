using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Common.Models;
public class LoginInput 
{

    [Display(Name = "Korisničko ime*:")]
    [Required(ErrorMessage = "Korisničko ime je obavezno.")]
    public string UserName { get; set; }

    [Display(Name = "Šifra*:")]
    [MinLength(8, ErrorMessage = "Vaša šifra mora sadržati minimum 8 znakova.")]
    [Required(ErrorMessage = "Vaša šifra mora sadržati minimum 8 znakova.")]
    public string Password { get; set; }

}
