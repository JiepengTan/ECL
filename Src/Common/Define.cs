namespace LockstepECL {
    public class Define {
        /* 运算符及分隔符 */

        public static int TK_PLUS = 0;
        public static int TK_MINUS = 1;
        public static int TK_STAR = 2;
        public static int TK_DIVIDE = 3;
        public static int TK_MOD = 4;
        public static int TK_EQ = 5;
        public static int TK_NEQ = 6;
        public static int TK_LT = 7;
        public static int TK_LEQ = 8;
        public static int TK_GT = 9;
        public static int TK_GEQ = 10;
        public static int TK_ASSIGN = 11;
        public static int TK_POINTSTO = 12;
        public static int TK_DOT = 13;
        public static int TK_AND = 14;
        public static int TK_OPENPA = 15;
        public static int TK_CLOSEPA = 16;
        public static int TK_OPENBR = 17;
        public static int TK_CLOSEBR = 18;
        public static int TK_BEGIN = 19;
        public static int TK_END = 20;
        public static int TK_SEMICOLON = 21;
        public static int TK_COMMA = 22;
        public static int TK_ELLIPSIS = 23;
        public static int TK_EOF = 24;
        public static int TK_CINT = 25;
        public static int TK_CCHAR = 26;
        public static int TK_CSTR = 27;
        public static int KW_CHAR = 28;
        public static int KW_SHORT = 29;
        public static int KW_INT = 30;
        public static int KW_VOID = 31;
        public static int KW_STRUCT = 32;
        public static int KW_IF = 33;
        public static int KW_ELSE = 34;
        public static int KW_FOR = 35;
        public static int KW_CONTINUE = 36;
        public static int KW_BREAK = 37;
        public static int KW_RETURN = 38;
        public static int KW_SIZEOF = 39;
        public static int KW_ALIGN = 40;
        public static int KW_CDECL = 41;
        public static int KW_STDCALL = 42;
        public static int TK_IDENT = 43;

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
            new Token(TK_POINTSTO, null, "_.__", null, null),//TODO 去重复
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