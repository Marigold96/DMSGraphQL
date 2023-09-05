using Common.Models;

namespace DMSGraphQL.Interfaces;

public interface IDocumentTypeRepository
{

    DocumentTypeModel ById(Guid Id);

}
