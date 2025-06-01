using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechGalaxyProject.Data.Models;

namespace TechGalaxyProject.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<CompletedFields> completedFields { get; set; }
        public DbSet<ExpertVerificationRequest> ExpertVerificationRequests { get; set; }
        public DbSet<Field> fields { get; set; }
        public DbSet<Roadmap> roadmaps { get; set; }
        public DbSet<FollowedRoadmap> FollowedRoadmaps { get; set; }
        public DbSet<FieldResource> fieldResources { get; set; }
        public DbSet<RoadmapLike> RoadmapLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* modelBuilder.Entity<ExpertVerificationRequest>()
                 .HasOne(e => e.Expert)
                 .WithMany()
                 .HasForeignKey(e => e.UserId)
                 .OnDelete(DeleteBehavior.Restrict); */

           


            modelBuilder.Entity<ExpertVerificationRequest>()
    .HasOne(e => e.Expert)
    .WithOne(u => u.request)
    .HasForeignKey<ExpertVerificationRequest>(e => e.UserId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExpertVerificationRequest>()
                .HasOne(e => e.Admin)
                .WithMany(f => f.requests)
                .HasForeignKey(e => e.ReviewedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FollowedRoadmap>()
        .HasOne(fr => fr.Roadmap)
        .WithMany(r => r.followedBy)
        .HasForeignKey(fr => fr.RoadmapId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CompletedFields>()
    .HasOne(cf => cf.field)
    .WithMany(f => f.completedFields)
    .HasForeignKey(cf => cf.FieldId)
    .OnDelete(DeleteBehavior.Restrict); // أو DeleteBehavior.NoAction

            modelBuilder.Entity<RoadmapLike>()
    .HasOne(rl => rl.Roadmap)
    .WithMany(r => r.Likes)
    .HasForeignKey(rl => rl.RoadmapId)
    .OnDelete(DeleteBehavior.Restrict);
         
            
            
            modelBuilder.Entity<RoadmapLike>()
                .HasOne(rl => rl.User)
                .WithMany(u => u.likedRoadmaps)
                .HasForeignKey(rl => rl.UserId)
                .OnDelete(DeleteBehavior.Cascade);



        }

    }
}