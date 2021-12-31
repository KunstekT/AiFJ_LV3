using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AiFJ_LV3
{
    class LexicalAnalyzer
    {
        private List<String> Identificators;
        private List<String> Keywords;
        private List<String> Operators;
        private List<String> Separators;
        private List<String> CommentKeywords;
        private List<String> ConstantKeywords;

        int c_identificators;
        int c_keywords;
        int c_separators;
        int c_operators;
        int c_constants;
        int c_comments;

        List<String> s_identificators;
        List<String> s_keywords;
        List<String> s_separators;
        List<String> s_operators;
        List<String> s_constants;
        List<String> s_comments;

        public LexicalAnalyzer()
        {
            this.SetRules();
        }
        private void ResetValues()
        {
            c_identificators = 0;
            c_keywords = 0;
            c_separators = 0;
            c_operators = 0;
            c_constants = 0;
            c_comments = 0;

            s_identificators = new List<string>();
            s_keywords = new List<string>();
            s_separators = new List<string>();
            s_operators = new List<string>();
            s_constants = new List<string>();
            s_comments = new List<string>();
        }
        private void SetRules()
        {
            this.Keywords = new List<string>();
            this.Keywords.Add("int");
            this.Keywords.Add("float");
            this.Keywords.Add("for");
            this.Keywords.Add("if");
            this.Keywords.Add("operator");
            this.Keywords.Add("switch");
            this.Keywords.Add("Return");

            this.Operators = new List<string>();
            this.Operators.Add("+");
            this.Operators.Add("-");
            this.Operators.Add("*");
            this.Operators.Add("/");
            this.Operators.Add("%");
            this.Operators.Add("==");
            this.Operators.Add("!");
            this.Operators.Add("=");
            this.Operators.Add("sizeOf");
            this.Operators.Add("<");
            this.Operators.Add(">");
            this.Operators.Add("<=");
            this.Operators.Add(">=");
            this.Operators.Add("|");
            this.Operators.Add("&");
            this.Operators.Add("!");

            this.Separators = new List<string>();
            this.Separators.Add(" ");
            this.Separators.Add(";");
            this.Separators.Add(":");
            this.Separators.Add(",");

            this.CommentKeywords = new List<string>();
            this.CommentKeywords.Add("#");

            this.ConstantKeywords = new List<string>();
            this.ConstantKeywords.Add("constant");

            this.Identificators = new List<string>();
        }
        public void Analyze(String filepath)
        {
            this.ResetValues();

            List<String> allWords = new List<string>();

            StreamReader reader;
            using (FileStream fs = File.Open(filepath, FileMode.Open))
            {
                reader = new StreamReader(fs);

                if (reader == null)
                {
                    Console.WriteLine("ERROR: reader is null");
                }
                else
                {
                    CreateWordsFromFile(reader);
                }
            }
            Console.WriteLine("------------------------");
            PrintResultValues();
        }

        /* Also prints out file rows */
        private void CreateWordsFromFile(StreamReader reader)
        {
            String line;
            int lineCounter = 0;
            List<String> words = new List<string>();
            String word = "";

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                //if (line == "") break;

                Console.WriteLine("\nLine " + (++lineCounter).ToString() + ": " + line);

                word = "";
                words.Clear();

                foreach (char c in line)
                {
                    if (this.Operators.Contains(c.ToString()) || this.Separators.Contains(c.ToString()) || c == '(' || c == ')' || c == '[' || c == ']' || c == '{' || c == '}' /*!((c >= 48 && c <= 57) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122) || (c == '_') || !this.Separators.Contains(c.ToString()) || !(c != '(' && c != ')' && c != '[' && c != ']' && c != '{' && c != '}'))*/)
                    {
                        if (word != "")
                        {
                            words.Add(word);
                        }
                        words.Add(c.ToString());
                        word = "";
                        continue;
                    }
                    word = word + c;
                }
                words.Add(word);
                ClassifyWordsAndPrint(words);
            }
            reader.Dispose();
            reader.Close();
        }
        private void ClassifyWordsAndPrint(List<String> words)
        {
            String word = "";
            bool commentedLineFlag = false;
            bool constantFlag = false;
            String appendString = "";

            foreach (String w in words)
            {
                if (w == "") continue;  // w[0] must exist
                appendString = "";

                if (commentedLineFlag)
                {
                    word = word + w;
                }
                else
                {
                    if (CommentKeywords.Contains(w))
                    {
                        commentedLineFlag = true;
                        word = w;
                        c_comments++;
                    }
                    else if (Operators.Contains(w))
                    {
                        appendString = "operator";
                        c_operators++;
                        s_operators.Add(w);
                    }
                    else if (Separators.Contains(w))
                    {
                        appendString = "separator";
                        c_separators++;
                        s_separators.Add(w);
                    }
                    else if (Keywords.Contains(w))
                    {
                        appendString = "keyword";
                        c_keywords++;
                        s_keywords.Add(w);
                    }
                    else if (ConstantKeywords.Contains(w))
                    {
                        constantFlag = true;

                        appendString = "keyword";
                        Console.WriteLine("('" + w + "', " + appendString + ")");
                        c_keywords++;
                        s_keywords.Add(w);
                        continue;
                    }
                    else if (w[0] >= 97 && w[0] <= 122)
                    {
                        foreach (char c in w)
                        {
                            if ((c >= 48 && c <= 57) || // number
                                (c >= 65 && c <= 90) || // capital
                                (c >= 97 && c <= 122) ||// non-capital
                                (c == '_')
                                )
                            {
                                if (constantFlag == true)
                                {
                                    constantFlag = false;
                                    appendString = "constant";
                                    c_constants++;
                                    s_constants.Add(w);
                                    break;
                                }
                                appendString = "identifier";
                                c_identificators++;
                                s_identificators.Add(w);
                                break;
                            }
                            else
                            {
                                appendString = "unknown";
                            }
                        }
                    }
                    else { appendString = "unknown"; }

                }

                if (!commentedLineFlag)
                {
                    Console.WriteLine("('" + w + "', " + appendString + ")");
                }
            }
            if (commentedLineFlag)
            {
                commentedLineFlag = false;
                Console.WriteLine("('" + word + "', " + "comment" + ")");
                s_comments.Add(word);
            }
        }
        private void PrintResultValues()
        {
            Console.Write("- identifikatori[" + c_identificators + "]: ");
            PrintSingleLexicType(s_identificators);
            Console.Write("- ključne riječi[" + c_keywords + "]: ");
            PrintSingleLexicType(s_keywords);
            Console.Write("- separatori[" + c_separators + "]: ");
            PrintSingleLexicType(s_separators);
            Console.Write("- operatori[" + c_operators + "]: ");
            PrintSingleLexicType(s_operators);
            Console.Write("- konstante[" + c_constants + "]: ");
            PrintSingleLexicType(s_constants);
            Console.Write("- komentari[" + c_comments + "]: ");
            PrintSingleLexicType(s_comments);
            Console.WriteLine("");
        }
        private void PrintSingleLexicType(List<String> singleLexicTypeStrings)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();

            foreach (String s in singleLexicTypeStrings)
            {
                if (!dict.ContainsKey(s))
                {
                    dict.Add(s, 1);
                }
                else
                {
                    dict[s] += 1;
                }
            }

            bool firstWordFlag = true;
            foreach (var item in dict)
            {
                if (firstWordFlag == false) Console.Write(", ");

                firstWordFlag = false;

                Console.Write("'");
                Console.Write(item.Key);
                Console.Write("'");
                Console.Write("[");
                Console.Write(item.Value);
                Console.Write("]");
            }
            Console.WriteLine("");
        }
        private void PrintWordsDebug(List<String> words)
        {
            Console.WriteLine("\n-----------------------------------");
            foreach (String w in words)
            {
                Console.Write("'" + w + "'  ");
            }
            Console.WriteLine("\n-----------------------------------");
        }
    }
}
