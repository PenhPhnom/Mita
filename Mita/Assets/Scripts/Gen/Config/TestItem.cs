//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;


namespace cfg.Config
{
public sealed partial class TestItem :  Bright.Config.BeanBase 
{
    public TestItem(ByteBuf _buf) 
    {
        PramIntKey = _buf.ReadInt();
        if(_buf.ReadBool()){ PramInt = _buf.ReadInt(); } else { PramInt = null; }
        PramStr = _buf.ReadString();
        if(_buf.ReadBool()){ PramFloat = _buf.ReadFloat(); } else { PramFloat = null; }
        if(_buf.ReadBool()){ PramLong = _buf.ReadLong(); } else { PramLong = null; }
        if(_buf.ReadBool()){ PramShort = _buf.ReadShort(); } else { PramShort = null; }
        if(_buf.ReadBool()){ PramDouble = _buf.ReadDouble(); } else { PramDouble = null; }
        if(_buf.ReadBool()){ PramVector2 = _buf.ReadUnityVector2(); } else { PramVector2 = null; }
        if(_buf.ReadBool()){ PramVector3 = _buf.ReadUnityVector3(); } else { PramVector3 = null; }
        if(_buf.ReadBool()){ PramVector4 = _buf.ReadUnityVector4(); } else { PramVector4 = null; }
        {int __n0 = System.Math.Min(_buf.ReadSize(), _buf.Size);PramArrStr = new string[__n0];for(var __index0 = 0 ; __index0 < __n0 ; __index0++) { string __e0;__e0 = _buf.ReadString(); PramArrStr[__index0] = __e0;}}
        {int n0 = System.Math.Min(_buf.ReadSize(), _buf.Size);PramListStr = new System.Collections.Generic.List<string>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { string _e0;  _e0 = _buf.ReadString(); PramListStr.Add(_e0);}}
        {int n0 = System.Math.Min(_buf.ReadSize(), _buf.Size);PramListInt = new System.Collections.Generic.List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = _buf.ReadInt(); PramListInt.Add(_e0);}}
        PramFariable = Config.Fariable.DeserializeFariable(_buf);
        {int n0 = System.Math.Min(_buf.ReadSize(), _buf.Size);PramListInt2 = new System.Collections.Generic.List<int>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { int _e0;  _e0 = _buf.ReadInt(); PramListInt2.Add(_e0);}}
        {int n0 = System.Math.Min(_buf.ReadSize(), _buf.Size);PramListString2 = new System.Collections.Generic.List<string>(n0);for(var i0 = 0 ; i0 < n0 ; i0++) { string _e0;  _e0 = _buf.ReadString(); PramListString2.Add(_e0);}}
        PramDatetime = _buf.ReadLong();
        PostInit();
    }

    public static TestItem DeserializeTestItem(ByteBuf _buf)
    {
        return new Config.TestItem(_buf);
    }

    /// <summary>
    /// 整型(主键不可以为null，所以不能有问号)
    /// </summary>
    public int PramIntKey { get; private set; }
    /// <summary>
    /// 整型
    /// </summary>
    public int? PramInt { get; private set; }
    /// <summary>
    /// 字符串
    /// </summary>
    public string PramStr { get; private set; }
    /// <summary>
    /// 浮点型
    /// </summary>
    public float? PramFloat { get; private set; }
    /// <summary>
    /// 长整型
    /// </summary>
    public long? PramLong { get; private set; }
    /// <summary>
    /// 短整型
    /// </summary>
    public short? PramShort { get; private set; }
    /// <summary>
    /// 双精度浮点
    /// </summary>
    public double? PramDouble { get; private set; }
    /// <summary>
    /// 二维
    /// </summary>
    public UnityEngine.Vector2? PramVector2 { get; private set; }
    /// <summary>
    /// 三维
    /// </summary>
    public UnityEngine.Vector3? PramVector3 { get; private set; }
    /// <summary>
    /// 四维
    /// </summary>
    public UnityEngine.Vector4? PramVector4 { get; private set; }
    /// <summary>
    /// 字符数组
    /// </summary>
    public string[] PramArrStr { get; private set; }
    /// <summary>
    /// 字符列表
    /// </summary>
    public System.Collections.Generic.List<string> PramListStr { get; private set; }
    /// <summary>
    /// 整型列表
    /// </summary>
    public System.Collections.Generic.List<int> PramListInt { get; private set; }
    /// <summary>
    /// 自定义类型
    /// </summary>
    public Config.Fariable PramFariable { get; private set; }
    /// <summary>
    /// 整型列表
    /// </summary>
    public System.Collections.Generic.List<int> PramListInt2 { get; private set; }
    /// <summary>
    /// 字符串列表
    /// </summary>
    public System.Collections.Generic.List<string> PramListString2 { get; private set; }
    /// <summary>
    /// 时间
    /// </summary>
    public long PramDatetime { get; private set; }
    public long PramDatetime_Millis => PramDatetime * 1000L;

    public const int __ID__ = -1077104655;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PramFariable?.Resolve(_tables);
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
        PramFariable?.TranslateText(translator);
    }

    public override string ToString()
    {
        return "{ "
        + "PramIntKey:" + PramIntKey + ","
        + "PramInt:" + PramInt + ","
        + "PramStr:" + PramStr + ","
        + "PramFloat:" + PramFloat + ","
        + "PramLong:" + PramLong + ","
        + "PramShort:" + PramShort + ","
        + "PramDouble:" + PramDouble + ","
        + "PramVector2:" + PramVector2 + ","
        + "PramVector3:" + PramVector3 + ","
        + "PramVector4:" + PramVector4 + ","
        + "PramArrStr:" + Bright.Common.StringUtil.CollectionToString(PramArrStr) + ","
        + "PramListStr:" + Bright.Common.StringUtil.CollectionToString(PramListStr) + ","
        + "PramListInt:" + Bright.Common.StringUtil.CollectionToString(PramListInt) + ","
        + "PramFariable:" + PramFariable + ","
        + "PramListInt2:" + Bright.Common.StringUtil.CollectionToString(PramListInt2) + ","
        + "PramListString2:" + Bright.Common.StringUtil.CollectionToString(PramListString2) + ","
        + "PramDatetime:" + PramDatetime + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}