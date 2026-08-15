namespace WhatsappBot.Controllers;

using System.Text.Json.Serialization;

public class EvolutionPayload
{
    [JsonPropertyName("event")]
    public string eventName { get; set; }
    public string instance { get; set; }
    public Data data { get; set; }
    [JsonIgnore]
    public string destination { get; set; }
    public string date_time { get; set; }
    [JsonIgnore]
    public string sender { get; set; }
    [JsonIgnore]
    public string server_url { get; set; }
    [JsonIgnore]
    public string apikey { get; set; }
    
    public const string MessagesUpsert = "messages.upsert";
    public const string AudioMessage = "audioMessage";
}

public class Data
{
    public Key key { get; set; }
    public Message message { get; set; }
    public string messageType { get; set; }
    public int messageTimestamp { get; set; }
}

public class Key
{
    public bool fromMe { get; set; }
    public string id { get; set; }

    private string _participant;
    public string participant
    {
        get => _participant; 
        set => _participant = '@' + new string(value.Where(char.IsDigit).ToArray());
    }
}

public class Message
{
    public string conversation { get; set; }
    public AudioMessage audioMessage { get; set; }
    public string base64 { get; set; }
}

public class AudioMessage
{
    // The audio file (.enc extension)
    public string url { get; set; }
    public string mimetype { get; set; }
    public Dictionary<string, int> fileSha256 { get; set; }
    public FileLength fileLength { get; set; }
    public int seconds { get; set; }
    public bool ptt { get; set; }
    // For decrypt the .enc audio file
    public Dictionary<string, int> mediaKey { get; set; }
    public Dictionary<string, int> fileEncSha256 { get; set; }
    
    public class FileSha256
    {
        public Dictionary<string, int> _0 { get; set; }
    }
    
    public class FileLength
    {
        public int low { get; set; }
        public int high { get; set; }
        public bool unsigned { get; set; }
    }
    
    public class MediaKey
    {
        public Dictionary<string, int> _0 { get; set; }
    }
}