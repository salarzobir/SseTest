namespace SseTest;

public class User : NotificationObject
{
    private string? _name;

    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
}
