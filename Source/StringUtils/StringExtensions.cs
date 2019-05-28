namespace StringUtils
{
    public static class StringExtensions
    {
        public static bool IsAlike(this string lhs, string rhs)
        {
            return new Comparer(lhs, rhs).Match();
        }
    }
}
