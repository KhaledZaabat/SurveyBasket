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

        public async Task<Question?> GetWithAnswersAsync(int pollId, int questionId, CancellationToken token = default)
        {
            return await db.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => (q.Id == questionId) && (q.PollId == pollId), token);
        }



        public async Task<bool> IsDuplicateQuestionAsync(string content, CancellationToken token = default)
        {
            return await db.Questions.AnyAsync(x => x.Content == content, token);
        }
        public async Task<ICollection<Question>> GetAllAsync(int poolId, CancellationToken token = default) => await db.Questions.Where(q => q.PollId == poolId).Include(p => p.Answers).AsNoTracking().ToListAsync(token);


    }
}
