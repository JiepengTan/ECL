namespace LockstepECL {
    public enum ESynTaxState {
        SNTX_NUL, // 空状态，没有语法缩进动作
        SNTX_SP, // 空格 int a; int __stdcall MessageBoxA(); return 1;
        SNTX_LF_HT, // 换行并缩进，每一个声明、函数定义、语句结束都要置为此状态
        SNTX_DELAY // 延迟取出下一单词后确定输出格式，取出下一个单词后，根据单词类型单独调用syntax_indent确定格式进行输出 
    };
}