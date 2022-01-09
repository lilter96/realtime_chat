using Microsoft.AspNetCore.SignalR;
using RealtimeChat.Api.Data;
using RealtimeChat.Api.Hubs.Clients;
using RealtimeChat.Api.Models;

namespace RealtimeChat.Api.Hubs;

public class ChatHub : Hub<IChatClient>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoomRepository _roomRepository;

    public ChatHub(IUserRepository userRepository, IRoomRepository roomRepository)
    {
        _userRepository = userRepository;
        _roomRepository = roomRepository;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        var user = _userRepository.GetUserByConnectionId(connectionId);
        var userName = user?.Name;
        var room = _roomRepository.GetRoomByName(user?.Room.Name!);
        if (room != null)
        {
            room.NumberOfParticipants--;
        }
        else
        {
            Task.FromException(exception);
        }
        _userRepository.RemoveUserByConnectionId(connectionId);
        if (room.NumberOfParticipants == 0)
        {
            _roomRepository.RemoveRoom(room.Name);
            return Task.CompletedTask;
        }

        var chatMessage = new ChatMessage("Admin", room.Name, $"{userName} has left!");
        Clients.Group(room.Name).ReceiveMessage(chatMessage);
        
        var roomUsers = _userRepository.GetUsersByRoomName(room.Name);
        var roomUsersNames = roomUsers.Select(u => u.Name).ToList();
        var roomInfo = new RoomInfo
        {
            Name = room.Name,
            UserNames = roomUsersNames!
        };

        Clients.Group(room.Name).ReceiveRoomInfo(roomInfo);

        return Task.CompletedTask;
    }
}