﻿using System;
using System.IO;

namespace LockstepECL {
    internal class Program {
        public static void Main(string[] args){
            //TestLex();
            TestGrammar();
            //TestSymbol();
        }

        private static void TestLex(){
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../data/ErrorHelloWorld.c"); //HelloWorld
            var test = new TestLex();
            test.Init(path);
            test.ShowLexResult();
        }

        private static void TestGrammar(){
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../data/SyntaxIndent.c");
            var test = new TestGrammar();
            test.Init(path);
            test.ShowGrammarResult();
        }

        private static void TestSymbol(){
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../data/HelloWorld.c");
            var test = new TestGrammar();
            test.Init(path);
            test.ShowGrammarResult();
        }
    }
}