public class SubmissionDetailConfiguration : IEntityTypeConfiguration<SubmissionDetail>
{
    public void Configure(EntityTypeBuilder<SubmissionDetail> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();


        builder.HasOne(d => d.Submission)
            .WithMany(s => s.SubmissionDetails)
            .HasForeignKey(d => d.VoteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_SubmissionDetails_UserSubmissions_VoteId");


        builder.HasOne(d => d.Question)
            .WithMany()
            .HasForeignKey(d => d.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_SubmissionDetails_SurveyQuestions_QuestionId");


        builder.HasOne(d => d.Option)
            .WithMany()
            .HasForeignKey(d => d.AnswerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_SubmissionDetails_SurveyOptions_AnswerId");

        builder.ToTable("SubmissionDetails");
    }
}