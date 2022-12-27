namespace KOM.Scribere.Common
{
    public static class RandomExtensions
    {
        public static int NextIntRange(this System.Random random, int minNumber, int maxNumber)
        {
            return random.Next(minNumber, maxNumber);
        }
    }
}
