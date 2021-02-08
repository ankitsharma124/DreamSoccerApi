using DreamSoccer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamSoccer.Repository.Context
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasKey(c => c.Id);
            builder.ToTable("Teams");
            builder.HasOne<User>(c => c.Owner)
               .WithOne(c => c.Team)
               .HasForeignKey<User>(c => c.TeamId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}