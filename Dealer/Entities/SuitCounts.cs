using Dealer.Enums;

namespace Dealer.Entities;

public readonly record struct SuitCounts
{
    public SuitCounts(byte spades, byte hearts, byte diamonds, byte clubs)
    {
        Spades = spades;
        Hearts = hearts;
        Diamonds = diamonds;
        Clubs = clubs;
    }

    public byte Spades { get; }
    public byte Hearts { get; }
    public byte Diamonds { get; }
    public byte Clubs { get; }

    public void Deconstruct(out byte Longest, out byte SecondLongest, out byte SecondShortest, out byte Shortest)
    {
        Span<byte> values = stackalloc byte[] { Spades, Hearts, Diamonds, Clubs };
        values.Sort();
        Shortest = values[0];
        SecondShortest = values[1];
        SecondLongest = values[2];
        Longest = values[3];
    }

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

    internal (byte, byte, byte, byte) ToTuple()
    {
        Deconstruct(out var a, out var b, out var c, out var d);
        return (a, b, c, d);
    }
}