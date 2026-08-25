using Microsoft.EntityFrameworkCore;
using Whatsapp.DAL.data;
using Whatsapp.DAL.Helpers;
using Whatsapp.DAL.models;

namespace Whatsapp.DAL.Services;

public class ConversationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ConversationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ConversationMember>> GetUserConversationsIdsAsync(string userId)
    {
        return await _dbContext.ConversationMembers
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<ConversationResponse>> GetUserConversationsAsync(
        string userId)
    {
        return await _dbContext.Conversations
            .AsNoTracking()
            .Where(c => c.Members.Any(x => x.UserId == userId))
            .Select(c => new ConversationResponse
            {
                ConversationId = c.Id,
                ImageUrl = c.IsGroup 
                    ? c.ImageUrl
                    :c.Members.Where(m => m.UserId != userId)
                        .Select(m => m.User.ImageUrl)
                        .FirstOrDefault(),

                LastMessage = c.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => m.Content)
                    .FirstOrDefault(),

                LastMessageTime = c.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => (DateTime?)m.SentAt)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }
    
    public async Task<bool> IsInConversation(string conversationId, string userId)
    {
         var result =  await 
             _dbContext.ConversationMembers.AnyAsync 
            (x => x.UserId == userId 
                  && x.ConversationId.ToString() == conversationId);
         return result;
    }
    public async Task CreateConversation(Conversation conv)
    {
        await _dbContext.Conversations.AddAsync(conv);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<Conversation?> IsConversationExist(string MemberId, string OtherMemberId)
    {
        return await _dbContext.Conversations
            .Where(c => c.Members.Count == 2)
            .Where(c => c.Members.Any(x => x.UserId == MemberId))
            .Where(c => c.Members.Any(x => x.UserId == OtherMemberId))
            .FirstOrDefaultAsync();
    }
}