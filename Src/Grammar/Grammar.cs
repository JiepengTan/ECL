using System;
using System.Collections.Generic;
using static ErrorHandler;
using static LockstepECL.Define;
using static LockstepECL.EStorageClass;
using static LockstepECL.ESynTaxState;

namespace LockstepECL {
    public partial class Lex {
        public int syntax_state; //语法状态
        public int syntax_level; //缩进级别

        public Action FuncSyntaxIndent;

        public void TranslationUnit(){
            while (curTokenId != TK_EOF) {
                external_declaration(SC_GLOBAL);
            }
        }

        void external_declaration(int l){
            if (!type_specifier()) {
                Expect("<类型区分符>");
            }

            if (curTokenId == TK_SEMICOLON) {
                GetToken();
                return;
            }

            while (true) // 逐个分析声明或函数定义
            {
                declarator();
                if (curTokenId == TK_BEGIN) {
                    if (l == SC_LOCAL)
                        Error("不支持函数嵌套定义");
                    funcbody();
                    break;
                }
                else {
                    if (curTokenId == TK_ASSIGN) {
                        GetToken();
                        initializer();
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


        void initializer(){
            //	GetToken();
            assignment_expression();
        }

        bool type_specifier(){
            bool type_found = false;
            switch (curTokenId) {
                case KW_CHAR:
                    type_found = true;
                    syntax_state = SNTX_SP;
                    GetToken();
                    break;
                case KW_SHORT:
                    type_found = true;
                    syntax_state = SNTX_SP;
                    GetToken();
                    break;
                case KW_VOID:
                    type_found = true;
                    syntax_state = SNTX_SP;
                    GetToken();
                    break;
                case KW_INT:
                    syntax_state = SNTX_SP;
                    type_found = true;
                    GetToken();
                    break;
                case KW_FLOAT:
                    syntax_state = SNTX_SP;
                    type_found = true;
                    GetToken();
                    break;
                case KW_STRUCT:
                    syntax_state = SNTX_SP;
                    struct_specifier();
                    type_found = true;
                    break;
                default:
                    break;
            }

            return type_found;
        }

        void struct_specifier(){
            int v;

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

            if (curTokenId == TK_BEGIN) {
                struct_declaration_list();
            }
        }

        void struct_declaration_list(){
            int maxalign, offset;

            syntax_state = SNTX_LF_HT; // 第一个结构体成员与'{'不写在一行
            syntax_level++; // 结构体成员变量声明，缩进增加一级

            GetToken();
            while (curTokenId != TK_END) {
                struct_declaration();
            }

            SkipToken(TK_END);

            syntax_state = SNTX_LF_HT;
        }

        void struct_declaration(){
            type_specifier();
            while (true) {
                declarator();

                if (curTokenId == TK_SEMICOLON || curTokenId == TK_EOF)
                    break;
                SkipToken(TK_COMMA);
            }

            syntax_state = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
        }


        void declarator(){
            while (curTokenId == TK_STAR) {
                GetToken();
            }

            function_calling_convention();
            struct_member_alignment();
            direct_declarator();
        }

        void function_calling_convention(){
            if (curTokenId == KW_CDECL || curTokenId == KW_STDCALL) {
                syntax_state = SNTX_SP;
                GetToken();
            }
        }

        void struct_member_alignment(){
            if (curTokenId == KW_ALIGN) {
                GetToken();
                SkipToken(TK_OPENPA);
                if (curTokenId == TK_CINT || curTokenId == TK_LFloat) {
                    GetToken();
                }
                else
                    Expect("整数常量");

                SkipToken(TK_CLOSEPA);
            }
        }

        void direct_declarator(){
            if (curTokenId >= TK_IDENT) {
                GetToken();
            }
            else {
                Expect("标识符");
            }

            direct_declarator_postfix();
        }

        void direct_declarator_postfix(){
            int n;

            if (curTokenId == TK_OPENPA) {
                parameter_type_list();
            }
            else if (curTokenId == TK_OPENBR) {
                GetToken();
                if (curTokenId == TK_CINT) {
                    GetToken();
                    //n = (int) tkvalue;
                }
                else if (curTokenId == TK_LFloat) {
                    GetToken();
                    //n = (float) tkvalue;
                }

                SkipToken(TK_CLOSEBR);
                direct_declarator_postfix();
            }
        }

        private int func_call;

        void parameter_type_list(){
            GetToken();
            while (curTokenId != TK_CLOSEPA) {
                if (!type_specifier()) {
                    Error("无效类型标识符");
                }

                declarator();
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
        }

        void funcbody(){
            /* 放一匿名符号在局部符号表中 */
            compound_statement();
        }

        bool is_type_specifier(int v){
            switch (v) {
                case KW_CHAR:
                case KW_SHORT:
                case KW_INT:
                case KW_FLOAT:
                case KW_VOID:
                case KW_STRUCT:
                    return true;
                default:
                    break;
            }

            return false;
        }

        void statement(){
            switch (curTokenId) {
                case TK_BEGIN:
                    compound_statement();
                    break;
                case KW_IF:
                    if_statement();
                    break;
                case KW_RETURN:
                    return_statement();
                    break;
                case KW_BREAK:
                    break_statement();
                    break;
                case KW_CONTINUE:
                    continue_statement();
                    break;
                case KW_FOR:
                    for_statement();
                    break;
                default:
                    expression_statement();
                    break;
            }
        }

        void compound_statement(){
            syntax_state = SNTX_LF_HT;
            syntax_level++; // 复合语句，缩进增加一级

            GetToken();
            while (is_type_specifier(curTokenId)) {
                external_declaration(SC_LOCAL);
            }

            while (curTokenId != TK_END) {
                statement();
            }

            syntax_state = SNTX_LF_HT;
            GetToken();
        }

        void if_statement(){
            syntax_state = SNTX_SP;
            GetToken();
            SkipToken(TK_OPENPA);
            expression();
            syntax_state = SNTX_LF_HT;
            SkipToken(TK_CLOSEPA);
            statement();
            if (curTokenId == KW_ELSE) {
                syntax_state = SNTX_LF_HT;
                GetToken();
                statement();
            }
        }

        void for_statement(){
            GetToken();
            SkipToken(TK_OPENPA);
            if (curTokenId != TK_SEMICOLON) {
                expression();
            }

            SkipToken(TK_SEMICOLON);
            if (curTokenId != TK_SEMICOLON) {
                expression();
            }

            SkipToken(TK_SEMICOLON);
            if (curTokenId != TK_CLOSEPA) {
                expression();
            }

            syntax_state = SNTX_LF_HT;
            SkipToken(TK_CLOSEPA);
            statement(); //只有此处用到break,及continue,一个循环中可能有多个break,或多个continue,故需要拉链以备反填
        }

        void continue_statement(){
            GetToken();
            syntax_state = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
        }

        void break_statement(){
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
            }

            syntax_state = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
        }

        void expression_statement(){
            if (curTokenId != TK_SEMICOLON) {
                expression();
            }

            syntax_state = SNTX_LF_HT;
            SkipToken(TK_SEMICOLON);
        }

        void expression(){
            while (true) {
                assignment_expression();
                if (curTokenId != TK_COMMA)
                    break;
                GetToken();
            }
        }


        void assignment_expression(){
            equality_expression();
            if (curTokenId == TK_ASSIGN) {
                GetToken();
                assignment_expression();
            }
        }

        void equality_expression(){
            int t;
            relational_expression();
            while (curTokenId == TK_EQ || curTokenId == TK_NEQ) {
                t = curTokenId;
                GetToken();
                relational_expression();
            }
        }

        void relational_expression(){
            additive_expression();
            while ((curTokenId == TK_LT || curTokenId == TK_LEQ) ||
                   curTokenId == TK_GT || curTokenId == TK_GEQ) {
                GetToken();
                additive_expression();
            }
        }

        void additive_expression(){
            multiplicative_expression();
            while (curTokenId == TK_PLUS || curTokenId == TK_MINUS) {
                GetToken();
                multiplicative_expression();
            }
        }

        void multiplicative_expression(){
            int t;
            unary_expression();
            while (curTokenId == TK_STAR || curTokenId == TK_DIVIDE || curTokenId == TK_MOD) {
                t = curTokenId;
                GetToken();
                unary_expression();
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
                    break;
                case TK_PLUS:
                    GetToken();
                    unary_expression();
                    break;
                case TK_MINUS:
                    GetToken();
                    unary_expression();
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
            GetToken();
            SkipToken(TK_OPENPA);
            type_specifier();
            SkipToken(TK_CLOSEPA);
        }

        void postfix_expression(){
            primary_expression();
            while (true) {
                if (curTokenId == TK_DOT || curTokenId == TK_POINTSTO) {
                    GetToken();
                    curTokenId |= SC_MEMBER;
                    GetToken();
                }
                else if (curTokenId == TK_OPENBR) {
                    GetToken();
                    expression();
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
            int t;
            switch (curTokenId) {
                case TK_CINT:
                case TK_LFloat:
                case TK_CCHAR:
                    GetToken();
                    break;
                case TK_CSTR:
                    GetToken();
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
                    break;
            }
        }

        void argument_expression_list(){
            GetToken();
            if (curTokenId != TK_CLOSEPA) {
                for (;;) {
                    assignment_expression();
                    if (curTokenId == TK_CLOSEPA)
                        break;
                    SkipToken(TK_COMMA);
                }
            }

            SkipToken(TK_CLOSEPA);
            // return value
        }


        void syntax_indent(){
            FuncSyntaxIndent();
        }
    }
}