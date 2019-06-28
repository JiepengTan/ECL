using System.IO;

namespace LockstepECL {
    public class InputStream {
        private string _text;
        private int _size;
        private int _idx;

        public InputStream(string text){
            this._text = text;
            _size = text.Length;
            _idx = 0;
        }

        public  void UnChar(char ch){
            --_idx;
        }

        public  char GetChar(){
            if (_idx >= _size) {
                return '\0';
            }
            return _text[_idx++];
        }
    }
}