using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Whatsapp.DAL.data.sead;
using Whatsapp.DAL.models;

namespace Whatsapp.DAL.data;

public class ApplicationDbContext:IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        :base(options)
    {
        
    }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ConversationMember> ConversationMembers { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
            
        modelBuilder.Entity<IdentityRole>()
            .HasData(SeadData.LoadRoles());
            
        modelBuilder.Entity<ConversationMember>()
            .HasKey(x => new
            {
                x.ConversationId,
                x.UserId
            });
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}