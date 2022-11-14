using System.Diagnostics;

namespace Dealer.Simulations;

internal class NormalTwoLevelPreempt
{
    public static void Run()
    {
        var sw = Stopwatch.StartNew();

        var calc = Calculator.Create((HandGenerator deckSim, SimResult simData) =>
        {
            simData.TotalHands++;
            var (north, _, south, _) = deckSim.CreateHands();

            var hasEnoughPoints = south.Points is >= 6 and <= 10;

            if (!hasEnoughPoints)
            {
                return;
            }

            switch (south.SuitCounts.ToTuple())
            {
                case (6, 4, 2, 1):
                    simData.SixFourTwoOne++;
                    simData.HandsPreempted++;
                    break;
                case (6, 3, 3, 1):
                    simData.SixThreeThreeOne++;
                    simData.HandsPreempted++;
                    break;
                case (6, 3, 2, 2):
                    simData.SixThreeTwoTwo++;
                    simData.HandsPreempted++;
                    break;
            }
        });

        var result = calc.Run(10_000_000, threadCount: 12);

        sw.Stop();

        Console.WriteLine($"Simulated a total of {result.TotalHands} hands in {sw.ElapsedMilliseconds / 1000}s:");
        Console.WriteLine($"{result.HandsPreempted} hands were pre-emptable ({(result.HandsPreempted / (double)result.TotalHands) * 100:F3}% of all hands)");
        Console.WriteLine($"- 6421: {result.SixFourTwoOne} ({(result.SixFourTwoOne / (double)result.HandsPreempted) * 100:F3}% of pre-empted hands)");
        Console.WriteLine($"- 6331: {result.SixThreeThreeOne} ({(result.SixThreeThreeOne / (double)result.HandsPreempted) * 100:F3}% of pre-empted hands)");
        Console.WriteLine($"- 6322: {result.SixThreeTwoTwo} ({(result.SixThreeTwoTwo / (double)result.HandsPreempted) * 100:F3}% of pre-empted hands)");
    }

    record SimResult : ISimResult<SimResult>
    {
        public long TotalHands { get; set; }
        public long HandsPreempted { get; set; }
        public int SixFourTwoOne { get; set; }
        public int SixThreeThreeOne { get; set; }
        public int SixThreeTwoTwo { get; set; }

        public static SimResult Merge(SimResult[] results)
        {
            return new()
            {
                TotalHands = results.Sum(x => x.TotalHands),
                HandsPreempted = results.Sum(x => x.HandsPreempted),
                SixFourTwoOne = results.Sum(x => x.SixFourTwoOne),
                SixThreeThreeOne = results.Sum(x => x.SixThreeThreeOne),
                SixThreeTwoTwo = results.Sum(x => x.SixThreeTwoTwo),
            };
        }

        public static SimResult New()
        {
            return new();
        }
    }
}
