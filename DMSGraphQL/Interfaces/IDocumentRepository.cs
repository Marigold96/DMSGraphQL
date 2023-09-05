using Common.Models;

namespace DMSGraphQL.Interfaces;

public interface IDocumentRepository
{

    DocumentModel ById(Guid Id, Guid? currentUserId = null);

    IQueryable<DocumentModel> Documents(Guid currentUserId);

    bool Remove(Guid documentId, Guid currentUserId);

}
