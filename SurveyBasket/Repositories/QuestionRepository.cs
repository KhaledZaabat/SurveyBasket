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


        public async Task<bool> ExistByIdAsync(int pollId, int questionId, CancellationToken token = default) => await db.Questions.AnyAsync(p => p.Id == questionId && p.PollId == pollId, token);
        public async Task<bool> ExistByIdAsyncIgnoredFilter(int pollId, int questionId, CancellationToken token = default) => await db.Questions.IgnoreQueryFilters().AnyAsync(p => p.Id == questionId && p.PollId == pollId, token);

        public async Task<bool> IsDuplicateQuestionAsync(string content, CancellationToken token = default)
        {
            return await db.Questions.AnyAsync(x => x.Content == content, token);
        }
        public async Task<ICollection<Question>> GetAllAsync(int poolId, CancellationToken token = default) => await db.Questions.Where(q => q.PollId == poolId).Include(p => p.Answers).AsNoTracking().ToListAsync(token);

        public async Task<bool> RestoreQuestion(int pollId, int questionId, CancellationToken token = default)
        {

            var question = await db.Questions
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(q => q.Id == questionId && q.PollId == pollId, token);

            if (question == null)
                return false;

            question.IsDeleted = false;
            question.DeletedBy = null;
            question.DeletedOn = null;

            await db.SaveChangesAsync(token);
            return true;
        }
        public async Task<bool> DeleteQuestionAsync(int pollId, int questionId, CancellationToken token = default)
        {

            Question? question = await db.Questions
                .FirstOrDefaultAsync(q => q.Id == questionId && q.PollId == pollId, token);

            if (question == null)
                return false;
            db.Remove(question);

            await db.SaveChangesAsync(token);
            return true;
        }
    }
}
