using System.Text;

namespace LockstepECL {
    public class DynString {
        public string Data => _sb.ToString();

        private StringBuilder _sb = new StringBuilder();

        public void AddCh(char ch){
            _sb.Append(ch);
        }

        public void Clear(){
            _sb.Clear();
        }
    }
}