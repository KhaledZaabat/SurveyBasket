namespace SurveyBasket.Repositories;

public class EFPollRepository : IPollRepository
{
    private readonly List<Poll> _polls = [

    new Poll { Id = 1 },
    new Poll { Id = 2 },
    new Poll { Id = 3 },
    new Poll { Id = 4, Description = "Test One" },
    new Poll { Id = 5 }

];

    public Poll? Add(Poll poll)
    {
        poll.Id = _polls.Count + 1;
        _polls.Add(poll);
        return poll;
    }

    public ICollection<Poll> GetAll() => _polls;

    public Poll? GetById(int id) => _polls?.FirstOrDefault(p => p.Id == id);

    public bool Update(int id, Poll poll)
    {
        Poll? foundPoll = _polls.FirstOrDefault(p => p.Id == id);
        if (foundPoll is null) return false;

        foundPoll.Title = poll.Title;
        foundPoll.Description = poll.Description;// simulate the ef core tracking
        return true;

    }

    bool IPollRepository.Delete(int id)
    {
        Poll? foundPoll = _polls.FirstOrDefault(p => p.Id == id);
        if (foundPoll is null) return false;
        return _polls.Remove(foundPoll);// simulate the ef core deleting

    }


}
