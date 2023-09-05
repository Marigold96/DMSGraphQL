using Common.Models;
using DMSGraphQL.Data;
using DMSGraphQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DMSGraphQL.Repositories;

public class DocumentRepository : IDocumentRepository
{

    private readonly ApplicationDbContext _appDbContext;
    private DbSet<DocumentModel> AllDocuments => _appDbContext.Documents;

    public DocumentRepository(ApplicationDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public DocumentModel ById(Guid Id, Guid? currentUserId = null)
    {
        return AllDocuments.Single(u => u.Id == Id && (currentUserId == null || u.CreationUser.Id == currentUserId));
    }

    public bool Remove(Guid documentId, Guid currentUserId)
    {
        var document = ById(documentId, currentUserId);
        _appDbContext.Remove(document);
        _appDbContext.SaveChanges();
        return true;
    }

    public IQueryable<DocumentModel> Documents(Guid currentUserId)
    {
        return AllDocuments.Where(d => d.CreationUser.Id == currentUserId);
    }

}
