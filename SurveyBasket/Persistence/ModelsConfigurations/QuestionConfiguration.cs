public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasIndex(x => new { x.PollId, x.Content }).IsUnique();

        builder.Property(x => x.Content).HasMaxLength(1000);

        builder.HasOne(q => q.CreatedBy)
            .WithMany()
            .HasForeignKey(q => q.CreatedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Questions_Users_CreatedById");

        builder.HasOne(q => q.UpdatedBy)
            .WithMany()
            .HasForeignKey(q => q.UpdatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Questions_Users_UpdatedById");

        builder.HasOne(q => q.DeletedBy)
            .WithMany()
            .HasForeignKey(q => q.DeletedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Questions_Users_DeletedById");

        builder.HasOne(q => q.Poll)
            .WithMany(p => p.Questions)
            .HasForeignKey(q => q.PollId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Questions_Polls_PollId");

        builder.HasMany(q => q.Answers)
            .WithOne(a => a.Question)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Questions_Answers_QuestionId");

        builder.HasQueryFilter(f => f.IsDeleted == false);

        builder.ToTable("Questions", tb =>
        {
            tb.HasTrigger("trg_Question_CascadeSoftDelete");
            tb.HasTrigger("trg_Question_CascadeRestore");
        });
    }
}
