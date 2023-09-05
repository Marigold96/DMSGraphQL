using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Common.Models;
public class ChangePasswordData
{

    [Display(Name = "Trenutna šifra*:")]
    [MinLength(8, ErrorMessage = "Vaša šifra mora sadržati minimum 8 znakova.")]
    [Required(ErrorMessage = "Vaša šifra mora sadržati minimum 8 znakova.")]
    public string OldPassword  { get; set; }
    [Display(Name = "Nova šifra*:")]
    [MinLength(8, ErrorMessage = "Vaša šifra mora sadržati minimum 8 znakova.")]
    [Required(ErrorMessage = "Vaša šifra mora sadržati minimum 8 znakova.")]
    public string NewPassword { get; set; }

}
