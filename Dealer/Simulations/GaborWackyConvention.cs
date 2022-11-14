using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dealer.Simulations;

internal class GaborWackyConvention
{
    public static void Run()
    {
        var sw = Stopwatch.StartNew();

        var calc = Calculator.Create((HandGenerator deckSim, SimResult simData) =>
        {
            simData.TotalHands++;
            var (north, _, south, _) = deckSim.CreateHands();

            var hasEnoughPoints = south.Points is >= 4 and <= 11;

            if (!hasEnoughPoints)
            {
                return;
            }

            var hasSixcard = south.SuitCounts.HasCount(6, out var sixCardSuit);

            if (!hasSixcard)
            {
                return;
            }

            var hasFourCard = south.SuitCounts.HasCount(4, out var fourCardSuit);
            if (!hasFourCard)
            {
                return;
            }

            simData.HandsPreempted++;

            if (north.SuitCounts[fourCardSuit] == 1)
            {
            }

            switch (north.SuitCounts[fourCardSuit])
            {
                case 0:
                    simData.HandsWithPartnerVoid++;
                    break;
                case 1:
                    simData.HandsWithPartnerSingleton++;
                    break;
                case > 4:
                    simData.HandsWithPartnerFit++;
                    break;
            }
        });

        var result = calc.Run(10_000_000, threadCount: 12);

        sw.Stop();

        Console.WriteLine($"Simulated a total of {result.TotalHands} hands in {sw.ElapsedMilliseconds / 1000}s:");
        Console.WriteLine($"{result.HandsPreempted} hands were pre-emptable ({(result.HandsPreempted / (double)result.TotalHands) * 100:F3}% of all hands)");
        Console.WriteLine($"- {result.HandsWithPartnerVoid} partners had a void in the 4-card suit ({result.HandsWithPartnerVoid / (double)result.HandsPreempted * 100:F3}% of pre-empted hands)");
        Console.WriteLine($"- {result.HandsWithPartnerSingleton} partners had a singleton in the 4-card suit ({result.HandsWithPartnerSingleton / (double)result.HandsPreempted * 100:F3}% of pre-empted hands)");
        Console.WriteLine($"- {result.HandsWithPartnerFit} partners had a fit the 4-card suit with four or more cards ({result.HandsWithPartnerFit / (double)result.HandsPreempted * 100:F3}% of pre-empted hands)");
    }

    record SimResult : ISimResult<SimResult>
    {
        public long TotalHands { get; set; }
        public long HandsPreempted { get; set; }
        public long HandsWithPartnerVoid { get; set; }
        public long HandsWithPartnerSingleton { get; set; }
        public long HandsWithPartnerFit { get; set; }

        public static SimResult Merge(SimResult[] results)
        {
            return new()
            {
                TotalHands = results.Sum(x => x.TotalHands),
                HandsPreempted = results.Sum(x => x.HandsPreempted),
                HandsWithPartnerVoid = results.Sum(x => x.HandsWithPartnerVoid),
                HandsWithPartnerSingleton = results.Sum(x => x.HandsWithPartnerSingleton),
                HandsWithPartnerFit = results.Sum(x => x.HandsWithPartnerFit),
            };
        }

        public static SimResult New()
        {
            return new();
        }
    }
}
