using System.Collections.Generic;

namespace LockstepECL {
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