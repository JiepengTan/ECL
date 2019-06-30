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
            new Token(TK_PLUS       ,"+",                null),         
            new Token(TK_MINUS      ,"-",                null),         
            new Token(TK_STAR       ,"*",                null),         
            new Token(TK_DIVIDE     ,"/",                null),         
            new Token(TK_MOD        ,"%",                null),         
            new Token(TK_EQ         ,"==",               null),         
            new Token(TK_NEQ        ,"!=",               null),         
            new Token(TK_LT         ,"<",                null),         
            new Token(TK_LEQ        ,"<=",               null),         
            new Token(TK_GT         ,">",                null),         
            new Token(TK_GEQ        ,">=",               null),         
            new Token(TK_ASSIGN     ,"=",                null),         
            new Token(TK_POINTSTO   ,"->",               null),          
            new Token(TK_DOT        ,".",                null),         
            new Token(TK_AND        ,"",                 null),         
            new Token(TK_OPENPA     ,"(",                null),         
            new Token(TK_CLOSEPA    ,")",                null),         
            new Token(TK_OPENBR     ,"[",                null),         
            new Token(TK_CLOSEBR    ,"]",                null),         
            new Token(TK_BEGIN      ,"{",                null),         
            new Token(TK_END        ,"}",                null),         
            new Token(TK_SEMICOLON  ,";",                null),         
            new Token(TK_COMMA      ,",",                null),         
            new Token(TK_ELLIPSIS   ,"...",              null),         
            new Token(TK_EOF        ,"End_Of_File",      null),         
            new Token(TK_LFloat     ,"浮点常量",           null),         
            new Token(TK_CINT       ,"整型常量",           null),         
            new Token(TK_CCHAR      ,"字符常量",           null),         
            new Token(TK_CSTR       ,"字符串常量",         null),         
            new Token(KW_CHAR       ,"char",             null),         
            new Token(KW_SHORT      ,"short",            null),         
            new Token(KW_INT        ,"int",              null),           
            new Token(KW_FLOAT      ,"float",            null),       
            new Token(KW_VOID       ,"void",             null),         
            new Token(KW_STRUCT     ,"struct",           null),         
            new Token(KW_IF         ,"if",               null),         
            new Token(KW_ELSE       ,"else",             null),         
            new Token(KW_FOR        ,"for",              null),         
            new Token(KW_CONTINUE   ,"continue",         null),         
            new Token(KW_BREAK      ,"break",            null),         
            new Token(KW_RETURN     ,"return",           null),         
            new Token(KW_SIZEOF     ,"sizeof",           null),         
            new Token(KW_ALIGN      ,"__align",          null),         
            new Token(KW_CDECL      ,"__cdecl",          null),         
            new Token(KW_STDCALL    ,"__stdcall",        null),                    
            new Token(TK_IDENT      ,"__identify",       null),        
        };
    }
    
}

    
   
    
  
     
      
     
      
     
      
     
  

     
     
  
 
  
 
   
     
 
   
 
     
    
   
    
    
   
     
    
  
      
    
     

   
  
  
   
   
 
   















































