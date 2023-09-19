using System;
using System.Collections.Generic;


/// <summary>
/// 树节点
/// </summary>
public class TreeNode
{
    // 子节点
    private Dictionary<RangeString, TreeNode> m_Children = new Dictionary<RangeString, TreeNode>();

    // 节点值改变回调
    private Action<int> m_ChangeCallback;

    // 完整路径
    private string m_FullPath;

    // 节点名
    public string Name
    {
        get;
        private set;
    }

    /// <summary>
    /// 完整路径
    /// </summary>
    public string FullPath
    {
        get
        {
            if (string.IsNullOrEmpty(m_FullPath))
            {
                m_FullPath = Parent == null || Parent == RedDotMgr.Instance.Root ? Name : Parent.FullPath + RedDotMgr.Instance.SplitChar + Name;
            }

            return m_FullPath;
        }
    }

    /// <summary>
    /// 节点值
    /// </summary>
    public int Value
    {
        get;
        private set;
    }

    /// <summary>
    /// 父节点
    /// </summary>
    public TreeNode Parent
    {
        get;
        private set;
    }

    /// <summary>
    /// 子节点
    /// </summary>
    public Dictionary<RangeString, TreeNode>.ValueCollection Children
    {
        get
        {
            return m_Children?.Values;
        }
    }

    /// <summary>
    /// 子节点数量
    /// </summary>
    public int ChildrenCount
    {
        get
        {
            if (m_Children == null)
                return 0;

            int sum = m_Children.Count;
            foreach (TreeNode node in m_Children.Values)
                sum += node.ChildrenCount;

            return sum;
        }
    }

    public TreeNode(string name)
    {
        Name = name;
        Value = 0;
        m_ChangeCallback = null;
    }

    public TreeNode(string name, TreeNode parent) : this(name)
    {
        Parent = parent;
    }

    /// <summary>
    /// 添加节点值监听
    /// </summary>
    public void AddListener(Action<int> callback)
    {
        m_ChangeCallback += callback;
    }

    /// <summary>
    /// 移除节点值监听
    /// </summary>
    public void RemoveListener(Action<int> callback)
    {
        m_ChangeCallback -= callback;
    }

    /// <summary>
    /// 移除所有节点值监听
    /// </summary>
    public void RemoveAllListener()
    {
        m_ChangeCallback = null;
    }

    /// <summary>
    /// 改变节点值（使用传入的新值，只能在叶子节点上调用）
    /// 节点值得改变只能从叶子节点开始层层向上传递
    /// </summary>
    public void ChangeValue(int newValue, Action<TreeNode, int, TreeNode> changeValueCallBack)
    {
        ////校验是否叶子节点
        ////这个地方可以自行修改，在实际游戏中直接return最好，不要抛错
        if (m_Children != null && m_Children.Count != 0)
        {

            ClientLog.Instance.LogError($"不允许直接改变非叶子节点的值：{FullPath}");
            return;
        }
        //调用真正改变值的方法
        InternalChangeValue(newValue, changeValueCallBack);
    }

    /// <summary>
    /// 改变节点值（根据子节点值计算新值，只对非叶子节点有效）
    /// </summary>
    public void ChangeValue(Action<TreeNode, int, TreeNode> changeValueCallBack)
    {
        int sum = 0;
        //校验是非叶子节点才进入循环
        if (m_Children != null && m_Children.Count != 0)
        {
            //循环遍历统计子结点中的数据和
            //父节点中的数据始终是所有子结点的数据和
            foreach (KeyValuePair<RangeString, TreeNode> child in m_Children)
                sum += child.Value.Value;

            //sum = (int)MathF.Max(sum, Value);

            InternalChangeValue(sum, changeValueCallBack);
        }
    }

    /// <summary>
    /// 获取子节点，如果不存在则添加
    /// </summary>
    public TreeNode GetOrAddChild(RangeString key, Action nodeNumChangeCallback)
    {
        TreeNode child = GetChild(key);
        if (child == null)
            child = AddChild(key, nodeNumChangeCallback);
        return child;
    }

    /// <summary>
    /// 获取子节点
    /// </summary>
    public TreeNode GetChild(RangeString key)
    {
        if (m_Children == null)
            return null;

        m_Children.TryGetValue(key, out TreeNode child);
        return child;
    }

    /// <summary>
    /// 添加子节点
    /// </summary>
    public TreeNode AddChild(RangeString key, Action nodeNumChangeCallback)
    {
        if (m_Children == null)
            m_Children = new Dictionary<RangeString, TreeNode>();
        else if (m_Children.ContainsKey(key))
            throw new Exception($"子节点添加失败，不允许重复添加：{FullPath}");

        TreeNode child = new TreeNode(key.ToString(), this);
        m_Children.Add(key, child);
        nodeNumChangeCallback?.Invoke();
        return child;
    }

    /// <summary>
    /// 移除子节点
    /// </summary>
    public bool RemoveChild(RangeString key, Action<TreeNode> removeCallBack)
    {
        if (m_Children == null || m_Children.Count == 0)
            return false;

        TreeNode child = GetChild(key);

        if (child != null)
        {
            m_Children.Remove(key);//移除子节点
            removeCallBack?.Invoke(this);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 移除所有子节点
    /// </summary>
    public void RemoveAllChild(Action<TreeNode> removeCallBack)
    {
        if (m_Children == null || m_Children.Count == 0)
            return;

        m_Children.Clear();
        removeCallBack?.Invoke(this);
    }

    public override string ToString()
    {
        return FullPath;
    }

    /// <summary>
    /// 改变节点值
    /// </summary>
    private void InternalChangeValue(int newValue, Action<TreeNode, int, TreeNode> changeValueCallBack)
    {
        if (Value == newValue)
            return;

        //Value = newValue;
        Value = UnityEngine.Mathf.Clamp(newValue, 0, newValue);
        m_ChangeCallback?.Invoke(newValue);
        changeValueCallBack?.Invoke(this, Value, Parent);
    }
}