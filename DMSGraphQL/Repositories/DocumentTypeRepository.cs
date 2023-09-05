using DMSGraphQL.Data;
using DMSGraphQL.Interfaces;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace DMSGraphQL.Repositories;

public class DocumentTypeRepository : IDocumentTypeRepository
{

    private readonly ApplicationDbContext _appDbContext;
    private DbSet<DocumentTypeModel> DocumentTypes => _appDbContext.DocumentTypes;

    public DocumentTypeRepository(ApplicationDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public DocumentTypeModel ById(Guid Id)
    {
        return DocumentTypes.Single(u => u.Id == Id);
    }

}