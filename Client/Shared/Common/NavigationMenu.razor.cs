using Client.Services.Interfaces;
using Common.Extensions;
using Common.Models;
using Microsoft.AspNetCore.Components;
using System.Xml.Linq;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Client.Shared.Common
{
    public partial class NavigationMenu
    {
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;

        [Inject] public NavigationManager NavigationManager { get; set; } = default!;
        [Inject] public IDataProvider DataProvider { get; set; } = default!;

        public DrawerItemModel SelectedItem { get; set; } = default!;
        public string? UserInfo { get; set; }
        public TelerikDrawer<DrawerItemModel> DrawerRef { get; set; } = default!;
        
        public List<DrawerItemModel> Data { get; set; } = new List<DrawerItemModel>();

        private bool _expanded = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var userMe = await DataProvider.UserMe();
                if (userMe == null) 
                {
                    NavigationManager.NavigateTo(NavigationManager.BaseUri + "login");
                    return;
                }

                UserInfo = userMe.Initials();
                List<DocumentTypeModel>? documentTypes = await DataProvider.DocumentTypes();
                documentTypes ??= new List<DocumentTypeModel>();

                if (documentTypes.Any())
                {
                    documentTypes = documentTypes.OrderBy(x => x.Name).ToList();
                    foreach (DocumentTypeModel documentType in documentTypes)
                    {
                        Data.Add(new DrawerItemModel()
                        {
                            Url = $"documents/{documentType.Id.Value.ToString()}",
                            DocumentType = documentType
                        });
                    }
                }

                if (documentTypes.Any())
                {
                    Data.Add(new DrawerItemModel() { Separator = true });
                }

                DrawerItemModel allDocuments = new() { Url = "documents/all", DocumentType = new DocumentTypeModel() { Name = "Sva dokumenta", Icon = "k-icon k-i-folder-open" } };
                Data.Add(allDocuments);
                Data.Add(new DrawerItemModel() { Separator = true });
                Data.Add(new DrawerItemModel() { Url = "documents/sharedWithMe", DocumentType = new DocumentTypeModel() { Name = "Shared with me", Icon = "k-icon k-i-envelop-link" } });
                Data.Add(new DrawerItemModel() { Url = "documents/sharedByMe", DocumentType = new DocumentTypeModel() { Name = "Shared by me", Icon = "k-icon k-i-inherited" } });
                SelectedItemChangedHandler(allDocuments);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SelectedItemChangedHandler(DrawerItemModel selectedItem)
        {
            if (selectedItem == SelectedItem)
            {
                return;
            }

            SelectedItem = selectedItem;
            NavigationManager.NavigateTo(selectedItem.Url);
        }

        private async Task ToggleDrawer()
        {
            _expanded = !_expanded;
            await DrawerRef.ToggleAsync();
        }

        private void UserInfoClick()
        {
            NavigationManager.NavigateTo($"users");
            //NavigationManager.NavigateTo($"login/{true}");
        }

    }
}

public class DrawerItemModel
{
    public DocumentTypeModel DocumentType { get; set; } = default!;

    public string Url { get; set; } = default!;

    public bool Separator { get; set; }

    public string Name => DocumentType.Name;

    public string Icon => DocumentType.Icon;
}
