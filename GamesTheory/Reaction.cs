using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesTheory
{
    public class Reaction
    {
        private const string SubscriptDigits =
            "\u2080\u2081\u2082\u2083\u2084\u2085\u2086\u2087\u2088\u2089";

        public int Index { get; set; }
        public string IndexAnswer { get; set; }
        public int Answer { get; set; }

        public Reaction(int a, int[] b,int c, string player)
        {
            Index = a;
            if (b.Length == 1)
            {
                IndexAnswer = player + new string((b[0]+1).ToString().Select(x => SubscriptDigits[x - '0']).ToArray());
            }
            else
            {
                IndexAnswer = $"{{";
                foreach (var index in b)
                {
                    IndexAnswer += player + new string((index+1).ToString().Select(x => SubscriptDigits[x - '0']).ToArray()) + ",";
                }

                IndexAnswer += $"}}";
            }
            Answer = c;
        }
    }
}
