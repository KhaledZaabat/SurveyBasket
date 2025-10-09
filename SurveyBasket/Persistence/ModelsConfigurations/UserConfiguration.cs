namespace SurveyBasket.Persistence.ModelsConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {

        builder.OwnsMany(p => p.RefreshTokens, tokens =>
        {

            tokens.ToTable("RefreshTokens");
            tokens.WithOwner().HasForeignKey("UserId");
            tokens.HasKey(t => t.Id);


        });
        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);
    }
}