using Microsoft.EntityFrameworkCore;
using Whatsapp.DAL.data;
using Whatsapp.DAL.models;

namespace Whatsapp.DAL.Services;

public class MessageRepository
{
    private readonly ApplicationDbContext _dbContext;
    public MessageRepository( ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Message> SaveMessageAsync(Message message)
    {
        await _dbContext.Messages.AddAsync(message);
        await _dbContext.SaveChangesAsync();
        return message;
    }

    public async Task<List<Message>> GetMessagesAsync(string UserId,string ConversationId)
    {
        return await _dbContext
            .Messages
            .Where(m => m.SenderId == UserId && m.ConversationId == ConversationId)
            .OrderBy(c => c.SentAt).ToListAsync();
    }
}