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
public sealed partial class Fariable :  Bright.Config.BeanBase 
{
    public Fariable(ByteBuf _buf) 
    {
        if(_buf.ReadBool()){ Xq = _buf.ReadInt(); } else { Xq = null; }
        if(_buf.ReadBool()){ Xw = _buf.ReadFloat(); } else { Xw = null; }
        if(_buf.ReadBool()){ Xe = _buf.ReadDouble(); } else { Xe = null; }
        if(_buf.ReadBool()){ Xs = _buf.ReadString(); } else { Xs = null; }
        if(_buf.ReadBool()){ Xd = _buf.ReadUnityVector2(); } else { Xd = null; }
        PostInit();
    }

    public static Fariable DeserializeFariable(ByteBuf _buf)
    {
        return new Config.Fariable(_buf);
    }

    public int? Xq { get; private set; }
    public float? Xw { get; private set; }
    public double? Xe { get; private set; }
    public string Xs { get; private set; }
    public UnityEngine.Vector2? Xd { get; private set; }

    public const int __ID__ = 1000217016;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Xq:" + Xq + ","
        + "Xw:" + Xw + ","
        + "Xe:" + Xe + ","
        + "Xs:" + Xs + ","
        + "Xd:" + Xd + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}