using System.Collections.Generic;
using System.Text;

namespace LockstepECL {
    public class LexInfos {
        public struct Info {
            public int line;
            public int tokenId;
            public object tokenVal;

            public override string ToString(){
                return $"line:{line} tokenId{tokenId}";
            }
        }

        public LexInfos(List<Token> tokenTable){
            this.tokenTable = tokenTable;
            tokenInfos = new List<Info>();
        }

        public List<Token> tokenTable;
        public List<Info> tokenInfos;
        public string filePath;

        public void OnToken(int line, int tokenId, object tokenVal){
            tokenInfos.Add(new Info() {line = line, tokenId = tokenId, tokenVal = tokenVal});
        }

        private int _curIdx = -1;

        public void GetToken(){
            if (_curIdx >= tokenInfos.Count) {
                return;
            }

            _curIdx++;
        }

        public void SkipToken(){
            GetToken();
        }

        public void GetTokenInfo(int tokenId){ }

        public string GetTokenName(int tokenId){
            return tokenId > tokenTable.Count ? "" : tokenTable[tokenId].name;
        }

        public string GetTokenDebugString(){
            var tokenId = tokenInfos[_curIdx].tokenId;
            if (tokenId >= Define.TK_BOOL && tokenId <= Define.TK_CSTR) {
                if (tokenId == Define.TK_BOOL) {
                    return ((bool) tokenInfos[_curIdx].tokenVal == false) ? "false" : "true";
                }

                return tokenInfos[_curIdx].tokenVal.ToString();
            }
            else
                return tokenTable[tokenId].name;
        }

        public int TokenTableCount => tokenTable.Count;
        public int curTokenId => tokenInfos[_curIdx].tokenId;
        public object curTokenVal => tokenInfos[_curIdx].tokenVal;
        public int curLineNum => tokenInfos[_curIdx].line;

        public override string ToString(){
            int curLine = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var info in tokenInfos) {
                sb.Append(tokenTable[info.tokenId].name + " ");
                if (info.line != curLine) {
                    curLine = info.line;
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}