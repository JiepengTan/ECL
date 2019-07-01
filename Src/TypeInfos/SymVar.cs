namespace LockstepECL {
    public class SymVar : Symbol {
        public SymVar(){ }
        public int structTypeId; // 符号类型
        public bool isMember;
        public int offset;
        public object _value;
        public bool isArray;
        public int arraySize;
    }
}