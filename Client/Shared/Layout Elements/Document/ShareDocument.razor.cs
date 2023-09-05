using Client.Services.Interfaces;
using Client.Shared.Common;
using Common.Models;
using Common.Requests;
using Microsoft.AspNetCore.Components;

namespace Client.Shared.Layout_Elements.Document
{
    public partial class ShareDocument
    {
        [CascadingParameter] private NotificationComponent Notification { get; set; } = default!;

        [Inject] private IDataProvider DataProvider { get; set; } = default!;

        private DocumentModel Document { get; set; } = default!;
        private bool Visible { get; set; }
        private string AutoCompleteValue { get; set; } = string.Empty;
        private UserModel? SelectedUser { get; set; } 

        private List<UserModel> Users { get; set; } = new List<UserModel>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Users = await DataProvider.Users() ?? new List<UserModel>();

                UserModel? userMe = await DataProvider.UserMe();
                if (userMe != null)
                {
                    Users.RemoveAll(x => x.Id == userMe.Id);
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }

        #region Actions

        private void OnValueChangedHandler(string value)
        {
            try
            {
                AutoCompleteValue = value;
                SelectedUser = !string.IsNullOrEmpty(AutoCompleteValue) ?
                    Users.Find(x => x.Description == AutoCompleteValue) :
                    null;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }

        private async Task ShareDocumentAsync()
        {
            try
            {
                if (SelectedUser == null)
                {
                    Notification.Info("Izaberite korisnika sa kim želite da podelite dokuemnt!");
                    return;
                }

                bool documentShared = await DataProvider.ShareDocument(new DocumentShareRequest()
                {
                    DocumentId = Document.Id.Value,
                    ShareToId = SelectedUser.Id.Value
                });

                if (documentShared)
                {
                    Visible = false;
                    Notification.Success($"Uspešno ste podelili dokument sa {SelectedUser.Description}!");
                }
                else
                {
                    Notification.Error($"Došlo je do greške prilikom deljenja dokuemnta sa {SelectedUser.Description}!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

        public void ShowShareDialog(DocumentModel document)
        {
            Document = document;

            Visible = true;
            StateHasChanged();
        }
    }
}
