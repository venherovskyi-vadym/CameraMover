public class CompositeStrategy : ICameraStrategy
{
    private readonly ICameraStrategy[] _strategies;

    public CompositeStrategy(ICameraStrategy[] strategies)
    {
        _strategies = strategies;
    }

    public void Init(float time)
    {
        for (int i = 0; i < _strategies.Length; i++)
        {
            _strategies[i].Init(time);
        }
    }

    public void Update(float time, float timeDelta)
    {
        for (int i = 0; i < _strategies.Length; i++)
        {
            _strategies[i].Update(time, timeDelta);
        }
    }

    public void Finish()
    {
        for (int i = 0; i < _strategies.Length; i++)
        {
            _strategies[i].Finish();
        }
    }
}