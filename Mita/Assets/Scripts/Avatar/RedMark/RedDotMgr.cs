using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 红点管理器  用的是 前缀树 来实现的
/// 注：接收红点的地方 只能是 给 叶子节点 赋值 
/// </summary>
public class RedDotMgr : Singleton<RedDotMgr>
{
    /// <summary>
    /// 所有节点集合
    /// </summary>
    private Dictionary<string, TreeNode> m_AllNodes = new Dictionary<string, TreeNode>();

    /// <summary>
    /// 脏节点集合
    /// 脏节点为值发生改变或子节点值发生改变的节点，需要轮询更新
    /// </summary>
    private HashSet<TreeNode> m_DirtyNodes = new HashSet<TreeNode>();

    /// <summary>
    /// 临时脏节点集合
    /// </summary>
    private List<TreeNode> m_TempDirtyNodes = new List<TreeNode>();

    /// <summary>
    /// 节点数量改变回调
    /// </summary>
    public Action NodeNumChangeCallback;

    /// <summary>
    /// 节点值改变回调
    /// </summary>
    public Action<TreeNode, int> NodeValueChangeCallback;

    /// <summary>
    /// 路径分隔字符
    /// </summary>
    public char SplitChar
    {
        get;
        private set;
    }

    /// <summary>
    /// 缓存的StringBuild
    /// </summary>
    public StringBuilder CachedSb
    {
        get;
        private set;
    }

    /// <summary>
    /// 红点树根节点
    /// </summary>
    public TreeNode Root
    {
        get;
        private set;
    }

    public override void Init()
    {
        SplitChar = '.';
        m_AllNodes = new Dictionary<string, TreeNode>();
        Root = new TreeNode("Root");
        m_DirtyNodes = new HashSet<TreeNode>();
        m_TempDirtyNodes = new List<TreeNode>();
        CachedSb = new StringBuilder();
    }

    /// <summary>
    /// 添加节点值监听
    /// </summary>
    public TreeNode AddListener(string path, Action<int> callback)
    {
        if (callback == null)
            return null;
        //获取目标节点，如果没有获取到会自动创建到这个节点并返回
        TreeNode node = GetTreeNode(path);
        node?.AddListener(callback);

        return node;
    }

    /// <summary>
    /// 移除节点值监听
    /// </summary>
    public void RemoveListener(string path, Action<int> callback)
    {
        if (callback == null)
            return;

        TreeNode node = GetTreeNode(path);
        node?.RemoveListener(callback);
    }

    /// <summary>
    /// 移除所有节点值监听
    /// </summary>
    public void RemoveAllListener(string path)
    {
        TreeNode node = GetTreeNode(path);
        node?.RemoveAllListener();
    }

    /// <summary>
    /// 改变节点值
    /// </summary>
    private void ChangeValue(string path, int newValue)
    {
        TreeNode node = GetTreeNode(path);
        node.ChangeValue(newValue, InternalChangeValue);
    }

    /// <summary>
    /// 获取节点值
    /// </summary>
    public int GetValue(string path)
    {
        TreeNode node = GetTreeNode(path);
        if (node == null)
            return 0;

        return node.Value;
    }

    /// <summary>
    /// ChangeValueCount
    /// </summary>
    public void ChangeNodeValueCount(string path, int count = 1)
    {
        ChangeValue(path, GetValue(path) + count);
    }

    /// <summary>
    /// 获取节点
    /// 如果根据path不能获取到节点，此方法会根据path一路创建节点直到创建出key值为path的叶子节点
    /// </summary>
    public TreeNode GetTreeNode(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new Exception("路径不合法，不能为空");
        }
        //尝试获取目标节点，如果获取到就直接返回，没有获取到会执行接下来的代码块创建这个节点及其缺失的父节点
        if (m_AllNodes.TryGetValue(path, out TreeNode node))
            return node;

        TreeNode cur = Root;//当前节点为根节点
        int length = path.Length;//路径长度

        int startIndex = 0;

        for (int i = 0; i < length; i++)//遍历路径
        {
            //判断当前字符是否为分隔符，这里每次遇到分隔符都停下来将cur赋值为子结点，没有子结点创建子结点
            if (path[i] == SplitChar)
            {
                if (i == length - 1)
                    throw new Exception($"路径不合法，不能以路径分隔符结尾：{path}");

                int endIndex = i - 1;//更新endIndex
                if (endIndex < startIndex)
                    throw new Exception($"路径不合法，不能存在连续的路径分隔符或以路径分隔符开头：{path}");
                //获取子结点，没有获取到会自动创建子结点
                TreeNode child = cur.GetOrAddChild(new RangeString(path, startIndex, endIndex), NodeNumChangeCallback);
                //更新startIndex
                startIndex = i + 1;
                cur = child;//当前节点为子结点，继续查找子结点
            }
        }

        //最后一个节点 直接用length - 1作为endIndex
        TreeNode target = cur.GetOrAddChild(new RangeString(path, startIndex, length - 1), NodeNumChangeCallback);//创建目标节点，目标节点是叶子节点

        //添加新创建的节点
        m_AllNodes.Add(path, target);

        return target;
    }

    /// <summary>
    /// 移除节点
    /// </summary>
    public bool RemoveTreeNode(string path)
    {
        if (!m_AllNodes.ContainsKey(path))
            return false;

        TreeNode node = GetTreeNode(path);
        m_AllNodes.Remove(path);
        return node.Parent.RemoveChild(new RangeString(node.Name, 0, node.Name.Length - 1), (node) =>
        {
            //子节点被删除 需要进行一次父节点刷新
            MarkDirtyNode(node);
            //当前节点进行脏标记
            NodeNumChangeCallback?.Invoke();
        });
    }

    /// <summary>
    /// 移除所有节点
    /// </summary>
    public void RemoveAllTreeNode()
    {
        Root.RemoveAllChild((node) =>
        {
            MarkDirtyNode(node);
            NodeNumChangeCallback?.Invoke();
        });
        m_AllNodes.Clear();
    }

    /// <summary>
    /// 管理器轮询
    /// 定时处理脏节点（节点内容或其子节点内容有更改的节点）
    /// 由于脏节点的处理需要一定时间，为了安全性考虑，引入了脏节点缓存，先将要处理的脏节点移动到缓存中，再统一处理缓存中的脏节点
    /// </summary>
    public void Update()
    {
        if (m_DirtyNodes.Count == 0)
            return;

        m_TempDirtyNodes.Clear();//清除临时脏节点集合
        foreach (TreeNode node in m_DirtyNodes)
            m_TempDirtyNodes.Add(node);//将所有脏节点转存到临时脏节点集合中

        m_DirtyNodes.Clear();//清除脏节点集合

        //处理所有脏节点
        for (int i = 0; i < m_TempDirtyNodes.Count; i++)
            m_TempDirtyNodes[i].ChangeValue(InternalChangeValue);
    }

    /// <summary>
    /// 标记脏节点
    /// </summary>
    public void MarkDirtyNode(TreeNode node)
    {
        //根结点不能被标记为脏节点
        if (node == null || node.Name == Root.Name)
            return;

        m_DirtyNodes.Add(node);
    }

    private void InternalChangeValue(TreeNode node, int newValue, TreeNode parent)
    {
        NodeValueChangeCallback?.Invoke(node, newValue);
        //标记父节点为脏节点
        //父节点被标记后，接下来ReddotManager中就会自动更新父节点的值，然后继续标记父节点
        MarkDirtyNode(parent);
    }

    public override void OnRelease()
    {
        if (m_AllNodes != null)
            m_AllNodes.Clear();

        if (m_DirtyNodes != null)
            m_DirtyNodes.Clear();

        if (m_TempDirtyNodes != null)
            m_TempDirtyNodes.Clear();

        if (NodeNumChangeCallback != null)
            NodeNumChangeCallback = null;

        if (NodeValueChangeCallback != null)
            NodeValueChangeCallback = null;
    }
}