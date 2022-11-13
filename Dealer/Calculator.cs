namespace Dealer;

static class Calculator
{
    public static Calculator<TData> Create<TData>(Action<HandGenerator, TData> func)
        where TData : new()
    {
        return Calculator<TData>.Create(func);
    }
}

class Calculator<TData>
    where TData : new()
{
    private readonly HandGenerator _deckSim = new();
    private readonly Action<HandGenerator, TData> _func;

    private Calculator(Action<HandGenerator, TData> func) => _func = func;

    public static Calculator<TData> Create(Action<HandGenerator, TData> func)
    {
        return new Calculator<TData>(func);
    }

    public TData Run(int count)
    {
        TData result = new();
        for (int i = 0; i < count; i++)
        {
            _func(_deckSim, result);
        }
        return result;
    }
}
