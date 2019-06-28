using System;
using System.IO;

namespace LockstepECL {
    internal class Program {
        public static void Main(string[] args){
            //TestLex();
            TestGrammar();
        }

        private static void TestLex(){
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../data/HelloWorld.c");//HelloWorld
            var test = new TestLex();
            test.Init(path);
            test.ShowLexResult();
            ErrorHandler.DumpErrorInfo();
        }

        private static void TestGrammar(){
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../data/SyntaxIndent.c");
            var test = new TestGrammar();
            test.Init(path);
            test.ShowGrammarResult();
            ErrorHandler.DumpErrorInfo();
        }
    }
}