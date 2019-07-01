using System.Collections.Generic;

namespace LockstepECL {
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


        public int ParamsCount => Params.Count;

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
}