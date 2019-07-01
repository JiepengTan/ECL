using System;
using System.Collections.Generic;
using System.Diagnostics;
using static LockstepECL.Define;
using static LockstepECL.ConstDefine;
using static LockstepECL.BuildInTypeDefine;

namespace LockstepECL {
    public unsafe partial class Grammar : BaseParser {
        /// 词法分析结果
        public LexInfos LexInfos { get; private set; }

        /// 当前Token
        public int CurTokenId;

        /// 当前Token对应的值  用于存放字面常量
        public object CurTokenVal;

        /// 所有的类型
        public TypeRegister TypeContainer;

        //当前内置类型 BuildInTypeDefine
        public int CurBuildInTypeId;

        //当前符号
        public Symbol CurSymbol;

        /// 语法状态 用于代码格式化输出
        public int SyntaxState;

        /// 缩进级别 用于代码格式化输出
        public int SyntaxLevel;

        /// 当前操作数
        private Operand _optop;

        /// 全局作用域
        private SymDomain _gDomain;

        /// 当前作用域
        private SymDomain _curDomain;

        /// 所有作用域
        private Stack<SymDomain> _domains;

        private int _rsym = 0;
        private int _ind = 0;
        private int TokenCount => LexInfos.TokenCount;

        public string __debugTokenName;

        //语法输出回调
        private Action _funcSyntaxIndent;

        public void Init(LexInfos lexInfos, Action funcSyntaxIndent){
            this.LexInfos = lexInfos;
            this.filePath = lexInfos.filePath;
            this._funcSyntaxIndent = funcSyntaxIndent;
            _domains = new Stack<SymDomain>();
            _optop = new Operand();
            _gDomain = new SymDomain();
            PushDomain(_gDomain);
            TypeContainer = new TypeRegister(_gDomain);
        }

        public void TranslationUnit(){
            GetToken();
            while (CurTokenId != TK_EOF) {
                external_declaration(SC_GLOBAL);
            }
        }

        #region 词法分析 查询

        

        void SyntaxIndent(){
            _funcSyntaxIndent?.Invoke();
        }

        void SkipToken(int tokenId){
            if (CurTokenId != tokenId)
                Error(ETipsType.ExpectToken, LexInfos.GetTokenName(tokenId));
            LexInfos.SkipToken();
            CurTokenId = LexInfos.curTokenId;
            CurTokenVal = LexInfos.curTokenVal;
            lineNum = LexInfos.curLineNum;
            __debugTokenName = LexInfos.GetTokenName(CurTokenId);
            SyntaxIndent();
        }

        void GetToken(){
            LexInfos.GetToken();
            CurTokenId = LexInfos.curTokenId;
            CurTokenVal = LexInfos.curTokenVal;
            lineNum = LexInfos.curLineNum;
            __debugTokenName = LexInfos.GetTokenName(CurTokenId);
            SyntaxIndent();
        }

        public string GetTokenName(int tokenId){
            return LexInfos.GetTokenName(tokenId);
        }

        public string GetTokenDebugString(){
            return LexInfos.GetTokenDebugString();
        }
        #endregion

        #region 运行时作用域 维护

        bool HasStruct(int tokenId){
            if (tokenId >= TokenCount)
                return false;
            return _curDomain.HasStruct(tokenId);
        }

        SymFunction FindFunction(int tokenId, bool isRecursiveFind){
            if (tokenId >= TokenCount)
                return null;
            return _curDomain.FindSymbol(tokenId, isRecursiveFind) as SymFunction;
        }

        SymVar FindVariable(int tokenId, bool isRecursiveFind){
            if (tokenId >= TokenCount)
                return null;
            return _curDomain.FindSymbol(tokenId, isRecursiveFind) as SymVar;
        }

        SymStruct FindStruct(int tokenId){
            if (tokenId >= TokenCount)
                return null;
            return _curDomain.FindSymbol(tokenId, true) as SymStruct;
        }

        void PushDomain(SymDomain domain){
            domain.parentDomain = _curDomain;
            _domains.Push(domain);
            _curDomain = domain;
        }

        void PopDomain(){
            _domains.Pop();
            _curDomain = _domains.Peek();
        }

        public void AddVariable(SymVar val){
            if (CheckSymbol(val)) _curDomain.AddVariable(val);
        }

        public void AddFunction(SymFunction val){
            if (CheckSymbol(val)) _curDomain.AddFunction(val);
        }

        public void AddParam(SymVar val){
            if (CheckSymbol(val)) _curDomain.AddParam(val);
        }

        public void AddDomain(SymDomain val){
            if (CheckSymbol(val)) _curDomain.AddDomain(val);
        }

        public void AddStruct(SymStruct val){
            if (CheckSymbol(val)) {
                TypeContainer.RegisterType(val);
                _curDomain.AddStruct(val);
            }
        }

        bool CheckSymbol(Symbol symbol){
            if (_curDomain.FindSymbol(symbol.tokenId, false) != null) {
                Error(ETipsType.DuplicateDefine, symbol.__name);
                return false;
            }

            return true;
        }

        #endregion
        
        #region 语法规则 解析

        int calc_align(int n, int align){
            return ((n + align - 1) & (~(align - 1)));
        }

        int type_size(ref int align){
            var bt = CurBuildInTypeId & T_BTYPE;
            switch (bt) {
                case T_STRUCT: {
                    align = CurSymbol.align;
                    return CurSymbol.align;
                }
                case T_STRING:
                    align = 4;
                    return 4;
                case T_BOOL:
                    align = 1;
                    return 1;
                case T_FLOAT:
                    align = 4;
                    return 4;
                case T_CHAR:
                    align = 1;
                    return 1;
                case T_INT8:
                    align = 1;
                    return 1;
                case T_INT16:
                    align = 2;
                    return 2;
                case T_INT32:
                    align = 4;
                    return 4;
                case T_INT64:
                    align = 8;
                    return 8;
                case T_UINT8:
                    align = 1;
                    return 1;
                case T_UINT16:
                    align = 2;
                    return 2;
                case T_UINT32:
                    align = 4;
                    return 4;
                case T_UINT64:
                    align = 8;
                    return 8;
                default:
                    align = 1;
                    return 1;
            }
        }

        void external_declaration(int l){
            int tokenId = 0, r = 0, addr = 0;
            bool has_init = false;
            Section sec = null;

            var firstTokenId = CurTokenId;
            if (!type_specifier()) {
                Error(ETipsType.ExpectTypeIdentifier);
            }

            if (CurBuildInTypeId == T_STRUCT && CurTokenId == TK_SEMICOLON) {
                GetToken();
                return;
            }

            while (true) {
                var force_align = -1;
                var sym = declarator(ref tokenId, ref force_align);

                if (CurTokenId == TK_BEGIN) //函数定义
                {
                    var symFunc = sym as SymFunction;
                    if (symFunc == null)
                        Error(ETipsType.ExpectFunctionDefine);
                    AddFunction(symFunc);
                    sym.Type = TypeContainer.TypeFunction;
                    symFunc.RetTypeId = firstTokenId;
                    funcbody(symFunc);
                    break;
                }

                if (sym is SymFunction) // 函数声明
                {
                    Error(ETipsType.DontSurpportDeclareFunc);
                }
                else //变量声明
                {
                    var symVar = sym as SymVar;
                    sym.__name = GetTokenName(tokenId);
                    AddVariable(symVar);
                    symVar.tokenId = firstTokenId;
                    symVar.parentDomain = _curDomain;
                    symVar.Type = FindStruct(firstTokenId);
                    r = 0;
                    if (symVar.isArray)
                        r |= SC_LVAL;

                    r |= l;
                    has_init = (CurTokenId == TK_ASSIGN);

                    if (has_init) {
                        GetToken(); //不能放到后面，char str[]="abc"情况，需要allocate_storage求字符串长度				    
                    }
                    //sec = allocate_storage( r, has_init ? 1 : 0, tokenId, ref addr);
                    //if (l == SC_GLOBAL)
                    //    coffsym_add_update(sym, addr, sec.index, 0, IMAGE_SYM_CLASS_EXTERNAL);

                    if (has_init) {
                        initializer(symVar.isArray, addr);
                    }
                }

                if (CurTokenId == TK_COMMA) {
                    GetToken();
                }
                else {
                    SyntaxState = SNTX_LF_HT;
                    SkipToken(TK_SEMICOLON);
                    break;
                }
            }
        }

        void initializer(bool isArray, int c){
            if (isArray) {
                GetToken();
            }
            else {
                assignment_expression();
            }
        }

        bool type_specifier(){
            bool type_found = false;
            CurBuildInTypeId = 0;
            switch (CurTokenId) {
                case KW_STRING:
                case KW_BOOL:
                case KW_FLOAT:
                case KW_CHAR:
                case KW_INT8:
                case KW_INT16:
                case KW_INT32:
                case KW_INT64:
                case KW_UINT8:
                case KW_UINT16:
                case KW_UINT32:
                case KW_UINT64:
                    CurBuildInTypeId = T_STRING + (CurTokenId - KW_STRING);
                    type_found = true;
                    SyntaxState = SNTX_SP;
                    GetToken();
                    break;
                case KW_VOID:
                    CurBuildInTypeId = T_VOID;
                    type_found = true;
                    SyntaxState = SNTX_SP;
                    GetToken();
                    break;
                case KW_STRUCT:
                    SyntaxState = SNTX_SP;
                    struct_specifier();
                    CurBuildInTypeId = T_STRUCT;
                    type_found = true;
                    break;
                default:
                    var hasStruct = HasStruct(CurTokenId);
                    if (hasStruct) {
                        CurBuildInTypeId = T_STRUCT;
                        type_found = true;
                        SyntaxState = SNTX_SP;
                        GetToken();
                    }

                    break;
            }

            return type_found;
        }

        void struct_specifier(){
            GetToken();
            var tokenId = CurTokenId;

            SyntaxState = SNTX_DELAY; // 新取单词不即时输出，延迟到取出单词后根据单词类型判断输出格式
            GetToken();

            if (CurTokenId == TK_BEGIN) // 适用于结构体定义
                SyntaxState = SNTX_LF_HT;
            else if (CurTokenId == TK_CLOSEPA) // 适用于 sizeof(struct struct_name)
                SyntaxState = SNTX_NUL;
            else // 适用于结构变量声明
                SyntaxState = SNTX_SP;
            SyntaxIndent();

            if (tokenId < TK_IDENT) // 关键字不能作为结构名称
                Error(ETipsType.ExpectStruct);
            var sym = FindStruct(tokenId);
            if (sym == null) {
                // -1表示结构体未定义
                sym = new SymStruct();
                sym.__name = GetTokenName(tokenId);
                sym.align = 0;
                sym.tokenId = tokenId;
                AddStruct(sym);
            }

            PushDomain(sym);
            CurBuildInTypeId = T_STRUCT;
            CurSymbol = sym;
            if (CurTokenId == TK_BEGIN) {
                struct_declaration_list(sym);
            }

            PopDomain();
        }

        void struct_declaration_list(SymStruct curStruct){
            SyntaxState = SNTX_LF_HT; // 第一个结构体成员与'{'不写在一行
            SyntaxLevel++; // 结构体成员变量声明，缩进增加一级
            GetToken();
            int maxalign = 1;
            int offset = 0;
            while (CurTokenId != TK_END) {
                struct_declaration(ref maxalign, ref offset, curStruct);
            }

            SkipToken(TK_END);
            SyntaxState = SNTX_LF_HT;

            curStruct.memSize = calc_align(offset, maxalign); //结构体大小
            curStruct.align = maxalign; //结构体对齐
        }

        void struct_declaration(ref int maxalign, ref int offset, SymStruct curStruct){
            var rawTokenId = CurTokenId;
            type_specifier();
            while (true) {
                int force_align = 0;
                int align = 0;
                int tokenId = 0;
                var symVar = declarator(ref tokenId, ref force_align) as SymVar;
                var size = type_size(ref align);

                if ((force_align & ALIGN_SET) != 0)
                    align = force_align & ~ALIGN_SET;

                offset = calc_align(offset, align);
                if (align > maxalign)
                    maxalign = align;

                symVar.offset = offset;
                symVar.tokenId = tokenId;
                symVar.Type = FindStruct(rawTokenId);
                symVar.__name = GetTokenName(tokenId);
                curStruct.AddVariable(symVar);
                offset += size;
                if (CurTokenId == TK_SEMICOLON || CurTokenId == TK_EOF)
                    break;
                SkipToken(TK_COMMA);
            }

            SyntaxState = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
        }


        Symbol declarator(ref int tokenId, ref int force_align){
            int fc = 0;
            function_calling_convention(ref fc);
            if (force_align != -1)
                struct_member_alignment(ref force_align);
            return direct_declarator(ref tokenId, fc);
        }

        void function_calling_convention(ref int fc){
            fc = KW_CDECL;
            if (CurTokenId == KW_CDECL || CurTokenId == KW_STDCALL) {
                fc = CurTokenId;
                SyntaxState = SNTX_SP;
                GetToken();
            }
        }

        void struct_member_alignment(ref int force_align){
            int align = 1;
            if (CurTokenId == KW_ALIGN) {
                GetToken();
                SkipToken(TK_OPENPA);
                if (CurTokenId == TK_CINT) {
                    GetToken();
                    align = (int) CurTokenVal;
                }
                else
                    Error(ETipsType.ExpectConstInt);

                SkipToken(TK_CLOSEPA);
                if (align != 1 && align != 2 && align != 4)
                    align = 1;
                align |= ALIGN_SET;
                force_align = align;
            }
            else
                force_align = 1;
        }

        Symbol direct_declarator(ref int tokenId, int func_call){
            if (CurTokenId >= TK_IDENT) {
                tokenId = CurTokenId;
                GetToken();
            }
            else {
                Error(ETipsType.ExpectIdentifier);
            }

            return direct_declarator_postfix(func_call, tokenId);
        }

        Symbol direct_declarator_postfix(int func_call, int tokenId){
            if (CurTokenId == TK_OPENPA) { //function
                return parameter_type_list(func_call, tokenId);
            }
            else if (CurTokenId == TK_OPENBR) { //Array
                GetToken();
                var symbol = new SymVar();
                symbol.__name = GetTokenName(tokenId);
                symbol.isArray = true;
                symbol.tokenId = tokenId;
                if (CurTokenId == TK_CINT) {
                    GetToken();
                    symbol.arraySize = (int) CurTokenVal;
                }

                SkipToken(TK_CLOSEBR);
                return symbol;
                //direct_declarator_postfix(func_call,typeId);//不允许 声明函数数组
            }
            else { //normal Var
                var symbol = new SymVar();
                symbol.tokenId = tokenId;
                symbol.__name = GetTokenName(tokenId);
                return symbol;
            }
        }

        SymFunction parameter_type_list(int func_call, int tokenId){
            int n = 0;

            GetToken();
            Symbol first = null;
            Symbol plast = first;

            var symFunc = FindFunction(tokenId, false);
            bool hasDefined = symFunc != null;
            if (symFunc == null) {
                //add function
                symFunc = new SymFunction();
                symFunc.tokenId = tokenId;
                symFunc.__name = GetTokenName(tokenId);
            }

            while (CurTokenId != TK_CLOSEPA) {
                var rawTokenId = CurTokenId;
                if (!type_specifier()) {
                    Error(ETipsType.UnknowTypeIdentifier, CurTokenId);
                }

                int isForceAlign = -1;
                var s = declarator(ref n, ref isForceAlign);
                s.Type = FindStruct(rawTokenId);
                CurSymbol = s;
                if (!hasDefined) {
                    symFunc.AddParam(s as SymVar);
                }

                if (CurTokenId == TK_CLOSEPA)
                    break;
                SkipToken(TK_COMMA);
                if (CurTokenId == TK_ELLIPSIS) {
                    func_call = KW_CDECL;
                    GetToken();
                    break;
                }
            }

            SyntaxState = SNTX_DELAY;
            SkipToken(TK_CLOSEPA);
            if (CurTokenId == TK_BEGIN) // 函数定义
                SyntaxState = SNTX_LF_HT;
            else // 函数声明
                SyntaxState = SNTX_NUL;
            SyntaxIndent();
            CurBuildInTypeId = T_FUNC;
            CurSymbol = symFunc;
            return symFunc;
        }


        void funcbody(SymFunction sym){
            _ind = data_offset;
            gen_prolog(sym.typeId);
            _rsym = 0;
            var bsym = -1;
            var csym = -1;
            compound_statement(ref bsym, ref csym, sym);
            gen_back_patch(_rsym, _ind);
            gen_epilog();
            data_offset = _ind;
        }

        bool is_type_specifier(int tokenId){
            switch (tokenId) {
                case KW_STRING:
                case KW_BOOL:
                case KW_FLOAT:
                case KW_CHAR:
                case KW_INT8:
                case KW_INT16:
                case KW_INT32:
                case KW_INT64:
                case KW_UINT8:
                case KW_UINT16:
                case KW_UINT32:
                case KW_UINT64:
                case KW_VOID:
                case KW_STRUCT:
                    return true;
                default:
                    //self define class
                    return HasStruct(tokenId);
                    break;
            }

            return false;
        }

        void statement(ref int bsym, ref int csym){
            switch (CurTokenId) {
                case TK_BEGIN:
                    compound_statement(ref bsym, ref csym);
                    break;
                case KW_IF:
                    if_statement(ref bsym, ref csym);
                    break;
                case KW_RETURN:
                    return_statement();
                    break;
                case KW_BREAK:
                    break_statement(ref bsym);
                    break;
                case KW_CONTINUE:
                    continue_statement(ref csym);
                    break;
                case KW_FOR:
                    for_statement(ref bsym, ref csym);
                    break;
                default:
                    expression_statement();
                    break;
            }
        }

        void compound_statement(ref int bsym, ref int csym, SymDomain parent = null){
            SyntaxState = SNTX_LF_HT;
            SyntaxLevel++; // 复合语句，缩进增加一级

            SymDomain domain = parent;
            if (parent == null) {
                domain = new SymDomain();
                AddDomain(domain);
            }

            PushDomain(domain);
            GetToken();
            while (is_type_specifier(CurTokenId)) {
                external_declaration(SC_LOCAL);
            }

            while (CurTokenId != TK_END) {
                statement(ref bsym, ref csym);
            }

            PopDomain();
            SyntaxState = SNTX_LF_HT;
            GetToken();
        }

        void if_statement(ref int bsym, ref int csym){
            int a, b;
            SyntaxState = SNTX_SP;
            GetToken();
            SkipToken(TK_OPENPA);
            expression();
            SyntaxState = SNTX_LF_HT;
            SkipToken(TK_CLOSEPA);
            a = gen_jcc(0);
            var ifDomain = new SymDomain();
            AddDomain(ifDomain);
            PushDomain(ifDomain);
            statement(ref bsym, ref csym);
            PopDomain();
            if (CurTokenId == KW_ELSE) {
                SyntaxState = SNTX_LF_HT;
                GetToken();
                b = gen_jmp_forward(0);
                gen_back_patch(a, _ind);
                var elseDomain = new SymDomain();
                AddDomain(elseDomain);
                PushDomain(elseDomain);
                statement(ref bsym, ref csym);
                PopDomain();
                gen_back_patch(b, _ind); /* 反填else跳转 */
            }
            else
                gen_back_patch(a, _ind);
        }

        void for_statement(ref int bsym, ref int csym){
            var forDomain = new SymDomain();
            AddDomain(forDomain);
            PushDomain(forDomain);

            int a, b, c, d, e;
            GetToken();
            SkipToken(TK_OPENPA);
            if (CurTokenId != TK_SEMICOLON) {
                expression();
                operand_pop();
            }

            SkipToken(TK_SEMICOLON);
            d = _ind;
            c = _ind;
            a = 0;
            b = 0;
            if (CurTokenId != TK_SEMICOLON) {
                expression();
                a = gen_jcc(0);
            }

            SkipToken(TK_SEMICOLON);
            if (CurTokenId != TK_CLOSEPA) {
                e = gen_jmp_forward(0);
                c = _ind;
                expression();
                operand_pop();
                gen_jmp_back(d);
                gen_back_patch(e, _ind);
            }

            SyntaxState = SNTX_LF_HT;
            SkipToken(TK_CLOSEPA);
            statement(ref a, ref b); //只有此处用到break,及continue,一个循环中可能有多个break,或多个continue,故需要拉链以备反填
            gen_jmp_back(c);
            gen_back_patch(a, _ind);
            gen_back_patch(b, c);
            PopDomain();
        }

        void continue_statement(ref int csym){
            if (csym == -1)
                Error(ETipsType.ErrorToken, "continue");
            csym = gen_jmp_forward(csym);
            GetToken();
            SyntaxState = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
        }

        void break_statement(ref int bsym){
            if (bsym == -1)
                Error(ETipsType.ErrorToken, "break");
            bsym = gen_jmp_forward(bsym);
            GetToken();
            SyntaxState = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
        }

        void return_statement(){
            SyntaxState = SNTX_DELAY;
            GetToken();
            if (CurTokenId == TK_SEMICOLON) // 适用于 return;
                SyntaxState = SNTX_NUL;
            else // 适用于 return <expression>;
                SyntaxState = SNTX_SP;
            SyntaxIndent();

            if (CurTokenId != TK_SEMICOLON) {
                expression();
                gen_load_val(REG_IRET, _optop);
                operand_pop();
            }

            SyntaxState = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
            _rsym = gen_jmp_forward(_rsym);
        }

        void expression_statement(){
            if (CurTokenId != TK_SEMICOLON) {
                expression();
                operand_pop();
            }

            SyntaxState = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
        }

        void expression(){
            while (true) {
                assignment_expression();
                if (CurTokenId != TK_COMMA)
                    break;
                operand_pop();
                GetToken();
            }
        }

        void assignment_expression(){
            equality_expression();
            if (CurTokenId == TK_ASSIGN) {
                check_lvalue();
                GetToken();
                assignment_expression();
                gen_push_val();
            }
        }

        void equality_expression(){
            int t;
            relational_expression();
            while (CurTokenId == TK_EQ || CurTokenId == TK_NEQ) {
                t = CurTokenId;
                GetToken();
                relational_expression();
                gen_op(t);
            }
        }

        void relational_expression(){
            int t;
            additive_expression();
            while ((CurTokenId == TK_LT || CurTokenId == TK_LEQ) ||
                   CurTokenId == TK_GT || CurTokenId == TK_GEQ) {
                t = CurTokenId;
                GetToken();
                additive_expression();
                gen_op(t);
            }
        }

        void additive_expression(){
            int t;
            multiplicative_expression();
            while (CurTokenId == TK_PLUS || CurTokenId == TK_MINUS) {
                t = CurTokenId;
                GetToken();
                multiplicative_expression();
                gen_op(t);
            }
        }

        void multiplicative_expression(){
            int t;
            unary_expression();
            while (CurTokenId == TK_STAR || CurTokenId == TK_DIVIDE || CurTokenId == TK_MOD) {
                t = CurTokenId;
                GetToken();
                unary_expression();
                gen_op(t);
            }
        }

        void unary_expression(){
            switch (CurTokenId) {
                case TK_AND:
                    GetToken();
                    unary_expression();
                    break;
                case TK_STAR:
                    GetToken();
                    unary_expression();
                    indirection();
                    break;
                case TK_PLUS:
                    GetToken();
                    unary_expression();
                    break;
                case TK_MINUS:
                    GetToken();
                    operand_push(TypeContainer.TypeInt32, 0);
                    unary_expression();
                    gen_op(TK_MINUS);
                    break;
                case KW_SIZEOF:
                    sizeof_expression();
                    break;
                default:
                    postfix_expression();
                    break;
            }
        }

        void sizeof_expression(){
            int align = 0;

            GetToken();
            SkipToken(TK_OPENPA);
            type_specifier();
            SkipToken(TK_CLOSEPA);

            var size = type_size(ref align);
            if (size < 0)
                Error(ETipsType.SizeOfError, size);
            operand_push(TypeContainer.TypeInt32, size);
        }

        void postfix_expression(){
            Symbol s;
            primary_expression();
            while (true) {
                if (CurTokenId == TK_DOT || CurTokenId == TK_POINTSTO) {
                    if (CurTokenId == TK_DOT)
                        indirection();
                    cancel_lvalue();
                    GetToken();
                    var symVar = _optop.sym as SymVar;

                    var symStruct = (symVar?.parentDomain ?? _gDomain).FindSymbol(symVar.tokenId, true) as SymStruct;
                    if (symStruct == null)
                        Error(ETipsType.ExpectStructVar);
                    _optop.sym = symStruct?.FindSymbol(CurTokenId, false);
                    if (_optop.sym == null)
                        Error(ETipsType.MemberNotExist, GetTokenName(CurTokenId));
                    GetToken();
                }
                else if (CurTokenId == TK_OPENBR) {
                    GetToken();
                    expression();
                    gen_op(TK_PLUS);
                    indirection();
                    SkipToken(TK_CLOSEBR);
                }
                else if (CurTokenId == TK_OPENPA) {
                    argument_expression_list();
                }
                else
                    break;
            }
        }

        void primary_expression(){
            int t = 0, r = 0, addr = 0;
            Symbol s;
            Section sec = new Section();
            switch (CurTokenId) {
                case TK_BOOL:
                    operand_push(TypeContainer.TypeBool, (bool) CurTokenVal);
                    GetToken();
                    break;
                case TK_CINT:
                case TK_CCHAR:
                    operand_push(TypeContainer.TypeInt32, (int) CurTokenVal);
                    GetToken();
                    break;
                case TK_LFloat:
                    operand_push(TypeContainer.TypeFloat, (float) CurTokenVal);
                    GetToken();
                    break;
                case TK_CSTR:
                    t = T_CHAR;
                    initializer(true, addr);
                    break;
                case TK_OPENPA:
                    GetToken();
                    expression();
                    SkipToken(TK_CLOSEPA);
                    break;
                default:
                    t = CurTokenId;
                    GetToken();
                    if (t < TK_IDENT)
                        Error(ETipsType.ExpectIdentifierOrConst);
                    s = FindVariable(t, true);
                    if (s == null) {
                        if (CurTokenId != TK_OPENPA)
                            Error(ETipsType.VarNotDeclare, GetTokenName(t));
                        else {
                            s = FindFunction(t, true);
                        }
                    }

                    operand_push(s, null);

                    _optop.sym = s;
                    _optop.value = 0;
                    break;
            }
        }

        void argument_expression_list(){
            Operand ret;
            int nb_args = 0;
            GetToken();
            var r = REG_IRET;
            var value = 0;
            if (CurTokenId != TK_CLOSEPA) {
                for (;;) {
                    assignment_expression();
                    nb_args++;
                    if (CurTokenId == TK_CLOSEPA)
                        break;
                    SkipToken(TK_COMMA);
                }
            }

            var symFunction = _optop.sym as SymFunction;
            if (symFunction != null) {
                Debug.Assert(symFunction != null, nameof(symFunction) + " != null");
                if (nb_args != symFunction.ParamsCount)
                    Error(ETipsType.ParamsNumNotMatch); //讲一下形参，实参
            }

            SkipToken(TK_CLOSEPA);
            gen_invoke(nb_args);
            if (symFunction != null) {
                operand_push(FindStruct(symFunction.RetTypeId), value);
            }
        }

        #endregion
        
        #region 代码生成
        
        public int data_offset;
        void gen_back_patch(int rsym, int ind){ }

        int gen_jcc(int t){
            return 4;
        }

        int gen_jmp_forward(int sym){
            return 4;
        }

        void gen_jmp_back(int a){ }
        void gen_load_val(int rc, Operand opd){ }
        void gen_push_val(){ }
        void gen_op(int op){ }
        void gen_invoke(int nArgs){ }
        void gen_prolog(int funcType){ }
        void gen_epilog(){ }
        void check_lvalue(){ }
        void cancel_lvalue(){ }
        void indirection(){ }
        void operand_pop(){ }
        void operand_push(Symbol type, object val){ }
        #endregion
    }
}