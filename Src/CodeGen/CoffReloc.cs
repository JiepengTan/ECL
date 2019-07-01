namespace LockstepECL {
    public class CoffReloc {
        public int offset; // 需要进行重定位的代码或数据的地址
        public int cfsym; // 符号表的索引(从0开始)
        public byte section; // 此处讲一下为什么对COFF重定位结构进行修改记录Section信息*/
        public byte type;
    };
}