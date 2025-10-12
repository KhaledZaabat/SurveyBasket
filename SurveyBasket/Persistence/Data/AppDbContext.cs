namespace SurveyBasket.Persistence.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public bool DisableAuditing { get; set; } = false;
    public bool DisableSoftDeletion { get; set; } = false;
    public DbSet<SurveyOption> SurveyOptions { get; set; }
    public DbSet<Survey> Surveys { get; set; }
    public DbSet<SurveyQuestion> SurveyQuestions { get; set; }

    public DbSet<UserSubmission> UserSubmissions { get; set; }
    public DbSet<SubmissionDetail> SubmissionDetails { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

}
