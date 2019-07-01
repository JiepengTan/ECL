using System.Collections.Generic;

namespace LockstepECL {
    public class ImportInfo {
        public int dll_index;

        public List<object> imp_syms;
        //IMAGE_IMPORT_DESCRIPTOR imphdr;
    };
}