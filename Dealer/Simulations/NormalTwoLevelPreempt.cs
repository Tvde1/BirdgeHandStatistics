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

                var hasRightShape = south.SuitCounts switch
                {
                    (6, 4, 2, 1) => true,
                    (6, 3, 3, 1) => true,
                    (6, 3, 2, 2) => true,
                    _ => false,
                };

                if (!hasRightShape)
                {
                    return;
                }

                simData.HandsPreempted++;
            });

            var result = calc.Run(500_000);

            data.Add(result);
        });

        sw.Stop();

        var result = SimData.Merge(data);

        Console.WriteLine($"Simulated a total of {result.TotalHands} hands in {sw.ElapsedMilliseconds / 1000}s:");
        Console.WriteLine($"{result.HandsPreempted} hands were pre-emptable ({(result.HandsPreempted / (double)result.TotalHands) * 100:F3}% of all hands)");
    }

    record SimData
    {
        public long TotalHands { get; set; }
        public long HandsPreempted { get; set; }


        internal static SimData Merge(List<SimData> data)
        {
            return new()
            {
                TotalHands = data.Sum(x => x.TotalHands),
                HandsPreempted = data.Sum(x => x.HandsPreempted),
            };
        }
    }
}
