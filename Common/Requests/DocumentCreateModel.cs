using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Requests;
public class DocumentCreateModel : BaseModel
{

    public string? Name { get; set; }

    public Guid? DocumentTypeId { get; set; }

    public Guid? CreationUserId { get; set; }

    public Guid? ExtensionId { get; set; }

    public string? Content { get; set; }

}
