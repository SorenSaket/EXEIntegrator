using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static EXEIntegrator.Scripts.MathAddons;

namespace EXEIntegrator
{
    static class StringMatcher
    {
        // Gets a percentage depending on how precisely the query matches the keywords
        public static float MatchPercentage(string[] keywords, string query)
        {
            List<float> percentages = new List<float>();
            List<string> currentKeywords = new List<string>();

            //removes the file extension
            query = query.Substring(0, query.LastIndexOf(".")).ToLower();

            Console.WriteLine("Matching " + query + " With: ");

            if (query.Contains("update") || query.Contains("install"))
                return 0;

            //Split keywords
            for (int x = 0; x < keywords.Length; x++)
            {
                string[] tempKeywords = SplitCamelCase(keywords[x]);
                for (int y = 0; y < tempKeywords.Length; y++)
                {
                    if(!string.IsNullOrWhiteSpace( tempKeywords[y]))
                    {
                        currentKeywords.Add(tempKeywords[y]);
                        Console.WriteLine(tempKeywords[y]);
                    }
                }
            }

            for (int x = 0; x < currentKeywords.Count; x++)
            {
                if (query.Contains(currentKeywords[x].ToLower()))
                {
                    percentages.Add(1);
                }
                else
                    percentages.Add(0);
            }

            return percentages.FloatListSum() / percentages.Count;
        }
        //
        #region Custom Includes string
        private static bool IncludesString(string text, string query)
        {
            text = text.ToLower();
            query = query.ToLower();

            if (!string.IsNullOrEmpty(query))
            {
                List<int> charPositions = FindCharacters(text, query[0]);

                for (int x = 0; x < charPositions.Count; x++)
                {
                    if (CheckChar(charPositions[x], text, query))
                        return true;
                }
            }
            return false;
        }

        private static bool CheckChar(int start, string text, string query)
        {
            // Check om ordert er for langt
            if (start + query.Length > text.Length)
                return false;
            // Check om det første Midterste og sidste bogstav er det samme
            // Er godt at checke det midterste hvis du har et sprog med mange ensartede ord
            /*int offset = query.Length - 1;
            int middle = (int)Math.Floor(offset / 2f);
            if (text[start] != query[0] || text[start + offset] != query[offset] || text[start + offset - middle] != query[offset - middle])
                return false;*/
            // Check om de andre char passer
            for (int y = 1; y < query.Length; y++)
            {
                if (!text[y + start].Equals(query[y]))
                    return false;
            }
            return true;
        }

        private static List<int> FindCharacters(string text, char charToFind)
        {
            List<int> charPositions = new List<int>();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].Equals(charToFind))
                    charPositions.Add(i);
            }
            return charPositions;
        }
        #endregion
        //
        public static string[] SplitAndFormatKeyword(string c)
        {
            return Regex.Split(c, @"\b[A-Z]");
        }
        //
        public static string[] SplitCamelCase(string str)
        {
            return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2").Split(' ');
        }
        //
        public static string RemoveSpecialCharacters(string input)
        {
            Regex r = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return r.Replace(input, String.Empty);
        }
    }
}