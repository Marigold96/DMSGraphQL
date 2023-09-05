using Common.Models;

namespace Client.Services;

public class GraphData
{
    public List<UserModel>? Users { get; set; }

    public List<DocumentModel>? Documents { get; set; }

    public List<DocumentModel>? SharedWithMe { get; set; }

    public List<DocumentModel>? SharedByMe { get; set; }

    public List<DocumentContentModel>? DocumentContents { get; set; }

    public DocumentContentModel? DocumentContent => DocumentContents?.Single();

    public DocumentModel DocumentCreate { get; set; } = default!;

    public DocumentModel DocumentUpdate { get; set; } = default!;

    public bool DocumentDelete { get; set; }

    public List<DocumentTypeModel>? DocumentTypes { get; set; }

    public List<ExtensionModel>? Extensions { get; set; }

    public LoginData Login { get; set; }

    public LoginData RenewToken { get; set; }

    public SharedDocumentModel DocumentShared { get; set; }

    public UserModel? Me { get; set; }

    public bool ChangePassword { get; set; }

    public bool ShareDocument { get; set; }

    public UserModel? UserUpdate { get; set; }

}
