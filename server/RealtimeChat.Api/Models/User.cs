namespace RealtimeChat.Api.Models;

public class User
{
    public User(Guid id, string connectionId, string name, Room room)
    {
        Id = id;
        ConnectionId = connectionId;
        Name = name;
        Room = room;
    }

    public Guid Id { get; set; }
    public string ConnectionId { get; set; }
    public string Name { get; set; }
    public Room Room { get; set; }
}