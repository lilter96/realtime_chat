﻿namespace RealtimeChat.Api.Models;

public class JoinToRoomModel
{
    public string RoomName { get; set; }
    
    public string ConnectionId { get; set; }

    public string UserName { get; set; }
}