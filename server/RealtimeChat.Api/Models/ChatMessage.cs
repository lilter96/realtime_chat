namespace RealtimeChat.Api.Models;

public class ChatMessage
{
    public ChatMessage(string userName, string roomName, string message)
    {
        UserName = userName;
        RoomName = roomName;
        Message = message;
    }

    public string UserName { get; set; }
    public string RoomName { get; set; }
    public string Message { get; set; }
}