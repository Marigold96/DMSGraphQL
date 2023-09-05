using Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models;
public class SharedDocumentModel : BaseModel
{

    public DocumentModel Document { get; set; }
    public UserModel Owner { get; set; }
    public UserModel Shared { get; set; }


}
