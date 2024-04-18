using Microsoft.EntityFrameworkCore;


public class MessengerDBContext : DbContext
{
    public MessengerDBContext(DbContextOptions<MessengerDBContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Interaction>().HasKey(e => new { e.User1Id, e.User2Id });
        modelBuilder.Entity<Interaction>().HasQueryFilter(filter => filter.User1Id < filter.User2Id);
        modelBuilder.Entity<User>().HasIndex(i => i.Email).IsUnique();
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Interaction> Interactions { get; set; }
    public DbSet<Message> Chats { get; set; }

}
