using Blazored.LocalStorage;
using Client.Models;
using Client.Services.Interfaces;
using Client.Services.Providers;
using Common.Models;
using Common.Requests;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Services;

public class DataProvider : IDataProvider
{

    private readonly AuthenticationStateProvider _authProvider;
    private readonly ILocalStorageService _localStorageService;
    private readonly NavigationManager _navigationManager;

    public DataProvider(
        ILocalStorageService localStorageService,
        AuthenticationStateProvider authProvider,
        NavigationManager navigationManager)
    {
        _authProvider = authProvider;
        _localStorageService = localStorageService;
        _navigationManager = navigationManager;
    }

    #region Documents

    private string documentFragment =
        @"fragment DocumentFields on DocumentModel {
            id
            name
            documentType {
                id
            }
            creationUser {
                id,
                description
            }
            modificationUser {
                id
            }
            creationTime
            modificationTime
            extension {
                id,
                icon,
                name
            }
            sizeKB 
        }";

    public async Task<DocumentContentModel?> DocumentContent(Guid documentId)
    {
        var documentContent = await SendQueryProtectedAsync<GraphData>(new GraphQLRequest()
        {
            Query = @"query ($docId: UUID){
                          documentContents 
                          (where: { document: {id: {eq: $docId} }}) 
                          {
                            id,
                            document{
                              extension{
                                mimeType
                              }
                            }
                            content
                          }
                        }",
            Variables = new
            {
                docId = documentId
            }
        });
        return documentContent?.Data.DocumentContent!;
    }

    public async Task<List<DocumentModel>?> QueryDocuments(DocumentQueryModel documentQuery)
    {
        string query = documentFragment + $"query ";
        List<string> queryArguments = new();
        List<string> whereArguments = new();
        Dictionary<string, object> variables = new Dictionary<string, object>();

        if (documentQuery.DocumentTypeId != null)
        {
            queryArguments.Add("$docTypeId: UUID");
            whereArguments.Add("documentType: { id: {eq: $docTypeId } }");
            variables.Add("docTypeId", documentQuery.DocumentTypeId);
        }

        if (!string.IsNullOrEmpty(documentQuery.Name))
        {
            queryArguments.Add("$docName: String");
            whereArguments.Add("name: {contains: $docName}");
            variables.Add("docName", documentQuery.Name);
        }

        if (documentQuery.CreatorId != null)
        {
            queryArguments.Add("$creationUserId: UUID");
            whereArguments.Add("creationUser: { id: {eq: $creationUserId}}");
            variables.Add("creationUserId", documentQuery.CreatorId);
        }

        if (documentQuery.CreationTime != null)
        {
            DateTime nextDay = documentQuery.CreationTime.Value.AddDays(1);
            queryArguments.Add("$creationTime: DateTime");
            queryArguments.Add("$nextDay: DateTime");
            whereArguments.Add("creationTime: {gte: $creationTime, lte: $nextDay}");
            variables.Add("creationTime", documentQuery.CreationTime);
            variables.Add("nextDay", nextDay);
        }

        if (documentQuery.Size != null)
        {
            queryArguments.Add("$size: Decimal");
            whereArguments.Add("sizeKB: {gte: $size }");
            variables.Add("size", documentQuery.Size);
        }

        string stringQueryArguments = string.Empty;
        for (int i = 0; i < queryArguments.Count; i++)
        {
            stringQueryArguments += queryArguments[i];
            if (i != queryArguments.Count - 1)
            {
                stringQueryArguments += ", ";
            }
        }

        string stringWhereArguments = string.Empty;
        for (int i = 0; i < whereArguments.Count; i++)
        {
            stringWhereArguments += whereArguments[i];
            if (i != whereArguments.Count - 1)
            {
                stringWhereArguments += ", ";
            }
        }

        query += !string.IsNullOrEmpty(stringQueryArguments) ? "(" + stringQueryArguments + ")" : "";
        query += "{ ";
        if (documentQuery.SharedByMe == true)
        {
            query += "sharedByMe ";
        }
        else if (documentQuery.SharedWithMe == true)
        {
            query += "sharedWithMe ";
        }
        else
        {
            query += "documents ";
        }

        query += !string.IsNullOrEmpty(stringWhereArguments) ? "( where: {" + stringWhereArguments + "}, " + documentQuery.OrderBy + ")" : $"({documentQuery.OrderBy})";
        query += "{ ...DocumentFields } }";

        var queryRequest = new GraphQLRequest()
        {
            Query = query,
            Variables = variables
        };
        var result = await SendQueryProtectedAsync<GraphData>(queryRequest);

        if (documentQuery.SharedByMe == true)
        {
            return result?.Data.SharedByMe!;
        }
        else if (documentQuery.SharedWithMe == true)
        {
            return result?.Data.SharedWithMe!;
        }
        else
        {
            return result?.Data.Documents!;
        }
    }

    public async Task<DocumentModel?> CreateDocument(DocumentCreateModel documentCreate)
    {
        var documentContent = await SendMutationProtectedAsync<GraphData>(new GraphQLRequest()
        {
            Query = @"mutation createDocument($item: DocumentCreateModelInput!) {
                      documentCreate(item: $item) {
                        id
                      }
                    }",
            Variables = new
            {
                item = documentCreate
            }
        });
        return documentContent?.Data.DocumentCreate;
    }

    public async Task<DocumentModel?> UpdateDocument(DocumentCreateModel documentCreate)
    {
        var documentContent = await SendMutationProtectedAsync<GraphData>(new GraphQLRequest()
        {
            Query = @"mutation modifyDocument($item: DocumentCreateModelInput!) {
                      documentUpdate(item: $item) {
                        id
                      }
                    }",
            Variables = new
            {
                item = documentCreate
            }
        });
        return documentContent?.Data.DocumentUpdate;
    }

    public async Task<bool> ShareDocument(DocumentShareRequest documentShareRequest)
    {
        var documentContent = await SendMutationProtectedAsync<GraphData>(new GraphQLRequest()
        {
            Query = @"mutation shareDocument($shareRequest: DocumentShareRequestInput!) {  
                        shareDocument(shareRequest: $shareRequest) 
                            {}
                        }",
            Variables = new
            {
                shareRequest = documentShareRequest
            }
        });
        return documentContent?.Data?.ShareDocument == true;
    }

    public async Task<bool> DeleteDocument(Guid documentId)
    {
        var documentContent = await SendMutationProtectedAsync<GraphData>(new GraphQLRequest()
        {
            Query = @"mutation documentDelete($documentId: UUID!) {  
                        documentDelete(documentId: $documentId) 
                            {}
                        }",
            Variables = new
            {
                documentId = documentId
            }
        });
        return documentContent?.Data?.DocumentDelete == true;
    }

    #endregion

    #region Lookups

    public async Task<List<ExtensionModel>?> Extensions()
    {
        var query = new GraphQLRequest()
        {
            Query = @"query {
                        extensions {
                            name
                            id
                        }
                    }"
        };
        var result = await SendQueryProtectedAsync<GraphData>(query);
        return result?.Data.Extensions!;
    }

    public async Task<List<DocumentTypeModel>?> DocumentTypes()
    {
        var query = new GraphQLRequest()
        {
            Query = @"query {
                        documentTypes {
                            name
                            id
                            icon
                        }
                    }"
        };
        var result = await SendQueryProtectedAsync<GraphData>(query);
        return result?.Data?.DocumentTypes!;
    }

    #endregion

    #region Users

    public async Task<List<UserModel>?> Users()
    {
        var query = new GraphQLRequest()
        {
            Query = @"query {
                    users {
                        name
                        id
                        description
                        isAdmin
                        isActive
                    }
                }"
        };
        var result = await SendQueryProtectedAsync<GraphData>(query);
        return result?.Data?.Users!;
    }

    public async Task<UserModel?> UserMe()
    {
        var query = new GraphQLRequest()
        {
            Query = @"query {
                    me {
                        name
                        id
                        description
                    }
                }"
        };
        var result = await SendQueryProtectedAsync<GraphData>(query);
        return result?.Data?.Me!;
    }

    public async Task<List<UserModel>?> QueryUsers(string username)
    {
        var query = new GraphQLRequest()
        {
            Query = @"query ($username: String){
                    users (where: {name: {contains: $username}}){
                        name
                        id
                        description
                    }
                }",
            Variables = new
            {
                username = username
            }
        };
        var result = await SendQueryProtectedAsync<GraphData>(query);
        return result.Data.Users!;
    }

    public async Task<LoginData> Login(LoginInput loginInput)
    {
        var query = new GraphQLRequest()
        {
            Query = @"mutation login ($loginInput: LoginInput!)
                  {
                    login(loginInput: $loginInput){
                        accessToken
                        refreshToken
                        isSuccessful
                    }
                  }
                ",
            Variables = new
            {
                loginInput = new
                {
                    UserName = loginInput.UserName,
                    Password = loginInput.Password
                }
            }
        };
        var result = await new GraphQLHttpClientProtected().SendMutationAsync<GraphData>(query);
        return result.Data.Login!;
    }

    public async Task<bool> ChangePassword(ChangePasswordData passwordData)
    {
        var query = new GraphQLRequest()
        {
            Query = @"mutation changePassword ($passwordData: ChangePasswordDataInput!)
                  {
                    changePassword(passwordData: $passwordData){                        
                    }
                  }
                ",
            Variables = new
            {
                passwordData = new
                {
                    OldPassword = passwordData.OldPassword,
                    NewPassword = passwordData.NewPassword
                }
            }
        };
        var result = await SendMutationProtectedAsync<GraphData>(query);
        return result?.Data?.ChangePassword! == true;
    }

    public async Task<UserModel> UserUpdate(UserModel userModel)
    {
        var query = new GraphQLRequest()
        {
            Query = @"mutation userUpdate ($userModel: UserModelInput!)
                  {
                    userUpdate(item: $userModel){     
                        name
                        id
                        description
                        isAdmin
                        isActive
                    }
                  }
                ",
            Variables = new
            {
                userModel = userModel
            }
        };
        var result = await SendMutationProtectedAsync<GraphData>(query);
        return result?.Data?.UserUpdate!;
    }

    private async Task<LoginData> RenewTokenAsync(LoginData loginData)
    {
        var query = new GraphQLRequest()
        {
            Query = @"mutation renewToken ($loginData: LoginDataInput!)
                      {
                        renewToken(loginData: $loginData){
                            accessToken
                            refreshToken
                            isSuccessful
                        }
                      }",
            Variables = new
            {
                loginData = new
                {
                    AccessToken = loginData.AccessToken,
                    RefreshToken = loginData.RefreshToken
                }
            }
        };
        var result = await new GraphQLHttpClientProtected().SendMutationAsync<GraphData>(query);
        var loginDataResponse = result.Data.RenewToken!;
        if (!loginDataResponse.IsSuccessful)
        {
            _navigationManager.NavigateTo(_navigationManager.BaseUri + "login");
        }
        return loginDataResponse;
    }

    #endregion

    #region Subscriptions

    public void Subscribe(Action<SharedDocumentModel> documentShared)
    {
        var query = new GraphQLRequest()
        {
            Query = @"subscription {
                          documentShared {
                            document{
                              name
                              id
                            }
                            owner{
                              id
                              name
                              description
                            }
                            shared{
                              id
                              name
                            }
                          }
                        }"
        };
        var subscriptionStream = new GraphQLHttpClientProtected().CreateSubscriptionStream<GraphData>(query);
        subscriptionStream.Subscribe(response =>
        {
            documentShared(response.Data.DocumentShared);
        });
    }

    #endregion

    private async Task<GraphQLResponse<TResponse>?> SendQueryProtectedAsync<TResponse>(GraphQLRequest request, CancellationToken cancellationToken = default)
    {
        var token = await _localStorageService.GetItemAsStringAsync(CommonConstants.TOKEN);
        token = await TryRefreshToken(token);
        if (token != null)
        {
            return await new GraphQLHttpClientProtected().SendQueryProtectedAsync<TResponse>(request, token, cancellationToken);
        }
        _navigationManager.NavigateTo(_navigationManager.BaseUri + "login");
        return null;
    }

    private async Task<GraphQLResponse<TResponse>?> SendMutationProtectedAsync<TResponse>(GraphQLRequest request, CancellationToken cancellationToken = default)
    {
        var token = await _localStorageService.GetItemAsStringAsync(CommonConstants.TOKEN);
        token = await TryRefreshToken(token);
        if (token != null)
        {
            return await new GraphQLHttpClientProtected().SendMutationProtectedAsync<TResponse>(request, token, cancellationToken);
        }
        _navigationManager.NavigateTo(_navigationManager.BaseUri + "login");
        return null;
    }

    private async Task<string?> TryRefreshToken(string token)
    {
        var authState = await _authProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var expClaim = user.Claims.FirstOrDefault(c => c.Type.Equals("exp"));
        if (expClaim == null)
        {
            return null;
        }
        var exp = expClaim.Value;
        var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
        var timeUTC = DateTime.UtcNow;
        var diff = expTime - timeUTC;
        if (diff.TotalMinutes <= 2)
        {
            var refreshToken = await _localStorageService.GetItemAsStringAsync(CommonConstants.REFRESH_TOKEN);
            var newTokenData = await this.RenewTokenAsync(new LoginData(true, token, refreshToken));
            if (!newTokenData.IsSuccessful)
            {
                return null;
            }
            var newToken = newTokenData.AccessToken;
            refreshToken = newTokenData.RefreshToken;
            await _localStorageService.SetItemAsStringAsync(CommonConstants.TOKEN, newToken);
            await _localStorageService.SetItemAsStringAsync(CommonConstants.REFRESH_TOKEN, refreshToken);
            return newToken;
        }
        return token;
    }

}