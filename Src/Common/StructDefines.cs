using System.Collections.Generic;

namespace LockstepECL {
    public class StructDefines { }

    public class Type {
        public int t;
        public Symbol _ref;
    }

    public class Symbol {
        public int code; // 符号的单词编码
        public int register; // 符号关联的寄存器
        public int value; // 符号关联值
        public Type type; // 符号类型
        public Symbol next; // 关联的其它符号，结构体定义关联成员变量符号，函数定义关联参数符号
        public Symbol prev_tok; // 指向前一定义的同名符号
    }

    public class Section {
        public int data_offset; // 当前数据偏移位置
        public string data; // 节数据
        public int data_allocated; // 分配内存空间
        public char index; // 节序号
        public Section link; // 关联的其它节

        public int[] hashtab; // 哈希表，只用于存储符号表
        //public IMAGE_SECTION_HEADER sh;    // 节头
    };

    public class CoffReloc {
      public int offset; // 需要进行重定位的代码或数据的地址
      public int cfsym; // 符号表的索引(从0开始)
      public byte section; // 此处讲一下为什么对COFF重定位结构进行修改记录Section信息*/
      public byte type;
    };

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


    public class Operand {
       public Type type; // 数据类型
       public ushort r; // 寄存器或存储类型
       public int value; // 常量值，适用于SC_GLOBAL
       public Symbol sym; // 符号，适用于(SC_SYM | SC_GLOBAL)
    };

    public class ImportSym {
        public int iat_index;

        public int thk_offset;
        //IMAGE_IMPORT_BY_NAME imp_sym;
    };

    public class ImportInfo {
        public int dll_index;

        public List<object> imp_syms;
        //IMAGE_IMPORT_DESCRIPTOR imphdr;
    };

    public class PEInfo {
       public Section thunk;
       public string filename;
       public int entry_addr;
       public int imp_offs;
       public int imp_size;
       public int iat_offs;
       public int iat_size;
       public List<Section> secs;
       public int sec_size;
       public List<object> imps;
    };
}