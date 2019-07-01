namespace LockstepECL {
    enum ETypeCode {
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
}
