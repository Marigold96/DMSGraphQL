using Client.Services.Interfaces;
using Common.Requests;
using Common.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Client.Shared.Common;

namespace Client.Shared.Layout_Elements.Document
{
    public partial class DocumentCreation
    {
        [CascadingParameter] private NotificationComponent Notification { get; set; } = default!;

        [Parameter] public string DocumentType { get; set; } = default!;
        [Parameter] public EventCallback RefreshCallback { get; set; }

        [Inject] private IDataProvider DataProvider { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private DocumentCreateModel Document { get; set; } = new DocumentCreateModel();
        private DocumentTypeModel SelectedDocumentType { get; set; } = default!;
        private IBrowserFile? SelectedFile { get; set; }
        public RefreshableContent? FileNameRef { get; set; }
        private string SelectedFileName => string.IsNullOrEmpty(SelectedFile?.Name) ? "Prevucite datoteku ovde" : SelectedFile.Name;
        private bool DocumentTypeValidation => DocumentType != "all" && DocumentType != "sharedWithMe" && DocumentType != "sharedByMe";

        private List<DocumentTypeModel> DocumentTypes { get; set; } = new List<DocumentTypeModel>();
        private List<ExtensionModel> Extensions { get; set; } = new List<ExtensionModel>();
        private List<string> AllowedExtensions 
        { 
            get
            {
                return Extensions.Select(x => x.Name).ToList();
            }
        }

        Dictionary<string, object> _attributes = new ()
        {
            { "style", "position: absolute; opacity: 0; width: 100%; height: 100%; cursor: copy !important; left: 0;"},
            { "id", "inputFile" },
            { "title", " " }
        };
        private bool _isVisible;
        private bool _isEditDialog;
        private string? _documentType;
        private ElementReference _dropZone;

        protected override async Task OnInitializedAsync()
        {
            SelectedDocumentType = new DocumentTypeModel() { Id = Guid.NewGuid() };
            var documentTypes = await DataProvider.DocumentTypes();
            DocumentTypes = documentTypes ?? new List<DocumentTypeModel>();
            SelectedDocumentType = DocumentTypeValidation ?
                DocumentTypes.First(x => x.Id?.ToString() == DocumentType) :
                new DocumentTypeModel() { Id = Guid.NewGuid() };

            var extensions = await DataProvider.Extensions();
            Extensions = extensions ?? new List<ExtensionModel>();
        }

        protected override void OnParametersSet()
        {
            try
            {
                if (_documentType == DocumentType)
                {
                    return;
                }

                _documentType = DocumentType;
                if (_isEditDialog
                    && !DocumentTypeValidation)
                {
                    return;
                }

                SelectedDocumentType = DocumentTypeValidation ?
                    DocumentTypes.First(x => x.Id?.ToString() == DocumentType) :
                    new DocumentTypeModel() { Id = Guid.NewGuid() };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #region FileSelection

        private async Task OnFileUploadHandler(InputFileChangeEventArgs args)
        {
            try
            {
                SelectedFile = args.File;
                string? extension = Path.GetExtension(SelectedFile.Name)?.Replace(".", "");
                Document.ExtensionId = Extensions.First(x => x.Name.ToLower() == extension.ToLower()).Id.Value;

                byte[]? content = null;
                using (Stream fileStream = args.File.OpenReadStream(int.MaxValue))
                {
                    content = new byte[args.File.Size];
                    _ = await fileStream.ReadAsync(content);
                }

                Document.Content = Convert.ToBase64String(content);
                FileNameRef?.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async void ChangeDropZoneBackground(DragEventArgs args)
        {
            await JSRuntime.InvokeVoidAsync("changeDropZoneBackground", _dropZone);
        }

        private async void ResetDropZoneBackground(DragEventArgs args)
        {
            await JSRuntime.InvokeVoidAsync("resetDropZoneBackground", _dropZone);
        }

        #endregion


        #region Actions

        private void DocumentTypeChangedHandler(Guid id)
        {
            try
            {
                DocumentTypeModel? selectedDocumentType = DocumentTypes.Find(x => x.Id == id);

                SelectedDocumentType = selectedDocumentType ?? new DocumentTypeModel();
                Document.DocumentTypeId = selectedDocumentType?.Id ?? null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task CreateDocumentAsync()
        {
            try
            {
                if (Document.ExtensionId == null
                    || Document.DocumentTypeId == null
                    || string.IsNullOrEmpty(Document.Name)
                    || Document.Content == null)
                {
                    Notification.Info("Popunite sva obavezna polja!");
                    return;
                }

                DocumentModel? response = await DataProvider.CreateDocument(Document);
                if (response != null)
                {
                    Notification.Success("Uspešno sačuvan dokument!");
                    _isVisible = false;
                    await RefreshCallback.InvokeAsync();
                }
                else
                {
                    Notification.Error("Greška prilikom čuvanja dokumenta!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task UpdateDocumentAsync()
        {
            try
            {
                DocumentModel? response = await DataProvider.UpdateDocument(Document);
                if (response != null)
                {
                    Notification.Success("Uspešno izmenjen dokument!");
                    _isVisible = false;
                    await RefreshCallback.InvokeAsync();
                }
                else
                {
                    Notification.Error("Greška prilikom izmene dokumenta!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void Cancel() => _isVisible = false;

        #endregion

        public void ShowCreationDialog(DocumentModel? document = null)
        {
            Document = new DocumentCreateModel()
            { 
                Id = null,
                DocumentTypeId = null,
                CreationUserId = null,
                ExtensionId = null
            };
            SelectedFile = null;

            _isEditDialog = document != null;

            if (_isEditDialog)
            {
                Document.Id = document.Id;
                Document.Name = document.Name;
                Document.DocumentTypeId = document.DocumentType.Id.Value;
                
                SelectedDocumentType = DocumentTypes.First(x => x.Id == document.DocumentType.Id);
            }
            else
            {
                if (DocumentTypeValidation)
                {
                    SelectedDocumentType = DocumentTypes.First(x => x.Id?.ToString() == DocumentType);
                    Document.DocumentTypeId = SelectedDocumentType.Id;
                }
                else
                {
                    SelectedDocumentType = new DocumentTypeModel() { Id = Guid.NewGuid() };
                }
            }

            _isVisible = true;
            StateHasChanged();
        }
    }
}
