using Client.Services.Interfaces;
using Client.Shared.Common;
using Common.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Pages
{
    public partial class DocumentPage
    {
        [CascadingParameter] public NotificationComponent Notification { get; set; } = default!;

        [Parameter] public string? DocumentType { get; set; }

        [Inject] private IDataProvider DataProvider { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;

        private DocumentModel? SelectedDocument { get; set; }

        private void OnSelectedItemChanged(DocumentModel document)
        {
            SelectedDocument = document;
            StateHasChanged();
        }

        protected override Task OnInitializedAsync()
        {
            DataProvider.Subscribe(OnDocumentShared);
            return base.OnInitializedAsync();
        }

        private async void OnDocumentShared(SharedDocumentModel sharedDocumentModel)
        {
            var authState = await AuthProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if(user.Identity.Name != sharedDocumentModel.Shared.Name)
            {
                //not shared with me
                return;
            }
            Notification.Info($"Korisnik {sharedDocumentModel.Owner.Description} je podelio/la dokument {sharedDocumentModel.Document.Name} sa Vama.");
        }

    }
}
