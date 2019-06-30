using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static ErrorHandler;
using static LockstepECL.Define;
using static LockstepECL.ConstDefine;

namespace LockstepECL {
    public class Stack : Stack<object> {
        public object GetTop(){
            if (this.Count == 0) return null;
            return this.Peek();
        }
    }

    //code gen
    public unsafe partial class Lex {
        private char IMAGE_SYM_CLASS_EXTERNAL = '\t';
        private Section sec_text = new Section();
        void memcpy(void* src, void* dst, int size){ }

        Section allocate_storage(int r, int has_init, int v, ref int addr){
            return new Section();
        }

        void coffsym_add_update(Symbol s, int val, int sec_index, short type, char StorageClass){ }
        void init_variable(Type type, Section sec, int c, int v){ }

        void init_array(Type type, Section sec, int c, int v){
            //memcpy(sec.data + c, tkstr.data, tkstr.count);//TODO
        }


        void check_lvalue(){
            //if(!(optop->r & SC_LVAL))
            //Expect("左值");
        }

        void backpatch(int t, int a){ }

        int gen_jmpforward(int t){
            return 4;
        }

        int gen_jcc(int t){
            return 4;
        }


        void gen_jmpbackword(int a){ }

        int load_1(int rc, Operand opd){
            return 4;
        }

        void store0_1(){ }
        void gen_op(int op){ }
        void cancel_lvalue(){ }
        void indirection(){ }

        void gen_invoke(int nb_args){ }
        void gen_prolog(int func_type){ }
        void gen_epilog(){ }
    }

    public unsafe partial class Lex {
        void operand_pop(){ }
        void operand_push(Symbol type, object val){ }

        public int CurStructMaxIdx = (int) EBuildInTypes.NumOfEnum;

        public void GrammarInit(){
            //sym_sec_rdata = sec_sym_put(".rdata",0);

            int_type.typeId = T_INT;
            char_pointer_type.typeId = T_CHAR;
            default_func_type.typeId = T_FUNC;
            optop = opstack[0];

            //init_coff();	
        }

        Stack global_sym_stack = new Stack();
        Stack local_sym_stack = new Stack();

        private int rsym = 0;
        private int ind = 0;
        private int loc = 0;
        int func_begin_ind = 0;
        int func_ret_sub = 0;
        Symbol char_pointer_type = new Symbol();
        Symbol int_type = new Symbol();
        Symbol default_func_type = new Symbol();
        Operand[] opstack = new Operand[256];
        Operand optop = new Operand();
        public int syntax_state; //语法状态
        public int syntax_level; //缩进级别

        public Action FuncSyntaxIndent;

        int calc_align(int n, int align){
            return ((n + align - 1) & (~(align - 1)));
        }

        int type_size(ref int align){
            var bt = curTypeId & T_BTYPE;
            switch (bt) {
                case T_STRUCT: {
                    align = CurSymbol.align;
                    return CurSymbol.align;
                }
                case T_INT:
                    align = 4;
                    return 4;
                case T_SHORT:
                    align = 2;
                    return 2;
                default:
                    align = 1;
                    return 1;
            }
        }

        void syntax_indent(){
            FuncSyntaxIndent();
        }

        bool HasStruct(int tokenId){
            if (tokenId >= allTokens.Count)
                return false;
            return _curDomain.HasStruct(tokenId);
        }

        SymFunction FindFunction(int tokenId, bool isRecursiveFind){
            if (tokenId >= allTokens.Count)
                return null;
            return _curDomain.FindSymbol(tokenId, isRecursiveFind) as SymFunction;
        }

        SymVar FindVariable(int tokenId, bool isRecursiveFind){
            if (tokenId >= allTokens.Count)
                return null;
            return _curDomain.FindSymbol(tokenId, isRecursiveFind) as SymVar;
        }


        private SymDomain global;

        public void TranslationUnit(){
            optop = new Operand();
            global = new SymDomain();
            PushDomain(global);
            while (curTokenId != TK_EOF) {
                external_declaration(SC_GLOBAL);
            }
        }

        private Stack<SymDomain> _domains = new Stack<SymDomain>();
        private SymDomain _curDomain;

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
            _curDomain.AddVariable(val);
        }

        public void AddFunction(SymFunction val){
            _curDomain.AddFunction(val);
        }

        public void AddParam(SymVar val){
            _curDomain.AddParam(val);
        }

        public void AddStruct(SymStruct val){
            val.TypeId = CurStructMaxIdx++;
            _curDomain.AddStruct(val);
        }

        public void AddDomain(SymDomain val){
            _curDomain.AddDomain(val);
        }


        void external_declaration(int l){
            int tokenId = 0, r = 0, addr = 0;
            bool has_init = false;
            Section sec = null;

            if (!type_specifier()) {
                Expect("<类型区分符>");
            }

            var firstTokenId = lastTokenId;
            if (curTypeId == T_STRUCT && curTokenId == TK_SEMICOLON) {
                GetToken();
                return;
            }

            while (true) {
                var force_align = -1;
                var sym = declarator(ref tokenId, ref force_align);

                if (curTokenId == TK_BEGIN) //函数定义
                {
                    var symFunc = sym as SymFunction;
                    if (symFunc == null)
                        Expect("<函数定义>");
                    AddFunction(symFunc);
                    symFunc.RetTypeId = firstTokenId;
                    funcbody(symFunc);
                    break;
                }

                if (sym is SymFunction) // 函数声明
                {
                    Error(" 不支持函数声明");
                }
                else //变量声明
                {
                    var symVar = sym as SymVar;
                    sym.__name = "var_" + (l == SC_GLOBAL ? "l_" : "g_") + allTokens[tokenId].name;
                    AddVariable(symVar);
                    symVar.typeId = firstTokenId;
                    symVar.parentDomain = _curDomain;
                    r = 0;
                    if (symVar.isArray)
                        r |= SC_LVAL;

                    r |= l;
                    has_init = (curTokenId == TK_ASSIGN);

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

                if (curTokenId == TK_COMMA) {
                    GetToken();
                }
                else {
                    syntax_state = SNTX_LF_HT;
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

        public int curTypeId; //当前类型
        public Symbol CurSymbol; //当前符号


        bool type_specifier(){
            bool type_found = false;
            curTypeId = 0;
            lastTokenId = curTokenId;
            switch (curTokenId) {
                case KW_CHAR:
                    curTypeId = T_CHAR;
                    type_found = true;
                    syntax_state = SNTX_SP;
                    GetToken();
                    break;
                case KW_SHORT:
                    curTypeId = T_SHORT;
                    type_found = true;
                    syntax_state = SNTX_SP;
                    GetToken();
                    break;
                case KW_VOID:
                    curTypeId = T_VOID;
                    type_found = true;
                    syntax_state = SNTX_SP;
                    GetToken();
                    break;
                case KW_INT:
                    curTypeId = T_INT;
                    syntax_state = SNTX_SP;
                    type_found = true;
                    GetToken();
                    break;
                case KW_STRUCT:
                    syntax_state = SNTX_SP;
                    struct_specifier();
                    curTypeId = T_STRUCT;
                    type_found = true;
                    break;
                default:
                    var hasStruct = HasStruct(curTokenId);
                    if (!hasStruct) {
                        Error("Unknown type" + curTokenId);
                    }
                    else {
                        lastTokenId = curTokenId;
                        curTypeId = T_CLASS;
                        type_found = true;
                        syntax_state = SNTX_SP;
                        GetToken();
                    }

                    break;
            }

            return type_found;
        }

        public int lastTokenId;

        void AddStruct(int typeId, SymStruct structInfo){
            if (allTokens[typeId].symbol != null) {
                Error("type already exist" + typeId + " name" + structInfo.__name);
                return;
            }

            structInfo.tokenId = typeId;
            allTokens[typeId].symbol = structInfo;
        }

        SymStruct FindStruct(int typeId){
            if (typeId >= allTokens.Count)
                return null;
            return allTokens[typeId]?.symbol as SymStruct;
        }

        Symbol FindIdentifier(int typeId){
            if (typeId >= allTokens.Count)
                return null;
            return allTokens[typeId]?.symbol;
        }

        void struct_specifier(){
            GetToken();
            var typeId = curTokenId;

            syntax_state = SNTX_DELAY; // 新取单词不即时输出，延迟到取出单词后根据单词类型判断输出格式
            GetToken();

            if (curTokenId == TK_BEGIN) // 适用于结构体定义
                syntax_state = SNTX_LF_HT;
            else if (curTokenId == TK_CLOSEPA) // 适用于 sizeof(struct struct_name)
                syntax_state = SNTX_NUL;
            else // 适用于结构变量声明
                syntax_state = SNTX_SP;
            syntax_indent();

            if (typeId < TK_IDENT) // 关键字不能作为结构名称
                Expect("结构体名");
            var sym = FindStruct(typeId);
            if (sym == null) {
                curTypeId = KW_STRUCT;
                // -1表示结构体未定义
                sym = new SymStruct();
                sym.__name = allTokens[typeId].name;
                sym.align = 0;
                AddStruct(typeId, sym);
                AddStruct(sym);
            }

            PushDomain(sym);
            curTypeId = T_STRUCT;
            CurSymbol = sym;
            if (curTokenId == TK_BEGIN) {
                struct_declaration_list(sym);
            }

            PopDomain();
        }

        void struct_declaration_list(SymStruct curStruct){
            syntax_state = SNTX_LF_HT; // 第一个结构体成员与'{'不写在一行
            syntax_level++; // 结构体成员变量声明，缩进增加一级
            GetToken();
            int maxalign = 1;
            int offset = 0;
            while (curTokenId != TK_END) {
                struct_declaration(ref maxalign, ref offset, curStruct);
            }

            SkipToken(TK_END);
            syntax_state = SNTX_LF_HT;

            curStruct.memSize = calc_align(offset, maxalign); //结构体大小
            curStruct.align = maxalign; //结构体对齐
        }

        void struct_declaration(ref int maxalign, ref int offset, SymStruct curStruct){
            type_specifier();
            while (true) {
                int force_align = 0;
                int align = 0;
                int tokenId = 0;
                var ss = declarator(ref tokenId, ref force_align) as SymVar;
                var size = type_size(ref align);

                if ((force_align & ALIGN_SET) != 0)
                    align = force_align & ~ALIGN_SET;

                offset = calc_align(offset, align);
                if (align > maxalign)
                    maxalign = align;

                ss.offset = offset;
                ss.tokenId = tokenId;
                ss.__name = curStruct.__name + ":" + allTokens[tokenId].name;
                curStruct.AddVariable(ss);
                offset += size;
                if (curTokenId == TK_SEMICOLON || curTokenId == TK_EOF)
                    break;
                SkipToken(TK_COMMA);
            }

            syntax_state = SNTX_LF_HT;
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
            if (curTokenId == KW_CDECL || curTokenId == KW_STDCALL) {
                fc = curTokenId;
                syntax_state = SNTX_SP;
                GetToken();
            }
        }

        void struct_member_alignment(ref int force_align){
            int align = 1;
            if (curTokenId == KW_ALIGN) {
                GetToken();
                SkipToken(TK_OPENPA);
                if (curTokenId == TK_CINT) {
                    GetToken();
                    align = (int) tkvalue;
                }
                else
                    Expect("整数常量");

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
            if (curTokenId >= TK_IDENT) {
                tokenId = curTokenId;
                GetToken();
            }
            else {
                Expect("标识符");
            }

            return direct_declarator_postfix(func_call, tokenId);
        }

        Symbol direct_declarator_postfix(int func_call, int tokenId){
            if (curTokenId == TK_OPENPA) { //function
                return parameter_type_list(func_call, tokenId);
            }
            else if (curTokenId == TK_OPENBR) { //Array
                GetToken();
                var symbol = new SymVar();
                symbol.__name = allTokens[tokenId].name;
                symbol.isArray = true;
                symbol.tokenId = tokenId;
                if (curTokenId == TK_CINT) {
                    GetToken();
                    symbol.arraySize = (int) tkvalue;
                }

                SkipToken(TK_CLOSEBR);
                return symbol;
                //direct_declarator_postfix(func_call,typeId);//不允许 声明函数数组
            }
            else { //normal Var
                var symbol = new SymVar();
                symbol.tokenId = tokenId;
                symbol.__name = allTokens[tokenId].name;
                return symbol;
            }
        }

        SymFunction parameter_type_list(int func_call, int tokenId){
            int n = 0;
            Type pt = new Type();

            GetToken();
            Symbol first = null;
            Symbol plast = first;

            var symFunc = FindFunction(tokenId, false);
            bool hasDefined = symFunc != null;
            if (symFunc == null) {
                //add function
                symFunc = new SymFunction();
                symFunc.tokenId = tokenId;
                symFunc.__name = allTokens[tokenId].name;
            }

            while (curTokenId != TK_CLOSEPA) {
                if (!type_specifier()) {
                    Error("无效类型标识符");
                }

                int isForceAlign = -1;
                var symVar = new SymVar();
                CurSymbol = symVar;
                var s = declarator(ref n, ref isForceAlign);
                if (!hasDefined) {
                    symFunc.AddParam(s as SymVar);
                }

                if (curTokenId == TK_CLOSEPA)
                    break;
                SkipToken(TK_COMMA);
                if (curTokenId == TK_ELLIPSIS) {
                    func_call = KW_CDECL;
                    GetToken();
                    break;
                }
            }

            syntax_state = SNTX_DELAY;
            SkipToken(TK_CLOSEPA);
            if (curTokenId == TK_BEGIN) // 函数定义
                syntax_state = SNTX_LF_HT;
            else // 函数声明
                syntax_state = SNTX_NUL;
            syntax_indent();
            curTypeId = T_FUNC;
            CurSymbol = symFunc;
            return symFunc;
        }


        void funcbody(SymFunction sym){
            ind = sec_text.data_offset;
            coffsym_add_update(sym, ind, sec_text.index, CST_FUNC, IMAGE_SYM_CLASS_EXTERNAL);
            gen_prolog(sym.typeId);
            rsym = 0;
            var bsym = -1;
            var csym = -1;
            compound_statement(ref bsym, ref csym, sym);
            backpatch(rsym, ind);
            gen_epilog();
            sec_text.data_offset = ind;
        }

        bool is_type_specifier(int tokenId){
            switch (tokenId) {
                case KW_CHAR:
                case KW_SHORT:
                case KW_INT:
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
            switch (curTokenId) {
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
            syntax_state = SNTX_LF_HT;
            syntax_level++; // 复合语句，缩进增加一级

            SymDomain domain = parent;
            if (parent == null) {
                domain = new SymDomain();
                AddDomain(domain);
            }
            PushDomain(domain);
            GetToken();
            while (is_type_specifier(curTokenId)) {
                external_declaration(SC_LOCAL);
            }

            while (curTokenId != TK_END) {
                statement(ref bsym, ref csym);
            }

            PopDomain();
            syntax_state = SNTX_LF_HT;
            GetToken();
        }

        void if_statement(ref int bsym, ref int csym){
            int a, b;
            syntax_state = SNTX_SP;
            GetToken();
            SkipToken(TK_OPENPA);
            expression();
            syntax_state = SNTX_LF_HT;
            SkipToken(TK_CLOSEPA);
            a = gen_jcc(0);
            var ifDomain = new SymDomain();
            AddDomain(ifDomain);
            PushDomain(ifDomain);
            statement(ref bsym, ref csym);
            PopDomain();
            if (curTokenId == KW_ELSE) {
                syntax_state = SNTX_LF_HT;
                GetToken();
                b = gen_jmpforward(0);
                backpatch(a, ind);
                var elseDomain = new SymDomain();
                AddDomain(elseDomain);
                PushDomain(elseDomain);
                statement(ref bsym, ref csym);
                PopDomain();
                backpatch(b, ind); /* 反填else跳转 */
            }
            else
                backpatch(a, ind);
        }

        void for_statement(ref int bsym, ref int csym){
            var forDomain = new SymDomain();
            AddDomain(forDomain);
            PushDomain(forDomain);

            int a, b, c, d, e;
            GetToken();
            SkipToken(TK_OPENPA);
            if (curTokenId != TK_SEMICOLON) {
                expression();
                operand_pop();
            }

            SkipToken(TK_SEMICOLON);
            d = ind;
            c = ind;
            a = 0;
            b = 0;
            if (curTokenId != TK_SEMICOLON) {
                expression();
                a = gen_jcc(0);
            }

            SkipToken(TK_SEMICOLON);
            if (curTokenId != TK_CLOSEPA) {
                e = gen_jmpforward(0);
                c = ind;
                expression();
                operand_pop();
                gen_jmpbackword(d);
                backpatch(e, ind);
            }

            syntax_state = SNTX_LF_HT;
            SkipToken(TK_CLOSEPA);
            statement(ref a, ref b); //只有此处用到break,及continue,一个循环中可能有多个break,或多个continue,故需要拉链以备反填
            gen_jmpbackword(c);
            backpatch(a, ind);
            backpatch(b, c);
            PopDomain();
        }

        void continue_statement(ref int csym){
            if (csym == -1)
                Error("此处不能用continue");
            csym = gen_jmpforward(csym);
            GetToken();
            syntax_state = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
        }

        void break_statement(ref int bsym){
            if (bsym == -1)
                Error("此处不能用break");
            bsym = gen_jmpforward(bsym);
            GetToken();
            syntax_state = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
        }

        void return_statement(){
            syntax_state = SNTX_DELAY;
            GetToken();
            if (curTokenId == TK_SEMICOLON) // 适用于 return;
                syntax_state = SNTX_NUL;
            else // 适用于 return <expression>;
                syntax_state = SNTX_SP;
            syntax_indent();

            if (curTokenId != TK_SEMICOLON) {
                expression();
                load_1(REG_IRET, optop);
                operand_pop();
            }

            syntax_state = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
            rsym = gen_jmpforward(rsym);
        }

        void expression_statement(){
            if (curTokenId != TK_SEMICOLON) {
                expression();
                operand_pop();
            }

            syntax_state = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
        }

        void expression(){
            while (true) {
                assignment_expression();
                if (curTokenId != TK_COMMA)
                    break;
                operand_pop();
                GetToken();
            }
        }

        void assignment_expression(){
            equality_expression();
            if (curTokenId == TK_ASSIGN) {
                check_lvalue();
                GetToken();
                assignment_expression();
                store0_1();
            }
        }

        void equality_expression(){
            int t;
            relational_expression();
            while (curTokenId == TK_EQ || curTokenId == TK_NEQ) {
                t = curTokenId;
                GetToken();
                relational_expression();
                gen_op(t);
            }
        }

        void relational_expression(){
            int t;
            additive_expression();
            while ((curTokenId == TK_LT || curTokenId == TK_LEQ) ||
                   curTokenId == TK_GT || curTokenId == TK_GEQ) {
                t = curTokenId;
                GetToken();
                additive_expression();
                gen_op(t);
            }
        }

        void additive_expression(){
            int t;
            multiplicative_expression();
            while (curTokenId == TK_PLUS || curTokenId == TK_MINUS) {
                t = curTokenId;
                GetToken();
                multiplicative_expression();
                gen_op(t);
            }
        }

        void multiplicative_expression(){
            int t;
            unary_expression();
            while (curTokenId == TK_STAR || curTokenId == TK_DIVIDE || curTokenId == TK_MOD) {
                t = curTokenId;
                GetToken();
                unary_expression();
                gen_op(t);
            }
        }

        void unary_expression(){
            switch (curTokenId) {
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
                    operand_push(int_type, 0);
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
                Error("sizeof计算类型尺寸失败");
            operand_push(int_type, size);
        }

        void postfix_expression(){
            Symbol s;
            primary_expression();
            while (true) {
                if (curTokenId == TK_DOT || curTokenId == TK_POINTSTO) {
                    if (curTokenId == TK_DOT)
                        indirection();
                    cancel_lvalue();
                    GetToken();
                    var symVar = optop.sym as SymVar;
                    var symStruct = (symVar?.parentDomain ?? global).FindSymbol(symVar.typeId, true) as SymStruct;
                    if (symStruct == null)
                        Expect("结构体变量");
                    optop.sym = symStruct?.FindSymbol(curTokenId, false);
                    if (optop.sym == null)
                        Error("没有此成员变量: %s", allTokens[curTokenId].name);
                    GetToken();
                }
                else if (curTokenId == TK_OPENBR) {
                    GetToken();
                    expression();
                    gen_op(TK_PLUS);
                    indirection();
                    SkipToken(TK_CLOSEBR);
                }
                else if (curTokenId == TK_OPENPA) {
                    argument_expression_list();
                }
                else
                    break;
            }
        }

        void primary_expression(){
            int t = 0, r = 0, addr = 0;
            Type type = new Type();
            Symbol s;
            Section sec = new Section();
            switch (curTokenId) {
                case TK_CINT:
                case TK_CCHAR:
                    operand_push(int_type, (int) tkvalue);
                    GetToken();
                    break;
                case TK_CSTR:
                    t = T_CHAR;
                    type.typeId = t;
                    type.typeId |= T_ARRAY;
                    initializer(true, addr);
                    break;
                case TK_OPENPA:
                    GetToken();
                    expression();
                    SkipToken(TK_CLOSEPA);
                    break;
                default:
                    t = curTokenId;
                    GetToken();
                    if (t < TK_IDENT)
                        Expect("标识符或常量");
                    s = FindVariable(t, true);
                    if (s == null) {
                        if (curTokenId != TK_OPENPA)
                            Error("'%s'未声明 \n", allTokens[t].name);
                        else {
                            s = FindFunction(t, true);
                        }
                    }

                    operand_push(s, null);

                    optop.sym = s;
                    optop.value = 0;
                    break;
            }
        }

        void argument_expression_list(){
            Operand ret;
            int nb_args = 0;
            GetToken();
            var r = REG_IRET;
            var value = 0;
            if (curTokenId != TK_CLOSEPA) {
                for (;;) {
                    assignment_expression();
                    nb_args++;
                    if (curTokenId == TK_CLOSEPA)
                        break;
                    SkipToken(TK_COMMA);
                }
            }

            var symFunction = optop.sym as SymFunction;
            if (symFunction != null) {
                Debug.Assert(symFunction != null, nameof(symFunction) + " != null");
                if (nb_args != symFunction.ParamsCount)
                    Error("实参个数少于函数形参个数"); //讲一下形参，实参
            }

            SkipToken(TK_CLOSEPA);
            gen_invoke(nb_args);
            if (symFunction != null) {
                operand_push(FindStruct(symFunction.RetTypeId), value);
            }
        }
    }
}