namespace Hash.Services;

class MessageBoxService : IMessageBoxService
{
    public bool ShowMessage(string message, MessageLevel messageLevel)
    {
        System.Windows.MessageBoxResult result = System.Windows.MessageBoxResult.None;
        switch (messageLevel)
        {
            case MessageLevel.Information:
                {
                    result = System.Windows.MessageBox.Show(
                        message,
                        "Information",
                        System.Windows.MessageBoxButton.OKCancel,
                        System.Windows.MessageBoxImage.Information,
                        System.Windows.MessageBoxResult.None,
                        System.Windows.MessageBoxOptions.DefaultDesktopOnly
                    );
                }
                break;
            case MessageLevel.Warning:
                {
                    result = System.Windows.MessageBox.Show(
                        message,
                        "Warning",
                        System.Windows.MessageBoxButton.OKCancel,
                        System.Windows.MessageBoxImage.Warning,
                        System.Windows.MessageBoxResult.None,
                        System.Windows.MessageBoxOptions.DefaultDesktopOnly
                    );
                }
                break;
            case MessageLevel.Error:
                {
                    result = System.Windows.MessageBox.Show(
                        message,
                        "Error",
                        System.Windows.MessageBoxButton.OKCancel,
                        System.Windows.MessageBoxImage.Error,
                        System.Windows.MessageBoxResult.None,
                        System.Windows.MessageBoxOptions.DefaultDesktopOnly
                    );
                }
                break;
            default:
                break;
        }
        return result == System.Windows.MessageBoxResult.OK;
    }
}
