namespace LockstepECL {
    public static class ValExtension {
        public static int GetStorageType(this int v){return v & ~ConstDefine.SC_STRUCT;}
        public static bool IsDefine(this int v){return v < ConstDefine.SC_ANOM;}
        public static bool IsSTRUCT(this int v){return (v & ConstDefine.SC_STRUCT) != 0;}
        public static bool IsMEMBER(this int v){return (v & ConstDefine.SC_MEMBER) != 0;}
        public static bool IsPARAMS(this int v){return (v & ConstDefine.SC_PARAMS) != 0;}
        
        public static bool IsGLOBAL(this int v){return  (v & ConstDefine.SC_VALMASK  ) ==  ConstDefine.SC_GLOBAL  ;}
        public static bool IsLOCAL(this int v) {return  (v & ConstDefine.SC_VALMASK  ) ==  ConstDefine.SC_LOCAL  ;}
        public static bool IsLLOCAL(this int v){return  (v & ConstDefine.SC_VALMASK  ) ==  ConstDefine.SC_LLOCAL  ;}
        public static bool IsCMP(this int v)   {return  (v & ConstDefine.SC_VALMASK  ) ==  ConstDefine.SC_CMP ;}
        public static bool IsLVAL(this int v){return    (v & ConstDefine.SC_VALMASK  ) ==  ConstDefine.SC_LVAL ;}
        public static bool IsSYM(this int v){return     (v & ConstDefine.SC_VALMASK  ) ==  ConstDefine.SC_SYM ;}
    }
}