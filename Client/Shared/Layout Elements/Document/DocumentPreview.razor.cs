using Client.Services.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Components;

namespace Client.Shared.Layout_Elements.Document
{
    public partial class DocumentPreview
    {
        [Parameter] public Guid? DocumentId { get; set; }

        [Inject] IDataProvider DataProvider { get; set; } = default!;
        
        private Guid? CurrentDocumentId { get; set; }

        private bool _isBusy;
        private string? _documentContent;

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                if (CurrentDocumentId == DocumentId)
                {
                    return;
                }
                CurrentDocumentId = DocumentId;

                if (DocumentId == null)
                {
                    _documentContent = null;
                    return;
                }

                _isBusy = true;
                DocumentContentModel? documentContent = await DataProvider.DocumentContent(DocumentId.Value);
                _documentContent = documentContent != null ?
                    $"data:{documentContent.Document.Extension.MimeType};base64,{Convert.ToBase64String(documentContent.Content)}" :
                    null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _isBusy = false;
            }
        }

    }
}
