using System;
using UnityEngine;
using UnityEngine.UI;

public class RandNumText : Text
{
    public bool m_EnableAnim = true;

    private string m_TargetTxt;
    private int m_TargetNum;
    private int m_OriginNum;
    private bool m_PlayAnim = false;
    private float m_LastTime;

    public float m_TotalTime = 1.0f;

    public override string text
    {
        get
        {
            return base.text;
        }
        set
        {
            m_PlayAnim = false;
            base.text = value;
        }
    }

    protected override void OnDisable()
    {
        StopAnim();
        base.OnDisable();
    }

    public void SetText(string value)
    {
        m_TargetTxt = value;
        if (!int.TryParse(value, out m_TargetNum)
          || !int.TryParse(text, out m_OriginNum))
        {
            text = value;
            return;
        }
        PlayAnim();
    }

    private void PlayAnim()
    {
        if (!m_EnableAnim)
        {
            return;
        }
        m_PlayAnim = true;
        m_LastTime = Time.time;
    }

    private void StopAnim()
    {
        if (m_PlayAnim)
        {
            SetValue(m_TargetTxt);
            m_PlayAnim = false;
        }
    }

    private void Update()
    {
        if (!m_PlayAnim && !m_EnableAnim)
        {
            return;
        }

        if (Time.time - m_LastTime >= m_TotalTime)
        {
            StopAnim();
            return;
        }
        SetValue(UnityEngine.Random.Range(m_OriginNum, m_TargetNum).ToString());
    }

    private void SetValue(string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            if (String.IsNullOrEmpty(m_Text))
                return;
            m_Text = "";
            SetVerticesDirty();
        }
        else if (m_Text != value)
        {
            m_Text = value;
            SetVerticesDirty();
            SetLayoutDirty();
        }
    }
}

