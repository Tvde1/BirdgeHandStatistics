using Dealer.Entities;
using Dealer.Enums;

namespace Dealer;

internal class HandGenerator
{
    private readonly Card[] _deck = CreateDeck().ToArray();
    private readonly Random _rng = new();

    public Deck ShuffleNewDeck()
    {
        Shuffle();
        return new Deck(new Hand(_deck.AsSpan()[0..13]), 
            new Hand(_deck.AsSpan()[13..26]),
            new Hand(_deck.AsSpan()[26..39]),
            new Hand(_deck.AsSpan()[39..52]));
    }

    private static IEnumerable<Card> CreateDeck()
    {
        var suits = Enum.GetValues<Suit>();
        var values = Enum.GetValues<Value>();

        foreach (var suit in suits)
            foreach (var @value in values)
                yield return new Card(suit, @value);
    }

    private void Shuffle()
    {
        var n = _deck.Length;
        while (n > 1)
        {
            var k = _rng.Next(n--);
            (_deck[k], _deck[n]) = (_deck[n], _deck[k]);
        }
    }
}

internal ref struct Deck
{
    public Hand North { get; }
    public Hand East { get; }
    public Hand South { get; }
    public Hand West { get; }

    public Deck(Hand north, Hand east, Hand south, Hand west)
    {
        North = north;
        East = east;
        South = south;
        West = west;
    }
}