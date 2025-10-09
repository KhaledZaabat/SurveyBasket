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

        builder.HasOne(p => p.CreatedBy)
            .WithMany()
            .HasForeignKey(p => p.CreatedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.UpdatedBy)
             .WithMany()
             .HasForeignKey(p => p.UpdatedById)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);


    }
}
