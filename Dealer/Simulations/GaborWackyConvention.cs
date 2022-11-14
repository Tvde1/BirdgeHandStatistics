namespace Dealer.Simulations;

internal static class GaborWackyConvention
{
    public static async Task Run()
    {
        var calc = SimRunner.Create((HandGenerator deckSim, SimResult simData) =>
        {
            simData.TotalHands++;
            var deck = deckSim.ShuffleNewDeck();

            var hasEnoughPoints = deck.South.Points is >= 4 and <= 11;

            if (!hasEnoughPoints)
            {
                return;
            }

            var hasSixCard = deck.South.SuitCounts.HasCount(6, out _);

            if (!hasSixCard)
            {
                return;
            }

            var hasFourCard = deck.South.SuitCounts.HasCount(4, out var fourCardSuit);
            if (!hasFourCard)
            {
                return;
            }

            simData.HandsPreempted++;

            if (deck.North.SuitCounts[fourCardSuit] == 1)
            {
            }

            switch (deck.North.SuitCounts[fourCardSuit])
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

        var result = await calc.Run(50_000_000, threadCount: 8);
        
        result.PrintResult();
    }

    record SimResult : ISimResult<SimResult>
    {
        public long TotalHands { get; set; }
        public long HandsPreempted { get; set; }
        public long HandsWithPartnerVoid { get; set; }
        public long HandsWithPartnerSingleton { get; set; }
        public long HandsWithPartnerFit { get; set; }
        public long ElapsedMilliseconds { get; private init; }

        public static SimResult Merge(ICollection<SimResult> results, long elapsedMilliseconds)
        {
            return new()
            {
                TotalHands = results.Sum(x => x.TotalHands),
                HandsPreempted = results.Sum(x => x.HandsPreempted),
                HandsWithPartnerVoid = results.Sum(x => x.HandsWithPartnerVoid),
                HandsWithPartnerSingleton = results.Sum(x => x.HandsWithPartnerSingleton),
                HandsWithPartnerFit = results.Sum(x => x.HandsWithPartnerFit),
                ElapsedMilliseconds = elapsedMilliseconds
            };
        }

        public void PrintResult()
        {
            Console.WriteLine($"Simulated a total of {TotalHands} hands in {ElapsedMilliseconds / 1000}s:");
            Console.WriteLine($"{HandsPreempted} hands were preemptable ({HandsPreempted / (double)TotalHands * 100:F3}% of all hands)");
            Console.WriteLine($"- {HandsWithPartnerVoid} partners had a void in the 4-card suit ({HandsWithPartnerVoid / (double)HandsPreempted * 100:F3}% of preempted hands)");
            Console.WriteLine($"- {HandsWithPartnerSingleton} partners had a singleton in the 4-card suit ({HandsWithPartnerSingleton / (double)HandsPreempted * 100:F3}% of preempted hands)");
            Console.WriteLine($"- {HandsWithPartnerFit} partners had a fit the 4-card suit with four or more cards ({HandsWithPartnerFit / (double)HandsPreempted * 100:F3}% of preempted hands)");
        }

        public static SimResult New()
        {
            return new();
        }
    }
}
