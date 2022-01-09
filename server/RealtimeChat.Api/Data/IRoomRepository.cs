using RealtimeChat.Api.Models;

namespace RealtimeChat.Api.Data;

public interface IRoomRepository
{
    public IList<Room?> GetAllRooms();

    public Room? GetRoomByName(string roomName);
    
    public bool CreateRoom(Room? room);

    public bool RemoveRoom(string roomName);
}