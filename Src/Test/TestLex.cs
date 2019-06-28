using System;

namespace LockstepECL {
    public class TestLex {
        protected Lex lex = new Lex();
        private int syntax_level;
        private int syntax_state;
        public int SNTX_NUL = 0;

        public void Init(){
            lex.lineNum = 1;
            lex.Init();

            syntax_state = SNTX_NUL;
            syntax_level = 0;
        }

        public void Test(){
            do {
                lex.GetToken();
                ColorToken(ELexState.LEX_NORMAL);
            } while (lex.curTokenId != Define.TK_EOF);

            Console.WriteLine($"\n代码行数: {ErrorHandler.line_num}行\n");
        }

        void ColorToken(ELexState lex_state){
            string p;
            switch (lex_state) {
                case ELexState.LEX_NORMAL: {
                    if (lex.curTokenId >= Define.TK_IDENT)
                        SetConsoleTextAttribute(ConsoleColor.Gray);
                    else if (lex.curTokenId >= Define.KW_CHAR)
                        SetConsoleTextAttribute(ConsoleColor.Green, ConsoleColor.Black);
                    else if (lex.curTokenId >= Define.TK_CINT)
                        SetConsoleTextAttribute(ConsoleColor.Red, ConsoleColor.Green);
                    else
                        SetConsoleTextAttribute(ConsoleColor.Red, ConsoleColor.Gray);
                    p = lex.GetTokenName(lex.curTokenId);
                    Output(p);
                    break;
                }
                case ELexState.LEX_SEP:
                    Output(lex.curChar);
                    break;
            }
        }

        void SetConsoleTextAttribute(ConsoleColor fgColor, ConsoleColor bgColor = ConsoleColor.Black){
            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = fgColor;
        }

        public void Output(string str){
            Console.Write(str);
        }

        public void Output(char ch){
            Console.Write(ch);
        }
    }
}