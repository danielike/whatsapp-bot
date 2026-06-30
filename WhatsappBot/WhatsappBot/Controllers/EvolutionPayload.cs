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
}