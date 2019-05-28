using System;
using System.Collections.Generic;
using System.Linq;

namespace StringUtils
{
    public class Comparer
    {
        private readonly string _lhs;
        private readonly string _rhs;

        private readonly List<string> _lhsParts;
        private readonly List<string> _rhsParts;

        private readonly List<int> _matchingCharacters;


        public Comparer(string lhs, string rhs)
        {
            _lhs = lhs.ToLowerInvariant();
            _lhs = _lhs.Replace('.', ' ');
            _rhs = rhs.ToLowerInvariant();
            _rhs = _rhs.Replace('.', ' ');

            _lhsParts = new List<string>(_lhs.Split(new[] { " " }, StringSplitOptions.None));
            _rhsParts = new List<string>(_rhs.Split(new[] { " " }, StringSplitOptions.None));

            _matchingCharacters = new List<int>();
        }

        public bool Match()
        {
            var result = false;

            foreach (var lhsPart in _lhsParts)
            {
                if (!string.IsNullOrEmpty(lhsPart))
                {
                    var matchingRhsPart = _rhsParts.FirstOrDefault(item => item.Equals(lhsPart));
                    if (matchingRhsPart != null)
                    {
                        _matchingCharacters.Add(lhsPart.Length);
                    }
                }
            }

            var sumMatchingCharacters = _matchingCharacters.Sum();

            if (sumMatchingCharacters > 0)
            {
                var lhsWithoutSpaces = _lhs.Replace(" ", "");
                var rhsWithoutSpaces = _rhs.Replace(" ", "");
                var mostCharactersForComparison = Math.Max(lhsWithoutSpaces.Length, rhsWithoutSpaces.Length);

                var match = (sumMatchingCharacters / (float)mostCharactersForComparison) * 100.0;

                result = match > 40.0;
            }

            return result;
        }
    }
}
