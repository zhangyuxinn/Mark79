
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;


namespace cfg.stats
{
public partial class TbGuardStats
{
    private readonly System.Collections.Generic.Dictionary<int, stats.Stats> _dataMap;
    private readonly System.Collections.Generic.List<stats.Stats> _dataList;
    
    public TbGuardStats(JSONNode _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, stats.Stats>();
        _dataList = new System.Collections.Generic.List<stats.Stats>();
        
        foreach(JSONNode _ele in _buf.Children)
        {
            stats.Stats _v;
            { if(!_ele.IsObject) { throw new SerializationException(); }  _v = stats.Stats.DeserializeStats(_ele);  }
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
    }

    public System.Collections.Generic.Dictionary<int, stats.Stats> DataMap => _dataMap;
    public System.Collections.Generic.List<stats.Stats> DataList => _dataList;

    public stats.Stats GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public stats.Stats Get(int key) => _dataMap[key];
    public stats.Stats this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}

