public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Articles> Articles { get; set; }
    public DbSet<Purchases> Purchases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.Email).IsRequired().HasColumnType("varchar(255)");
            entity.Property(u => u.UserName).HasColumnType("varchar(100)");
            entity.Property(u => u.RegistrationDate).HasColumnType("datetime");
            entity.Property(u => u.LastLogin).HasColumnType("datetime").IsRequired(false);
            entity.Property(u => u.IsPremium).HasColumnType("bit").HasDefaultValue(false);
        });

        modelBuilder.Entity<Articles>(entity =>
        {
            entity.HasKey(a => a.ArticleID);
            entity.Property(a => a.Title).IsRequired().HasColumnType("varchar(255)");
            entity.Property(a => a.IsPremium).HasColumnType("bit");
        });

        modelBuilder.Entity<Purchases>(entity =>
        {
            entity.HasKey(p => p.PurchaseID);
            entity.Property(p => p.PurchaseDate).HasColumnType("datetime");
            entity.Property(p => p.Amount).HasColumnType("decimal(10,2)");

            entity.HasOne(p => p.ApplicationUser)
                  .WithMany(m => m.Purchases)
                  .HasForeignKey(p => p.ApplicationUserId)
                  .IsRequired();
        });
    }
}
