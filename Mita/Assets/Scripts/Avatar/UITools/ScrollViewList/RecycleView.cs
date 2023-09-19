using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum EDirection
{
    HORIZONTAL,
    VERTICAL
}
public enum EalignmentHorizontal
{
    UPPER,
    MIDDLE,
    LOWER,
}
public enum EalignmentVertical
{
    LEFT,
    CENTER,
    RIGHT,
}

public class RecycleView : MonoBehaviour
{
    public EDirection Dir = EDirection.VERTICAL;

    private CellInfo[] m_CellInfos;
    private bool m_IsClearList = false;
    //指定的cell
    public GameObject Cell;

    private GameObject m_Content;
    private ScrollRect m_ScrollRect;
    private RectTransform m_RectTrans;
    private RectTransform m_ContentRectTrans;
    private bool m_IsInit = false;

    //对象池
    private Stack<GameObject> m_Pool = new Stack<GameObject>();
    private bool m_IsInited = false;

    private Action<GameObject, int> m_FuncCallBack;

    public int Lines = 1;//默认显示1行
    public float SquareSpacing = 1f;//方针间距
    public Vector2 Spacing = Vector2.zero;
    public float Row = 0f;//行间距
    public float Col = 0f;//列间距
    public float PaddingTop = 0f;//顶部空隙
    public float PaddingLeft = 0f;//左侧空隙
    public EalignmentHorizontal m_alignmentH = EalignmentHorizontal.UPPER;
    public EalignmentVertical m_alignmentV = EalignmentVertical.LEFT;

    private float m_PlaneW;
    private float m_PlaneH;
    private float m_ContentW;
    private float m_ContentH;
    private float m_CellW;
    private float m_CellH;


    private int m_MaxCount = -1;
    private int m_MinIndex = -1;
    private int m_MaxIndex = -1;

    public void Init()
    {
        if (m_IsInit) return;

        m_Content = GetComponent<ScrollRect>().content.gameObject;
        if (Cell == null)
        {
            Cell = m_Content.transform.GetChild(0).gameObject;
        }

        /////////////////////Cell 处理//////////////////////
        SetPoolObj(Cell);

        RectTransform cellRectTrans = Cell.GetComponent<RectTransform>();
        cellRectTrans.pivot = new Vector2(0f, 1f);
        CheckAnchor(cellRectTrans);
        cellRectTrans.anchoredPosition = Vector2.zero;

        //记录 Cell 信息
        m_CellH = cellRectTrans.rect.height;
        m_CellW = cellRectTrans.rect.width;

        //记录 Plane 信息
        m_RectTrans = GetComponent<RectTransform>();
        Rect planeRect = m_RectTrans.rect;
        m_PlaneH = planeRect.height;
        m_PlaneW = planeRect.width;

        //记录 Content 信息
        m_ContentRectTrans = m_Content.GetComponent<RectTransform>();
        Rect contentRect = m_ContentRectTrans.rect;
        m_ContentH = contentRect.height;
        m_ContentW = contentRect.width;

        // 记录间距信息  如果存在行列设置就引用  没有就是用方阵间距
        Row = Spacing.x;
        Col = Spacing.y;
        if (Row == 0 && Col == 0) Row = Col = SquareSpacing;
        else SquareSpacing = 0;

        m_ContentRectTrans.pivot = new Vector2(0f, 1f);
        CheckAnchor(m_ContentRectTrans);
        m_ScrollRect = GetComponent<ScrollRect>();
        m_ScrollRect.onValueChanged.RemoveAllListeners();

        //添加滑动事件
        m_ScrollRect.onValueChanged.AddListener(delegate (Vector2 value) { ScrollRectListener(value); });

        m_IsInit = true;
    }

    protected void ScrollRectListener(Vector2 value)
    {
        UpdateCheck();
    }

    private void UpdateCheck()
    {
        if (m_CellInfos == null) return;

        // 检查超出范围
        for (int i = 0, length = m_CellInfos.Length; i < length; i++)
        {
            CellInfo cellInfo = m_CellInfos[i];
            GameObject obj = cellInfo.obj;
            Vector3 pos = cellInfo.pos;

            float rangePos = Dir == EDirection.VERTICAL ? pos.y : pos.x;
            // 判断是否超出显示范围
            if (IsOutRange(rangePos))
            {
                // 把超出范围的cell 扔进 poolsObj里
                if (obj != null)
                {
                    SetPoolObj(obj);
                    m_CellInfos[i].obj = null;
                }
            }
            else
            {
                if (obj == null)
                {
                    // 优先从 poolsObj中 取出 （poolsObj为空则返回 实例化的cell）
                    GameObject cell = GetPoolObj();
                    cell.transform.localPosition = pos;
                    cell.gameObject.name = i.ToString();
                    m_CellInfos[i].obj = cell;

                    m_FuncCallBack?.Invoke(m_CellInfos[i].obj, i);
                }
            }
        }
    }

    public void ShowList(int num, Action<GameObject, int> callback = null)
    {
        m_MinIndex = -1;
        m_MaxIndex = -1;

        //计算content尺寸
        if (Dir == EDirection.VERTICAL)
        {
            m_ScrollRect.horizontal = false;
            m_ScrollRect.vertical = true;
            float contentSize = (Col + m_CellH) * Mathf.CeilToInt((float)num / Lines) + PaddingTop;
            m_ContentH = contentSize;
            m_ContentW = m_ContentRectTrans.sizeDelta.x + PaddingLeft;
            contentSize = contentSize < m_RectTrans.rect.height ? m_RectTrans.rect.height : contentSize;
            m_ContentRectTrans.sizeDelta = new Vector2(m_ContentW, contentSize);
            if (num != m_MaxCount)
            {
                m_ContentRectTrans.anchoredPosition = new Vector2(m_ContentRectTrans.anchoredPosition.x, 0);
            }
        }
        else
        {
            m_ScrollRect.horizontal = true;
            m_ScrollRect.vertical = false;
            float contentSize = (Row + m_CellW) * Mathf.CeilToInt((float)num / Lines) + PaddingTop;
            m_ContentW = contentSize;
            m_ContentH = m_ContentRectTrans.sizeDelta.x + PaddingLeft;
            contentSize = contentSize < m_RectTrans.rect.width ? m_RectTrans.rect.width : contentSize;
            m_ContentRectTrans.sizeDelta = new Vector2(contentSize, m_ContentH);
            if (num != m_MaxCount)
            {
                m_ContentRectTrans.anchoredPosition = new Vector2(0, m_ContentRectTrans.anchoredPosition.y);
            }
        }

        //计算  开始索引
        int lastEndIndex = 0;

        //过多的物体  扔到对象池  （首次调用ShowList函数时 则无效）
        if (m_IsInited)
        {
            lastEndIndex = num - m_MaxCount > 0 ? m_MaxCount : num;
            lastEndIndex = m_IsClearList ? 0 : lastEndIndex;

            int count = m_IsClearList ? m_CellInfos.Length : m_MaxCount;
            for (int i = lastEndIndex; i < count; i++)
            {
                if (m_CellInfos[i].obj != null)
                {
                    SetPoolObj(m_CellInfos[i].obj);
                    m_CellInfos[i].obj = null;
                }
            }
        }

        //以下代码在for循环所用
        CellInfo[] tempCellInfos = m_CellInfos;
        m_CellInfos = new CellInfo[num];

        //1.计算 每个cell坐标并存储  2：显示范围内的cell
        for (int i = 0; i < num; i++)
        {
            //存储  已有的数据（首次调用ShowList函数时 则无效）
            if (m_MaxCount != -1 && i < lastEndIndex)
            {
                CellInfo tempCellInfo = tempCellInfos[i];

                //计算是否超出范围
                float rPos = Dir == EDirection.VERTICAL ? tempCellInfo.pos.y : tempCellInfo.pos.x;
                if (!IsOutRange(rPos))
                {
                    //记录显示范围中的 首位index和末尾index
                    m_MinIndex = m_MinIndex == -1 ? i : m_MinIndex;//首位index
                    m_MaxIndex = i;//末尾index

                    if (tempCellInfo.obj == null)
                    {
                        tempCellInfo.obj = GetPoolObj();
                    }

                    tempCellInfo.obj.transform.GetComponent<RectTransform>().localPosition = tempCellInfo.pos;
                    tempCellInfo.obj.name = i.ToString();
                    tempCellInfo.obj.SetActive(true);

                    m_FuncCallBack?.Invoke(tempCellInfo.obj, i);
                }
                else
                {
                    SetPoolObj(tempCellInfo.obj);
                    tempCellInfo.obj = null;
                }

                m_CellInfos[i] = tempCellInfo;
                continue;
            }

            CellInfo cellInfo = new CellInfo();

            float pos = 0;//坐标（isVertical？记录Y:记录X）
            float rowPos = 0;//计算每排里面的cell坐标

            //计算每个cell坐标
            if (Dir == EDirection.VERTICAL)
            {
                pos = m_CellH * Mathf.FloorToInt(i / Lines) + Col * Mathf.FloorToInt(i / Lines);
                rowPos = m_CellW * (i % Lines) + Row * (i % Lines);
                //为每个cell加入留白边距
                float mid = m_ContentRectTrans.rect.size.x - Lines * m_CellW - (Lines - 1) * SquareSpacing;
                if (m_alignmentV == EalignmentVertical.LEFT)
                {
                    cellInfo.pos = new Vector3(rowPos + PaddingLeft, -pos - PaddingTop, 0);
                }
                else if (m_alignmentV == EalignmentVertical.CENTER)
                {
                    cellInfo.pos = new Vector3(rowPos + PaddingLeft + mid / 2, -pos - PaddingTop, 0);
                }
                else if (m_alignmentV == EalignmentVertical.RIGHT)
                {
                    cellInfo.pos = new Vector3(rowPos + PaddingLeft + mid, -pos - PaddingTop, 0);
                }
            }
            else
            {
                pos = m_CellW * Mathf.FloorToInt(i / Lines) + Row * Mathf.FloorToInt(i / Lines);
                rowPos = m_CellH * (i % Lines) + Col * (i % Lines);
                float mid = m_ContentRectTrans.rect.size.y - Lines * m_CellH - (Lines - 1) * SquareSpacing;
                if (m_alignmentH == EalignmentHorizontal.UPPER)
                {
                    cellInfo.pos = new Vector3(pos + PaddingLeft, -rowPos - PaddingTop, 0);
                }
                else if (m_alignmentH == EalignmentHorizontal.MIDDLE)
                {
                    cellInfo.pos = new Vector3(pos + PaddingLeft, -rowPos - PaddingTop - mid / 2, 0);
                }
                else if (m_alignmentH == EalignmentHorizontal.LOWER)
                {
                    cellInfo.pos = new Vector3(pos + PaddingLeft, -rowPos - PaddingTop - mid, 0);
                }
            }

            //计算是否超出范围
            float cellPos = Dir == EDirection.VERTICAL ? cellInfo.pos.y : cellInfo.pos.x;
            if (IsOutRange(cellPos))
            {
                cellInfo.obj = null;
                m_CellInfos[i] = cellInfo;
                continue;
            }

            //记录显示范围中的  首位index和末尾index
            m_MinIndex = m_MinIndex == -1 ? i : m_MinIndex;
            m_MaxIndex = i;

            //取或创建cell
            GameObject cell = GetPoolObj();
            cell.transform.GetComponent<RectTransform>().localPosition = cellInfo.pos;
            cell.gameObject.name = i.ToString();

            //存数据
            cellInfo.obj = cell;
            m_CellInfos[i] = cellInfo;

            m_FuncCallBack = callback;
            m_FuncCallBack?.Invoke(cellInfo.obj, i);
        }

        m_MaxCount = num;
        m_IsInited = true;
    }

    //判断是否超出显示范围c
    protected bool IsOutRange(float pos)
    {
        Vector3 listP = m_ContentRectTrans.anchoredPosition;
        if (Dir == EDirection.VERTICAL)
        {
            if (pos + listP.y > m_CellH || pos + listP.y < -m_RectTrans.rect.height)
            {
                return true;
            }
        }
        else
        {
            if (pos + listP.x < -m_CellW || pos + listP.x > m_RectTrans.rect.width)
            {
                return true;
            }
        }

        return false;
    }

    //检查 Anchor 是否正确
    private void CheckAnchor(RectTransform rectTrans)
    {
        if (Dir == EDirection.VERTICAL)
        {
            if (!((rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(0, 1)) ||
                  (rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(1, 1))))
            {
                rectTrans.anchorMin = new Vector2(0, 1);
                rectTrans.anchorMax = new Vector2(1, 1);
            }
        }
        else
        {
            if (!((rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(0, 1)) ||
                  (rectTrans.anchorMin == new Vector2(0, 0) && rectTrans.anchorMax == new Vector2(0, 1))))
            {
                rectTrans.anchorMin = new Vector2(0, 0);
                rectTrans.anchorMax = new Vector2(0, 1);
            }
        }
    }

    //取出cell
    protected GameObject GetPoolObj()
    {
        GameObject cell = null;
        if (m_Pool.Count > 0) cell = m_Pool.Pop();
        if (cell == null) cell = Instantiate(this.Cell);

        cell.transform.SetParent(m_Content.transform);
        cell.transform.localScale = Vector3.one;
        SetActive(cell, true);

        return cell;
    }

    //存入cell
    protected void SetPoolObj(GameObject cell)
    {
        if (cell != null)
        {
            m_Pool.Push(cell);
            SetActive(cell, false);
        }
    }

    protected void SetActive(GameObject obj, bool isActive)
    {
        if (obj != null)
        {
            obj.SetActive(isActive);
        }
    }

    //记录 物体的坐标  和  gameobject
    public struct CellInfo
    {
        public Vector3 pos;
        public GameObject obj;
    }

    /// 定位到第一行，也是还原到初始位置
    public void GoToOneLine()
    {
        GoToCellPos(0);
    }

    /// <summary>
    /// 通过index定位到某一单元格的坐标位置
    /// </summary>
    /// <param name="index">索引ID</param>
    public void GoToCellPos(int index)
    {
        if (index > m_CellInfos.Length - 1 || index < 0)  return;
        // 如果cellInfo不存在坐标，说明没有被初始化过，当前没有数据，直接return
        if (m_CellInfos.Length == 0) return;

        // 当前索引所在行的第一个索引
        int theFirstIndex = index - index % Lines;
        // 假设在第一行最大索引
        var tmpIndex = theFirstIndex + m_MaxIndex;

        int theLastIndex = tmpIndex > m_MaxCount - 1 ? m_MaxCount - 1 : tmpIndex;

        // 如果最大索引就是边界的话，边界的
        if (theLastIndex == m_MaxCount - 1)
        {
            // 余数不为0的情况下，第一个索引位置需要考虑最大数到最后显示位置的边距
            var shortOfNum = m_MaxCount % Lines == 0 ? 0 : Lines - m_MaxCount % Lines;
            theFirstIndex = theLastIndex - m_MaxIndex + shortOfNum;
        }

        Vector2 newPos = m_CellInfos[theFirstIndex].pos;
        if (Dir == EDirection.VERTICAL)
        {
            // 纵滑时定位到某一点，需要进行布局上的显示判断
            // 如果index是第0行，即index<=Lines, 回到的位置应该是第一行坐标y+顶部空隙 (x,y+top)
            // index>Lines,显示的index的布局应该 (x,y+Col)
            var posY = index <= Lines ? -newPos.y - PaddingTop : -newPos.y - Col;
            m_ContentRectTrans.anchoredPosition = new Vector2(m_ContentRectTrans.anchoredPosition.x, posY);
        }
        else
        {
            // 横向滑动时
            // 如果index是第0行，即index<=Lines, 回到的位置 (x+left,y)
            // index>Lines,位置应该为 (x+Row,y)
            var posX = index <= Lines ? -newPos.x + PaddingLeft : -newPos.x + Row;
            m_ContentRectTrans.anchoredPosition = new Vector2(posX, m_ContentRectTrans.anchoredPosition.y);
        }
    }

    public void DisposeAll()
    {
        if (m_FuncCallBack != null) m_FuncCallBack = null;
    }

    protected void OnDestroy()
    {
        DisposeAll();
    }
}
