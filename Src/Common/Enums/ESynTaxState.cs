namespace LockstepECL {
    public class ESynTaxState {
        /// // 空状态，没有语法缩进动作
        public const int SNTX_NUL = 0; 
        /// // 空格 int a; int __stdcall MessageBoxA(); return 1;
        public const int SNTX_SP = 1; 
        /// 换行并缩进，每一个声明、函数定义、语句结束都要置为此状态
        public const int SNTX_LF_HT = 2; 
        /// 延迟取出下一单词后确定输出格式，取出下一个单词后，根据单词类型单独调用syntax_indent确定格式进行输出 
        public const int SNTX_DELAY = 3; 
    };
}