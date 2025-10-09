

namespace SurveyBasket.Persistence.ModelsConfigurations;

public class PollConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => x.Title).IsUnique();

        builder.Property(x => x.Title).HasMaxLength(100);
        builder.Property(x => x.Summary).HasMaxLength(1500);
    }
}
