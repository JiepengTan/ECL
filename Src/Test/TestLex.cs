using System;
using System.IO;
using static LockstepECL.Define;
using static LockstepECL.ESynTaxState;

using static LockstepECL.ESynTaxState;

namespace LockstepECL {
    public class TestLex {
        protected InputStream input;
        protected Lex lex = new Lex();

        protected ConsoleColor defaultColor;

        public virtual void Init(string path){
            var text = File.ReadAllText(path).Replace("\r\n", "\n").Replace("\r", "\n");
            defaultColor = Console.ForegroundColor;
            input = new InputStream(text);
            lex.lineNum = 1;
            lex.fileName = path;
            lex.Init(input.GetChar, input.UnChar, OnSpace);
            lex.FuncSyntaxIndent = SyntaxIndent;
        }

        public void ShowLexResult(){
            lex.GetChar();
            do {
                lex.GetToken();
            } while (lex.curTokenId != Define.TK_EOF);

            Console.ForegroundColor = defaultColor;
            //Output($"\nTotalLineCount: {lex.lineNum}\n");
        }

        protected virtual void OnSpace(char ch){
            Output(ch);
        }

        protected void ColorToken(ELexState lex_state){
            string p;
            switch (lex_state) {
                case ELexState.LEX_NORMAL: {
                    if (lex.curTokenId >= Define.TK_IDENT)
                        SetConsoleTextAttribute(ConsoleColor.Gray, defaultColor);
                    else if (lex.curTokenId >= Define.KW_CHAR)
                        SetConsoleTextAttribute(ConsoleColor.Green, defaultColor);
                    else if (lex.curTokenId >= Define.TK_CINT)
                        SetConsoleTextAttribute(ConsoleColor.Red, defaultColor);
                    else
                        SetConsoleTextAttribute(ConsoleColor.Red, defaultColor);
                    p = lex.GetTokenName(lex.curTokenId);
                    if (lex.curTokenId != Define.TK_EOF) {
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

        protected void Output(string str){
            Console.Write(str);
        }

        protected void Output(char ch){
            Console.Write(ch);
        }
        void OutputTab(int n){
            int i = 0;
            for (; i < n; i++)
                Output("    ");
        }
        void SyntaxIndent(){
            switch (lex.syntax_state) {
                case SNTX_NUL:
                    ColorToken(ELexState.LEX_NORMAL);
                    break;
                case SNTX_SP:
                    Output(" ");
                    ColorToken(ELexState.LEX_NORMAL);
                    break;
                case SNTX_LF_HT: {
                    if (lex.curTokenId == TK_END) // 遇到'}',缩进减少一级
                        lex.syntax_level--;
                    Output("\n");
                    OutputTab(lex.syntax_level);
                }
                    ColorToken(ELexState.LEX_NORMAL);
                    break;
                case SNTX_DELAY:
                    break;
            }

            lex.syntax_state = SNTX_NUL;
        }
    }
}