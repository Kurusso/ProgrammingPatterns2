namespace UserService.Helpers;

[System.Serializable]
public class BackendException : System.Exception
{
    public int? StatusCode { get; set; }
    private string? _userMessage;
    public string UserMessage
    {
        get { return _userMessage ?? Message ?? "Something went wrong!"; }
        private set { _userMessage = value; }
    }

    private string? _logMessage;
    public string LogMessage
    {
        get { return _logMessage ?? Message ?? ""; }
        private set { _logMessage = value; }
    }

    public BackendException() { }
    public BackendException(string message) : base(message) { }
    public BackendException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public BackendException(int statusCode, string userMessage, string logMessage) : base(logMessage)
    {
        StatusCode = statusCode;
        UserMessage = userMessage;
        LogMessage = logMessage;
    }

    public BackendException(string message, System.Exception inner) : base(message, inner) { }
    protected BackendException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}