namespace LockstepECL {
    public class CoffSym {
        public int Name; // 符号名称

        public int Next; // 用于保存冲突链表*/

        /* 
       struct {
           DWORD   Short;			// if 0, use LongName
           DWORD   Long;			// offset into string table
       } name;
       */
        public int Value; // 与符号相关的值
        public short Section; // 节表的索引(从1开始),用以标识定义此符号的节*/
        public short Type; // 一个表示类型的数字
        public byte StorageClass; // 这是一个表示存储类别的枚举类型值
        public byte NumberOfAuxSymbols; // 跟在本记录后面的辅助符号表项的个数
    };
}