namespace LockstepECL {
    public enum ETypeCode {
        T_INT = 0, // 整型                     
        T_CHAR = 1, // 字符型                  
        T_SHORT = 2, // 短整型                         
        T_VOID = 3, // 空类型                         
        T_PTR = 4, // 指针                          
        T_FUNC = 5, // 函数                     
        T_STRUCT = 6, // 结构体
    }

    public class ConstDefine {
        //public class EStorageClass {
        public static int SC_GLOBAL = 0x00f0; // 包括：包括整型常量，字符常量、字符串常量,全局变量,函数定义          
        public static int SC_LOCAL = 0x00f1; // 栈中变量
        public static int SC_LLOCAL = 0x00f2; // 寄存器溢出存放栈中
        public static int SC_CMP = 0x00f3; // 使用标志寄存器
        public static int SC_VALMASK = 0x00ff; // 存储类型掩码             
        public static int SC_LVAL = 0x0100; // 左值       
        public static int SC_SYM = 0x0200; // 符号	
        public static int SC_ANOM = 0x1000000; // 匿名符号
        public static int SC_STRUCT = 0x2000000; // 结构体符号
        public static int SC_MEMBER = 0x4000000; // 结构成员变量

        public static int SC_PARAMS = 0x8000000; // 函数参数

        //};
        //public class ESynTaxState {
        /// // 空状态，没有语法缩进动作
        public const int SNTX_NUL = 0;

        /// // 空格 int a; int __stdcall MessageBoxA(); return 1;
        public const int SNTX_SP = 1;

        /// 换行并缩进，每一个声明、函数定义、语句结束都要置为此状态
        public const int SNTX_LF_HT = 2;

        /// 延迟取出下一单词后确定输出格式，取出下一个单词后，根据单词类型单独调用syntax_indent确定格式进行输出 
        public const int SNTX_DELAY = 3;
        //};



                              
        public const int T_VOID       = 0 ; // 指针  
        public const int T_PTR        = 1 ; // 指针                          
        public const int T_FUNC       = 2 ; // 函数                     
        public const int T_STRUCT     = 3 ; // 结构体  
        public const int T_STRING     = 4 ; // 空类型  
        public const int T_BOOL       = 5 ; // 空类型  
        public const int T_FLOAT      = 6 ; // 空类型  
        public const int T_CHAR       = 7 ; // 空类型  
        public const int T_INT8       = 8 ; // 空类型  
        public const int T_INT16      = 9 ; // 空类型  
        public const int T_INT32      = 10; // 空类型  
        public const int T_INT64      = 11; // 空类型  
        public const int T_UINT8      = 12; // 空类型  
        public const int T_UINT16     = 13; // 空类型   
        public const int T_UINT32     = 14; // 空类型  
        public const int T_UINT64     = 15; // 空类型   

        public const int T_BTYPE = 0x000f; // 基本类型掩码          
        public const int T_ARRAY = 0x0010; // 数组

        //};
        public const int ALIGN_SET = 0x100;

        // public enum ELexState {
        public const int LEX_NORMAL = 0;

        public const int LEX_SEP = 1;
        //};

        //public enum EWorkStage {
        public const int STAGE_COMPILE = 0;

        public const int STAGE_LINK = 1;
        //};

        //enum e_OutType
        //{	
        public const int OUTPUT_OBJ = 0; // 目标文件
        public const int OUTPUT_EXE = 1; // EXE可执行文件

        public const int OUTPUT_MEMORY = 2; // 内存中直接运行，不输出
        //};

        public const int CST_FUNC = 0x20; //Coff符号类型，函数

        public const int CST_NOTFUNC = 0; //Coff符号类型，非函数

        //enum e_Register
        //{
        public const int REG_EAX = 0;
        public const int REG_ECX = 1;
        public const int REG_EDX = 2;
        public const int REG_EBX = 3;
        public const int REG_ESP = 4;
        public const int REG_EBP = 5;
        public const int REG_ESI = 6;
        public const int REG_EDI = 7;

        public const int REG_ANY = 8;

        //};
        public const int REG_IRET = REG_EAX;

        //enum e_AddrForm
        //{
        public const int ADDR_OTHER = 0; // 寄存器间接寻址 [EAX],[EBX]

        public const int ADDR_REG = 3; // 寄存器直接寻址，EAX,EBX等相当于mod=11000000(C0)
        //};


        public const int PTR_SIZE = 4;
    }

    public enum EBuildInTypes {
        Char,
        Short,
        CStr,
        Int,
        NumOfEnum
    }
}