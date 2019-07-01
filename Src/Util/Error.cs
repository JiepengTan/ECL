using System;
using System.Collections.Generic;
using System.Text;
using LockstepECL;

public struct LogInfo {
    public EErrorLevel level;
    public string info;
}


public class LogHandler {
    private List<LogInfo> _allInfos = new List<LogInfo>();
    public LogInfo[] AllLogInfo => _allInfos.ToArray();

    public void ClearLog(){
        _allInfos.Clear();
    }

    public void HandlerLog(EWorkStage stage, EErrorLevel level, ETipsType type, string fileName, int lineNum,
        int colNum, params object[] args){
        string lineInfo = (lineNum >= 0 ? ":" + lineNum.ToString() : "") +
                          (colNum >= 0 ? ", " + colNum.ToString() : "");
        Log(level, $"{stage} {level} : {fileName} {lineInfo} :  {string.Format(GetFormat(type), args)}\n");
        if (level == EErrorLevel.ERROR)
            Exit();
    }

    public void DumpErrorInfo(){
        var defaultColor = Console.ForegroundColor;
        var curColor = defaultColor;
        foreach (var line in _allInfos) {
            ConsoleColor? color = null;
            if (line.level == EErrorLevel.WARNING) {
                color = ConsoleColor.Yellow;
            }
            else if (line.level == EErrorLevel.ERROR) {
                color = ConsoleColor.Red;
            }

            if (color.HasValue && color.Value != curColor) {
                curColor = color.Value;
                Console.ForegroundColor = color.Value;
            }

            Console.WriteLine(line.info);
        }

        Console.ForegroundColor = defaultColor;
        _allInfos.Clear();
    }

    void Log(EErrorLevel level, string format, params object[] args){
        _allInfos.Add(new LogInfo() {level = level, info = string.Format(format, args)});
    }

    private string GetFormat(ETipsType type){
        if (TipsInfoMap.FormatInfo.TryGetValue(type, out var format)) {
            return format;
        }

        Log(0, "ERROR!!! miss errorType " + type);
        return "";
    }
    void Exit(){ }
}