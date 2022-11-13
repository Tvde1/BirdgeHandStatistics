using Dealer.Enums;

namespace Dealer.Entities;

public readonly record struct Card(Suit Suit, Value Value)
{
    public int Points => Value switch
    {
        Value.Jack => 1,
        Value.Queen => 2,
        Value.King => 3,
        Value.Ace => 4,
        _ => 0,
    };
}
