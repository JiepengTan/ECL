using System.Collections.Generic;

namespace LockstepECL {
    public class StructDefines { }

    public class Type {
        public int typeId;
        public Symbol _ref;

        public override string ToString(){
            return "type: " + ((ETypeCode) typeId).ToString();
        }
    }

    public class Symbol {
        public SymDomain parentDomain;
        public string __name;
        public int tokenId; // 符号的单词编码//v
        public int align; // 符号关联的寄存器
        public int value; // 符号关联值//c
        public Type type; // 符号类型
        public Symbol next; // 关联的其它符号，结构体定义关联成员变量符号，函数定义关联参数符号
        public Symbol prev_tok; // 指向前一定义的同名符号

        public override string ToString(){
            return $"name:{__name} code:{tokenId} value:{value} type:{type}";
        }

        public int ArraySize(){
            return value;
        }

        public int TypeSize(){
            return value;
        }
    }

    public class SymDomain : Symbol {
        public List<SymVar> Variables = new List<SymVar>();
        public List<SymFunction> Functions = new List<SymFunction>();
        public List<SymStruct> Structs = new List<SymStruct>();
        public List<SymVar> Params = new List<SymVar>();
        public List<SymDomain> Domains = new List<SymDomain>();
        public int RetTypeId;
        public List<OpCode> _opCodes = new List<OpCode>();

        public Dictionary<int, Symbol> tokenId2Symbol = new Dictionary<int, Symbol>();
        HashSet<int> domainTypes = new HashSet<int>();

        public Symbol FindSymbol(int tokenId, bool isRecursiveFind){
            if (tokenId2Symbol.TryGetValue(tokenId, out var val)) {
                return val;
            }

            if (!isRecursiveFind) {
                return null;
            }

            return parentDomain?.FindSymbol(tokenId, true) ?? null;
        }

        public bool HasStruct(int tokenId){
            if (domainTypes.Contains(tokenId)) {
                return true;
            }
            else {
                return parentDomain?.HasStruct(tokenId) ?? false;
            }
        }

        public bool HasParams(){
            return Params.Count > 0;
        }

        void RegisterSymbol(int tokenId, Symbol val){
            if (tokenId == 0) return; //忽略匿名Domain
            if (tokenId2Symbol.ContainsKey(tokenId)) {
                ErrorHandler.Error("重复定义同名成员 " + val.__name);
                return;
            }

            tokenId2Symbol.Add(val.tokenId, val);
        }

        public void AddParam(SymVar val){
            Params.Add(val);
        }

        public void AddVariable(SymVar val){
            RegisterSymbol(val.tokenId, val);
            Variables.Add(val);
        }

        public void AddFunction(SymFunction val){
            RegisterSymbol(val.tokenId, val);
            Functions.Add(val);
            Domains.Add(val);
        }

        public void AddStruct(SymStruct val){
            RegisterSymbol(val.tokenId, val);
            Structs.Add(val);
            Domains.Add(val);
            domainTypes.Add(val.tokenId);
        }

        public void AddDomain(SymDomain val){
            RegisterSymbol(val.tokenId, val);
            Domains.Add(val);
        }
    }

    public class SymFunction : SymDomain { }

    public class SymStruct : SymDomain {
        public int TypeId;
    }

    public class OpCode { }

    public class SymVar : Symbol {
        public int typeId;
        public bool isMember;
        public int offset;
        public SymStruct type;
        public object _value;
        public bool isArray;
        public int arraySize;
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
        public Type type = new Type(); // 数据类型
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