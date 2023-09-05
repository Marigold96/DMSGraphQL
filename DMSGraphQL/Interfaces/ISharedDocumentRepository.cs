using Common.Models;

namespace DMSGraphQL.Interfaces;

public interface ISharedDocumentRepository
{
    List<SharedDocumentModel> GetByDocumentId(Guid documentId);

    void Remove(Guid documentId);
}
