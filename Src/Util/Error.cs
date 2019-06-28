using System;
using System.Collections.Generic;
using System.Text;
using LockstepECL;

public static class ErrorHandler {
    public const int STAGE_COMPILE = 0;
    public const int STAGE_LINK = 1;
    public const int LEVEL_WARNING = 0;
    public const int LEVEL_ERROR = 1;

    public static Lex curLex;

    private struct LogInfo {
        public int level;
        public string info;
    }

    private static List<LogInfo> allInfos = new List<LogInfo>();
    
    public static void Warning(string format, params object[] args){
        HandleException(STAGE_COMPILE, LEVEL_WARNING, format, args);
    }

    public static void Error(string format, params object[] args){
        HandleException(STAGE_COMPILE, LEVEL_ERROR, format, args);
    }

    public static void LinkError(string format, params object[] args){
        HandleException(STAGE_LINK, LEVEL_ERROR, format, args);
    }

    private static void HandleException(int stage, int level, string format, params object[] args){
        if (stage == STAGE_COMPILE) {
            if (level == LEVEL_WARNING)
                Log(LEVEL_WARNING, $"Compile Warning :{curLex.fileName}(line:{curLex.lineNum} col:{curLex.colNum}) :  {string.Format(format, args)}\n");
            else {
                Log(LEVEL_ERROR, $"Compile Warning :{curLex.fileName}(line:{curLex.lineNum} col:{curLex.colNum}) :  {string.Format(format, args)}\n");
                Exit();
            }
        }
        else {
            Log(LEVEL_ERROR, "Link Error: {0}!\n", string.Format(format, args));
            Exit();
        }
    }


    static void Log(int level, string format, params object[] args){
        allInfos.Add(new LogInfo() {level = level, info = string.Format(format, args)});
    }

    public static void DumpErrorInfo(){
        var defaultColor = Console.ForegroundColor;
        foreach (var line in allInfos) {
            if (line.level == LEVEL_WARNING) {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else if (line.level == LEVEL_ERROR) {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine(line.info);
        }

        Console.ForegroundColor = defaultColor;
        allInfos.Clear();
    }

    public static void Exit(){ }
}