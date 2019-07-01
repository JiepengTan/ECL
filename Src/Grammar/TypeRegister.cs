using System.Collections.Generic;
using System.Runtime.Remoting;

namespace LockstepECL {
    public class TypeRegister {
        public SymStruct TypeStruct;
        public SymStruct TypeVar;
        public SymStruct TypeFunction;
        public SymStruct TypeDomain;
        public SymStruct TypeBool;
        public SymStruct TypeFloat;
        public SymStruct TypeChar;
        public SymStruct TypeString;
        public SymStruct TypeInt8;
        public SymStruct TypeInt16;
        public SymStruct TypeInt32;
        public SymStruct TypeInt64;
        public SymStruct TypeUInt8;
        public SymStruct TypeUInt16;
        public SymStruct TypeUInt32;
        public SymStruct TypeUInt64;
        
        private  List<SymStruct> _allTypes = new List<SymStruct>(32);
        private int _curTypeId = 0;


        public TypeRegister(SymDomain globalDomain){
            TypeStruct      = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.TK_SYMSTRUCT  ,__name = "Struct"        , }     );
            TypeVar         = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.TK_SYMVAR     ,__name = "Variable"     ,  }        );
            TypeFunction    = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.TK_SYMDOMAIN  ,__name = "Function"     ,   }        );
            TypeDomain      = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.TK_SYMFUNCTION,__name = "Domain "     ,  }        );
            TypeBool        = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_BOOL       ,__name = "Bool"     ,  }        );
            TypeFloat       = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_CHAR       ,__name = "Float"     ,  }        );
            TypeChar        = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_FLOAT      ,__name = "Char"     ,  }        );
            TypeString      = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_STRING     ,__name = "String"     ,   }        );
            TypeInt8        = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_INT8       ,__name = "Int8"     ,  }        );
            TypeInt16       = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_INT16      ,__name = "Int16"     ,  }        );
            TypeInt32       = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_INT32      ,__name = "Int32"     ,  }        );
            TypeInt64       = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_INT64      ,__name = "Int64"     ,   }        );
            TypeUInt8       = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_UINT8      ,__name = "UInt8"     ,  }        );
            TypeUInt16      = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_UINT16     ,__name = "UInt16"     ,  }        );
            TypeUInt32      = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_UINT32     ,__name = "UInt32"     ,  }        );
            TypeUInt64      = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_UINT64     ,__name = "UInt64"     ,   }        );
            TypeStruct.Type = TypeStruct;
            foreach (var type in _allTypes) {
                globalDomain.AddStruct(type);
            }

        }
         
        public SymStruct RegisterType(SymStruct type){
            type.typeId = _curTypeId;
            _allTypes.Add(type);
            return type;
        }
    }
}