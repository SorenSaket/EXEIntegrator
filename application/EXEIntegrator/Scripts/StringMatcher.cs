using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static EXEIntegrator.Scripts.MathAddons;

namespace EXEIntegrator
{
    static class StringMatcher
    {
        public static List<string> FormatKeyword(string keyword)
        {
            List<string> temp = new List<string>();
            List<string> keywords = Regex.Split(keyword, @"(\W)|(\s)|(\d)|([-_])|(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})").ToList();
            for (int i = 0; i < keywords.Count; i++)
            {
                if(!Regex.IsMatch(keywords[i], @"(\W)|(\s)|(\d)|([-_])", RegexOptions.None) && !string.IsNullOrWhiteSpace(keywords[i]))
                {
                    temp.Add(keywords[i].ToLower());
                }
            }
            return temp;
        }

        public static bool IsMatch(string query, List<string> keywords)
        {
            if (string.IsNullOrWhiteSpace(query))
                return false;
            query = Path.GetFileNameWithoutExtension(query).ToLower();

            for (int i = 0; i < keywords.Count; i++)
            {
                if (query.Contains(keywords[i]))
                    return true;
            }
            return false;
        }
        /*
        public static float MatchPercentage(string query, string[] keywords)
        {
            if (string.IsNullOrWhiteSpace(query))
                return false;
            query = Path.GetFileNameWithoutExtension(query).ToLower();


        }*/
       



            // Gets a percentage depending on how precisely the query matches the keywords
            /*public static float MatchPercentage(string[] keywords, string query)
            {
                List<float> percentages = new List<float>();
                List<string> currentKeywords = new List<string>();

                //removes the file extension
                query = query.Substring(0, query.LastIndexOf(".")).ToLower();

                if (query.Contains("update") || query.Contains("instal") || query.Contains("setup"))
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
                            //Console.WriteLine(tempKeywords[y]);
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
            }*/
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