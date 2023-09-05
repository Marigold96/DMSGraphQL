using Client.Services.Interfaces;
using Client.Shared.Common;
using Common.Models;
using Microsoft.AspNetCore.Components;
using System.Reflection.Metadata;

namespace Client.Shared.Layout_Elements.Document
{
    public partial class DeleteDocument
    {
        [CascadingParameter] private NotificationComponent Notification { get; set; }

        [Parameter] public EventCallback<DocumentModel> OnDeleteDocumentCallback { get; set; }

        [Inject] private IDataProvider DataProvider { get; set; } = default!;

        private DocumentModel Document { get; set; } = default!;
        private bool Visible { get; set; }

        private async Task DeleteDocumentAsync()
        {
            try
            {
                bool deleted = await DataProvider.DeleteDocument(Document.Id.Value);
                if (deleted)
                {
                    Visible = false;
                    Notification.Success("Uspešno ste obrisali dokument!");
                    await OnDeleteDocumentCallback.InvokeAsync(Document);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void ShowDeleteDialog(DocumentModel document)
        {
            Document = document;
            Visible = true;
            StateHasChanged();
        }

    }
}
