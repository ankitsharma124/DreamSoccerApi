using DreamSoccer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DreamSoccer.Repository.Context
{
    public class TransferListConfiguration :
        IEntityTypeConfiguration<TransferList>
    {
        public void Configure(EntityTypeBuilder<TransferList> builder)
        {
            builder.HasKey(c => c.Id);
            builder.ToTable("TransferList");
            var storeFK = builder.HasOne<Player>(c => c.Player)
               .WithOne(c => c.TransferList)
               .HasForeignKey<Player>(c => c.TransferListId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.HasKey(c => c.Id);
            builder.ToTable("Players");
            var storeFK = builder.HasOne<Team>(c => c.Team)
               .WithMany(c => c.Players)
               .HasForeignKey(c => c.TeamId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}