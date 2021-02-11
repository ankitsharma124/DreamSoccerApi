using DreamSoccer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamSoccer.Repository.Context
{
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.HasKey(c => c.Id);
            builder.ToTable("Players");
            builder.HasOne<Team>(c => c.Team)
               .WithMany(c => c.Players)
               .HasForeignKey(c => c.TeamId)
               .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(n => n.TransferListId).IsUnique(false);

        }
    }
}