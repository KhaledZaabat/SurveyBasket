namespace SurveyBasket.Persistence.ModelsConfigurations;

using SurveyBasket.Domain.Entities;
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


    }
}
