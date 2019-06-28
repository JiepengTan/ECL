using System.IO;

namespace LockstepECL {
    public class InputStream {
        public string text;
        public int size;
        public int idx;

        public void Init(string text){
            this.text = text;
            size = text.Length;
            idx = 0;
        }

        public  void UnChar(char ch){
            --idx;
        }

        public  char GetChar(){
            if (idx >= size) {
                return '\0';
            }
            return text[idx++];
        }
    }
}