using System.Threading;

namespace Nwpie.Foundation.Abstractions.Utilities
{
    public static class FailbackScoreInfo
    {
        public const int InitScore = 0;
        public const int SuccessScore = -1;
        public const int FailScore = 2;
        public const int ExceedScore = 7;

        public static IFailbackScore CreateNew => new FailbackScore();
    }

    public interface IFailbackScore
    {
        int Score(bool? isSuccess);
        int Success();
        int Fail();
        bool IsExceedLimit();
    }

    public class FailbackScore : FailbackScoreBase
    {

    }

    public class FailbackScoreBase : IFailbackScore
    {
        public int Score(bool? isSuccess)
        {
            return true == isSuccess ? Success() : Fail();
        }
        public int Success()
        {
            if (Interlocked.Add(ref Scores, SuccessScore) <= 0)
            {
                return Interlocked.Exchange(ref Scores, FailbackScoreInfo.InitScore);
            }

            return Scores;
        }

        public int Fail() => Interlocked.Add(ref Scores, FailScore);
        public bool IsExceedLimit() => Scores >= ExceedScore;

        public int SuccessScore { get; set; } = FailbackScoreInfo.SuccessScore;
        public int FailScore { get; set; } = FailbackScoreInfo.FailScore;
        public int ExceedScore { get; set; } = FailbackScoreInfo.ExceedScore;

        public int Scores = FailbackScoreInfo.InitScore;
    }
}
