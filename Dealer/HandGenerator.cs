using Dealer.Entities;
using Dealer.Enums;

namespace Dealer;

class HandGenerator
{
    private Card[] _deck = CreateDeck().ToArray();
    private readonly Random rng = new Random();

    public (Hand North, Hand East, Hand South, Hand West) CreateHands()
    {
        Shuffle();
        return (new Hand(_deck[0..13]), new Hand(_deck[13..26]), new Hand(_deck[26..39]), new Hand(_deck[39..52]));
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
        int n = _deck.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            (_deck[k], _deck[n]) = (_deck[n], _deck[k]);
        }
    }
}
