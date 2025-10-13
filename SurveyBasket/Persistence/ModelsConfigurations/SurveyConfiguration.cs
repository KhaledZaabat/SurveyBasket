public class SurveyConfiguration : IEntityTypeConfiguration<Survey>
{
    public void Configure(EntityTypeBuilder<Survey> builder)
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
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Surveys_Users_CreatedById");

        builder.HasOne(p => p.UpdatedBy)
            .WithMany()
            .HasForeignKey(p => p.UpdatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Surveys_Users_UpdatedById");

        builder.HasOne(p => p.DeletedBy)
            .WithMany()
            .HasForeignKey(p => p.DeletedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Surveys_Users_DeletedById");

        builder.OwnsOne(o => o.Status, st =>
        {
            st.Property(s => s.IsPublished)
              .HasColumnName("IsPublished")
              .IsRequired();
        });

        builder.HasMany(p => p.SurveyQuestions)
            .WithOne(q => q.Survey)
            .HasForeignKey(q => q.SurveyId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Surveys_SurveyQuestions_SurveyId");

        builder.HasQueryFilter(b => b.IsDeleted == false);
        builder.ToTable("Surveys", tb =>
        {
            tb.HasTrigger("trg_Survey_CascadeSoftDelete");
            tb.HasTrigger("trg_Survey_CascadeRestore");
        });

    }
}
