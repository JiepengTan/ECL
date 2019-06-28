using System;
using System.IO;

namespace LockstepECL {
    public class TestLex {
        InputStream input;
        protected Lex lex = new Lex();
        private int syntax_level;
        private int syntax_state;
        public int SNTX_NUL = 0;

        private ConsoleColor defaultColor;
        public void Init(string path){
            var text = File.ReadAllText(path).Replace("\r\n","\n").Replace("\r","\n");
            defaultColor = Console.ForegroundColor;
            input = new InputStream(text);
            lex.lineNum = 1;
            lex.fileName = path;
            lex.Init(input.GetChar, input.UnChar, OnSpace);

            syntax_state = SNTX_NUL;
            syntax_level = 0;
        }
        void OnSpace(char ch){
            Output(ch);
        }

        public void DumpContext(){
            lex.GetChar();
            do {
                lex.GetToken();
                ColorToken(ELexState.LEX_NORMAL);
            } while (lex.curTokenId != Define.TK_EOF);

            Console.ForegroundColor = defaultColor;
            //Output($"\nTotalLineCount: {lex.lineNum}\n");
        }

        void ColorToken(ELexState lex_state){
            string p;
            switch (lex_state) {
                case ELexState.LEX_NORMAL: {
                    if (lex.curTokenId >= Define.TK_IDENT)
                        SetConsoleTextAttribute(ConsoleColor.Gray,defaultColor);
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

        void SetConsoleTextAttribute(ConsoleColor fgColor, ConsoleColor bgColor = ConsoleColor.Black){
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