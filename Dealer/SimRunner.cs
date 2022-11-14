using System.Diagnostics;

namespace Dealer;

internal static class SimRunner
{
    public static SimRunner<TData> Create<TData>(Action<HandGenerator, TData> func)
        where TData : ISimResult<TData>
    {
        return SimRunner<TData>.Create(func);
    }
}

internal class SimRunner<TData>
    where TData : ISimResult<TData>
{
    private readonly Action<HandGenerator, TData> _func;

    private SimRunner(Action<HandGenerator, TData> func) => _func = func;

    public static SimRunner<TData> Create(Action<HandGenerator, TData> func)
    {
        return new SimRunner<TData>(func);
    }

    public Task<TData> Run(int count, int threadCount = 6)
    {
        count /= threadCount;
        var results = new List<TData>(threadCount);

        var actions = Enumerable.Repeat(() =>
        {
            var result = TData.New();
            var handGenerator = new HandGenerator();
            {
                for (var runCycle = 0; runCycle < count; runCycle++)
                {
                    _func(handGenerator, result);
                }
            }
            results.Add(result);
        }, threadCount).ToArray();

        var sw = Stopwatch.StartNew();
        Parallel.Invoke(new ParallelOptions
        {
            MaxDegreeOfParallelism = threadCount,
        }, actions);
        sw.Stop();

        return Task.FromResult(TData.Merge(results, sw.ElapsedMilliseconds));
    }
}

public interface ISimResult<TSelf>
{
    public long ElapsedMilliseconds { get; }
    
    public static abstract TSelf New();
    public static abstract TSelf Merge(ICollection<TSelf> results, long elapsedMilliseconds);

    public void PrintResult();
}