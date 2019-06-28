using System;
using System.IO;

namespace LockstepECL {
    internal class Program {
        public static void Main(string[] args){
            var path =  Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../data/HelloWorld.c");
            var test = new TestLex();
            test.Init(path);
            test.DumpContext();
            ErrorHandler.DumpErrorInfo();
        }
    }
}