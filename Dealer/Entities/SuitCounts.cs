using Dealer.Enums;

namespace Dealer.Entities;

public readonly record struct SuitCounts(byte Spades, byte Hearts, byte Diamonds, byte Clubs)
{
    public int this[Suit suit] => suit switch
    {
        Suit.Spades => Spades,
        Suit.Hearts => Hearts,
        Suit.Diamonds => Diamonds,
        Suit.Clubs => Clubs,
        _ => throw new ArgumentOutOfRangeException(),
    };

    public bool HasCount(int count, out Suit suit)
    {
        if (Spades == count)
        {
            suit = Suit.Spades;
            return true;
        }

        if (Hearts == count)
        {
            suit = Suit.Hearts;
            return true;
        }

        if (Diamonds == count)
        {
            suit = Suit.Diamonds;
            return true;
        }

        if (Clubs == count)
        {
            suit = Suit.Clubs;
            return true;
        }

        suit = default;
        return false;
    }

    public bool HasMinimumCount(int count, out Suit suit)
    {
        if (Spades >= count)
        {
            suit = Suit.Spades;
            return true;
        }

        if (Hearts >= count)
        {
            suit = Suit.Hearts;
            return true;
        }

        if (Diamonds >= count)
        {
            suit = Suit.Diamonds;
            return true;
        }

        if (Clubs >= count)
        {
            suit = Suit.Clubs;
            return true;
        }

        suit = default;
        return false;
    }
}