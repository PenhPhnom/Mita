using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// GM 测试类
/// </summary>
public partial class SROptions
{
    /// Category：分类名称
    /// DisplayName：介面显示名称
    /// Sort：功能排序
    /// 
    [Category("TestNameTitle"), DisplayName("这是一个测试的脚本")]
    public void TestName()
    {
        ClientLog.Instance.Log($"我是 测试的脚本的输出 {m_NameStrTest}");
    }

    private string m_NameStrTest = "";
    [Category("TestNameStr"), DisplayName("名字输入")]
    public string TestNameStr
    {
        get { return m_NameStrTest; }
        set { m_NameStrTest = value; }
    }
}
