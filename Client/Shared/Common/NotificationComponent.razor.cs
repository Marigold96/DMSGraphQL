using Telerik.Blazor.Components;

namespace Client.Shared.Common;

public partial class NotificationComponent
{

    private TelerikNotification Notification { get; set; }

    public void Error(string msg)
    {
        Notification.Show(new NotificationModel
        {
            Text = msg,
            ThemeColor = "error",
            CloseAfter = 10000,
            Closable = true
        });
    }

    public void Success(string msg)
    {
        Notification.Show(new NotificationModel
        {
            Text = msg,
            ThemeColor = "success",
            CloseAfter = 10000,
            Closable = true
        });
    }

    public void Info(string msg)
    {
        Notification.Show(new NotificationModel
        {
            Text = msg,
            ThemeColor = "info",
            CloseAfter = 10000,
            Closable = true
        });
    }

}
