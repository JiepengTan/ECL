namespace LockstepECL {
    public class Operand {
        public int value; // 常量值，适用于SC_GLOBAL
        public Symbol sym; // 符号，适用于(SC_SYM | SC_GLOBAL)
        public int storageType;
    };
}