namespace Dealer.Simulations;

internal static class NormalTwoLevelPreempt
{
    public static async Task Run()
    {
        var calc = SimRunner.Create((HandGenerator deckSim, SimResult simData) =>
        {
            simData.TotalHands++;
            var deck = deckSim.ShuffleNewDeck();

            var hasEnoughPoints = deck.South.Points is >= 6 and <= 10;

            if (!hasEnoughPoints)
            {
                return;
            }

            switch (deck.South.SuitCounts.ToTuple())
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

        var result = await calc.Run(50_000_000, threadCount: 8);

        result.PrintResult();
    }

    private record SimResult : ISimResult<SimResult>
    {
        public long TotalHands { get; set; }
        public long HandsPreempted { get; set; }
        public int SixFourTwoOne { get; set; }
        public int SixThreeThreeOne { get; set; }
        public int SixThreeTwoTwo { get; set; }
        public long ElapsedMilliseconds { get; private init; }

        public static SimResult Merge(ICollection<SimResult> results, long elapsedMilliseconds)
        {
            return new()
            {
                TotalHands = results.Sum(x => x.TotalHands),
                HandsPreempted = results.Sum(x => x.HandsPreempted),
                SixFourTwoOne = results.Sum(x => x.SixFourTwoOne),
                SixThreeThreeOne = results.Sum(x => x.SixThreeThreeOne),
                SixThreeTwoTwo = results.Sum(x => x.SixThreeTwoTwo),
                ElapsedMilliseconds = elapsedMilliseconds,
            };
        }

        public void PrintResult()
        {
            Console.WriteLine($"Simulated a total of {TotalHands} hands in {ElapsedMilliseconds / 1000}s:");
            Console.WriteLine($"{HandsPreempted} hands were preemptable ({HandsPreempted / (double)TotalHands * 100:F3}% of all hands)");
            Console.WriteLine($"- 6421: {SixFourTwoOne} ({SixFourTwoOne / (double)HandsPreempted * 100:F3}% of preempted hands)");
            Console.WriteLine($"- 6331: {SixThreeThreeOne} ({SixThreeThreeOne / (double)HandsPreempted * 100:F3}% of preempted hands)");
            Console.WriteLine($"- 6322: {SixThreeTwoTwo} ({SixThreeTwoTwo / (double)HandsPreempted * 100:F3}% of preempted hands)");
        }

        public static SimResult New()
        {
            return new();
        }
    }
}
