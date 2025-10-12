public class UserSubmissionConfiguration : IEntityTypeConfiguration<UserSubmission>
{
    public void Configure(EntityTypeBuilder<UserSubmission> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();


        builder.HasOne(s => s.Survey)
            .WithMany()
            .HasForeignKey(s => s.SurveyId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_UserSubmissions_Surveys_SurveyId");


        builder.HasOne(s => s.User)
            .WithMany(u => u.UserSubmissions)
            .HasForeignKey(s => s.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_UserSubmissions_Users_UserId");


        builder.HasMany(s => s.SubmissionDetails)
            .WithOne(d => d.Submission)
            .HasForeignKey(d => d.VoteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_UserSubmissions_SubmissionDetails_VoteId");

        builder.Property(s => s.SubmittedOn)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.ToTable("UserSubmissions");
    }
}
