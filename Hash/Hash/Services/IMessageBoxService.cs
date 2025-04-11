namespace Hash.Services;

interface IMessageBoxService
{
    bool ShowMessage(string message, MessageLevel messageLevel);
}

enum MessageLevel
{
    Information,

    Warning,

    Error,
}
