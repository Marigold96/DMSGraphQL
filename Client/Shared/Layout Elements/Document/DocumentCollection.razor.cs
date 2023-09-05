using Client.Models;
using Client.Services.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Dialog;
using Telerik.DataSource;

namespace Client.Shared.Layout_Elements.Document
{
    public partial class DocumentCollection
    {
        [Parameter] public string DocumentType { get; set; } = default!;
        [Parameter] public EventCallback<DocumentModel> OnSelectedItemChangedCallback { get; set; }

        [Inject] private IDataProvider DataProvider { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private DocumentModel? SelectedItem { get; set; }
        private DocumentTypeModel SelectedDocumentType { get; set; } = default!;
        private UserModel SelectedCreator { get; set; } = default!;
        private DocumentQueryModel DocumentQuery { get; set; } = default!;
        private DocumentCreation? DocumentCreationRef { get; set; }
        private UserModel? UserMe { get; set; }
        private TelerikAnimationContainer? FilterContainerRef { get; set; }
        private TelerikTextBox? SmartTextBox { get; set; }
        private TelerikAutoComplete<string>? SmartAutoComplete { get; set; }
        private TelerikAnimationContainer? SortContainerRef { get; set; }
        private ShareDocument? ShareDocumentRef { get; set; }
        private DeleteDocument? DeleteDocumentRef { get; set; }
        private bool DocumentTypeValidation => DocumentType != "all" && DocumentType != "sharedWithMe" && DocumentType != "sharedByMe";

        private ObservableCollection<DocumentModel> Documents { get; set; } = new ObservableCollection<DocumentModel>();
        private ObservableCollection<string> SmartSearchData { get; set; } = new ObservableCollection<string>();
        private List<DocumentTypeModel> DocumentTypes { get; set; } = new List<DocumentTypeModel>();
        private List<UserModel> Creators { get; set; } = new List<UserModel>();
        private List<UserModel> AllUsers { get; set; } = new List<UserModel>();
        private List<string> OrderData { get; set; } = new List<string>()
        {
            "Najnovije",
            "Najstarije",
            "Po nazivu A-Z",
            "Po nazivu Z-A"
        };

        private string _documenType = string.Empty;
        private bool _isBusy;
        private bool _isDownloading;
        private bool _attached;
        private bool _focusText;
        private bool _focusAutoComplete;
        private string _smartSearchText = string.Empty;
        private bool _smartSearchVisible;
        private bool _advancedFilterShown;
        private bool _advancedSortShown;
        private string _orderBy = "Najnovije";

        protected override async Task OnInitializedAsync()
        {
            AllUsers = await DataProvider.Users() ?? new List<UserModel>();
            SetEmptyQuery();
            SelectedDocumentType = new DocumentTypeModel();
            SelectedCreator = new UserModel();

            UserMe = await DataProvider.UserMe();
        }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                if (_documenType == DocumentType
                    || string.IsNullOrEmpty(DocumentType))
                {
                    return;
                }

                _documenType = DocumentType;
                if (_advancedFilterShown)
                {
                    FilterContainerRef?.ToggleAsync();
                    _advancedFilterShown = false;
                }

                if (_advancedSortShown)
                {
                    SortContainerRef?.ToggleAsync();
                    _advancedSortShown = false;
                }

                _smartSearchText = string.Empty;
                _orderBy = "Najnovije";

                if (!DocumentTypes.Any())
                {
                    DocumentTypes = await DataProvider.DocumentTypes() ?? new List<DocumentTypeModel>();
                }

                SelectedDocumentType = DocumentTypeValidation ?
                    DocumentTypes.First(x => x?.Id?.ToString() == DocumentType) :
                    new DocumentTypeModel();

                SetEmptyQuery();
                DocumentQuery.DocumentTypeId = DocumentTypeValidation ?
                    SelectedDocumentType.Id :
                    null;

                await GetDocumentsAsync();
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (SmartAutoComplete != null
                && !_attached)
            {
                // open on tab, click, FocusAsync:
                await JSRuntime.InvokeVoidAsync("attachFocusHandler", SmartAutoComplete.Id, ".k-autocomplete");
                _attached = true;
            }

            if (_focusText)
            {
                await Task.Delay(300);
                await SmartTextBox.FocusAsync();
                _focusText = false;
            }

            if (_focusAutoComplete)
            {
                await JSRuntime.InvokeVoidAsync("attachFocusHandler", SmartAutoComplete.Id, ".k-autocomplete");
                await Task.Delay(300);
                await SmartAutoComplete.FocusAsync();
                _focusAutoComplete = false;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task GetDocumentsAsync()
        {
            try
            {
                _isBusy = true;
                StateHasChanged();

                List<DocumentModel>? documents = await DataProvider.QueryDocuments(DocumentQuery);
                documents ??= new List<DocumentModel>();
                Documents = new ObservableCollection<DocumentModel>(documents);

                OnSelectedItemChanged(Documents.FirstOrDefault());
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _isBusy = false;
                StateHasChanged();
            }
        }

        #region Actions

        private void OnSelectedItemChanged(DocumentModel? selectedItem)
        {
            try
            {
                SelectedItem = selectedItem;
                OnSelectedItemChangedCallback.InvokeAsync(selectedItem);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }

        private async Task OnItemDownload(DocumentModel document)
        {
            try
            {
                _isDownloading = true;

                DocumentContentModel? response = await DataProvider.DocumentContent(document.Id.Value);
                if (response == null)
                {
                    return;
                }

                using var streamRef = new DotNetStreamReference(new MemoryStream(response.Content));
                await JSRuntime.InvokeVoidAsync("downloadDocumentFromStream", $"{document.Name}.{document.Extension.Name}", streamRef);
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex); 
            }
            finally
            {
                _isDownloading = false;
            }
        }

        #endregion

        #region Filter

        private async Task FilterDocuments()
        {
            try
            {
                await FilterContainerRef.ToggleAsync();

                _advancedFilterShown = false;
                await GetDocumentsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task ClearFilterAsync()
        {
            try
            {
                SetEmptyQuery();
                _advancedFilterShown = false;
                await GetDocumentsAsync();
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }

        private void DocumentTypeChangedHandler(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    SelectedDocumentType = new();
                    DocumentQuery.DocumentTypeId = null;
                    return;
                }

                SelectedDocumentType = DocumentTypes.First(x => x.Id == id);
                DocumentQuery.DocumentTypeId = SelectedDocumentType.Id;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }

        private async Task OnReadUsersAsync(AutoCompleteReadEventArgs args)
        {
            try
            {
                string username = string.Empty;
                if (args.Request.Filters?.Any() == true)
                {
                    username = ((FilterDescriptor)args.Request.Filters.First())?.Value?.ToString() ?? string.Empty;
                }

                Creators = await DataProvider.QueryUsers(username) ?? new List<UserModel>();
                args.Data = Creators;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }

        private void OnUserValueChangedHandler(string name)
        {
            try
            {
                UserModel? selectedCreator = Creators.Find(x => x.Name.Equals(name));
                if (selectedCreator == null)
                {
                    SelectedCreator.Name = name;
                    DocumentQuery.CreatorId = null;
                    return;
                }
                SelectedCreator = selectedCreator;
                DocumentQuery.CreatorId = SelectedCreator.Id;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

        #region Sort

        private async Task OrderChanged(string orderBy)
        {
            try
            {
                _orderBy = orderBy;
                DocumentQuery.OrderByField = orderBy;

                _advancedSortShown = false;
                await GetDocumentsAsync();
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

        #region SmartSearch

        private async Task SmartSearchTextChanged(string text)
        {
            if (SmartSearchData.Contains(text))
            {
                _smartSearchText = $"kreator: {text}";
                DocumentQuery.CreatorId = AllUsers.Single(u => u.Description == text).Id;
                DocumentQuery.Name = null;
                await GetDocumentsAsync();
            }
            else
            {
                await SmartTextCheck(text);
            }
        }

        private async Task SmartSearchTextBoxTextChanged(string text)
        {
            await SmartTextCheck(text);
        }

        private async Task SmartTextCheck(string text)
        {
            if (text?.StartsWith("kreator:") == true)
            {
                if (!_smartSearchVisible)
                {
                    SmartSearchData.Clear();
                    foreach (var user in AllUsers)
                    {
                        SmartSearchData.Add(user.Description);
                    }
                    _smartSearchVisible = true;
                    _focusAutoComplete = true;
                }
                _smartSearchText = text;
                return;
            }
            else if (_smartSearchVisible)
            {
                _smartSearchVisible = false;
                _focusText = true;
            }
            _smartSearchText = text;
            DocumentQuery.Name = text;
            DocumentQuery.CreatorId = null;

            await GetDocumentsAsync();
        }

        #endregion

        private void SetEmptyQuery()
        {
            DocumentQuery = new DocumentQueryModel()
            {
                CreatorId = null,
                DocumentTypeId = null,
                CreationTime = null,
                Name = null,
                Size = null,
                SharedByMe = DocumentType == "sharedByMe",
                SharedWithMe = DocumentType == "sharedWithMe",
                OrderByField = _orderBy
            };
        }
    }
}
