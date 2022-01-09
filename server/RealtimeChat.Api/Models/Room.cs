namespace RealtimeChat.Api.Models;

public class Room
{
    public Room(Guid id, string name, int numberOfParticipants = 0)
    {
        Id = id;
        Name = name;
        NumberOfParticipants = numberOfParticipants;
    }

    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public int NumberOfParticipants { get; set; }
}