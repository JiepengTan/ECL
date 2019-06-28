using System;
using System.IO;
using static ErrorHandler;

namespace LockstepECL {
    public unsafe class DynString {
        public int count; // 字符串长度  
        public int capacity; // 包含该字符串的缓冲区长度
        public string data; // 指向字符串的指针
    }

    public unsafe class DynArray {
        public int count; // 动态数组元素个数
        public int capacity; // 动态数组缓冲区长度度
        public object[] data; // 指向数据指针数组
    }

    public unsafe class Symbol {
        public int ss;
    }

    /* 词法状态 */

    public enum e_LexState {
        LEX_NORMAL,
        LEX_SEP
    };
    /* 单词存储结构定义 */

    public unsafe class TkWord {
        public int tkcode; // 单词编码 

        public TkWord next; // 指向哈希冲突的其它单词
        public string spelling; // 单词字符串

        public Symbol sym_struct; // 指向单词所表示的结构定义

        public Symbol sym_identifier; // 指向单词所表示的标识符

        public TkWord(
            int tkcode,
            TkWord next,
            char spelling,
            Symbol sym_struct,
            Symbol sym_identifier
        ){
            this.tkcode = tkcode;
            this.next = next;
            this.spelling = spelling;
            this.sym_struct = sym_struct;
            this.sym_identifier = sym_identifier;
        }
    }


    /* 错误级别 */

    public enum e_ErrorLevel {
        LEVEL_WARNING,
        LEVEL_ERROR,
    };
    /* 工作阶段 */

    public enum e_WorkStage {
        STAGE_COMPILE,
        STAGE_LINK,
    };
    /*语法状态 */

    public enum e_SynTaxState {
        SNTX_NUL, // 空状态，没有语法缩进动作
        SNTX_SP, // 空格 int a; int __stdcall MessageBoxA(); return 1;
        SNTX_LF_HT, // 换行并缩进，每一个声明、函数定义、语句结束都要置为此状态
        SNTX_DELAY // 延迟取出下一单词后确定输出格式，取出下一个单词后，根据单词类型单独调用syntax_indent确定格式进行输出 
    };
    /* 存储类型 */

    public enum e_StorageClass {
        SC_GLOBAL = 0x00f0, // 包括：包括整型常量，字符常量、字符串常量,全局变量,函数定义          
        SC_LOCAL = 0x00f1, // 栈中变量
        SC_LLOCAL = 0x00f2, // 寄存器溢出存放栈中
        SC_CMP = 0x00f3, // 使用标志寄存器
        SC_VALMASK = 0x00ff, // 存储类型掩码             
        SC_LVAL = 0x0100, // 左值       
        SC_SYM = 0x0200, // 符号	

        SC_ANOM = 0x1000000, // 匿名符号
        SC_STRUCT = 0x2000000, // 结构体符号
        SC_MEMBER = 0x4000000, // 结构成员变量
        SC_PARAMS = 0x8000000, // 函数参数
    };
    /*类型编码 */

    enum e_TypeCode {
        T_INT = 0, // 整型                     
        T_CHAR = 1, // 字符型                 
        T_SHORT = 2, // 短整型                       
        T_VOID = 3, // 空类型                        
        T_PTR = 4, // 指针                          
        T_FUNC = 5, // 函数                    
        T_STRUCT = 6, // 结构体 

        T_BTYPE = 0x000f, // 基本类型掩码          
        T_ARRAY = 0x0010, // 数组
    };
    public unsafe class Lex {
        int TK_PLUS = (int) e_TokenCode.TK_PLUS; // + 加号
        int TK_MINUS = (int) e_TokenCode.TK_MINUS; // - 减号
        int TK_STAR = (int) e_TokenCode.TK_STAR; //  星号
        int TK_DIVIDE = (int) e_TokenCode.TK_DIVIDE; // / 除号
        int TK_MOD = (int) e_TokenCode.TK_MOD; // % 求余运算符
        int TK_EQ = (int) e_TokenCode.TK_EQ; // == 等于号
        int TK_NEQ = (int) e_TokenCode.TK_NEQ; // != 不等于号
        int TK_LT = (int) e_TokenCode.TK_LT; // < 小于号
        int TK_LEQ = (int) e_TokenCode.TK_LEQ; // <= 小于等于号
        int TK_GT = (int) e_TokenCode.TK_GT; // > 大于号
        int TK_GEQ = (int) e_TokenCode.TK_GEQ; // >= 大于等于号
        int TK_ASSIGN = (int) e_TokenCode.TK_ASSIGN; // = 赋值运算符 
        int TK_POINTSTO = (int) e_TokenCode.TK_POINTSTO; // . 指向结构体成员运算符
        int TK_DOT = (int) e_TokenCode.TK_DOT; // . 结构体成员运算符
        int TK_AND = (int) e_TokenCode.TK_AND; //  地址与运算符
        int TK_OPENPA = (int) e_TokenCode.TK_OPENPA; // ( 左圆括号
        int TK_CLOSEPA = (int) e_TokenCode.TK_CLOSEPA; // ) 右圆括号
        int TK_OPENBR = (int) e_TokenCode.TK_OPENBR; // [ 左中括号
        int TK_CLOSEBR = (int) e_TokenCode.TK_CLOSEBR; // ] 右圆括号
        int TK_BEGIN = (int) e_TokenCode.TK_BEGIN; // { 左大括号
        int TK_END = (int) e_TokenCode.TK_END; // } 右大括号
        int TK_SEMICOLON = (int) e_TokenCode.TK_SEMICOLON; // ; 分号    
        int TK_COMMA = (int) e_TokenCode.TK_COMMA; //  =               逗号
        int TK_ELLIPSIS = (int) e_TokenCode.TK_ELLIPSIS; // ... 省略号
        int TK_EOF = (int) e_TokenCode.TK_EOF; // 文件结束符
        int TK_CINT = (int) e_TokenCode.TK_CINT; // 整型常量
        int TK_CCHAR = (int) e_TokenCode.TK_CCHAR; // 字符常量
        int TK_CSTR = (int) e_TokenCode.TK_CSTR; // 字符串常量
        int KW_CHAR = (int) e_TokenCode.KW_CHAR; // char关键字
        int KW_SHORT = (int) e_TokenCode.KW_SHORT; // short关键字
        int KW_INT = (int) e_TokenCode.KW_INT; // int关键字
        int KW_VOID = (int) e_TokenCode.KW_VOID; // void关键字  
        int KW_STRUCT = (int) e_TokenCode.KW_STRUCT; // struct关键字   
        int KW_IF = (int) e_TokenCode.KW_IF; // if关键字
        int KW_ELSE = (int) e_TokenCode.KW_ELSE; // else关键字
        int KW_FOR = (int) e_TokenCode.KW_FOR; // for关键字
        int KW_CONTINUE = (int) e_TokenCode.KW_CONTINUE; // continue关键字
        int KW_BREAK = (int) e_TokenCode.KW_BREAK; // break关键字   
        int KW_RETURN = (int) e_TokenCode.KW_RETURN; // return关键字
        int KW_SIZEOF = (int) e_TokenCode.KW_SIZEOF; // sizeof关键字
        int KW_ALIGN = (int) e_TokenCode.KW_ALIGN; // __align关键字 
        int KW_CDECL = (int) e_TokenCode.KW_CDECL; // __cdecl关键字 standard c call
        int KW_STDCALL = (int) e_TokenCode.KW_STDCALL; // __stdcall关键字 pascal c call
        int TK_IDENT = (int) e_TokenCode.TK_IDENT;

        public const int ALIGN_SET = 0x100; // 强制对齐标志
        public const int MAXKEY = 1024; // 哈希表容量
        public static TkWord[] tk_hashtable = new TkWord[MAXKEY]; // 单词哈希表
        public static DynArray tktable; // 单词表
        public static DynString sourcestr;
        public static DynString tkstr;
        public static int tkvalue;
        public static char ch;
        public static int token;
        public static int line_num;

        public void* realloc(void* pt, int size){
            return null;
        }

        public void* malloc(int size){
            return null;
        }

        public void memcpy(void* src, int start, void* dst, int dstIdx, int size){ }
        public void memset(void* src, int value, int size){ }

        public void free(void* pt){ }

        public void error(string msg){ }

        void* mallocz(int size){
            void* ptr;
            ptr = malloc(size);
            if (ptr != null && size != 0)
                error("mallocz error");
            memset(ptr, 0, size);
            return ptr;
        }

        int elf_hash(string key){
            return key.GetHashCode() % MAXKEY;
        }


        public int SNTX_NUL = 0;
        private int syntax_level;

        private int syntax_state;

        void init(){
            line_num = 1;
            init_lex();

            syntax_state = SNTX_NUL;
            syntax_level = 0;
        }


        void cleanup(){
            int i;
            for (i = TK_IDENT; i < tktable.count; i++) {
                free(tktable.data[i]);
            }

            free(tktable.data);
        }

        char get_file_ext(char fname){
            char p;
            p = strrchr(fname, '.');
            return p + 1;
        }


        void dynarray_realloc(DynArray parr, int new_size){
            int capacity;
            void data;

            capacity = parr.capacity;
            while (capacity < new_size)
                capacity = capacity
            2;
            data = realloc(parr.data, capacity);
            if (data == null)
                error("内存分配失败");
            parr.capacity = capacity;
            parr.data = (void**) data;
        }

        void dynarray_add(DynArray parr, void data){
            int count;
            count = parr.count + 1;
            if (count sizeof(void*) > parr.capacity)
            dynarray_realloc(parr, count sizeof(void*));
            parr.data[count - 1] = data;
            parr.count = count;
        }

        void dynarray_init(DynArray parr, int initsize){
            if (parr != null) {
                parr.data = (void**) malloc(sizeof(char) initsize);
                parr.count = 0;
                parr.capacity = initsize;
            }
        }

/***********************************************************
  功能:	删除动态数组中第i个元素
  parr:	动态数组指针
  i:		第不个元素	
 **********************************************************/
        void dynarray_delete(DynArray parr, int i){
            if (parr.data[i])
                free(parr.data[i]);
            memcpy(parr.data + i, parr.data + i + 1, sizeof(void*)(parr.count - i - 1));
            free(parr.data);
            parr.data = null;
        }

/***********************************************************
   功能:	释放动态数组使用的内存空间
   parr:	动态数组指针
 **********************************************************/
        void dynarray_free(DynArray parr){
            void* p;
            for (p = parr.data; parr.count; ++p, --parr.count)
                if (*p)
                    free(*p);
            free(parr.data);
            parr.data = null;
        }

/***********************************************************
   功能:	动态数组元素查找
   parr:	动态数组指针
   key:	要查找的元素
 **********************************************************/
        int dynarray_search(DynArray parr, int key){
            int i;
            int* p;
            p = (int**) parr.data;
            for (i = 0; i < parr.count; ++i, p++)
                if (key == **p)
                    return i;
            return -1;
        }

/***********************************************************
  功能: 将运算符、关键字、常量直接放入单词表
  tp:	 单词指针											
 **********************************************************/
        TkWord tkword_direct_insert(TkWord tp){
            int keyno;
            tp.sym_identifier = null;
            tp.sym_struct = null;
            dynarray_add(tktable, tp);
            keyno = elf_hash(tp.spelling);
            tp.next = tk_hashtable[keyno];
            tk_hashtable[keyno] = tp;
            return tp;
        }

        TkWord tkword_find(char p, int keyno){
            TkWord tp = null,  *tp1;
            for (tp1 = tk_hashtable[keyno]; tp1; tp1 = tp1.next) {
                if (!strcmp(p, tp1.spelling)) {
                    token = tp1.tkcode;
                    tp = tp1;
                }
            }

            return tp;
        }

        public int strlen(string str){
            return str.Length;
        }

        TkWord tkword_insert(char p){
            TkWord tp;
            int keyno;
            char s;
            char end;
            int length;

            keyno = elf_hash(p);
            tp = tkword_find(p, keyno);
            if (tp == null) {
                length = strlen(p);
                tp = (TkWord*) mallocz(sizeof(TkWord) + length + 1);
                tp.next = tk_hashtable[keyno];
                tk_hashtable[keyno] = tp;
                dynarray_add(tktable, tp);
                tp.tkcode = tktable.count - 1;
                s = (char*) tp + sizeof(TkWord);
                tp.spelling = (char*) s;
                for (end = p + length; p < end;) {
                    *s++ = *p++;
                }

                *s = (char) '\0';

                tp.sym_identifier = null;
                tp.sym_struct = null;
            }

            return tp;
        }

/***********************************************************
  功能:	判断c是否为字母(a-z,A-Z)或下划线(-)
  c:		字符值
 **********************************************************/
        bool is_nodigit(char c){
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   c == '_';
        }

/***********************************************************
  功能:	判断c是否为数字
  c:		字符值
 **********************************************************/
        bool is_digit(char c){
            return c >= '0' && c <= '9';
        }

/***********************************************************
  功能:	从SC源文件中读取一个字符
 **********************************************************/
        void getch(){
            ch = getc(fin); //文件尾返回EOF，其它返回实际字节值		
        }

        void dynstring_reset(DynString tkStr){ }

        void dynstring_chcat(DynString tkStr, char ch){ }

/***********************************************************
  功能:	解析标识符
 **********************************************************/
        TkWord parse_identifier(){
            dynstring_reset(tkstr);
            dynstring_chcat(tkstr, ch);
            getch();
            while (is_nodigit(ch) || is_digit(ch)) {
                dynstring_chcat(tkstr, ch);
                getch();
            }

            dynstring_chcat(tkstr, '\0');
            return tkword_insert(tkstr.data);
        }

        void parse_num(){
            dynstring_reset(tkstr);
            dynstring_reset(sourcestr);
            do {
                dynstring_chcat(tkstr, ch);
                dynstring_chcat(sourcestr, ch);
                getch();
            } while (is_digit(ch));

            if (ch == '.') {
                do {
                    dynstring_chcat(tkstr, ch);
                    dynstring_chcat(sourcestr, ch);
                    getch();
                } while (is_digit(ch));
            }

            dynstring_chcat(tkstr, '\0');
            dynstring_chcat(sourcestr, '\0');
            tkvalue = int.Parse(tkstr.data);
        }

        void parse_string(char sep){
            char c;
            dynstring_reset(tkstr);
            dynstring_reset(sourcestr);
            dynstring_chcat(sourcestr, sep);
            getch();
            for (;;) {
                if (ch == sep)
                    break;
                else if (ch == '\\') {
                    dynstring_chcat(sourcestr, ch);
                    getch();
                    switch (ch) // 解析转义字符
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
                            c = ch;
                            if (c >= '!' && c <= '~')
                                warning("非法转义字符: \'\\%c\'", c); // 33-126 0x21-0x7E可显示字符部分
                            else
                                warning("非法转义字符: \'\\0x%x\'", c);
                            break;
                    }

                    dynstring_chcat(tkstr, c);
                    dynstring_chcat(sourcestr, ch);
                    getch();
                }
                else {
                    dynstring_chcat(tkstr, ch);
                    dynstring_chcat(sourcestr, ch);
                    getch();
                }
            }

            dynstring_chcat(tkstr, '\0');
            dynstring_chcat(sourcestr, sep);
            dynstring_chcat(sourcestr, '\0');
            getch();
        }

        void color_token(e_LexState lex_state){
            string p;
            switch (lex_state) {
                case e_LexState.LEX_NORMAL: {
                    if (token >= TK_IDENT)
                        SetConsoleTextAttribute(ConsoleColor.Gray);
                    else if (token >= KW_CHAR)
                        SetConsoleTextAttribute(ConsoleColor.Green , ConsoleColor.Black);
                    else if (token >= TK_CINT)
                        SetConsoleTextAttribute(ConsoleColor.Red , ConsoleColor.Green);
                    else
                        SetConsoleTextAttribute(ConsoleColor.Red , ConsoleColor.Gray);
                    p = get_tkstr(token);
                    Output( p);
                    break;
                }
                case e_LexState.LEX_SEP:
                    Output( ch);
                    break;
            }
        }

        void SetConsoleTextAttribute(ConsoleColor fgColor,ConsoleColor bgColor = ConsoleColor.Black){
            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = fgColor;
        }

        public void Output(string str){
            Console.Write(str);
        }

        public void Output(char ch){
            Console.Write(ch);
        }

        void init_lex(){
            TkWord tp;

            TkWord[] keywords = new TkWord[] {
                new TkWord(TK_PLUS, null, "+", null, null),
                new TkWord(TK_MINUS, null, "-", null, null),
                new TkWord(TK_STAR, null, "*", null, null),
                new TkWord(TK_DIVIDE, null, "/", null, null),
                new TkWord(TK_MOD, null, "%", null, null),
                new TkWord(TK_EQ, null, "==", null, null),
                new TkWord(TK_NEQ, null, "!=", null, null),
                new TkWord(TK_LT, null, "<", null, null),
                new TkWord(TK_LEQ, null, "<=", null, null),
                new TkWord(TK_GT, null, ">", null, null),
                new TkWord(TK_GEQ, null, ">=", null, null),
                new TkWord(TK_ASSIGN, null, "=", null, null),
                new TkWord(TK_POINTSTO, null, ".", null, null),
                new TkWord(TK_DOT, null, ".", null, null),
                new TkWord(TK_AND, null, "", null, null),
                new TkWord(TK_OPENPA, null, "(", null, null),
                new TkWord(TK_CLOSEPA, null, ")", null, null),
                new TkWord(TK_OPENBR, null, "[", null, null),
                new TkWord(TK_CLOSEBR, null, "]", null, null),
                new TkWord(TK_BEGIN, null, "{", null, null),
                new TkWord(TK_END, null, "}", null, null),
                new TkWord(TK_SEMICOLON, null, ";", null, null),
                new TkWord(TK_COMMA, null, ",", null, null),
                new TkWord(TK_ELLIPSIS, null, "...", null, null),
                new TkWord(TK_EOF, null, "End_Of_File", null, null),

                new TkWord(TK_CINT, null, "整型常量", null, null),
                new TkWord(TK_CCHAR, null, "字符常量", null, null),
                new TkWord(TK_CSTR, null, "字符串常量", null, null),

                new TkWord(KW_CHAR, null, "char", null, null),
                new TkWord(KW_SHORT, null, "short", null, null),
                new TkWord(KW_INT, null, "int", null, null),
                new TkWord(KW_VOID, null, "void", null, null),
                new TkWord(KW_STRUCT, null, "struct", null, null),

                new TkWord(KW_IF, null, "if", null, null),
                new TkWord(KW_ELSE, null, "else", null, null),
                new TkWord(KW_FOR, null, "for", null, null),
                new TkWord(KW_CONTINUE, null, "continue", null, null),
                new TkWord(KW_BREAK, null, "break", null, null),
                new TkWord(KW_RETURN, null, "return", null, null),
                new TkWord(KW_SIZEOF, null, "sizeof", null, null),
                new TkWord(KW_ALIGN, null, "__align", null, null),
                new TkWord(KW_CDECL, null, "__cdecl", null, null),
                new TkWord(KW_STDCALL, null, "__stdcall", null, null),
            };

            dynarray_init(tktable, 8);
            for (tp = keywords[0]; tp.spelling != null; tp++)
                tkword_direct_insert(tp);
        }

/***********************************************************
   功能:	忽略空格,TAB及回车 
 **********************************************************/
        void skip_white_space(){
            while (ch == ' ' || ch == '\t' || ch == '\r') // 忽略空格,和TAB ch =='\n' ||
            {
                if (ch == '\r') {
                    getch();
                    if (ch != '\n')
                        return;
                    line_num++;
                }

                Console.Write(ch); //这句话，决定是否打印空格，如果不输出空格，源码中空格将被去掉，所有源码挤在一起
                getch();
            }
        }

        private FileStream fin;

        void ungetc(char ch, FileStream fin){
            
        }

        char getc(FileStream fin){
            //TODO
            return CH_EOF;
        }

/***********************************************************
   功能:	预处理，忽略分隔符及注释
 **********************************************************/
        void preprocess(){
            while (true) {
                if (ch == ' ' || ch == '\t' || ch == '\r')
                    skip_white_space();
                else if (ch == '/') {
                //向前多读一个字节看是否是注释开始符，猜错了把多读的字符再放回去
                    getch();
                    if (ch == '*') {
                        parse_comment();
                    }
                    else {
                        ungetc(ch, fin); //把一个字符退回到输入流中
                        ch = '/';
                        break;
                    }
                }
                else
                    break;
            }
        }

        private const char CH_EOF = '\0';
        void parse_comment(){
            getch();
            do {
                do {
                    if (ch == '\n' || ch == '*' || ch == CH_EOF)
                        break;
                    else
                        getch();
                } while (true);

                if (ch == '\n') {
                    line_num++;
                    getch();
                }
                else if (ch == '*') {
                    getch();
                    if (ch == '/') {
                        getch();
                        return;
                    }
                }
                else {
                    error("一直到文件尾未看到配对的注释结束符");
                    return;
                }
            } while (true);
        }

        void get_token(){
            preprocess();
            switch (ch) {
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
                    TkWord tp;
                    tp = parse_identifier();
                    token = tp.tkcode;
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
                    parse_num();
                    token = TK_CINT;
                    break;
                case '+':
                    getch();
                    token = TK_PLUS;
                    break;
                case '-':
                    getch();
                    if (ch == '>') {
                        token = TK_POINTSTO;
                        getch();
                    }
                    else
                        token = TK_MINUS;

                    break;
                case '/':
                    token = TK_DIVIDE;
                    getch();
                    break;
                case '%':
                    token = TK_MOD;
                    getch();
                    break;
                case '=':
                    getch();
                    if (ch == '=') {
                        token = TK_EQ;
                        getch();
                    }
                    else
                        token = TK_ASSIGN;

                    break;
                case '!':
                    getch();
                    if (ch == '=') {
                        token = TK_NEQ;
                        getch();
                    }
                    else
                        error("暂不支持'!'(非操作符)");

                    break;
                case '<':
                    getch();
                    if (ch == '=') {
                        token = TK_LEQ;
                        getch();
                    }
                    else
                        token = TK_LT;

                    break;
                case '>':
                    getch();
                    if (ch == '=') {
                        token = TK_GEQ;
                        getch();
                    }
                    else
                        token = TK_GT;

                    break;
                case '.':
                    getch();
                    if (ch == '.') {
                        getch();
                        if (ch != '.')
                            error("省略号拼写错误");
                        else
                            token = TK_ELLIPSIS;
                        getch();
                    }
                    else {
                        token = TK_DOT;
                    }

                    break;
                case '':
                    token = TK_AND;
                    getch();
                    break;
                case ';':
                    token = TK_SEMICOLON;
                    getch();
                    break;
                case ']':
                    token = TK_CLOSEBR;
                    getch();
                    break;
                case '}':
                    token = TK_END;
                    getch();
                    break;
                case ')':
                    token = TK_CLOSEPA;
                    getch();
                    break;
                case '[':
                    token = TK_OPENBR;
                    getch();
                    break;
                case '{':
                    token = TK_BEGIN;
                    getch();
                    break;
                case ',':
                    token = TK_COMMA;
                    getch();
                    break;
                case '(':
                    token = TK_OPENPA;
                    getch();
                    break;
                case '*':
                    token = TK_STAR;
                    getch();
                    break;
                case '\'':
                    parse_string(ch);
                    token = TK_CCHAR;
                    tkvalue = *(char*) tkstr.data;
                    break;
                case '\"': {
                    parse_string(ch);
                    token = TK_CSTR;
                    break;
                }
                case '\0':
                    token = TK_EOF;
                    break;
                default:
                    error($"不认识的字符:{ch}" ); //上面字符以外的字符，只允许出现在源码字符串，不允许出现的源码的其它位置
                    getch();
                    break;
            }
        }

        string get_tkstr(int v){
            if (v > tktable.count)
                return null;
            else if (v >= TK_CINT && v <= TK_CSTR)
                return sourcestr.data;
            else
                return ((TkWord) tktable.data[v]).spelling;
        }

        void test_lex(){
            do {
                get_token();
                color_token(e_LexState.LEX_NORMAL);
            } while (token != TK_EOF);

            Console.WriteLine($"\n代码行数: {line_num}行\n");
        }
    }
}