namespace LockstepECL {
    public class Token {
        public int id; // 单词编码 
        public string name; // 单词字符串
        public Symbol symStruct; // 指向单词所表示的结构定义
        public Symbol symIdentifier; // 指向单词所表示的标识符

        public Token(){ }
        public Token(
            int id,
            string name,
            Symbol symStruct,
            Symbol sym_identifier
        ){
            this.id = id;
            this.name = name;
            this.symStruct = symStruct;
            this.symIdentifier = sym_identifier;
        }

        public override string ToString(){
            return "" + id + " : " + name;
        }
    }
}