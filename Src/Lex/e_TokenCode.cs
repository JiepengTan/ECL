namespace LockstepECL {
    public enum e_TokenCode {
        /* 运算符及分隔符 */
        TK_PLUS            = 0,       // + 加号
        TK_MINUS           = 1,        // - 减号
        TK_STAR            = 2,       // * 星号
        TK_DIVIDE          = 3,         // / 除号
        TK_MOD             = 4,      // % 求余运算符
        TK_EQ              = 5,     // == 等于号
        TK_NEQ             = 6,      // != 不等于号
        TK_LT              = 7,     // < 小于号
        TK_LEQ             = 8,      // <= 小于等于号
        TK_GT              = 9,     // > 大于号
        TK_GEQ             = 10,     // >= 大于等于号
        TK_ASSIGN          = 11,        // = 赋值运算符 
        TK_POINTSTO        = 12,          // -> 指向结构体成员运算符
        TK_DOT             = 13,     // . 结构体成员运算符
        TK_AND             = 14,     // & 地址与运算符
        TK_OPENPA          = 15,        // ( 左圆括号
        TK_CLOSEPA         = 16,         // ) 右圆括号
        TK_OPENBR          = 17,        // [ 左中括号
        TK_CLOSEBR         = 18,         // ] 右圆括号
        TK_BEGIN           = 19,       // { 左大括号
        TK_END             = 20,     // } 右大括号
        TK_SEMICOLON       = 21,           // , 分号    
        TK_COMMA           = 22,       //                  逗号
        TK_ELLIPSIS        = 23,          // ... 省略号
        TK_EOF             = 24,     // 文件结束符
        TK_CINT            = 25,      // 整型常量
        TK_CCHAR           = 26,       // 字符常量
        TK_CSTR            = 27,      // 字符串常量
        KW_CHAR            = 28,      // char关键字
        KW_SHORT           = 29,       // short关键字
        KW_INT             = 30,     // int关键字
        KW_VOID            = 31,      // void关键字  
        KW_STRUCT          = 32,        // struct关键字   
        KW_IF              = 33,    // if关键字
        KW_ELSE            = 34,      // else关键字
        KW_FOR             = 35,     // for关键字
        KW_CONTINUE        = 36,          // continue关键字
        KW_BREAK           = 37,       // break关键字   
        KW_RETURN          = 38,        // return关键字
        KW_SIZEOF          = 39,        // sizeof关键字
        KW_ALIGN           = 40,       // __align关键字	
        KW_CDECL           = 41,       // __cdecl关键字 standard c call
        KW_STDCALL         = 42,         // __stdcall关键字 pascal c call
        TK_IDENT           = 43,      /* 标识符 */
    }
}