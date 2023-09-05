using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models;

public class DocumentModel : BaseModel
{

    [Required(ErrorMessage = "Please specify document name")]
    public string Name { get; set; }

    //[ForeignKey("DocumentTypeId")]
    //public Guid DocumentTypeId { get; set; }
    public DocumentTypeModel DocumentType { get; set; }

    public UserModel CreationUser { get; set; }
    public UserModel ModificationUser { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime ModificationTime { get; set; }
    public ExtensionModel Extension { get; set; }
    public decimal SizeKB { get; set; }

}
