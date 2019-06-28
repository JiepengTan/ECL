namespace LockstepECL {
    public class Define {
        /* 运算符及分隔符 */

        public const int TK_PLUS = 0;
        public const int TK_MINUS = 1;
        public const int TK_STAR = 2;
        public const int TK_DIVIDE = 3;
        public const int TK_MOD = 4;
        public const int TK_EQ = 5;
        public const int TK_NEQ = 6;
        public const int TK_LT = 7;
        public const int TK_LEQ = 8;
        public const int TK_GT = 9;
        public const int TK_GEQ = 10;
        public const int TK_ASSIGN = 11;
        public const int TK_POINTSTO = 12;
        public const int TK_DOT = 13;
        public const int TK_AND = 14;
        public const int TK_OPENPA = 15;
        public const int TK_CLOSEPA = 16;
        public const int TK_OPENBR = 17;
        public const int TK_CLOSEBR = 18;
        public const int TK_BEGIN = 19;
        public const int TK_END = 20;
        public const int TK_SEMICOLON = 21;
        public const int TK_COMMA = 22;
        public const int TK_ELLIPSIS = 23;
        public const int TK_EOF = 24;
        public const int TK_CINT = 25;
        public const int TK_CCHAR = 26;
        public const int TK_CSTR = 27;
        public const int KW_CHAR = 28;
        public const int KW_SHORT = 29;
        public const int KW_INT = 30;
        public const int KW_VOID = 31;
        public const int KW_STRUCT = 32;
        public const int KW_IF = 33;
        public const int KW_ELSE = 34;
        public const int KW_FOR = 35;
        public const int KW_CONTINUE = 36;
        public const int KW_BREAK = 37;
        public const int KW_RETURN = 38;
        public const int KW_SIZEOF = 39;
        public const int KW_ALIGN = 40;
        public const int KW_CDECL = 41;
        public const int KW_STDCALL = 42;
        public const int TK_IDENT = 43;

        private static int NumOfToken = 43;

        public static Token[] keywords = new Token[] {
            new Token(TK_PLUS, null, "+", null, null),
            new Token(TK_MINUS, null, "-", null, null),
            new Token(TK_STAR, null, "*", null, null),
            new Token(TK_DIVIDE, null, "/", null, null),
            new Token(TK_MOD, null, "%", null, null),
            new Token(TK_EQ, null, "==", null, null),
            new Token(TK_NEQ, null, "!=", null, null),
            new Token(TK_LT, null, "<", null, null),
            new Token(TK_LEQ, null, "<=", null, null),
            new Token(TK_GT, null, ">", null, null),
            new Token(TK_GEQ, null, ">=", null, null),
            new Token(TK_ASSIGN, null, "=", null, null),
            new Token(TK_POINTSTO, null, "__.___", null, null),//TODO 去重复
            new Token(TK_DOT, null, ".", null, null),
            new Token(TK_AND, null, "", null, null),
            new Token(TK_OPENPA, null, "(", null, null),
            new Token(TK_CLOSEPA, null, ")", null, null),
            new Token(TK_OPENBR, null, "[", null, null),
            new Token(TK_CLOSEBR, null, "]", null, null),
            new Token(TK_BEGIN, null, "{", null, null),
            new Token(TK_END, null, "}", null, null),
            new Token(TK_SEMICOLON, null, ";", null, null),
            new Token(TK_COMMA, null, ",", null, null),
            new Token(TK_ELLIPSIS, null, "...", null, null),
            new Token(TK_EOF, null, "End_Of_File", null, null),
            new Token(TK_CINT, null, "整型常量", null, null),
            new Token(TK_CCHAR, null, "字符常量", null, null),
            new Token(TK_CSTR, null, "字符串常量", null, null),
            new Token(KW_CHAR, null, "char", null, null),
            new Token(KW_SHORT, null, "short", null, null),
            new Token(KW_INT, null, "int", null, null),
            new Token(KW_VOID, null, "void", null, null),
            new Token(KW_STRUCT, null, "struct", null, null),
            new Token(KW_IF, null, "if", null, null),
            new Token(KW_ELSE, null, "else", null, null),
            new Token(KW_FOR, null, "for", null, null),
            new Token(KW_CONTINUE, null, "continue", null, null),
            new Token(KW_BREAK, null, "break", null, null),
            new Token(KW_RETURN, null, "return", null, null),
            new Token(KW_SIZEOF, null, "sizeof", null, null),
            new Token(KW_ALIGN, null, "__align", null, null),
            new Token(KW_CDECL, null, "__cdecl", null, null),
            new Token(KW_STDCALL, null, "__stdcall", null, null),
            new Token(TK_IDENT, null, "__identify", null, null)
        };
    }
}