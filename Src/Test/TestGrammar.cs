using static ErrorHandler;

namespace LockstepECL {
    public class TestGrammar : TestLex {

        public void ShowGrammarResult(){
            lex.GetChar();
            lex.GetToken();
            lex.TranslationUnit();
        }
        protected override void OnSpace(char ch){
        }
    }
}