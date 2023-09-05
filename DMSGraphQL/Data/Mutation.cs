using Common.Models;
using Common.Requests;
using DMSGraphQL.Interfaces;
using Common.Models;
using HotChocolate.Authorization;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Subscriptions;
using GraphQL.Reflection;
using DMSGraphQL.Repositories;

namespace DMSGraphQL.Data;

public class Mutation
{

    [Authorize]
    public DocumentModel DocumentCreate(
        [Service] ApplicationDbContext context,
        [Service] IUserRepository userRepository,
        [Service] IDocumentTypeRepository documenTypeRepository,
        [Service] IExtensionRepository extensionRepository,
        [Service] IHttpContextAccessor accessor,
        DocumentCreateModel item)
    {
        var username = accessor.HttpContext.User.Identity.Name;
        var currentUser = userRepository.ByName(username);
        var content = Convert.FromBase64String(item.Content);
        var documentModel = new DocumentModel()
        {
            CreationTime = DateTime.Now,
            CreationUser = currentUser,
            DocumentType = documenTypeRepository.ById(item.DocumentTypeId.Value),
            Extension = extensionRepository.ById(item.ExtensionId.Value),
            ModificationTime = DateTime.Now,
            ModificationUser = currentUser,
            Name = item.Name,
            SizeKB = content.Length
        };
        var documentContent = new DocumentContentModel()
        {
            Document = documentModel,
            Content = content
        };
        context.Documents.Add(documentModel);
        context.DocumentContents.Add(documentContent);
        context.SaveChanges();
        return documentModel;
    }

    [Authorize]
    public DocumentModel DocumentUpdate([Service] ApplicationDbContext context,
        [Service] IUserRepository userRepository,
        [Service] IDocumentTypeRepository documenTypeRepository,
        [Service] IDocumentContentRepository documentContentRepository,
        [Service] IExtensionRepository extensionRepository,
        [Service] IHttpContextAccessor accessor,
        DocumentCreateModel item)
    {
        var username = accessor.HttpContext.User.Identity.Name;
        var currentUser = userRepository.ByName(username);
        DocumentModel document = context.Documents.Include(x => x.DocumentType).Include(x => x.Extension).Single(x => x.Id == item.Id);
        document.Name = !string.IsNullOrEmpty(item.Name) ? item.Name : document.Name;
        document.ModificationUser = currentUser;
        document.ModificationTime = DateTime.Now;

        if (item.DocumentTypeId != null
            && item.DocumentTypeId != document.DocumentType.Id)
        {
            document.DocumentType = documenTypeRepository.ById(item.DocumentTypeId.Value);
        }

        if (item.ExtensionId != null
            && item.ExtensionId != document.Extension.Id)
        {
            document.Extension = extensionRepository.ById(item.ExtensionId.Value);
        }

        if (item.Content != null)
        {
            var content = Convert.FromBase64String(item.Content);
            document.SizeKB = content.Length;

            DocumentContentModel documentContent = documentContentRepository.ByDocumentId(document.Id.Value);
            documentContent.Content = content;
            context.DocumentContents.Update(documentContent);
        }
        context.Documents.Update(document);
        context.SaveChanges();
        return document;
    }

    [Authorize]
    public bool DocumentDelete(
        [Service] IDocumentRepository documentRepository,
        [Service] IUserRepository userRepository,
        [Service] IDocumentContentRepository documentContentRepository,
        [Service] ISharedDocumentRepository sharedDocumentRepository,
        [Service] IHttpContextAccessor accessor,
        Guid documentId)
    {
        var userId = CurrentUserId(accessor, userRepository);
        documentContentRepository.Remove(documentId);
        sharedDocumentRepository.Remove(documentId);
        return documentRepository.Remove(documentId, userId);
    }

    public DocumentContentModel DocumentContentCreate([Service] ApplicationDbContext context, DocumentContentModel item)
    {
        context.DocumentContents.Add(item);
        context.SaveChanges();
        return item;
    }

    [Authorize]
    public async Task<bool> ShareDocument(
        [Service] ApplicationDbContext context,
        [Service] IDocumentRepository documentRepository,
        [Service] IUserRepository userRepository,
        [Service] IHttpContextAccessor accessor,
        [Service] ITopicEventSender sender,
        DocumentShareRequest shareRequest)
    {
        var username = accessor.HttpContext.User.Identity.Name;
        var sharedModel = new SharedDocumentModel();
        sharedModel.Document = documentRepository.ById(shareRequest.DocumentId);
        sharedModel.Shared = userRepository.ById(shareRequest.ShareToId);
        sharedModel.Owner = userRepository.ByName(username);
        context.SharedDocuments.Add(sharedModel);
        await context.SaveChangesAsync();
        await sender.SendAsync("DocumentShared", sharedModel);
        return true;
    }

    public DocumentContentModel DocumentContentUpdate([Service] ApplicationDbContext context, DocumentContentModel item)
    {
        context.DocumentContents.Update(item);
        context.SaveChanges();
        return item;
    }

    [Authorize(Roles = new[] { "Administrator" })]
    public UserModel UserCreate([Service] IUserRepository userRepository, UserModel item)
    {
        userRepository.Add(item);
        return item;
    }

    [Authorize(Roles = new[] { "Administrator" })]
    public UserModel UserUpdate([Service] IUserRepository userRepository, UserModel item)
    {
        return userRepository.Update(item);
    }

    [Authorize(Roles = new[] { "Administrator" })]
    public bool UserDelete([Service] IUserRepository userRepository, Guid userId)
    {
        return userRepository.Remove(userId);
    }

    public LoginData Login([Service] IUserRepository userRepository, LoginInput loginInput)
    {
        return userRepository.Login(loginInput);
    }

    [Authorize]
    public bool ChangePassword([Service] IUserRepository userRepository, [Service] IHttpContextAccessor accessor, ChangePasswordData passwordData)
    {
        var username = accessor.HttpContext.User.Identity.Name;
        return userRepository.ChangePassword(username, passwordData);
    }

    public LoginData RenewToken([Service] IUserRepository userRepository, LoginData loginData)
    {
        return userRepository.RenewAccessToken(loginData);
    }

    public ExtensionModel ExtensionCreate([Service] ApplicationDbContext context, ExtensionModel item)
    {
        context.Extensions.Add(item);
        context.SaveChanges();
        return item;
    }

    public ExtensionModel ExtensionUpdate([Service] ApplicationDbContext context, ExtensionModel item)
    {
        context.Extensions.Update(item);
        context.SaveChanges();
        return item;
    }

    public DocumentTypeModel DocumentTypeCreate([Service] ApplicationDbContext context, DocumentTypeModel item)
    {
        context.DocumentTypes.Add(item);
        context.SaveChanges();
        return item;
    }

    public DocumentTypeModel DocumentTypeUpdate([Service] ApplicationDbContext context, DocumentTypeModel item)
    {
        context.DocumentTypes.Update(item);
        context.SaveChanges();
        return item;
    }

    private Guid CurrentUserId(
       IHttpContextAccessor accessor,
       IUserRepository userRepository)
    {
        var username = accessor.HttpContext.User.Identity.Name;
        return userRepository.ByName(username).Id ?? throw new Exception("User not found!");
    }

}
