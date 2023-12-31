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
   
public partial class TbTest
{
    private readonly Dictionary<int, Config.TestItem> _dataMap;
    private readonly List<Config.TestItem> _dataList;
    
    public TbTest(ByteBuf _buf)
    {
        _dataMap = new Dictionary<int, Config.TestItem>();
        _dataList = new List<Config.TestItem>();
        
        for(int n = _buf.ReadSize() ; n > 0 ; --n)
        {
            Config.TestItem _v;
            _v = Config.TestItem.DeserializeTestItem(_buf);
            _dataList.Add(_v);
            _dataMap.Add(_v.PramIntKey, _v);
        }
        PostInit();
    }

    public Dictionary<int, Config.TestItem> DataMap => _dataMap;
    public List<Config.TestItem> DataList => _dataList;

    public Config.TestItem GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public Config.TestItem Get(int key) => _dataMap[key];
    public Config.TestItem this[int key] => _dataMap[key];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}