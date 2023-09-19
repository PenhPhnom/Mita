using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RecycleView))]
public class RecycleViewEditor : Editor
{
    RecycleView m_Rv;

    public override void OnInspectorGUI()
    {
        m_Rv = (RecycleView)target;

        m_Rv.Dir = (EDirection)EditorGUILayout.EnumPopup("Direction", m_Rv.Dir);
        m_Rv.Lines = EditorGUILayout.IntSlider("Row Or Column", m_Rv.Lines, 1, 10);
        m_Rv.SquareSpacing = EditorGUILayout.FloatField("Square Spacing", m_Rv.SquareSpacing);
        m_Rv.Spacing = EditorGUILayout.Vector2Field("Spacing", m_Rv.Spacing);
        m_Rv.PaddingLeft = EditorGUILayout.FloatField("Padding Left", m_Rv.PaddingLeft) ;
        m_Rv.PaddingTop = EditorGUILayout.FloatField("Padding Top", m_Rv.PaddingTop);
        if (m_Rv.Dir == EDirection.HORIZONTAL)
        {
            m_Rv.m_alignmentH = (EalignmentHorizontal)EditorGUILayout.EnumPopup("Alignment", m_Rv.m_alignmentH);
        }
        else
        {
            m_Rv.m_alignmentV = (EalignmentVertical)EditorGUILayout.EnumPopup("Alignment", m_Rv.m_alignmentV);
        }
        m_Rv.Cell =
            (GameObject)EditorGUILayout.ObjectField("Cell", m_Rv.Cell, typeof(GameObject), true);
    }
}
