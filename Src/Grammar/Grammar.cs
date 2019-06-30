using System;
using System.Collections.Generic;
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

        Section allocate_storage(Type type, int r, int has_init, int v, ref int addr){
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

        void operand_pop(){ }
        void gen_jmpbackword(int a){ }


        int load_1(int rc, Operand opd){
            return 4;
        }

        void store0_1(){ }
        void gen_op(int op){ }
        void cancel_lvalue(){ }
        void indirection(){ }
        void operand_push(Type type, int r, int value){ }

        void gen_invoke(int nb_args){ }
        void gen_prolog(Type func_type){ }
        void gen_epilog(){ }
    }

    public unsafe partial class Lex {
        public void GrammarInit(){
            //sym_sec_rdata = sec_sym_put(".rdata",0);

            int_type.t = T_INT;
            char_pointer_type.t = T_CHAR;
            mk_pointer(char_pointer_type);
            default_func_type.t = T_FUNC;
            default_func_type._ref = sym_push(SC_ANOM, int_type, KW_CDECL, 0);

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
        Type char_pointer_type = new Type();
        Type int_type = new Type();
        Type default_func_type = new Type();
        Operand[] opstack = new Operand[256];
        Operand optop = new Operand();
        public int syntax_state; //语法状态
        public int syntax_level; //缩进级别

        public Action FuncSyntaxIndent;

        int calc_align(int n, int align){
            return ((n + align - 1) & (~(align - 1)));
        }

        int type_size(Type type, ref int a){
            var bt = type.t & T_BTYPE;
            var size = 0;
            switch (bt) {
                case T_STRUCT: {
                    var s = type._ref;
                    a = s.align;
                    return s.value;
                }
                case T_PTR:
                    if ((type.t & T_ARRAY) != 0) {
                        return type_size(type._ref.type, ref a) * type._ref.value;
                    }
                    else {
                        a = PTR_SIZE;
                        return PTR_SIZE;
                    }
                case T_INT:
                    a = 4;
                    return 4;
                case T_SHORT:
                    a = 2;
                    return 2;
                default:
                    a = 1;
                    return 1;
            }
        }

        void syntax_indent(){
            FuncSyntaxIndent();
        }

//stack
        void stack_init(Stack stack, int initsize){ }

        object stack_push(Stack stack, object element, int size){
            stack.Push(element);
            return null;
        }

        void stack_pop(Stack stack){
            stack.Pop();
        }

        object stack_get_top(Stack stack){
            return stack.GetTop();
        }

        bool stack_is_empty(Stack stack){
            return stack.Count == 0;
        }

        void stack_destroy(Stack stack){ }

        void mk_pointer(Type t){
            Symbol s = sym_push(SC_ANOM, t, 0, -1);
            t.t = T_PTR;
            t._ref = s;
        }

        //symbol
        Symbol struct_search(int v){
            if (v >= allTokens.Count)
                return null;
            return allTokens[v].symStruct;
        }

        Symbol struct_search_or_create(int v){
            if (v >= allTokens.Count)
                return null;
            if (allTokens[v].symStruct == null) {
                allTokens[v].symStruct = new Symbol();
            }

            return allTokens[v].symStruct;
        }

        Symbol sym_search(int v){
            if (v >= allTokens.Count)
                return null;
            return allTokens[v].symIdentifier;
        }

        Symbol sym_direct_push(Stack ss, int v, Type type, int c){
            var s = new Symbol {
                type = type,
                value = c,
                code = v,
                next = null
            };
            ss.Push(s);
            return s;
        }

        Symbol sym_push(int v, Type type, int r, int c){
            var ss = local_sym_stack.Count == 0 ? local_sym_stack : global_sym_stack;
            var sb = sym_direct_push(ss, v, type, c);
            sb.align = r;
            if (v.IsSTRUCT() || v.IsDefine()) {
                var ts = allTokens[v.GetStorageType()];
                if (v.IsSTRUCT()) {
                    sb.prev_tok = ts.symStruct;
                    ts.symStruct = sb;
                }
                else {
                    sb.prev_tok = ts.symIdentifier;
                    ts.symIdentifier = sb;
                }
            }

            return sb;
        }

        void sym_pop(Stack ptop, Symbol b){
            var s = (Symbol) stack_get_top(ptop);
            while (s != b) {
                var v = s.code;
                if (v.IsSTRUCT() || v.IsDefine()) {
                    var ts = allTokens[v.GetStorageType()];
                    if (v.IsSTRUCT()) {
                        ts.symStruct = s.prev_tok;
                    }
                    else {
                        ts.symIdentifier = s.prev_tok;
                    }
                }

                stack_pop(ptop);
                s = stack_get_top(ptop) as Symbol;
            }
        }

        Symbol func_sym_push(int v, Type type){
            var s = sym_direct_push(global_sym_stack, v, type, 0);
            s.prev_tok = null;
            s.__name = "func_" + allTokens[v].name;
            var ps = allTokens[v].symIdentifier;
            if (ps == null) {
                allTokens[v].symIdentifier = s;
                return s;
            }
            else {
                while (ps.prev_tok != null) {
                    ps = ps.prev_tok;
                }

                ps.prev_tok = s;
                return s;
            }
        }


        Symbol var_sym_put(Type type, int r, int v, int addr){
            Symbol sym = null;
            if (r.IsLOCAL()) {
                sym = sym_push(v, type, r, addr);
            }
            else if (v != 0 && r.IsGLOBAL()) {
                sym = sym_search(v);
            }

            if (sym != null) {
                Error($"!duplicate define {allTokens[v]}");
            }
            else {
                sym = sym_push(v, type, r | SC_SYM, 0);
            }

            return sym;
        }

        Symbol sec_sym_put(string sec, int c){
            var type = new Type();
            type.t = T_INT; //TODO 确认是INt
            var tp = InsertToken(sec);
            curTokenId = tp.id;
            return sym_push(curTokenId, type, SC_GLOBAL, c);
        }

        string get_tkstr(int v){
            if (v >= allTokens.Count) {
                return null;
            }
            else if (v >= TK_CINT && v <= TK_CSTR)
                return sourcestr.Data;
            else
                return allTokens[v].name;
        }


        public void TranslationUnit(){
            optop = new Operand();
            while (curTokenId != TK_EOF) {
                external_declaration(SC_GLOBAL, null);
            }
        }

        void external_declaration(int l, Symbol parent){
            Type btype = new Type();
            Type type;
            int v = 0, r = 0, addr = 0;
            bool has_init = false;
            Symbol sym;
            Section sec = null;

            if (!type_specifier(btype)) {
                Expect("<类型区分符>");
            }

            if (btype.t == T_STRUCT && curTokenId == TK_SEMICOLON) {
                GetToken();
                return;
            }

            while (true) {
                type = btype;
                var force_align = -1;
                declarator(type, ref v, ref force_align);

                if (curTokenId == TK_BEGIN) //函数定义
                {
                    if (l == SC_LOCAL)
                        Error("不支持函数嵌套定义");

                    if ((type.t & T_BTYPE) != T_FUNC)
                        Expect("<函数定义>");

                    sym = sym_search(v);
                    if (sym != null) // 函数前面声明过，现在给出函数定义
                    {
                        if ((sym.type.t & T_BTYPE) != T_FUNC)
                            Error("'%s'重定义", get_tkstr(v));
                        sym.type = type;
                    }
                    else {
                        sym = func_sym_push(v, type);
                    }

                    sym.align = SC_SYM | SC_GLOBAL;
                    sym.__parent = parent;
                    funcbody(sym);
                    break;
                }
                else {
                    if ((type.t & T_BTYPE) == T_FUNC) // 函数声明
                    {
                        if (sym_search(v) == null) {
                            sym_push(v, type, SC_GLOBAL | SC_SYM, 0);
                        }
                    }
                    else //变量声明
                    {
                        r = 0;
                        if ((type.t & T_ARRAY) != 0)
                            r |= SC_LVAL;

                        r |= l;
                        has_init = (curTokenId == TK_ASSIGN);

                        if (has_init) {
                            GetToken(); //不能放到后面，char str[]="abc"情况，需要allocate_storage求字符串长度				    
                        }

                        sec = allocate_storage(type, r, has_init ? 1 : 0, v, ref addr);
                        sym = var_sym_put(type, r, v, addr);
                        sym.__name = "var_" + (l == SC_GLOBAL ? "l_" : "g_") + allTokens[v].name;
                        sym.__parent = parent;
                        
                        if (l == SC_GLOBAL)
                            coffsym_add_update(sym, addr, sec.index, 0, IMAGE_SYM_CLASS_EXTERNAL);

                        if (has_init) {
                            initializer(type, addr, sec);
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
        }


        void initializer(Type type, int c, Section sec){
            if ((type.t & T_ARRAY) != 0 && sec != null) {
                init_array(type, sec, c, 0);
                GetToken();
            }
            else {
                assignment_expression();
                init_variable(type, sec, c, 0);
            }
        }

        bool type_specifier(Type type){
            bool type_found = false;
            Type type1 = new Type();
            int t = 0;
            switch (curTokenId) {
                case KW_CHAR:
                    t = T_CHAR;
                    type_found = true;
                    syntax_state = SNTX_SP;
                    GetToken();
                    break;
                case KW_SHORT:
                    t = T_SHORT;
                    type_found = true;
                    syntax_state = SNTX_SP;
                    GetToken();
                    break;
                case KW_VOID:
                    t = T_VOID;
                    type_found = true;
                    syntax_state = SNTX_SP;
                    GetToken();
                    break;
                case KW_INT:
                    t = T_INT;
                    syntax_state = SNTX_SP;
                    type_found = true;
                    GetToken();
                    break;
                case KW_STRUCT:
                    syntax_state = SNTX_SP;
                    struct_specifier(type1);
                    type._ref = type1._ref;
                    t = T_STRUCT;
                    type_found = true;
                    break;
                default:
                    break;
            }

            type.t = t;
            return type_found;
        }

        void struct_specifier(Type type){
            int v;
            Symbol s;
            Type type1 = new Type();

            GetToken();
            v = curTokenId;

            syntax_state = SNTX_DELAY; // 新取单词不即时输出，延迟到取出单词后根据单词类型判断输出格式
            GetToken();

            if (curTokenId == TK_BEGIN) // 适用于结构体定义
                syntax_state = SNTX_LF_HT;
            else if (curTokenId == TK_CLOSEPA) // 适用于 sizeof(struct struct_name)
                syntax_state = SNTX_NUL;
            else // 适用于结构变量声明
                syntax_state = SNTX_SP;
            syntax_indent();

            if (v < TK_IDENT) // 关键字不能作为结构名称
                Expect("结构体名");
            s = struct_search(v);
            if (s == null) {
                type1.t = KW_STRUCT;
                // -1表示结构体未定义
                s = sym_push(v | SC_STRUCT, type1, 0, -1);
                s.__name = allTokens[v].name;
                s.align = 0;
            }

            type.t = T_STRUCT;
            type._ref = s;

            if (curTokenId == TK_BEGIN) {
                struct_declaration_list(type);
            }
        }

        void struct_declaration_list(Type type){
            var s = type._ref;
            syntax_state = SNTX_LF_HT; // 第一个结构体成员与'{'不写在一行
            syntax_level++; // 结构体成员变量声明，缩进增加一级
            GetToken();
            if (s.value != -1)
                Error("结构体已定义");
            int maxalign = 1;
            int offset = 0;
            while (curTokenId != TK_END) {
                struct_declaration(ref maxalign, ref offset, s);
            }

            SkipToken(TK_END);
            syntax_state = SNTX_LF_HT;

            s.value = calc_align(offset, maxalign); //结构体大小
            s.align = maxalign; //结构体对齐
        }

        void struct_declaration(ref int maxalign, ref int offset, Symbol structSym){
            int v, size, align = 0;
            Type btype = new Type();
            int force_align = 0;
            type_specifier(btype);
            while (true) {
                v = 0;
                var type1 = btype;
                declarator(type1, ref v, ref force_align);
                size = type_size(type1, ref align);

                if ((force_align & ALIGN_SET) != 0)
                    align = force_align & ~ALIGN_SET;

                offset = calc_align(offset, align);

                if (align > maxalign)
                    maxalign = align;
                var ss = sym_push(v | SC_MEMBER, type1, 0, offset);
                offset += size;
                ss.__parent = structSym;
                ss.__name = structSym.__name + ":" + allTokens[v].name;
                ss.next = structSym.next;
                structSym.next = ss;

                if (curTokenId == TK_SEMICOLON || curTokenId == TK_EOF)
                    break;
                SkipToken(TK_COMMA);
            }

            syntax_state = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
        }


        void declarator(Type type, ref int v, ref int force_align){
            int fc = 0;
            while (curTokenId == TK_STAR) {
                mk_pointer(type);
                GetToken();
            }

            function_calling_convention(ref fc);
            if (force_align != -1)
                struct_member_alignment(ref force_align);
            direct_declarator(type, ref v, fc);
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

        void direct_declarator(Type type, ref int v, int func_call){
            if (curTokenId >= TK_IDENT) {
                v = curTokenId;
                GetToken();
            }
            else {
                Expect("标识符");
            }

            direct_declarator_postfix(type, func_call);
        }

        void direct_declarator_postfix(Type type, int func_call){
            if (curTokenId == TK_OPENPA) {
                parameter_type_list(type, func_call);
            }
            else if (curTokenId == TK_OPENBR) {
                GetToken();
                var n = -1;
                if (curTokenId == TK_CINT) {
                    GetToken();
                    n = (int) tkvalue;
                }

                SkipToken(TK_CLOSEBR);
                direct_declarator_postfix(type, func_call);
                var s = sym_push(SC_ANOM, type, 0, n);
                type.t = T_ARRAY | T_PTR;
                type._ref = s;
            }
        }

        void parameter_type_list(Type type, int func_call){
            int n = 0;
            Symbol s;
            Type pt = new Type();

            GetToken();
            Symbol first = null;
            Symbol plast = first;

            while (curTokenId != TK_CLOSEPA) {
                if (!type_specifier(pt)) {
                    Error("无效类型标识符");
                }

                int isForceAlign = -1;
                declarator(pt, ref n, ref isForceAlign);
                s = sym_push(n | SC_PARAMS, pt, 0, 0);
                plast = s;
                plast = s.next;
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

            // 此处将函数返回类型存储，然后指向参数，最后将type设为函数类型，引用的相关信息放在ref中
            s = sym_push(SC_ANOM, type, func_call, 0);
            s.next = first;
            type.t = T_FUNC;
            type._ref = s;
        }


        void funcbody(Symbol sym){
            ind = sec_text.data_offset;
            coffsym_add_update(sym, ind, sec_text.index, CST_FUNC, IMAGE_SYM_CLASS_EXTERNAL);
            /* 放一匿名符号在局部符号表中 */
            sym_direct_push(local_sym_stack, SC_ANOM, int_type, 0);
            gen_prolog(sym.type);
            rsym = 0;
            var bsym = -1;
            var csym = -1;
            compound_statement(ref bsym, ref csym, sym);
            backpatch(rsym, ind);
            gen_epilog();
            sec_text.data_offset = ind;
            sym_pop(local_sym_stack, null); /* 清空局部符号栈*/
        }

        bool is_type_specifier(int v){
            switch (v) {
                case KW_CHAR:
                case KW_SHORT:
                case KW_INT:
                case KW_VOID:
                case KW_STRUCT:
                    return true;
                default:
                    break;
            }

            return false;
        }

        void statement(ref int bsym, ref int csym, Symbol parent){
            switch (curTokenId) {
                case TK_BEGIN:
                    compound_statement(ref bsym, ref csym, parent);
                    break;
                case KW_IF:
                    if_statement(ref bsym, ref csym, parent);
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
                    for_statement(ref bsym, ref csym, parent);
                    break;
                default:
                    expression_statement();
                    break;
            }
        }

        void compound_statement(ref int bsym, ref int csym, Symbol parent){
            Symbol s;
            s = (Symbol) stack_get_top(local_sym_stack);
            syntax_state = SNTX_LF_HT;
            syntax_level++; // 复合语句，缩进增加一级

            GetToken();
            while (is_type_specifier(curTokenId)) {
                external_declaration(SC_LOCAL, parent);
            }

            while (curTokenId != TK_END) {
                statement(ref bsym, ref csym, parent);
            }

            sym_pop(local_sym_stack, s);
            syntax_state = SNTX_LF_HT;
            GetToken();
        }

        void if_statement(ref int bsym, ref int csym, Symbol parent){
            int a, b;
            syntax_state = SNTX_SP;
            GetToken();
            SkipToken(TK_OPENPA);
            expression();
            syntax_state = SNTX_LF_HT;
            SkipToken(TK_CLOSEPA);
            a = gen_jcc(0);
            statement(ref bsym, ref csym, parent);
            if (curTokenId == KW_ELSE) {
                syntax_state = SNTX_LF_HT;
                GetToken();
                b = gen_jmpforward(0);
                backpatch(a, ind);
                statement(ref bsym, ref csym, parent);
                backpatch(b, ind); /* 反填else跳转 */
            }
            else
                backpatch(a, ind);
        }

        void for_statement(ref int bsym, ref int csym, Symbol parent){
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
            statement(ref a, ref b, parent); //只有此处用到break,及continue,一个循环中可能有多个break,或多个continue,故需要拉链以备反填
            gen_jmpbackword(c);
            backpatch(a, ind);
            backpatch(b, c);
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
                    if ((optop.type.t & T_BTYPE) != T_FUNC &&
                        (optop.type.t & T_ARRAY) != 0)
                        cancel_lvalue();
                    mk_pointer(optop.type);
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
                    operand_push(int_type, SC_GLOBAL, 0);
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
            Type type = new Type();

            GetToken();
            SkipToken(TK_OPENPA);
            type_specifier(type);
            SkipToken(TK_CLOSEPA);

            var size = type_size(type, ref align);
            if (size < 0)
                Error("sizeof计算类型尺寸失败");
            operand_push(int_type, SC_GLOBAL, size);
        }

        void postfix_expression(){
            Symbol s;
            primary_expression();
            while (true) {
                if (curTokenId == TK_DOT || curTokenId == TK_POINTSTO) {
                    if (curTokenId == TK_POINTSTO)
                        indirection();
                    cancel_lvalue();
                    GetToken();
                    if ((optop.type.t & T_BTYPE) != T_STRUCT)
                        Expect("结构体变量");
                    s = optop.type._ref;
                    curTokenId |= SC_MEMBER;
                    while ((s = s.next) != null) {
                        if (s.code == curTokenId)
                            break;
                    }

                    if (s == null)
                        Error("没有此成员变量: %s", get_tkstr(curTokenId & ~SC_MEMBER));
                    /* 成员变量地址 = 结构变量指针 + 成员变量偏移 */
                    optop.type = char_pointer_type; /* 成员变量的偏移是指相对于结构体首地址的字节偏移，因此此处变换类型为字节变量指针 */
                    operand_push(int_type, SC_GLOBAL, s.value);
                    gen_op(TK_PLUS); //执行后optop.value记忆了成员地址
                    /* 变换类型为成员变量数据类型 */
                    optop.type = s.type;
                    /* 数组变量不能充当左值 */
                    if ((optop.type.t & T_ARRAY) != 0) {
                        optop.r |= (ushort) SC_LVAL;
                    }

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
            Section sec = null;
            switch (curTokenId) {
                case TK_CINT:
                case TK_CCHAR:
                    operand_push(int_type, SC_GLOBAL, (int) tkvalue);
                    GetToken();
                    break;
                case TK_CSTR:
                    t = T_CHAR;
                    type.t = t;
                    mk_pointer(type);
                    type.t |= T_ARRAY;
                    sec = allocate_storage(type, SC_GLOBAL, 2, 0, ref addr);
                    var_sym_put(type, SC_GLOBAL, 0, addr);
                    initializer(type, addr, sec);
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
                    s = sym_search(t);
                    if (s == null) {
                        if (curTokenId != TK_OPENPA)
                            Error("'%s'未声明\n", get_tkstr(t));

                        s = func_sym_push(t, default_func_type); //允许函数不声明，直接引用
                        s.align = SC_GLOBAL | SC_SYM;
                    }

                    r = s.align;
                    operand_push(s.type, r, s.value);
                    /* 符号引用，操作数必须记录符号地址 */
                    if (optop!= null && (optop.r & (ushort) SC_SYM) != 0) {
                        optop.sym = s;
                        optop.value = 0; //用于函数调用，及全局变量引用 printf("g_cc=%c\n",g_cc);
                    }

                    break;
            }
        }

        void argument_expression_list(){
            Operand ret;
            Symbol s, sa;
            int nb_args;
            s = optop.type._ref;
            GetToken();
            sa = s.next;
            nb_args = 0;
            var type = s.type;
            var r = REG_IRET;
            var value = 0;
            if (curTokenId != TK_CLOSEPA) {
                for (;;) {
                    assignment_expression();
                    nb_args++;
                    if (sa != null)
                        sa = sa.next;
                    if (curTokenId == TK_CLOSEPA)
                        break;
                    SkipToken(TK_COMMA);
                }
            }

            if (sa != null)
                Error("实参个数少于函数形参个数"); //讲一下形参，实参
            SkipToken(TK_CLOSEPA);
            gen_invoke(nb_args);

            operand_push(type, r, value);
        }
    }
}