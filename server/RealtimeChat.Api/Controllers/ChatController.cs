using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealtimeChat.Api.Data;
using RealtimeChat.Api.Hubs;
using RealtimeChat.Api.Hubs.Clients;
using RealtimeChat.Api.Models;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Route("chat")]
public class ChatController : ControllerBase
{
    private readonly IHubContext<ChatHub, IChatClient> _chatHubContext;
    private readonly IUserRepository _userRepository;
    private readonly IRoomRepository _roomRepository;

    public ChatController(
        IHubContext<ChatHub, IChatClient> chatHubContext,
        IUserRepository userRepository,
        IRoomRepository romRoomRepository)
    {
        _chatHubContext = chatHubContext;
        _userRepository = userRepository;
        _roomRepository = romRoomRepository;
    }

    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageModel sendMessageModel)
    {
        var user = _userRepository.GetUserByConnectionId(sendMessageModel.ConnectionId);
        if (user == null)
        {
            return NotFound($"User with {sendMessageModel.ConnectionId} was not found!");
        }

        var chatMessage = new ChatMessage(user.Name, user.Room.Name, sendMessageModel.Message);
        await _chatHubContext.Clients.All.ReceiveMessage(chatMessage);

        return Ok();
    }

    [HttpPost("room/join")]
    public IActionResult JoinToRoom([FromBody] JoinToRoomModel joinToRoomModel)
    {
        var user = _userRepository.GetUserByName(joinToRoomModel.UserName);
        if (user != null)
        {
            if (user.Room.Name == joinToRoomModel.RoomName)
            {
                return Conflict($"User with {joinToRoomModel.UserName} in room {joinToRoomModel.RoomName} already exists.");
            }
        }

        var isRoomCreated = false;
        var room = _roomRepository.GetRoomByName(joinToRoomModel.RoomName);
        if (room == null)
        {
            room = new Room(Guid.NewGuid(), joinToRoomModel.RoomName, 1);
            
            _roomRepository.CreateRoom(room);
            isRoomCreated = true;
        }
        else
        {
            room.NumberOfParticipants++;
        }

        var foundUser = _userRepository.GetUserByConnectionId(joinToRoomModel.ConnectionId);

        if (foundUser == null)
        {
            foundUser = new User(Guid.NewGuid(), joinToRoomModel.ConnectionId, joinToRoomModel.UserName, room);

            _userRepository.AddUser(foundUser);
        }
        else
        {
            return StatusCode(500, $"User with connection id {joinToRoomModel.ConnectionId} was not found.");
        }

        _chatHubContext.Groups.AddToGroupAsync(joinToRoomModel.ConnectionId, joinToRoomModel.RoomName);
        var chatMessage = new ChatMessage("Admin", joinToRoomModel.RoomName, $"{joinToRoomModel.UserName} joined {joinToRoomModel.RoomName} room!");
        _chatHubContext.Clients.Group(joinToRoomModel.RoomName).ReceiveMessage(chatMessage);

        var roomUsersNames = _userRepository.GetUsersByRoomName(joinToRoomModel.RoomName).Select(u => u.Name)
            .ToList();

        _chatHubContext.Clients.Group(joinToRoomModel.RoomName).ReceiveRoomInfo(new RoomInfo
        {
            Name = joinToRoomModel.RoomName,
            UserNames = roomUsersNames
        });

        return (isRoomCreated) ? StatusCode(201) : Ok();
    }

    [HttpPost("room/leave")]
    public IActionResult LeaveFromRoom([FromBody] LeaveFromRoomModel leaveFromRoomModel)
    {
        var room = _roomRepository.GetRoomByName(leaveFromRoomModel.RoomName);
        if (room == null)
        {
            return BadRequest($"Room with name {leaveFromRoomModel.RoomName} does not exist.");
        }

        room.NumberOfParticipants--;

        var user = _userRepository.GetUserByConnectionId(leaveFromRoomModel.ConnectionId);

        if (user == null)
        {
            return BadRequest($"User with connection id {leaveFromRoomModel.ConnectionId} does not exist.");
        }

        _userRepository.RemoveUserByConnectionId(leaveFromRoomModel.ConnectionId);

        _chatHubContext.Groups.RemoveFromGroupAsync(leaveFromRoomModel.ConnectionId, leaveFromRoomModel.RoomName);
        var chatMessage = new ChatMessage("Admin", leaveFromRoomModel.RoomName, $"{leaveFromRoomModel.UserName} leaved {leaveFromRoomModel.RoomName} room!");
        _chatHubContext.Clients.Group(leaveFromRoomModel.RoomName).ReceiveMessage(chatMessage);

        var roomUsersNames = _userRepository.GetUsersByRoomName(leaveFromRoomModel.RoomName).Select(u => u.Name)
            .ToList();

        _chatHubContext.Clients.Group(leaveFromRoomModel.RoomName).ReceiveRoomInfo(new RoomInfo
        {
            Name = leaveFromRoomModel.RoomName,
            UserNames = roomUsersNames
        });

        return Ok();
    }
}
