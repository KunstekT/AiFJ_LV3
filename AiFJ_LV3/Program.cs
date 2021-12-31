using System;

namespace AiFJ_LV3
{
    class Program
    {
        static void Main(string[] args)
        {
            LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer();
            lexicalAnalyzer.Analyze("tests/test1.txt");
        }
    }
}

