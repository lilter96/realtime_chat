namespace RealtimeChat.Api.Models;

public class RoomInfo
{
    public string Name { get; set; }
    public List<string> UserNames { get; set; } = new();
}