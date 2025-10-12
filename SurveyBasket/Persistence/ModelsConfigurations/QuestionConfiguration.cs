public class SurveyQuestionConfiguration : IEntityTypeConfiguration<SurveyQuestion>
{
    public void Configure(EntityTypeBuilder<SurveyQuestion> builder)
    {
        builder.HasIndex(x => new { x.SurveyId, x.Content }).IsUnique();

        builder.Property(x => x.Content).HasMaxLength(1000);

        builder.HasOne(q => q.CreatedBy)
            .WithMany()
            .HasForeignKey(q => q.CreatedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_SurveyQuestions_Users_CreatedById");

        builder.HasOne(q => q.UpdatedBy)
            .WithMany()
            .HasForeignKey(q => q.UpdatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_SurveyQuestions_Users_UpdatedById");

        builder.HasOne(q => q.DeletedBy)
            .WithMany()
            .HasForeignKey(q => q.DeletedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_SurveyQuestions_Users_DeletedById");

        builder.HasOne(q => q.Survey)
            .WithMany(p => p.SurveyQuestions)
            .HasForeignKey(q => q.SurveyId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_SurveyQuestions_Surveys_SurveyId");

        builder.HasMany(q => q.SurveyOptions)
            .WithOne(a => a.SurveyQuestion)
            .HasForeignKey(a => a.SurveyQuestionId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_SurveyQuestions_SurveyOptions_SurveyQuestionId");

        builder.HasQueryFilter(f => f.IsDeleted == false);

        builder.ToTable("SurveyQuestions", tb =>
        {
            tb.HasTrigger("trg_SurveyQuestion_CascadeSoftDelete");
            tb.HasTrigger("trg_SurveyQuestion_CascadeRestore");
        });
    }
}
