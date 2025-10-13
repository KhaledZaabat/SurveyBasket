public class SurveyOptionConfiguration : IEntityTypeConfiguration<SurveyOption>
{
    public void Configure(EntityTypeBuilder<SurveyOption> builder)
    {
        builder.HasIndex(x => new { x.SurveyQuestionId, x.Content }).IsUnique();

        builder.Property(x => x.Content).HasMaxLength(1000);

        builder.HasOne(a => a.CreatedBy)
            .WithMany()
            .HasForeignKey(a => a.CreatedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_SurveyOptions_Users_CreatedById");

        builder.HasOne(a => a.UpdatedBy)
            .WithMany()
            .HasForeignKey(a => a.UpdatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_SurveyOptions_Users_UpdatedById");

        builder.HasOne(a => a.DeletedBy)
            .WithMany()
            .HasForeignKey(a => a.DeletedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_SurveyOptions_Users_DeletedById");

        builder.HasOne(a => a.SurveyQuestion)
            .WithMany(q => q.SurveyOptions)
            .HasForeignKey(a => a.SurveyQuestionId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_SurveyOptions_SurveyQuestions_SurveyQuestionId");

        builder.HasQueryFilter(f => f.IsDeleted == false);
        builder.ToTable("SurveyOptions", tb =>
        {
            tb.HasTrigger("trg_SurveyOption_CascadeSoftDelete");
            tb.HasTrigger("trg_SurveyOption_CascadeRestore");
        });
    }
}
