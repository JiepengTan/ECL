namespace LockstepECL {
    public class BaseParser {
        protected LogHandler _logHandler = new LogHandler();
        public string filePath = "";
        public int lineNum;
        public int colNum = 0;

        public virtual void Warning(ETipsType type, params object[] args){
            _logHandler.HandlerLog(EWorkStage.COMPILE, EErrorLevel.WARNING, type, filePath, lineNum, colNum, args);
        }

        public virtual void Error(ETipsType type, params object[] args){
            _logHandler.HandlerLog(EWorkStage.COMPILE, EErrorLevel.ERROR, type, filePath, lineNum, colNum, args);
        }

        public void DumpErrorInfo(){
            _logHandler.DumpErrorInfo();
        }
    }
}