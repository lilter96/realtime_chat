using RealtimeChat.Api.Models;

namespace RealtimeChat.Api.Data;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    public bool AddUser(User user)
    {
        _users.Add(user);

        return true;
    }

    public bool RemoveUserByConnectionId(string connectionId)
    {
        var user = _users.FirstOrDefault(user => user.ConnectionId == connectionId);
        return user != null && _users.Remove(user);
    }

    public User? GetUserByConnectionId(string connectionId)
    {
        return _users.Find(user => user.ConnectionId == connectionId);
    }

    public List<User> GetUsersByRoomName(string roomName)
    {
        return _users.Where(user => user.Room.Name == roomName).ToList();
    }

    public User? GetUserByName(string userName)
    {
        return _users.Find(user => user.Name == userName);
    }
}