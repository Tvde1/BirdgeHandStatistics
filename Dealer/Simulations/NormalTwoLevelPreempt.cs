using System.Diagnostics;

namespace Dealer.Simulations;

internal class NormalTwoLevelPreempt
{
    public static void Run()
    {
        var data = new List<SimData>();

        var sw = Stopwatch.StartNew();

        Parallel.For(0, 50, x =>
        {
            var calc = Calculator.Create((HandGenerator deckSim, SimData simData) =>
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

            var result = calc.Run(1_000_000);

            data.Add(result);
        });

        sw.Stop();

        var result = SimData.Merge(data);

        Console.WriteLine($"Simulated a total of {result.TotalHands} hands in {sw.ElapsedMilliseconds / 1000}s:");
        Console.WriteLine($"{result.HandsPreempted} hands were pre-emptable ({(result.HandsPreempted / (double)result.TotalHands) * 100:F3}% of all hands)");
        Console.WriteLine($"- 6421: {result.SixFourTwoOne} ({(result.SixFourTwoOne / (double)result.HandsPreempted) * 100:F3}% of pre-empted hands)");
        Console.WriteLine($"- 6331: {result.SixThreeThreeOne} ({(result.SixThreeThreeOne / (double)result.HandsPreempted) * 100:F3}% of pre-empted hands)");
        Console.WriteLine($"- 6322: {result.SixThreeTwoTwo} ({(result.SixThreeTwoTwo / (double)result.HandsPreempted) * 100:F3}% of pre-empted hands)");
    }

    record SimData
    {
        public long TotalHands { get; set; }
        public long HandsPreempted { get; set; }
        public int SixFourTwoOne { get; set; }
        public int SixThreeThreeOne { get; set; }
        public int SixThreeTwoTwo { get; set; }

        internal static SimData Merge(List<SimData> data)
        {
            return new()
            {
                TotalHands = data.Sum(x => x.TotalHands),
                HandsPreempted = data.Sum(x => x.HandsPreempted),
                SixFourTwoOne = data.Sum(x => x.SixFourTwoOne),
                SixThreeThreeOne = data.Sum(x => x.SixThreeThreeOne),
                SixThreeTwoTwo = data.Sum(x => x.SixThreeTwoTwo),
            };
        }
    }
}
