using Element34.StringMetrics.Similarity;

namespace Element34Tests.Similarity
{
    [TestClass]
    public class CosineSimilarityTests
    {
        private bool CosineSimilarityTest(string sValue1, string sValue2, double expected)
        {
            //CosineMetric cosineMetric = new CosineMetric();
            //return cosineMetric.Compute(sValue1,sValue2).Equals(expected);
            return false;
        }

        [TestMethod] public void CosineSimilarityTest001() { Assert.IsTrue(CosineSimilarityTest("Deep Learning can be hard", "Deep Learning can be simple", 0.8D)); }
        [TestMethod] public void CosineSimilarityTest002() { Assert.IsTrue(CosineSimilarityTest("the best data science course", "data science is popular", 0.44721D)); }
        [TestMethod] public void CosineSimilarityTest003() { Assert.IsTrue(CosineSimilarityTest("Data is the oil of the digital economy", "Data is a new oil", 0.327871D)); }

    }
}