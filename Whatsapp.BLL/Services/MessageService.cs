using Whatsapp.BLL.DTOs;
using Whatsapp.BLL.DTOs.Messages;
using Whatsapp.DAL.models;
using Whatsapp.DAL.Services;

namespace Whatsapp.BLL.Services;

public class MessageService
{
    private readonly MessageRepository _messageRepository;
    private readonly ConversationRepository _conversationRepository;
    public MessageService(MessageRepository messageRepository,
        ConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

    public async Task<MessageResponseDTo> SendMessageAsync(CreateMessageDto MessageDto)
    {
        // need to know if this user in conversation ?
        bool result = 
            await _conversationRepository.IsInConversation(
                MessageDto.SenderId, MessageDto.ConversationId);
        
        if (!result)
            throw new UnauthorizedAccessException("Usre Nott In Group");

        var message = new Message()
        {
            ConversationId = MessageDto.ConversationId,
            SenderId = MessageDto.SenderId,
            SentAt = MessageDto.CreatedAt,
            Content = MessageDto.Content
        };
        
        _messageRepository.SaveMessageAsync(message);
        
        return new MessageResponseDTo()
        {
            ConversationId = MessageDto.ConversationId,
            SenderId = MessageDto.SenderId,
            CreatedAt = MessageDto.CreatedAt,
            Content = MessageDto.Content
        };
    }

    public async Task<List<MessageResponseDTo>> GetMessagesAsync(string userId, string conversationId)
    {
        bool isMember = 
            await _conversationRepository.IsInConversation(userId, conversationId);
        if (!isMember)
            throw new UnauthorizedAccessException("Usre Nott In Group");
        
        var result = await _messageRepository.
            GetMessagesAsync(userId,conversationId);

        return result.Select(x => new MessageResponseDTo()
        {
            ConversationId = x.ConversationId,
            SenderId = x.SenderId,
            CreatedAt = x.SentAt,
            Content = x.Content
        }).ToList();
    }
}