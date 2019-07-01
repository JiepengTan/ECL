using System;
using System.IO;
using static LockstepECL.Define;
using static LockstepECL.ConstDefine;

namespace LockstepECL {
    public class TestGrammar : TestLex {
        protected Grammar grammar;

        public override void Init(string path){
            base.Init(path);
            defaultColor = Console.ForegroundColor;
            grammar = new Grammar();
        }

        public void ShowGrammarResult(){
            base.ShowLexResult();
            grammar.Init(lex.LexInfos, SyntaxIndent);
            try {
                grammar.TranslationUnit();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
            finally {
                grammar.DumpErrorInfo();
            }
        }

        protected override void LexIndent(){ }
        protected override void OnSpace(char ch){ }

        void SyntaxIndent(){
            switch (grammar.syntax_state) {
                case SNTX_NUL:
                    ColorToken(ELexState.LEX_NORMAL);
                    break;
                case SNTX_SP:
                    Output(" ");
                    ColorToken(ELexState.LEX_NORMAL);
                    break;
                case SNTX_LF_HT: {
                    if (grammar.curTokenId == TK_END) // 遇到'}',缩进减少一级
                        grammar.syntax_level--;
                    Output("\n");
                    OutputTab(grammar.syntax_level);
                }
                    ColorToken(ELexState.LEX_NORMAL);
                    break;
                case SNTX_DELAY:
                    break;
            }

            grammar.syntax_state = SNTX_NUL;
        }

        protected void ColorToken(ELexState lex_state){
            string p;
            switch (lex_state) {
                case ELexState.LEX_NORMAL: {
                    if (grammar.curTokenId >= Define.TK_IDENT)
                        SetConsoleTextAttribute(ConsoleColor.Gray, defaultColor);
                    else if (grammar.curTokenId >= Define.KW_CHAR)
                        SetConsoleTextAttribute(ConsoleColor.Green, defaultColor);
                    else if (grammar.curTokenId >= Define.TK_CINT)
                        SetConsoleTextAttribute(ConsoleColor.Red, defaultColor);
                    else
                        SetConsoleTextAttribute(ConsoleColor.Red, defaultColor);
                    p = grammar.GetTokenDebugString();
                    if (grammar.curTokenId != Define.TK_EOF) {
                        Output(p);
                    }

                    break;
                }
                case ELexState.LEX_SEP:
                    Output(lex.curChar);
                    break;
            }
        }

        protected void SetConsoleTextAttribute(ConsoleColor fgColor, ConsoleColor bgColor = ConsoleColor.Black){
            Console.ForegroundColor = fgColor;
        }

        void OutputTab(int n){
            int i = 0;
            for (; i < n; i++)
                Output("    ");
        }
    }
}