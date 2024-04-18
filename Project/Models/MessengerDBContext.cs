using Microsoft.EntityFrameworkCore;

public class MessengerDBContext : DbContext
{
    public MessengerDBContext(DbContextOptions<MessengerDBContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Interaction>().HasKey(nameof(Interaction.User1Id), nameof(Interaction.User2Id));
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Interaction> Interactions { get; set; }
    public DbSet<Message> Chats { get; set; }

}
