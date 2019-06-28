using System;

public static class ErrorHandler {
    public const int STAGE_COMPILE = 0;
    public const int STAGE_LINK = 1;
    public const int LEVEL_WARNING = 0;
    public const int LEVEL_ERROR = 1;

    public static string filename;
    public static int line_num;




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
                Console.WriteLine("{0}(第{1}行) : 编译警告: {2}!\n", filename, line_num, string.Format(format, args));
            else {
                Console.WriteLine("{0}(第{1}行) : 编译错误: {2}!\n", filename, line_num, string.Format(format, args));
                Exit();
            }
        }
        else {
            Console.WriteLine(" 链接错误: {0}!\n", string.Format(format, args));
            Exit();
        }
    }

    public static void Exit(){
        //TODO
    }
}