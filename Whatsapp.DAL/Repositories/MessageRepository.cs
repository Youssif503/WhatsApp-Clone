using Microsoft.EntityFrameworkCore;
using Whatsapp.DAL.data;
using Whatsapp.DAL.Helpers;
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

    public async Task<List<Message>> GetMessagesAsync( 
        string ConversationId,CursorPaginationRequest request)
    {
        
        var query = _dbContext.Messages.Where(m =>
                m.ConversationId == ConversationId);
        
        if (request.Cursor.HasValue)
        {
            query = query.Where(m => m.SentAt < request.Cursor.Value);
        }
        
        
        var messages =  await query
            .OrderByDescending(m => m.SentAt)
            .ThenByDescending(m=>m.Id)
            .Take((request.Limit ?? 20) +1)
            .ToListAsync();
        
        return messages;
    }
}