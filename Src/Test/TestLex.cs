using System;
using System.IO;
using static LockstepECL.Define;
using static LockstepECL.ConstDefine;

namespace LockstepECL {
    public class TestBaseParser {
        protected InputStream input;
        protected ConsoleColor defaultColor;

        protected virtual void OnSpace(char ch){
            Output(ch);
        }

        protected void Output(string str){
            Console.Write(str);
        }

        protected void Output(char ch){
            Console.Write(ch);
        }
    }

    public class TestLex : TestBaseParser {
        protected Lex lex;

        public virtual void Init(string path){
            lex = new Lex();
            var text = File.ReadAllText(path).Replace("\r\n", "\n").Replace("\r", "\n");
            input = new InputStream(text);
            lex.lineNum = 1;
            lex.filePath = path;
            lex.Init(input.GetChar, input.UnChar, OnSpace, LexIndent);
        }

        protected virtual void LexIndent(){
            var name = lex.GetTokenName(lex.curTokenId);
            if (lex.curTokenId != Define.TK_EOF) {
                Output(name);
            }
        }

        public void ShowLexResult(){
            try {
                lex.DoParse();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
            finally {
                lex.DumpErrorInfo();
            }
        }
    }
}