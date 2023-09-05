using System.ComponentModel.DataAnnotations;

namespace Common.Models;

public class DocumentTypeModel : BaseModel
{

    [Required(ErrorMessage = "Please specify the document type name")]
    public string Name { get; set; }

    public string Icon { get; set; }

}
