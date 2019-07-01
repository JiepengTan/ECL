namespace LockstepECL {
    public class Section {
        public int data_offset; // 当前数据偏移位置
        public string data; // 节数据
        public int data_allocated; // 分配内存空间
        public char index; // 节序号
        public Section link; // 关联的其它节

        public int[] hashtab; // 哈希表，只用于存储符号表
        //public IMAGE_SECTION_HEADER sh;    // 节头
    };
}