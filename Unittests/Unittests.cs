using NUnit.Framework;
using OptimalMemorizationBatching;

namespace Tests
{
    public class Tests
    {
        [Test]
        public void Simple()
        {
            var batches =
                new string[] { "a", "b", "c" }
                .SampleBatches(1, 1, 3);
        }

        [Test]
        public void NoNewItems()
        {
            new string[] { "a", "b", "c" }.SampleBatches(0, 1, 1);
        }

        [Test]
        public void Edges()
        {
            new string[] { "a", "b", "c" }.SampleBatches(100, 1, 20);
            new string[] { "a", "b", "c" }.SampleBatches(1, 1, 100);
            new string[] { "a", "b", "c" }.SampleBatches(1, 0, 1);
            new string[] { "a", "b", "c" }.SampleBatches(1, 1, 0);
        }
    }
}