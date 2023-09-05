using Common.Models;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace DMSGraphQL.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<DocumentContentModel> DocumentContents { get; set; }
    public DbSet<DocumentTypeModel> DocumentTypes { get; set; }
    public DbSet<ExtensionModel> Extensions { get; set; }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<DocumentModel> Documents { get; set; }
    public DbSet<SharedDocumentModel> SharedDocuments { get; set; }

}
