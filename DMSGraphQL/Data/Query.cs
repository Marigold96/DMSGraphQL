using Common.Models;
using DMSGraphQL.Interfaces;
using HotChocolate.Authorization;

namespace DMSGraphQL.Data;

public class Query
{

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<UserModel> Users([Service] ApplicationDbContext context) =>
            context.Users;

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize]
    public UserModel Me(
        [Service] IUserRepository userRepository,
        [Service] IHttpContextAccessor accessor)
    {
        var username = accessor.HttpContext.User.Identity.Name;
        return userRepository.ByName(username);
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize]
    public IQueryable<DocumentModel> Documents(
        [Service] IDocumentRepository documentRepository,
        [Service] IUserRepository userRepository,
        [Service] IHttpContextAccessor accessor)
    {
        var userId = CurrentUserId(accessor, userRepository);
        return documentRepository.Documents(userId);
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize]
    public IQueryable<DocumentModel> SharedByMe(
        [Service] ApplicationDbContext context,
        [Service] IHttpContextAccessor accessor)
    {
        var username = accessor.HttpContext.User.Identity.Name;
        return context.SharedDocuments.Where(d => d.Owner.Name == username).Select(sd => sd.Document).Distinct();
    }


    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize]
    public IQueryable<DocumentModel> SharedWithMe(
        [Service] ApplicationDbContext context,
        [Service] IHttpContextAccessor accessor)
    {
        var username = accessor.HttpContext.User.Identity.Name;
        return context.SharedDocuments.Where(d => d.Shared.Name == username).Select(sd => sd.Document).Distinct();
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<ExtensionModel> Extensions([Service] ApplicationDbContext context) =>
            context.Extensions;

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<DocumentTypeModel> DocumentTypes([Service] ApplicationDbContext context) =>
           context.DocumentTypes;

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<DocumentContentModel> DocumentContents([Service] ApplicationDbContext context) =>
          context.DocumentContents;

    private Guid CurrentUserId(
        IHttpContextAccessor accessor,
        IUserRepository userRepository)
    {
        var username = accessor.HttpContext.User.Identity.Name;
        return userRepository.ByName(username).Id ?? throw new Exception("User not found!");
    }

}