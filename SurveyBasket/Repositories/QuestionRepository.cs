namespace SurveyBasket.Repositories
{
    public class QuestionRepository(AppDbContext db) : IQuestionRepository
    {
        public async Task<Question?> AddAsync(Question question)
        {
            await db.Questions.AddAsync(question);
            await db.SaveChangesAsync();
            return question;
        }

        public async Task<Question?> GetWithAnswersAsync(int id)
        {
            return await db.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);
        }



        public async Task<bool> IsDuplicateQuestionAsync(string content)
        {
            return await db.Questions.AnyAsync(x => x.Content == content);
        }
    }
}
