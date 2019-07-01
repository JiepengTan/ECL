using System;
using System.Collections.Generic;
using System.Text;
using static LogHandler;
using static LockstepECL.Define;
using static ETipsType;

namespace LockstepECL {
    public class BaseParser {
        protected LogHandler _logHandler = new LogHandler();
        public string fileName = "";
        public int lineNum;
        public int colNum = 0;

        public virtual void Warning(ETipsType type, params object[] args){
            _logHandler.HandlerLog(EWorkStage.COMPILE, EErrorLevel.WARNING, type, fileName, lineNum, colNum, args);
        }

        public virtual void Error(ETipsType type, params object[] args){
            _logHandler.HandlerLog(EWorkStage.COMPILE, EErrorLevel.ERROR, type, fileName, lineNum, colNum, args);
        }

        public void DumpErrorInfo(){
            _logHandler.DumpErrorInfo();
        }
    }

    public interface ILex {
        LexInfos LexInfos { get; }
        void Init(Func<char> funcGetChar, Action<char> funcUnChar, Action<char> funcDealSpace,Action funcSyntaxIndent);
        void Reset();
        void DoParse();
    }

    public class LexInfos {
        public struct Info {
            public int line;
            public int tokenId;
            public object tokenVal;

            public override string ToString(){
                return $"line:{line} tokenId{tokenId}";
            }
        }

        public LexInfos(List<Token> tokenTable){
            this.tokenTable = tokenTable;
            tokenInfos = new List<Info>();
        }

        public List<Token> tokenTable;
        public List<Info> tokenInfos;

        public void OnToken(int line, int tokenId, object tokenVal){
            tokenInfos.Add(new Info() {line = line, tokenId = tokenId, tokenVal = tokenVal});
        }

        private int _curIdx = -1;

        public void GetToken(){
            if (_curIdx >= tokenInfos.Count) {
                return;
            }

            _curIdx++;
        }

        public void SkipToken(){
            GetToken();
        }

        public void GetTokenInfo(int tokenId){
            
        }

        public string GetTokenName(int tokenId){
            return tokenId > tokenTable.Count ? "" : tokenTable[tokenId].name;
        }

        public string GetTokenDebugString(){
            var tokenId = tokenInfos[_curIdx].tokenId;
            if (tokenId >= TK_CINT && tokenId <= TK_CSTR)
                return tokenInfos[_curIdx].tokenVal.ToString();
            else
                return tokenTable[tokenId].name;
        }

        public int TokenTableCount => tokenTable.Count;
        public int curTokenId => tokenInfos[_curIdx].tokenId;
        public object curTokenVal => tokenInfos[_curIdx].tokenVal;

        public override string ToString(){
            int curLine = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var info in tokenInfos) {
                sb.Append(tokenTable[info.tokenId].name + " ");
                if (info.line != curLine) {
                    curLine = info.line;
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }

    public partial class Lex : BaseParser, ILex {
        public int curTokenId;
        public Dictionary<string, Token> str2Token;
        public List<Token> allTokens;
        public DynString sourcestr;
        public DynString tkstr;
        public object tkvalue;
        public char curChar;
        private bool hasTokenInLine = false; // cur line has some identifier 

        public Token __debugToken => allTokens[curTokenId];

        private Action<char> FuncDealSpace;
        private Action<char> FuncUnChar;
        private Func<char> FuncGetChar;
        public Action FuncSyntaxIndent;

        public LexInfos LexInfos { get; private set; }


        public void Init(
            Func<char> funcGetChar, Action<char> funcUnChar,
            Action<char> funcDealSpace,Action funcSyntaxIndent
        ){
            this.FuncUnChar = funcUnChar;
            this.FuncGetChar = funcGetChar;
            this.FuncDealSpace = funcDealSpace;
            this.FuncSyntaxIndent = funcSyntaxIndent;
            Reset();
        }

        public void Reset(){
            str2Token = new Dictionary<string, Token>();
            allTokens = new List<Token>();
            sourcestr = new DynString();
            tkstr = new DynString();
            LexInfos = new LexInfos(allTokens);
            allTokens.AddRange(keywords);
            for (int i = 0; i < keywords.Length; i++) {
                str2Token.Add(keywords[i].name, keywords[i]);
            }
        }

        public void DoParse(){
            GetChar();
            do {
                GetToken();
            } while (curTokenId != Define.TK_EOF);

        }


        void OnReadLine(){
            hasTokenInLine = false;
            colNum = 0;
            lineNum++;
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
                tp = new Token {name = p, id = allTokens.Count};
                allTokens.Add(tp);
                str2Token.Add(p, tp);
            }

            return tp;
        }

        #region Parse Func

        Token ParseIdentifier(){
            tkstr.Clear();
            tkstr.AddCh(curChar);
            GetChar();
            while (IsLetter(curChar) || IsDigit(curChar)) {
                tkstr.AddCh(curChar);
                GetChar();
            }

            return InsertToken(tkstr.Data);
        }

        bool ParseNumber(){
            tkstr.Clear();
            sourcestr.Clear();
            do {
                tkstr.AddCh(curChar);
                sourcestr.AddCh(curChar);
                GetChar();
            } while (IsDigit(curChar));

            bool isFloat = false;
            if (curChar == '.') {
                isFloat = true;
                do {
                    tkstr.AddCh(curChar);
                    sourcestr.AddCh(curChar);
                    GetChar();
                } while (IsDigit(curChar));
            }

            if (isFloat) {
                tkvalue = (float) double.Parse(tkstr.Data);
            }
            else {
                tkvalue = int.Parse(tkstr.Data);
            }

            return isFloat;
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
                                Warning(ETipsType.IllegalEscapeCharacter, c.ToString());
                            else
                                Warning(ETipsType.IllegalEscapeCharacter, c.ToString());
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
            while (curChar == ' ' || curChar == '\t' || curChar == '\r' || curChar == '\n') // 忽略空格,和TAB ch =='\n' ||
            {
                if (curChar == '\r') {
                    GetChar();
                    if (curChar != '\n')
                        return;
                    OnReadLine();
                }

                if (curChar == '\n') {
                    OnReadLine();
                }

                FuncDealSpace?.Invoke(curChar); //这句话，决定是否打印空格，如果不输出空格，源码中空格将被去掉，所有源码挤在一起
                GetChar();
            }
        }

        void ParseCommentCppStyle(){
            while (curChar != '\n') {
                GetChar();
            }

            if (hasTokenInLine) {
                FuncDealSpace(curChar);
            }

            OnReadLine();
            GetChar();
        }


        void ParseCommentCStyle(){
            GetChar();
            do {
                do {
                    if (curChar == '\n' || curChar == '*' || curChar == '\0')
                        break;
                    else
                        GetChar();
                } while (true);

                if (curChar == '\n') {
                    OnReadLine();
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
                    Error(ETipsType.MissChar, "* or //");
                    return;
                }
            } while (true);
        }

        void Preprocess(){
            while (true) {
                if (curChar == ' ' || curChar == '\t' || curChar == '\r' || curChar == '\n')
                    ParseSpace();
                else if (curChar == '/') {
                    //向前多读一个字节看是否是注释开始符，猜错了把多读的字符再放回去
                    GetChar();
                    if (curChar == '*') {
                        ParseCommentCStyle();
                    }
                    else if (curChar == '/') {
                        ParseCommentCppStyle();
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

        #endregion

        #region Reader Func

        void UnChar(char ch){
            FuncUnChar(ch);
            --colNum;
        }

        void GetChar(){
            curChar = FuncGetChar(); //文件尾返回EOF，其它返回实际字节值		
            ++colNum;
        }

        public void SkipToken(int tokenId){
            if (curTokenId != tokenId)
                Error(ETipsType.ExpectToken, (allTokens[tokenId]).name);
            GetToken();
        }

        public void GetToken(){
            Preprocess();
            var rawLine = lineNum;
            hasTokenInLine = true;
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
                    var isFloat = ParseNumber();
                    curTokenId = isFloat ? TK_LFloat : TK_CINT;
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
                        Error(ETipsType.ErrorChar, "!");

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
                            Error(ETipsType.ErrorChar, curChar.ToString());
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
                    tkvalue = tkstr.Data;
                    break;
                case '\"': {
                    ParseString(curChar);
                    curTokenId = TK_CSTR;
                    tkvalue = sourcestr.Data;
                    break;
                }
                case '\0':
                    curTokenId = TK_EOF;
                    break;
                default:
                    //上面字符以外的字符，只允许出现在源码字符串，不允许出现的源码的其它位置
                    Error(ETipsType.ErrorChar, curChar.ToString());
                    GetChar();
                    return;
            }

            LexInfos.OnToken(rawLine, curTokenId, tkvalue);
            SyntaxIndent();
        }


        void SyntaxIndent(){
            FuncSyntaxIndent?.Invoke();
        }
        
        public string GetTokenName(int v){
            if (v > allTokens.Count)
                return null;
            else if (v >= TK_CINT && v <= TK_CSTR)
                return sourcestr.Data;
            else
                return (allTokens[v]).name;
        }

        #endregion

        public override string ToString(){
            return $"line:{lineNum} col:{colNum} ch:{curChar} id:{curTokenId} tkstr:{tkstr.Data}";
        }

        static bool IsLetter(char c){
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   c == '_';
        }

        static bool IsDigit(char c){
            return c >= '0' && c <= '9';
        }
    }
}