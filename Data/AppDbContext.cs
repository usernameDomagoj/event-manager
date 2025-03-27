using EventManager.Entities;
using EventManager.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace EventManager.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.CreatedBy);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Participants)
                .WithMany(u => u.Events)
                .UsingEntity(
                    "UserEvent",
                    entity => entity
                        .HasOne(typeof(User))
                        .WithMany()
                        .HasForeignKey("UserId"),
                    entity => entity
                        .HasOne(typeof(Event))
                        .WithMany()
                        .HasForeignKey("EventId"),
                    entity => entity
                        .HasKey("UserId", "EventId"));


            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin",
                    Username = "admin",
                    Email = "admin@admin",
                    Status = UserStatus.Approved,
                    Role = UserRole.Admin,
                    CreatedDate = new DateTime(2000, 5, 20)
                }
             );

            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Title = "Event u Osijeku",
                    Description = "Lorem ipsum dolor sit amet.",
                    Date = new DateTime(2025, 5, 20),
                    Location = "Osijek",
                    CreatedDate = new DateTime(2025, 1, 5),
                    CreatedById = 1
                },
                new Event
                {
                    Id = 2,
                    Title = "Event u Rijeci",
                    Description = "Lorem ipsum dolor sit amet.",
                    Location = "Rijeka",
                    Date = new DateTime(2025, 6, 11),
                    CreatedDate = new DateTime(2025, 1, 5),
                    CreatedById = 1
                },
                new Event
                {
                    Id = 3,
                    Title = "Event u Zagrebu",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam maximus libero.",
                    Location = "Zagreb",
                    Date = new DateTime(2025, 6, 17),
                    CreatedDate = new DateTime(2025, 1, 5),
                    CreatedById = 1
                },
                new Event
                {
                    Id = 4,
                    Title = "Event u Sisku",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam maximus libero.",
                    Location = "Sisak",
                    Date = new DateTime(2025, 7, 31),
                    CreatedDate = new DateTime(2025, 1, 5),
                    CreatedById = 1
                },
                new Event
                {
                    Id = 5,
                    Title = "Event u Karlovcu",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam ultricies, quam quis lacinia congue, lorem velit rutrum enim, non congue.",
                    Location = "Karlovac",
                    Date = new DateTime(2025, 9, 15),
                    CreatedDate = new DateTime(2025, 1, 5),
                    CreatedById = 1
                },
                new Event
                {
                    Id = 6,
                    Title = "Event u Zadru",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam ultricies, quam quis lacinia congue, lorem velit rutrum enim, non congue.",
                    Location = "Zadar",
                    Date = new DateTime(2025, 10, 18),
                    CreatedDate = new DateTime(2025, 1, 5),
                    CreatedById = 1
                },
                new Event
                {
                    Id = 7,
                    Title = "Event u Puli",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam ultricies, quam quis lacinia congue, lorem velit rutrum enim, non congue.",
                    Location = "Pula",
                    Date = new DateTime(2025, 11, 22),
                    CreatedDate = new DateTime(2025, 1, 5),
                    CreatedById = 1
                }
             );
        }
    }
}
