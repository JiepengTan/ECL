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


        public TypeRegister(){
            TypeStruct      = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.TK_SYMSTRUCT}     );
            TypeVar         = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.TK_SYMVAR}        );
            TypeFunction    = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.TK_SYMDOMAIN}        );
            TypeDomain      = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.TK_SYMFUNCTION}        );
            TypeBool        = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_BOOL }        );
            TypeFloat       = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_CHAR  }        );
            TypeChar        = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_FLOAT }        );
            TypeString      = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_STRING }        );
            TypeInt8        = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_INT8 }        );
            TypeInt16       = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_INT16 }        );
            TypeInt32       = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_INT32 }        );
            TypeInt64       = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_INT64  }        );
            TypeUInt8       = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_UINT8 }        );
            TypeUInt16      = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_UINT16 }        );
            TypeUInt32      = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_UINT32 }        );
            TypeUInt64      = RegisterType(new SymStruct() {Type = TypeStruct,  tokenId = Define.KW_UINT64  }        );
            TypeStruct.Type = TypeStruct; 

        }
        SymStruct RegisterType(SymStruct type){
            type.typeId = _curTypeId;
            _allTypes.Add(type);
            return type;
        }
    }
}