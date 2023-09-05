using Blazored.LocalStorage;
using Client.Services.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Components;

namespace Client.Shared.Common;

public partial class LoginForm
{

    [CascadingParameter] public NotificationComponent Notification { get; set; }
    [CascadingParameter(Name = "SetIsBusy")] public Action<bool> SetIsBusy { get; set; }

    [Parameter] public bool? ChangePassword { get; set; }

    [Inject] public IDataProvider DataProvider { get; set; }
    [Inject] public ILocalStorageService LocalStorageService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    private LoginInput LoginInputData { get; set; } = new LoginInput();
    private ChangePasswordData ChangePasswordInfo { get; set; } = new ChangePasswordData();

    private bool _isBusy = false;
    private bool _passwordChanging = false;

    protected override Task OnParametersSetAsync()
    {
        _passwordChanging = ChangePassword.HasValue ? ChangePassword.Value : false;
        return base.OnParametersSetAsync();
    }

    private async Task LoginAsync()
    {
        try
        {
            SetIsBusy(true);
            var loginData = await DataProvider.Login(LoginInputData);
            if (!loginData.IsSuccessful)
            {
                Notification.Error("Invalid username or password");
                return;
            }
            await LocalStorageService.SetItemAsStringAsync(CommonConstants.TOKEN, loginData.AccessToken);
            await LocalStorageService.SetItemAsStringAsync(CommonConstants.REFRESH_TOKEN, loginData.RefreshToken);
            NavigationManager.NavigateTo(NavigationManager.BaseUri, true);
        }
        finally
        {
            SetIsBusy(false);
        }
    }

    private async Task ChangePasswordAsync()
    {
        try
        {
            SetIsBusy(true);
            var changed = await DataProvider.ChangePassword(ChangePasswordInfo);
            if (!changed)
            {
                Notification.Error("Neuspešna promena šifre.");
                return;
            }
            NavigationManager.NavigateTo("/login");
        }
        finally
        {
            SetIsBusy(false);
        }
    }

}
