using Dealer.Enums;
using System.Diagnostics;

namespace Dealer.Entities;

ref struct Hand
{
    public Span<Card> Cards;

    public Hand(Span<Card> cards)
    {
        Cards = cards;
        Debug.Assert(cards.Length == 13);

        var totalPoints = 0;

        byte spadeCount = 0;
        byte heartCount = 0;
        byte diamondCount = 0;
        byte clubCount = 0;

        for (var index = 0; index < Cards.Length; index++)
        {
            var c = Cards[index];
            totalPoints += c.Points;
            switch (c.Suit)
            {
                case Suit.Spades:
                    spadeCount++;
                    break;
                case Suit.Hearts:
                    heartCount++;
                    break;
                case Suit.Diamonds:
                    diamondCount++;
                    break;
                case Suit.Clubs:
                    clubCount++;
                    break;
            }
        }

        Points = totalPoints;
        SuitCounts = new SuitCounts(spadeCount, heartCount, diamondCount, clubCount);
    }

    public int Points { get; }

    public SuitCounts SuitCounts { get; }
}
