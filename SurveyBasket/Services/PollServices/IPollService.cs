namespace SurveyBasket.Services.PollServices;

public interface IPollService : IScopedService
{
    public ICollection<Poll> GetAll();
    public Poll GetById(int id);
    public Poll? Add(Poll poll);

    public bool Update(int id, Poll poll);
    public bool Delete(int id);
}
