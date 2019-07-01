using System;

namespace LockstepECL {
    public interface ILex {
        LexInfos LexInfos { get; }
        void Init(Func<char> funcGetChar, Action<char> funcUnChar, Action<char> funcDealSpace,Action funcSyntaxIndent);
        void Reset();
        void DoParse();
    }
}