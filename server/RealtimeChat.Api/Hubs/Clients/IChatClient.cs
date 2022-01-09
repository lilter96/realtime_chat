using RealtimeChat.Api.Models;

namespace RealtimeChat.Api.Hubs.Clients;

public interface IChatClient
{
    Task ReceiveMessage(ChatMessage message);
    Task ReceiveRoomInfo(RoomInfo roomInfo);
}