namespace SurveyBasket.Services.PollServices;

public class PollService(IPollRepository _pollRepository) : IPollService
{


    public ICollection<Poll> GetAll()
    {
        return _pollRepository.GetAll();
    }

    public Poll? GetById(int id)
    {
        return _pollRepository?.GetById(id);
    }
    public Poll? Add(Poll poll)
    {
        return _pollRepository?.Add(poll);
    }

    public bool Update(int id, Poll poll)
    {
        return _pollRepository.Update(id, poll);
    }
    public bool Delete(int id)
    {
        return _pollRepository.Delete(id);
    }
}

