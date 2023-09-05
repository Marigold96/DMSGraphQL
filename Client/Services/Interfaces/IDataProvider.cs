using Client.Models;
using Common.Models;
using Common.Requests;

namespace Client.Services.Interfaces;

public interface IDataProvider
{
    #region Documents

    Task<List<DocumentModel>?> QueryDocuments(DocumentQueryModel documentQuery);
    Task<DocumentModel?> CreateDocument(DocumentCreateModel documentCreate);
    Task<DocumentModel?> UpdateDocument(DocumentCreateModel documentCreate);
    Task<bool> ShareDocument(DocumentShareRequest documentShareRequest);
    Task<bool> DeleteDocument(Guid documentId);
    Task<DocumentContentModel?> DocumentContent(Guid documentId);

    #endregion

    #region Lookups

    Task<List<DocumentTypeModel>?> DocumentTypes();
    Task<List<ExtensionModel>?> Extensions();

    #endregion

    #region Users

    Task<List<UserModel>?> Users();
    Task<UserModel?> UserMe();
    Task<List<UserModel>?> QueryUsers(string username);
    Task<LoginData> Login(LoginInput loginInput);
    Task<bool> ChangePassword(ChangePasswordData changePasswordData);
    Task<UserModel> UserUpdate(UserModel userModel);

    #endregion

    void Subscribe(Action<SharedDocumentModel> documentShared);


}
