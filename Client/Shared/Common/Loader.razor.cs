using Microsoft.AspNetCore.Components;

namespace Client.Shared.Common;

public partial class Loader
{

    [Parameter] public RenderFragment ChildContent { get; set; }

    private bool _isBusy = false;

    private void SetIsBusy(bool isBusy)
    {
        _isBusy = isBusy;
        StateHasChanged();
    }

}
