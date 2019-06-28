namespace LockstepECL {
    public class EStorageClass {
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
    };
}