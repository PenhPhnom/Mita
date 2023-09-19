using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// GM ������
/// </summary>
public partial class SROptions
{
    /// Category����������
    /// DisplayName��������ʾ����
    /// Sort����������
    /// 
    [Category("TestNameTitle"), DisplayName("����һ�����ԵĽű�")]
    public void TestName()
    {
        ClientLog.Instance.Log($"���� ���ԵĽű������ {m_NameStrTest}");
    }

    private string m_NameStrTest = "";
    [Category("TestNameStr"), DisplayName("��������")]
    public string TestNameStr
    {
        get { return m_NameStrTest; }
        set { m_NameStrTest = value; }
    }
}
