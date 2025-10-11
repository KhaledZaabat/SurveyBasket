public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasIndex(x => new { x.QuestionId, x.Content }).IsUnique();

        builder.Property(x => x.Content).HasMaxLength(1000);

        builder.HasOne(a => a.CreatedBy)
            .WithMany()
            .HasForeignKey(a => a.CreatedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Answers_Users_CreatedById");

        builder.HasOne(a => a.UpdatedBy)
            .WithMany()
            .HasForeignKey(a => a.UpdatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Answers_Users_UpdatedById");

        builder.HasOne(a => a.DeletedBy)
            .WithMany()
            .HasForeignKey(a => a.DeletedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Answers_Users_DeletedById");

        builder.HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Answers_Questions_QuestionId");

        builder.HasQueryFilter(f => f.IsDeleted == false);
    }
}
