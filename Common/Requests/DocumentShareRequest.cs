using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Requests;
public class DocumentShareRequest
{

    public Guid DocumentId { get; set; }

    public Guid ShareToId { get; set; }

}
