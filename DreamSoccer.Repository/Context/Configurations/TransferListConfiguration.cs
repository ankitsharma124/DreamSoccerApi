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
            builder.HasOne<Player>(c => c.Player)
               .WithOne(c => c.TransferList)
               .HasForeignKey<TransferList>(c => c.Id);
            builder.HasOne(c => c.Player)
               .WithOne(c => c.TransferList)
               .HasForeignKey<TransferList>(c => c.PlayerId);
            builder.HasIndex(n => n.PlayerId).IsUnique(false);
        }
    }
}