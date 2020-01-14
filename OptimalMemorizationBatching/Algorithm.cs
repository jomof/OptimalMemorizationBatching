using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimalMemorizationBatching
{
    public static class Algorithm
    {
        private class WeightedPoint
        {
            public int index;
            public int weight;
        }

        private static WeightedPoint PointOf(int index, int weight)
            => new WeightedPoint { index = index, weight = weight };

        private static WeightedPoint Sample(List<WeightedPoint> weights, Random generator)
        {
            var sum = weights.Sum(p => 0.0 + p.weight);
            var random = generator.NextDouble();
            var seen = 0.0;
            for (int i = 0; i <= weights.Count(); ++i)
            {
                seen += weights[i].weight / sum;
                if (seen > random) return weights[i];
            }
            throw new Exception("Unexpected");
        }

        private static int[] Sample1(int[] weights, int n, Random generator)
        {
            var points = weights
                .Select((weight, index) => PointOf(index, weight))
                .ToList();
            return Sample(points, n, generator).Select(point => point.index).ToArray();
        }

        private static List<WeightedPoint> Sample(List<WeightedPoint> points, int n, Random generator)
        {
            var result = new List<WeightedPoint>();
            while (result.Count < n)
            {
                var selected = Sample(points, generator);
                points.Remove(selected);
            }
            return result;
        }

        public static List<List<T>> SampleBatches<T>(
           this IEnumerable<T> source,
           int newItemsInBatch,
           int oldItemsInBatch,
           int maxReviewsPerItem)
        {
            var result = new List<List<T>>();
            var original = source.ToList();
            var points = source.Select((item, index) => PointOf(index, 0)).ToList();
            var generator = new Random(0);

            while (true)
            {
                var batch = new List<WeightedPoint>();

                // First, add the new items
                batch = points
                    .Where(point => point.weight == 0)
                    .Take(Math.Max(1, newItemsInBatch))
                    .ToList();
                // Then sample the old items
                var oldPoints = points
                    .Where(point => point.weight > 0)
                    .ToList();
                while (batch.Count < newItemsInBatch + oldItemsInBatch)
                {
                    if (oldPoints.Count == 0) break;
                    var sample = Sample(oldPoints, generator);
                    oldPoints.Remove(sample);
                    batch.Add(sample);
                }

                // Top off any remaining deficit with 'new' items
                batch.AddRange(points
                    .Where(point => point.weight == 0 && !batch.Contains(point))
                    .Take(newItemsInBatch + oldItemsInBatch - batch.Count)
                    .ToList());

                // Record everything in the batch as seen
                batch.ForEach(point => ++point.weight);

                // Add the original items back into the result.
                result.Add(
                    batch
                    .Select(point => original[point.index])
                    .OrderBy(_ => generator.NextDouble())
                    .ToList());

                // Check whether we're done
                if (points.All(point => point.weight >= maxReviewsPerItem))
                {
                    return result;
                }
            }
        }
    }
}
