namespace LockstepECL {
    public enum EKeyWord{
        //base type 
        e_Boolean,
        e_Int8,
        e_Int16,
        e_Int32,
        e_Int64,
        e_UInt8,
        e_UInt16,
        e_UInt32,
        e_UInt64,
        e_LFloat,
        e_LVector2,
        e_LVector3,
        e_LMatrix33,
        e_LQuaternion,
        e_String,
        //for ecs
        e_entity,
        e_fields,
        e_use,
        e_synced,
        e_signal,
        e_asset,
        e_entity_ref,
        e_asset_ref,
        e_array,
        e_collision,
        e_global,
        //remain for program language
        e_const,
        e_struct,
        e_enum,
        
        e_if,
        e_else,
        e_return,
        e_break,
        e_continue,
        e_for,
    }
}