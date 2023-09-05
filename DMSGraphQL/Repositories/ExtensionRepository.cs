using DMSGraphQL.Data;
using DMSGraphQL.Interfaces;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace DMSGraphQL.Repositories;


public class ExtensionRepository : IExtensionRepository
{

    private readonly ApplicationDbContext _appDbContext;
    private DbSet<ExtensionModel> Extensions => _appDbContext.Extensions;

    public ExtensionRepository(ApplicationDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public ExtensionModel ById(Guid Id)
    {
        return Extensions.Single(u => u.Id == Id);
    }

}