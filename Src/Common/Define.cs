namespace LockstepECL {
    public class Define {
        public const int TK_PLUS        = 0 ;
        public const int TK_MINUS       = 1 ;
        public const int TK_STAR        = 2 ;
        public const int TK_DIVIDE      = 3 ;
        public const int TK_MOD         = 4 ;
        public const int TK_EQ          = 5 ;
        public const int TK_NEQ         = 6 ;
        public const int TK_LT          = 7 ;
        public const int TK_LEQ         = 8 ;
        public const int TK_GT          = 9 ;
        public const int TK_GEQ         = 10;
        public const int TK_ASSIGN      = 11;
        public const int TK_POINTSTO    = 12;
        public const int TK_DOT         = 13;
        public const int TK_AND         = 14;
        public const int TK_OPENPA      = 15;
        public const int TK_CLOSEPA     = 16;
        public const int TK_OPENBR      = 17;
        public const int TK_CLOSEBR     = 18;
        public const int TK_BEGIN       = 19;
        public const int TK_END         = 20;
        public const int TK_SEMICOLON   = 21;
        public const int TK_COMMA       = 22;
        public const int TK_ELLIPSIS    = 23;
        public const int TK_EOF         = 24;
        public const int TK_CINT        = 25;
        public const int TK_LFloat      = 26;
        public const int TK_CCHAR       = 27;
        public const int TK_CSTR        = 28;
        public const int KW_CHAR        = 29;
        public const int KW_SHORT       = 30;
        public const int KW_INT         = 31;
        public const int KW_FLOAT       = 32;
        public const int KW_VOID        = 33;
        public const int KW_STRUCT      = 34;
        public const int KW_IF          = 35;
        public const int KW_ELSE        = 36;
        public const int KW_FOR         = 37;
        public const int KW_CONTINUE    = 38;
        public const int KW_BREAK       = 39;
        public const int KW_RETURN      = 40;
        public const int KW_SIZEOF      = 41;
        public const int KW_ALIGN       = 42;
        public const int KW_CDECL       = 43;
        public const int KW_STDCALL     = 44;
        public const int TK_IDENT       = 45;

        private static int NumOfToken = TK_IDENT + 1;
           
        
        public static Token[] keywords = new Token[] {
            new Token(TK_PLUS       ,"+",               null, null),         
            new Token(TK_MINUS      ,"-",               null, null),         
            new Token(TK_STAR       ,"*",               null, null),         
            new Token(TK_DIVIDE     ,"/",               null, null),         
            new Token(TK_MOD        ,"%",               null, null),         
            new Token(TK_EQ         ,"==",              null, null),         
            new Token(TK_NEQ        ,"!=",              null, null),         
            new Token(TK_LT         ,"<",               null, null),         
            new Token(TK_LEQ        ,"<=",              null, null),         
            new Token(TK_GT         ,">",               null, null),         
            new Token(TK_GEQ        ,">=",              null, null),         
            new Token(TK_ASSIGN     ,"=",               null, null),         
            new Token(TK_POINTSTO   ,"__.___",          null, null), //TODO 去重复         
            new Token(TK_DOT        ,".",               null, null),         
            new Token(TK_AND        ,"",                null, null),         
            new Token(TK_OPENPA     ,"(",               null, null),         
            new Token(TK_CLOSEPA    ,")",               null, null),         
            new Token(TK_OPENBR     ,"[",               null, null),         
            new Token(TK_CLOSEBR    ,"]",               null, null),         
            new Token(TK_BEGIN      ,"{",               null, null),         
            new Token(TK_END        ,"}",               null, null),         
            new Token(TK_SEMICOLON  ,";",               null, null),         
            new Token(TK_COMMA      ,",",               null, null),         
            new Token(TK_ELLIPSIS   ,"...",             null, null),         
            new Token(TK_EOF        ,"End_Of_File",     null, null),         
            new Token(TK_LFloat     ,"浮点常量",          null, null),         
            new Token(TK_CINT       ,"整型常量",          null, null),         
            new Token(TK_CCHAR      ,"字符常量",          null, null),         
            new Token(TK_CSTR       ,"字符串常量",         null, null),         
            new Token(KW_CHAR       ,"char",            null, null),         
            new Token(KW_SHORT      ,"short",           null, null),         
            new Token(KW_INT        ,"int",             null, null),           
            new Token(KW_FLOAT      ,"float",           null, null),       
            new Token(KW_VOID       ,"void",            null, null),         
            new Token(KW_STRUCT     ,"struct",          null, null),         
            new Token(KW_IF         ,"if",              null, null),         
            new Token(KW_ELSE       ,"else",            null, null),         
            new Token(KW_FOR        ,"for",             null, null),         
            new Token(KW_CONTINUE   ,"continue",        null, null),         
            new Token(KW_BREAK      ,"break",           null, null),         
            new Token(KW_RETURN     ,"return",          null, null),         
            new Token(KW_SIZEOF     ,"sizeof",          null, null),         
            new Token(KW_ALIGN      ,"__align",         null, null),         
            new Token(KW_CDECL      ,"__cdecl",         null, null),         
            new Token(KW_STDCALL    ,"__stdcall",       null, null),                    
            new Token(TK_IDENT      ,"__identify",      null, null),        
        };
    }
    
}

    
   
    
  
     
      
     
      
     
      
     
  

     
     
  
 
  
 
   
     
 
   
 
     
    
   
    
    
   
     
    
  
      
    
     

   
  
  
   
   
 
   















































