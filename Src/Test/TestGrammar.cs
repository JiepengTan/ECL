using System;
using System.Runtime.InteropServices;
using static LogHandler;

namespace LockstepECL {
    public class TestGrammar : TestLex {

        public void ShowGrammarResult(){
            try {
                lex.GrammarInit();
                lex.GetChar();
                lex.GetToken();
                lex.TranslationUnit();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
            finally {
                lex.DumpErrorInfo();
            }
        }
        protected override void OnSpace(char ch){
        }
    }
}