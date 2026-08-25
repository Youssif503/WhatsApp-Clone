using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Whatsapp.API.Helpers;
using Whatsapp.BLL.DTOs.Hubs;
using Whatsapp.BLL.DTOs.Messages;
using Whatsapp.BLL.Services;
using Whatsapp.DAL.data;
using Whatsapp.DAL.models;

namespace Whatsapp.API.Hubs;

public class ChatHUb : Hub
{
    private readonly ILogger<ChatHUb> _logger;
    public static readonly ConcurrentDictionary<string, OnlineUser> _onlineUsers = new();
    private readonly ConversationService _conversationService;
    private readonly MessageService _messageService;
    private readonly UserManager<User> _userManager;

    public ChatHUb(ILogger<ChatHUb> logger,
        ConversationService conversationService,
        MessageService messageService,
        UserManager<User> userManager)
    {
        _logger = logger;
        _conversationService = conversationService;
        _messageService = messageService;
        _userManager = userManager;
    }


    public async Task SendMessageAsync(CreateMessageDto Message)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            throw new HubException("User is not authenticated User.");

        // Always use the authenticated sender, never an ID supplied by a client.
        // Recipients need this real ID to place incoming messages on the left.
        Message.SenderId = userId;

        // save message to db
        var message =
            await _messageService.SendMessageAsync(Message);

        await Clients
            .Group(message.ConversationId!)
            .SendAsync(
                "ReceiveMessage",
                message
            );
    }

    public async Task NotifyTyping(string conversationId)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            throw new HubException("User is not authenticated User.");
        
        await Clients
            .OthersInGroup(conversationId)
            .SendAsync("NotifyTyping", userId);
    }

    public async Task StopedTyping(string conversationId)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            throw new HubException("User is not authenticated User.");

        await Clients.OthersInGroup(conversationId)
            .SendAsync("UserStoppedTyping", userId);
    }

    public async override Task OnConnectedAsync()
    {
        var userId = Context.User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (userId == null)
            return;

        var connectionId = Context.ConnectionId;

        var currentUser = await _userManager.FindByIdAsync(userId);

        if (currentUser == null)
            return;

        if (_onlineUsers.TryGetValue(userId, out var onlineUser))
        {
            onlineUser.ConnectionId = connectionId;
        }
        else
        {
            _onlineUsers.TryAdd(
                userId,
                new OnlineUser
                {
                    UserId = userId,
                    ConnectionId = connectionId,
                    ImageUrl = currentUser.ImageUrl
                }
            );
        }

        var conversations =
            await _conversationService
                .GetUserConversationIdsAsync(userId);

        foreach (var conversation in conversations)
        {
            await Groups.AddToGroupAsync(
                connectionId,
                conversation.ConversationId.ToString()
            );
        }

        await Clients.All.SendAsync(
            "Notify",
            await GetAllUsers()
        );

        await base.OnConnectedAsync();
    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (userId != null &&
            _onlineUsers.TryGetValue(userId, out var onlineUser))
        {
            if (onlineUser.ConnectionId == Context.ConnectionId)
            {
                _onlineUsers.TryRemove(userId, out _);

                await Clients.All.SendAsync(
                    "Notify",
                    await GetAllUsers()
                );
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task<IEnumerable<OnlineUser>> GetAllUsers()
    {
        var onlineUserIds =
            new HashSet<string>(_onlineUsers.Keys);

        var users = await _userManager.Users
            .Select(x => new OnlineUser
            {
                UserId = x.Id,
                Name = x.UserName,
                ImageUrl = x.ImageUrl,
                isOnline = onlineUserIds.Contains(x.Id)
            })
            .OrderByDescending(x => x.isOnline)
            .ToListAsync();

        return users;
    }
}
