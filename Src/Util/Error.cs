using System;

public static class ErrorHandler {
    public const int STAGE_COMPILE = 0;
    public const int STAGE_LINK = 1;
    public const int LEVEL_WARNING = 0;
    public const int LEVEL_ERROR = 1;

    public static string filename;
    public static int line_num;



    public static void expect(string msg){
        error("缺少%s", msg);
    }

    public static void skip(int v){
        if (token != v)
            error("缺少'%s'", get_tkstr(v));
        get_token();
    }
    
    public static void warning(string format, params object[] args){
        handle_exception(STAGE_COMPILE, LEVEL_WARNING, format, args);
    }

    public static void error(string format, params object[] args){
        handle_exception(STAGE_COMPILE, LEVEL_ERROR, format, args);
    }


    public static void link_error(string format, params object[] args){
        handle_exception(STAGE_LINK, LEVEL_ERROR, format, args);
    }
    public static void handle_exception(int stage, int level, string format, params object[] args){
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