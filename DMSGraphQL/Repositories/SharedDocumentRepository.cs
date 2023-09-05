using Common.Models;
using DMSGraphQL.Data;
using DMSGraphQL.Interfaces;

namespace DMSGraphQL.Repositories;

public class SharedDocumentRepository : ISharedDocumentRepository
{

    private readonly ApplicationDbContext _appDbContext;

    public SharedDocumentRepository(ApplicationDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public List<SharedDocumentModel> GetByDocumentId(Guid documentId)
    {
        return _appDbContext.SharedDocuments.Where(x => x.Document.Id == documentId).ToList();
    }

    public void Remove(Guid documentId)
    {
        _appDbContext.SharedDocuments.RemoveRange(GetByDocumentId(documentId));
    }
}
