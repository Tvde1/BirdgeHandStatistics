namespace Dealer;

static class Calculator
{
    public static Calculator<TData> Create<TData>(Action<HandGenerator, TData> func)
        where TData : ISimResult<TData>
    {
        return Calculator<TData>.Create(func);
    }
}

class Calculator<TData>
    where TData : ISimResult<TData>
{
    private readonly Action<HandGenerator, TData> _func;

    private Calculator(Action<HandGenerator, TData> func) => _func = func;

    public static Calculator<TData> Create(Action<HandGenerator, TData> func)
    {
        return new Calculator<TData>(func);
    }

    public TData Run(int count, int threadCount = 5)
    {
        count /= threadCount;
        var results = new TData[threadCount];

        Parallel.For(0, threadCount, threadNum =>
        {
            var handGenerator = new HandGenerator();

            TData result = TData.New();
            for (int i = 0; i < count; i++)
            {
                _func(handGenerator, result);
            }
            results[threadNum] = result;
        });

        return TData.Merge(results);
    }
}

public interface ISimResult<TSelf>
{
    public static abstract TSelf New();
    public static abstract TSelf Merge(TSelf[] results);
}