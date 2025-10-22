public class UserSubmissionConfiguration : IEntityTypeConfiguration<UserSubmission>
{
    public void Configure(EntityTypeBuilder<UserSubmission> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasIndex(x => new { x.SurveyId, x.UserId }).IsUnique();
        builder.HasOne(s => s.Survey)
            .WithMany()
            .HasForeignKey(s => s.SurveyId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_UserSubmissions_Surveys_SurveyId");

        builder.Property(x => x.DeletedOn).HasColumnType("timestamp with time zone");

        builder.HasOne(s => s.User)
            .WithMany(u => u.UserSubmissions)
            .HasForeignKey(s => s.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_UserSubmissions_Users_UserId");


        builder.HasQueryFilter(f => f.IsDeleted == false);

        builder.Property(s => s.SubmittedOn)
            .IsRequired()
            .HasColumnType("timestamp with time zone");
        builder.ToTable("UserSubmissions", tb =>
        {
            tb.HasTrigger("trg_UserSubmission_CascadeSoftDelete");
            tb.HasTrigger("trg_UserSubmission_CascadeRestore");
        });
        builder.ToTable("UserSubmissions");
    }
}
