using System.ComponentModel.DataAnnotations;

namespace Common.Models;

public class DocumentContentModel : BaseModel
{
    public DocumentModel Document { get; set; }

    public byte[] Content { get; set; }

}
