namespace WhatsappBot.Controllers;

using System.Text.Json.Serialization;

public class EvolutionPayload
{
    [JsonPropertyName("event")]
    public string eventName { get; set; } = string.Empty;
    public string instance { get; set; } = string.Empty;
    public Data data { get; set; }  = new();
    [JsonIgnore]
    public string destination { get; set; } = string.Empty;
    public string date_time { get; set; } = string.Empty;
    [JsonIgnore]
    public string sender { get; set; } = string.Empty;
    [JsonIgnore]
    public string server_url { get; set; } = string.Empty;
    [JsonIgnore]
    public string apikey { get; set; } = string.Empty;
    
    public const string MessagesUpsert = "messages.upsert";
    public const string AudioMessage = "audioMessage";
}

public class Data
{
    public Key key { get; set; } = new();
    public Message message { get; set; } = new();
    public string messageType { get; set; } = string.Empty;
    public int messageTimestamp { get; set; }
}

public class Key
{
    public string remoteJid
    {
        get;
        set => field = new string(value.Where(char.IsDigit).ToArray());
    } = string.Empty;

    public bool fromMe { get; set; }
    public string id { get; set; } = string.Empty;

    public string participant
    {
        get;
        set => field = '@' + new string(value.Where(char.IsDigit).ToArray());
    } = string.Empty;
}

public class Message
{
    public string conversation { get; set; } = string.Empty;
    public AudioMessage audioMessage { get; set; }  = new();
    public string base64 { get; set; } = string.Empty;
}

public class AudioMessage
{
    // The audio file (.enc extension)
    public string url { get; set; } = string.Empty;
    public string mimetype { get; set; } = string.Empty;
    public Dictionary<string, int> fileSha256 { get; set; } = new();
    public FileLength fileLength { get; set; } = new();
    public int seconds { get; set; }
    public bool ptt { get; set; }
    // For decrypt the .enc audio file
    public Dictionary<string, int> mediaKey { get; set; } = new();
    public Dictionary<string, int> fileEncSha256 { get; set; } = new();
    
    public class FileSha256
    {
        public Dictionary<string, int> _0 { get; set; } = new();
    }
    
    public class FileLength
    {
        public int low { get; set; }
        public int high { get; set; }
        public bool unsigned { get; set; }
    }
    
    public class MediaKey
    {
        public Dictionary<string, int> _0 { get; set; } = new();
    }
}