using FuzzySharp.SimilarityRatio.Scorer.Composite;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using FuzzySharp.SimilarityRatio.Scorer;
using FuzzySharp.SimilarityRatio;
using FuzzySharp;
using FuzzySharp.Extractor;
using System.Collections.Generic;

namespace Element34
{
    public class FuzzySearchUtils
    {
        IRatioScorer ratio = ScorerCache.Get<DefaultRatioScorer>();
        IRatioScorer partialRatio = ScorerCache.Get<PartialRatioScorer>();
        IRatioScorer tokenSet = ScorerCache.Get<TokenSetScorer>();
        IRatioScorer partialTokenSet = ScorerCache.Get<PartialTokenSetScorer>();
        IRatioScorer tokenSort = ScorerCache.Get<TokenSortScorer>();
        IRatioScorer partialTokenSort = ScorerCache.Get<PartialTokenSortScorer>();
        IRatioScorer tokenAbbreviation = ScorerCache.Get<TokenAbbreviationScorer>();
        IRatioScorer partialTokenAbbreviation = ScorerCache.Get<PartialTokenAbbreviationScorer>();
        IRatioScorer weighted = ScorerCache.Get<WeightedRatioScorer>();

        void ex1()
        {
            string query1 = "strng";
            string[] choices = new[] { "stríng", "stráng", "stréng" };
            IEnumerable<ExtractedResult<string>> results = Process.ExtractAll(query1, choices, (s) => s);
        }

        void ex2()
        {
            string[][] events = new[]
            {
                new[] { "chicago cubs vs new york mets", "CitiField", "2011-05-11", "8pm" },
                new[] { "new york yankees vs boston red sox", "Fenway Park", "2011-05-11", "8pm" },
                new[] { "atlanta braves vs pittsburgh pirates", "PNC Park", "2011-05-11", "8pm" }
            };
            string[] query2 = new[] { "new york mets vs chicago cubs", "CitiField", "2017-03-19", "8pm" };
            ExtractedResult<string[]> best = Process.ExtractOne(query2, events, strings => strings[0]);
        }

        void ex3()
        {
            string[][] events = new[]
{
                new[] { "chicago cubs vs new york mets", "CitiField", "2011-05-11", "8pm" },
                new[] { "new york yankees vs boston red sox", "Fenway Park", "2011-05-11", "8pm" },
                new[] { "atlanta braves vs pittsburgh pirates", "PNC Park", "2011-05-11", "8pm" }
            };
            string[] query2 = new[] { "new york mets vs chicago cubs", "CitiField", "2017-03-19", "8pm" };
            ExtractedResult<string[]> best = Process.ExtractOne(query2, events, strings => strings[0], weighted);
        }

        IEnumerable<ExtractedResult<string>> sorted = Process.ExtractSorted("goolge", new[] { "google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl" });

        IEnumerable<ExtractedResult<string>> top = Process.ExtractTop("goolge", new[] { "google", "bing", "facebook", "linkedin", "twitter", "googleplus", "bingnews", "plexoogl" }, limit: 3);

    }
}
