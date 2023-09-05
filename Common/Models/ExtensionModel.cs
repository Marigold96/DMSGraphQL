using System.ComponentModel.DataAnnotations;

namespace Common.Models;

public class ExtensionModel : BaseModel
{

    [Required(ErrorMessage = "Please specify the extension name")]
    public string Name { get; set; }

    public string MimeType { get; set; }

    public string Icon { get; set; }

}
