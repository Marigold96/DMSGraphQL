using System.ComponentModel.DataAnnotations;

namespace Common.Models;

public abstract class BaseModel
{

    [Key]
    public Guid? Id { get; set; } = Guid.NewGuid();

}
