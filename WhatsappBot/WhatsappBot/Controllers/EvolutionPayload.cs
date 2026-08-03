using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WhatsappBot.Controllers;

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
    [JsonIgnore]
    public Key key { get; set; }
    public Message message { get; set; }
    public string messageType { get; set; }
    public int messageTimestamp { get; set; }
}

public class Key
{
    public string remoteJid { get; set; }
    public bool fromMe { get; set; }
    public string id { get; set; }
}

public class Message
{
    public string conversation { get; set; }
    public AudioMessage audioMessage { get; set; }
}

public class AudioMessage
{
    // The audio file (.enc extension)
    public string url { get; set; }
    public string mimetype { get; set; }
    public byte[] fileSha256 { get; set; } = new byte[32];
    public FileLength fileLength { get; set; }
    public int seconds { get; set; }
    public bool ptt { get; set; }
    // For decrypt the .enc audio file
    public byte[] mediaKey { get; set; } = new byte[32];
    public byte[] fileEncSha256 { get; set; } = new byte[32];
    
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