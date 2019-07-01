namespace LockstepECL {
    public class Symbol {
        public SymDomain parentDomain;
        public string __name;
        public int tokenId; // 符号的单词编码//v
        public int align; // 符号关联的寄存器
        public int memSize; // 符号关联的寄存器
        public int typeId; // 符号类型
        public Symbol next; // 关联的其它符号，结构体定义关联成员变量符号，函数定义关联参数符号
        public Symbol prev_tok; // 指向前一定义的同名符号

        public override string ToString(){
            return $"name:{__name} type:{typeId} tokenId:{tokenId} memSize:{memSize} align:{align}";
        }

    }
}