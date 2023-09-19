using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollRectParam<T> : IEnhancedScrollerDelegate
{
    private int m_CellCount;
    private float m_CellViewSize;
    private Action<EnhancedScrollerCellView, int, int> m_CellItemAction;
    private Func<int, int> m_CellPrefabItemAction;
    private Func<int, float> m_CellPrefabItemSizeAction;
    private EnhancedScroller m_EnhancedScroller;
    private T m_Datas;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="datas">数据</param>
    /// <param name="scroller">EnhancedScroller</param>
    /// <param name="numberOfCells">个数</param>
    /// <param name="cellPrefabItemAction">使用哪个Prefab</param>
    /// <param name="cellPrefabItemSizeAction">item的Size</param>
    /// <param name="cellViewAction"></param>
    public ScrollRectParam(T datas, EnhancedScroller scroller, int cellCount, float cellViewSize, Func<int, int> cellPrefabItemAction = null
        , Func<int, float> cellPrefabItemSizeAction = null, Action<EnhancedScrollerCellView, int, int> cellViewAction = null)
    {
        m_Datas = datas;
        scroller.Delegate = this;
        m_EnhancedScroller = scroller;
        m_CellCount = cellCount;
        m_CellViewSize = cellViewSize;
        m_CellPrefabItemAction = cellPrefabItemAction;
        m_CellPrefabItemSizeAction = cellPrefabItemSizeAction;
        m_CellItemAction = cellViewAction;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        int index = 0;
        if (m_CellPrefabItemAction != null)
            index = m_CellPrefabItemAction(dataIndex);
        var cellView = scroller.GetCellView(index);
        cellView.OnSetData(m_Datas, dataIndex, cellIndex);
        m_CellItemAction?.Invoke(cellView, dataIndex, cellIndex);
        return cellView;
    }


    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        if (m_CellPrefabItemSizeAction != null)
            m_CellViewSize = m_CellPrefabItemSizeAction(dataIndex);

        return m_CellViewSize;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return m_CellCount;
    }

    public void OnRelease()
    {
        m_CellCount = 0;
        m_CellViewSize = 0;
        if (m_CellItemAction != null) m_CellItemAction = null;
        if (m_EnhancedScroller != null) m_EnhancedScroller = null;
        if (m_Datas != null) m_Datas = default;
    }
}
