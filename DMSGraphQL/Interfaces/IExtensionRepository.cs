using Common.Models;

namespace DMSGraphQL.Interfaces;

public interface IExtensionRepository
{

    ExtensionModel ById(Guid Id);

}
