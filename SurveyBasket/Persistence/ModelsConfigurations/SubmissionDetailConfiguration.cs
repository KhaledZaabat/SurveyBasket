public class SubmissionDetailConfiguration : IEntityTypeConfiguration<SubmissionDetail>
{
    public void Configure(EntityTypeBuilder<SubmissionDetail> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasIndex(x => new { x.UserSubmissionId, x.QuestionId }).IsUnique();
        builder.HasOne(d => d.Submission)
            .WithMany(s => s.SubmissionDetails)
            .HasForeignKey(d => d.UserSubmissionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_SubmissionDetails_UserSubmissions_UserSubmissionId");



        builder.HasOne(d => d.Option)
            .WithMany()
            .HasForeignKey(d => d.OptionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_SubmissionDetails_SurveyOptions_OptionId");// When we delete an option all 
        builder.HasQueryFilter(f => f.IsDeleted == false);
        builder.ToTable("SubmissionDetails");
    }
}