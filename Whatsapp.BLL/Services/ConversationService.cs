using Microsoft.EntityFrameworkCore;
using Whatsapp.BLL.DTOs;
using Whatsapp.BLL.DTOs.Conversations;
using Whatsapp.DAL;
using Whatsapp.DAL.data;
using Whatsapp.DAL.models;
using Whatsapp.DAL.Services;
namespace Whatsapp.BLL.Services;

public class ConversationService
{
    private readonly ConversationRepository _ConversationRepository;
    public ConversationService(ConversationRepository ConversationRepository)
    {
        this._ConversationRepository = ConversationRepository;
    }
    public async Task<List<UserConversationIdsDto>> GetUserConversationIdsAsync(string userId)
    {
        var result = 
            await _ConversationRepository.GetUserConversationsIdsAsync(userId);
        var userConversations = 
            result.Select(x => new UserConversationIdsDto()
        {
            ConversationId = x.ConversationId,
        }).ToList();
        return userConversations;
    }

    public async Task<List<ConversationResponse>> GetUserConversationsAsync(string userId)
    {
        var conversations =
            await _ConversationRepository.GetUserConversationsAsync(userId);

        var result = conversations.Select(x =>
        {
            var lastMessage = x.Messages
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefault();

            return new ConversationResponse
            {
                ConversationId = x.Id,
                ImageUrl = x.ImageUrl,
                LastMessage = lastMessage?.Content,
                LastMessageTime = lastMessage.SentAt
            };
        });

        return result.ToList();
    }
    public async Task<Conversation> CreateConversation(CreateConverstaionDto Dto)
    {
        // if the conversation is exist ?
        var conversation =
            await _ConversationRepository.IsConversationExist(Dto.UserMemberId,Dto.OtherUserId);
        
        if (conversation is not null)
        {
            return conversation;
        }

        Conversation NewConversation = new Conversation()
        {
            Id = Guid.NewGuid().ToString(),
            IsGroup = false
        };
        
        NewConversation.Members.Add(new ConversationMember()
        {
            UserId = Dto.UserMemberId,
            JoinedAt = DateTime.UtcNow,
        });
        
        NewConversation.Members.Add(new ConversationMember()
        {
            UserId = Dto.OtherUserId,
            JoinedAt = DateTime.UtcNow,
        });

        await _ConversationRepository.CreateConversation(NewConversation);
        
        return NewConversation;
    }

    public async Task<Conversation> CreateGroup(CreateGroupDto Dto)
    {
        Conversation NewConversation = new Conversation()
        {
            Id = Guid.NewGuid().ToString(),
            IsGroup = true,
            Name = Dto.GroupName,
        };
        
        NewConversation.Members.Add(new ConversationMember
        {
            UserId = Dto.MemberId
        });
        
        foreach (var userId in Dto.Members.Distinct())
        {
            
            NewConversation.Members.Add(new ConversationMember
            {
                UserId = userId
            });
        }
        
        await _ConversationRepository.CreateConversation(NewConversation);
        
        return NewConversation;
    }
}