using System;
using System.Collections.Generic;
using static ErrorHandler;
using static LockstepECL.Define;

namespace LockstepECL {
    public class Lex {
        public const int ALIGN_SET = 0x100; // 强制对齐标志
        public const int MAXKEY = 1024; // 哈希表容量
        public Dictionary<string, Token> str2Token = new Dictionary<string, Token>();
        public List<Token> allTokens; // 单词表
        public DynString sourcestr;
        public DynString tkstr;
        public object tkvalue;
        public char curChar;
        public int curTokenId;
        public int lineNum;


        InputStream input = new InputStream();
        public Action<char> FuncDealSpace;
        private const char CH_EOF = '\0';

        void UnChar(char ch){
            input.ungetc(ch);
        }
        void GetChar(){
            curChar = input.getc(); //文件尾返回EOF，其它返回实际字节值		
        }

        public static void Expect(string msg){
            Error("缺少 "+ msg);
        }
        void SkipToken(int v){
            if (curTokenId != v)
                Error("缺少"+ GetTokenName(v));
            GetToken();
        }

        Token FindToken(string p){
            if (str2Token.TryGetValue(p, out var tk)) {
                return tk;
            }

            return null;
        }

        Token InsertToken(string p){
            var tp = FindToken(p);
            if (tp == null) {
                tp = new Token();
                tp.name = p;
                tp.symIdentifier = null;
                tp.symStruct = null;
                tp.id = allTokens.Count;
                allTokens.Add(tp);
                str2Token.Add(p, tp);
            }

            return tp;
        }

        bool IsLetter(char c){
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   c == '_';
        }

        bool IsDigit(char c){
            return c >= '0' && c <= '9';
        }


        Token ParseIdentifier(){
            tkstr.Clear();
            tkstr.AddCh(curChar);
            GetChar();
            while (IsLetter(curChar) || IsDigit(curChar)) {
                tkstr.AddCh(curChar);
                GetChar();
            }
            return InsertToken(tkstr.data);
        }

        void ParseNumber(){
            tkstr.Clear();
            sourcestr.Clear();
            do {
                tkstr.AddCh(curChar);
                sourcestr.AddCh(curChar);
                GetChar();
            } while (IsDigit(curChar));

            if (curChar == '.') {
                do {
                    tkstr.AddCh(curChar);
                    sourcestr.AddCh(curChar);
                    GetChar();
                } while (IsDigit(curChar));
            }

            tkvalue = int.Parse(tkstr.data);
        }

        void ParseString(char sep){
            tkstr.Clear();
            sourcestr.Clear();
            sourcestr.AddCh(sep);
            char c = '\0';
            GetChar();
            for (;;) {
                if (curChar == sep)
                    break;
                else if (curChar == '\\') {
                    sourcestr.AddCh(curChar);
                    GetChar();
                    switch (curChar) // 解析转义字符
                    {
                        case '0':
                            c = '\0';
                            break;
                        case 'a':
                            c = '\a';
                            break;
                        case 'b':
                            c = '\b';
                            break;
                        case 't':
                            c = '\t';
                            break;
                        case 'n':
                            c = '\n';
                            break;
                        case 'v':
                            c = '\v';
                            break;
                        case 'f':
                            c = '\f';
                            break;
                        case 'r':
                            c = '\r';
                            break;
                        case '\"':
                            c = '\"';
                            break;
                        case '\'':
                            c = '\'';
                            break;
                        case '\\':
                            c = '\\';
                            break;
                        default:
                            c = curChar;
                            if (c >= '!' && c <= '~')
                                Warning($"非法转义字符: \'\\{c}\'"); // 33-126 0x21-0x7E可显示字符部分
                            else
                                Warning($"非法转义字符: \'\\{c}\'");
                            break;
                    }

                    tkstr.AddCh(c);
                    sourcestr.AddCh(curChar);
                    GetChar();
                }
                else {
                    tkstr.AddCh(curChar);
                    sourcestr.AddCh(curChar);
                    GetChar();
                }
            }

            sourcestr.AddCh(sep);
            GetChar();
        }

        void ParseSpace(){
            while (curChar == ' ' || curChar == '\t' || curChar == '\r') // 忽略空格,和TAB ch =='\n' ||
            {
                if (curChar == '\r') {
                    GetChar();
                    if (curChar != '\n')
                        return;
                    lineNum++;
                }

                FuncDealSpace(curChar); //这句话，决定是否打印空格，如果不输出空格，源码中空格将被去掉，所有源码挤在一起
                GetChar();
            }
        }


        public void Init(){
            allTokens.AddRange(keywords);
            for (int i = 0; i < keywords.Length; i++) {
                str2Token.Add(keywords[i].name, keywords[i]);
            }
        }

        void Preprocess(){
            while (true) {
                if (curChar == ' ' || curChar == '\t' || curChar == '\r')
                    ParseSpace();
                else if (curChar == '/') {
                    //向前多读一个字节看是否是注释开始符，猜错了把多读的字符再放回去
                    GetChar();
                    if (curChar == '*') {
                        ParseComment();
                    }
                    else {
                        UnChar(curChar); //把一个字符退回到输入流中
                        curChar = '/';
                        break;
                    }
                }
                else
                    break;
            }
        }


        void ParseComment(){
            GetChar();
            do {
                do {
                    if (curChar == '\n' || curChar == '*' || curChar == CH_EOF)
                        break;
                    else
                        GetChar();
                } while (true);

                if (curChar == '\n') {
                    lineNum++;
                    GetChar();
                }
                else if (curChar == '*') {
                    GetChar();
                    if (curChar == '/') {
                        GetChar();
                        return;
                    }
                }
                else {
                    Error("一直到文件尾未看到配对的注释结束符");
                    return;
                }
            } while (true);
        }

        public void GetToken(){
            Preprocess();
            switch (curChar) {
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '_': {
                    Token tp;
                    tp = ParseIdentifier();
                    curTokenId = tp.id;
                    break;
                }
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    ParseNumber();
                    curTokenId = TK_CINT;
                    break;
                case '+':
                    GetChar();
                    curTokenId = TK_PLUS;
                    break;
                case '-':
                    GetChar();
                    if (curChar == '>') {
                        curTokenId = TK_POINTSTO;
                        GetChar();
                    }
                    else
                        curTokenId = TK_MINUS;

                    break;
                case '/':
                    curTokenId = TK_DIVIDE;
                    GetChar();
                    break;
                case '%':
                    curTokenId = TK_MOD;
                    GetChar();
                    break;
                case '=':
                    GetChar();
                    if (curChar == '=') {
                        curTokenId = TK_EQ;
                        GetChar();
                    }
                    else
                        curTokenId = TK_ASSIGN;

                    break;
                case '!':
                    GetChar();
                    if (curChar == '=') {
                        curTokenId = TK_NEQ;
                        GetChar();
                    }
                    else
                        Error("暂不支持'!'(非操作符)");

                    break;
                case '<':
                    GetChar();
                    if (curChar == '=') {
                        curTokenId = TK_LEQ;
                        GetChar();
                    }
                    else
                        curTokenId = TK_LT;

                    break;
                case '>':
                    GetChar();
                    if (curChar == '=') {
                        curTokenId = TK_GEQ;
                        GetChar();
                    }
                    else
                        curTokenId = TK_GT;

                    break;
                case '.':
                    GetChar();
                    if (curChar == '.') {
                        GetChar();
                        if (curChar != '.')
                            Error("省略号拼写错误");
                        else
                            curTokenId = TK_ELLIPSIS;
                        GetChar();
                    }
                    else {
                        curTokenId = TK_DOT;
                    }

                    break;
                case '&':
                    curTokenId = TK_AND;
                    GetChar();
                    break;
                case ';':
                    curTokenId = TK_SEMICOLON;
                    GetChar();
                    break;
                case ']':
                    curTokenId = TK_CLOSEBR;
                    GetChar();
                    break;
                case '}':
                    curTokenId = TK_END;
                    GetChar();
                    break;
                case ')':
                    curTokenId = TK_CLOSEPA;
                    GetChar();
                    break;
                case '[':
                    curTokenId = TK_OPENBR;
                    GetChar();
                    break;
                case '{':
                    curTokenId = TK_BEGIN;
                    GetChar();
                    break;
                case ',':
                    curTokenId = TK_COMMA;
                    GetChar();
                    break;
                case '(':
                    curTokenId = TK_OPENPA;
                    GetChar();
                    break;
                case '*':
                    curTokenId = TK_STAR;
                    GetChar();
                    break;
                case '\'':
                    ParseString(curChar);
                    curTokenId = TK_CCHAR;
                    tkvalue = tkstr.data;
                    break;
                case '\"': {
                    ParseString(curChar);
                    curTokenId = TK_CSTR;
                    break;
                }
                case '\0':
                    curTokenId = TK_EOF;
                    break;
                default:
                    Error($"不认识的字符:{curChar}"); //上面字符以外的字符，只允许出现在源码字符串，不允许出现的源码的其它位置
                    GetChar();
                    break;
            }
        }

        public string GetTokenName(int v){
            if (v > allTokens.Count)
                return null;
            else if (v >= TK_CINT && v <= TK_CSTR)
                return sourcestr.data;
            else
                return (allTokens[v]).name;
        }
    }
}