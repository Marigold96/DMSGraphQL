using Client.Services.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Components;

namespace Client.Shared.Layout_Elements;

public partial class UserDetails
{
    [Inject] public IDataProvider DataProvider { get; set; }

    [Parameter] public UserModel? UserModel { get; set; }


    public async Task UpdateAsync()
    {
        if (UserModel == null)
        {
            return;
        }
        UserModel = await DataProvider.UserUpdate(UserModel);
    }

}
