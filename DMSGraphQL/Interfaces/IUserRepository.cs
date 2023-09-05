using Common.Models;
using Common.Models;

namespace DMSGraphQL.Interfaces;

public interface IUserRepository
{

    LoginData Login(LoginInput loginInput);
    LoginData RenewAccessToken(LoginData renewTokenData);

    UserModel ById(Guid Id);
    UserModel? ByName(string name);
    bool ChangePassword(string useraname, ChangePasswordData passwordData);
    UserModel Add(UserModel userModel);
    UserModel Update(UserModel userModel);
    bool Remove(Guid userId);

}
