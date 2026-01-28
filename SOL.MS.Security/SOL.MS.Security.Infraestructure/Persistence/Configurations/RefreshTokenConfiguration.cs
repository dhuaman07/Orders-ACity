using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SOL.MS.Security.Domain.Entities;

namespace SOL.MS.Security.Infraestructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Id)
                .ValueGeneratedNever();

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(rt => rt.ExpiresAt)
                .IsRequired();

            builder.Property(rt => rt.CreatedAt)
                .IsRequired();

            builder.Property(rt => rt.CreatedByIp)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(rt => rt.RevokedByIp)
                .HasMaxLength(50);

            builder.Property(rt => rt.ReplacedByToken)
                .HasMaxLength(500);

           
            builder.HasIndex(rt => rt.Token)
                .IsUnique()
                .HasDatabaseName("IX_RefreshTokens_Token");

            builder.HasIndex(rt => rt.UserId)
                .HasDatabaseName("IX_RefreshTokens_UserId");

          
            builder.Ignore(rt => rt.IsExpired);
            builder.Ignore(rt => rt.IsRevoked);
            builder.Ignore(rt => rt.IsActive);
        }
    }
}
