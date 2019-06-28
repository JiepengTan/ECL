using System.Text;

namespace LockstepECL {
    public class DynString {
        public string data {
            get { return sb.ToString(); }
        } // 指向字符串的指针  ss

        protected StringBuilder sb = new StringBuilder();

        public void AddCh(char ch){ }

        public void Clear(){
            sb.Clear();
        }
    }
}