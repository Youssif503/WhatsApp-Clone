using Microsoft.EntityFrameworkCore;
using Whatsapp.DAL.data;
using Whatsapp.DAL.models;

namespace Whatsapp.DAL.Services;

public class ConversationRepository
{
    private readonly ApplicationDbContext _dbContext;
    public async Task<List<ConversationMember>> GetUserConversationsIdsAsync(string userId)
    {
        return await _dbContext.ConversationMembers
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Conversation>> GetUserConversationsAsync(string userId)
    {
        return await _dbContext.Conversations
            .AsNoTracking()
            .Include(c => c.Messages)
            .Where(c => c.Members.Any(x => x.UserId == userId))
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