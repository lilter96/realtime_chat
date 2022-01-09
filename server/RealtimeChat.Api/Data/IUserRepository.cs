using RealtimeChat.Api.Models;

namespace RealtimeChat.Api.Data;

public interface IUserRepository
{
    public bool AddUser(User user);
    public bool RemoveUserByConnectionId(string connectionId);
    public User? GetUserByConnectionId(string connectionId);
    public List<User> GetUsersByRoomName(string roomName);
    public User? GetUserByName(string userName);
}