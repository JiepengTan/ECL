namespace LockstepECL {
    public class Token {
        public int id; // 单词编码 
        public string name; // 单词字符串
        public Symbol symbol; // 指向单词所表示的结构定义

        public Token(){ }
        public Token(
            int id,
            string name,Symbol symbol
        ){
            this.id = id;
            this.name = name; 
            this.symbol = symbol;
        }

        public override string ToString(){
            return "" + id + " : " + name;
        }
    }
}