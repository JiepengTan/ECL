public enum ETipsType {
    IllegalEscapeCharacter,
    MissChar,
    ErrorChar,
    ErrorToken,
    DontSurpportDeclareFunc,
    ExpectToken,
    UnknowTypeIdentifier,
    SizeOfError,
    TypeAlreadyExist,
    VarNotDeclare,
    MemberNotExist,
    ParamsNumNotMatch,
    ExpectTypeIdentifier,
    ExpectFunctionDefine,
    ExpectStruct,
    ExpectConstInt,
    ExpectIdentifier,
    ExpectStructVar,
    ExpectIdentifierOrConst,
    DuplicateDefine,
}


public class TipsInfoMap {
    public static System.Collections.Generic.Dictionary<ETipsType, string> FormatInfo = new System.Collections.Generic.Dictionary<ETipsType, string>() {
        {ETipsType.IllegalEscapeCharacter, "Illegal escape character: \'\\{0}\'"},
        {ETipsType.MissChar, "miss character {0}"},
        {ETipsType.ErrorChar, "error character {0}"},
        {ETipsType.DontSurpportDeclareFunc, "不支持函数声明"},
        {ETipsType.ExpectToken, "expect token {0}"},
        {ETipsType.UnknowTypeIdentifier, "无效类型标识符 {0}"},
        {ETipsType.ErrorToken, "can not use {0}"},
        {ETipsType.SizeOfError, "sizeof计算类型尺寸失败 {0}"},
        {ETipsType.TypeAlreadyExist, "type already exist id:{0} name:{1}"},
        {ETipsType.VarNotDeclare, "未声明 {0}"},
        {ETipsType.MemberNotExist, "没有此成员变量 {0}"},
        {ETipsType.ParamsNumNotMatch,"实参个数不匹配函数形参个数"},
        {ETipsType.ExpectTypeIdentifier,"类型区分符"},
        {ETipsType.ExpectFunctionDefine,"函数定义"},
        {ETipsType.ExpectStruct,"结构体名"},
        {ETipsType.ExpectConstInt,"整数常量"},
        {ETipsType.ExpectIdentifier,"标识符"},
        {ETipsType.ExpectStructVar,"结构体变量"},
        {ETipsType.ExpectIdentifierOrConst,"标识符或常量"},
        {ETipsType.DuplicateDefine,"重复定义 {0}"},
        
    };
}