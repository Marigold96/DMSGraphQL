using Common.Models;
using DMSGraphQL.Data;
using DMSGraphQL.Interfaces;

namespace DMSGraphQL.Repositories;

public class DocumentContentRepository : IDocumentContentRepository
{

    private readonly ApplicationDbContext _appDbContext;

    public DocumentContentRepository(ApplicationDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public DocumentContentModel ByDocumentId(Guid documentId)
    {
        return _appDbContext.DocumentContents.Single(x => x.Document.Id == documentId);
    }

    public bool Remove(Guid documentId)
    {
        var documentContent = _appDbContext.DocumentContents.Single(x => x.Document.Id == documentId);
        _appDbContext.Remove(documentContent);
        return true;
    }
}