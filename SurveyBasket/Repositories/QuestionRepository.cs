namespace SurveyBasket.Repositories
{
    public class QuestionRepository(AppDbContext db) : IQuestionRepository
    {
        public async Task<Question?> AddAsync(Question question, CancellationToken token = default)
        {
            await db.Questions.AddAsync(question, token);
            await db.SaveChangesAsync(token);
            return question;
        }

        public async Task<Question?> GetWithAnswersAsync(int id, CancellationToken token = default)
        {
            return await db.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id, token);
        }



        public async Task<bool> IsDuplicateQuestionAsync(string content, CancellationToken token = default)
        {
            return await db.Questions.AnyAsync(x => x.Content == content, token);
        }
    }
}
