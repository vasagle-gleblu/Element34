﻿using System.Collections.Generic;

namespace Element34.StringMetric
{
    public class ReverseSoundEx : IStringEncoder, IStringComparison
    {
        private const int tokenLength = 4;
        private IReadOnlyDictionary<char, char> map = new Dictionary<char, char>()
            { { 'B', '1' }, { 'F', '1' }, { 'P', '1' }, { 'V', '1' }, { 'C', '2' },
              { 'G', '2' }, { 'J', '2' }, { 'K', '2' }, { 'Q', '2' }, { 'S', '2' },
              { 'X', '2' }, { 'Z', '2' }, { 'D', '3' }, { 'T', '3' }, { 'L', '4' },
              { 'M', '5' }, { 'N', '5' }, { 'R', '6' } };

        public bool Compare(string value1, string value2)
        {
            throw new System.NotImplementedException();
        }

        public string Encode(string source)
        {
            int i = 0, s = 0, p = 0;
            char c, j;
            char[] token = new char[tokenLength];

            for (int k = 0; k < tokenLength; k++)
                token[k] = '0';

            source = source.ToUpper();

            if (source == null)
            {
                return "";
            }

            while (s < tokenLength)
            {
                if (i < source.Length)
                    c = source[i++];
                else
                    break;

                if (map.TryGetValue(c, out j))
                {
                    if (j != p)
                    {
                        token[s] = j;
                        p = j;
                        s++;
                    }
                }
                else
                {
                    if (i == 1)
                        s = 1;

                    p = 0;
                }
            }

            token[0] = source[source.Length - 1];
            return new string(token);
        }

    }
}
