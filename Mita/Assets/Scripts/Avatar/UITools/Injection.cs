/**
===========================================================
  - FileName: Injection.cs
  - CreatorName: 王唐新
  - CreateTime: 2023 三月 28 星期二
  - 功能描述: 存储当前序列化后的集合
  - 请不要更改这个脚本的任何内容 如有更改需求先联系作者说明情况！！！
===========================================================
*/

using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[DisallowMultipleComponent]
public class Injection : MonoBehaviour
{
    public List<UnityObject> UIObjects = new List<UnityObject>();
}

[System.Serializable]
public class UnityObject
{
    [SerializeField] private string m_name;
    public string Name => m_name;

    [SerializeField] private Object m_target;
    public Object Target => m_target;

    [SerializeField] private Component m_component;
    public Component Component => m_component;
}