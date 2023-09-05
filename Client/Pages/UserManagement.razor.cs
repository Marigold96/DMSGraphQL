using Client.Services.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Components;

namespace Client.Pages;

public partial class UserManagement
{

    [Inject] public IDataProvider DataProvider { get; set; }

    public List<UserModel> Users { get; set; }
    private UserModel? SelectedItem { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Users = await DataProvider.Users() ?? new List<UserModel>();
        await base.OnInitializedAsync();
    }

    private void OnSelectedItemChanged(UserModel? selectedItem)
    {
        SelectedItem = selectedItem;
    }

}
