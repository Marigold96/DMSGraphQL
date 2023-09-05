using Common.Models;

namespace DMSGraphQL.Interfaces;

public interface IDocumentContentRepository
{
    DocumentContentModel ByDocumentId(Guid documentId);

    bool Remove(Guid documentId);
}
