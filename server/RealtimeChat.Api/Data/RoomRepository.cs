using RealtimeChat.Api.Models;

namespace RealtimeChat.Api.Data;

public class RoomRepository : IRoomRepository
{
    private readonly List<Room?> _rooms = new ();

    public IList<Room?> GetAllRooms()
    {
        return _rooms;
    }

    public Room? GetRoomByName(string roomName)
    {
        return _rooms.FirstOrDefault(room => (room != null) && room.Name == roomName);
    }

    public bool CreateRoom(Room? room)
    {
        _rooms.Add(room);
        return true;
    }

    public bool RemoveRoom(string roomName)
    {
        return _rooms.Remove(GetRoomByName(roomName));
    }
}