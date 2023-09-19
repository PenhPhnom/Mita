/**
===================================
  - FileName: InjectionEditor.cs
  - CreatorName: 王唐新
  - CreateTime: 2023 三月 28 星期二
  - 功能描述: 一键注入UI
===================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Injection))]
public class InjectionEditor : Editor
{
    private SerializedProperty m_SerializedProperty;

    private ReorderableList m_ReorderableList;

    #region 用于创建对应脚本

    //view层代码路径
    const string ViewDir = "Assets/Scripts/Avatar/UI/GUIView";

    //view层模版文件路径
    const string ViewTemplatePath = "Assets/Scripts/Avatar/UI/Templeta/ViewTemplate.txt";

    //view层Item模版文件路径
    const string ItemTemplatePath = "Assets/Scripts/Avatar/UI/Templeta/ItemTemplate.txt";

    // //命名空间模板
    // const string NameSpaceTemplate = "using {0};";

    //字段模板
    const string FieldTemplate = "public {0} {1};\t";

    //方法模板
    const string MethodTemplate = "{0} = m_Injection.UIObjects[{1}].Component.GetComponent<{2}>();\t\t";

    //方法模板 Go
    const string MethodTemplateGo = "{0} = m_Injection.UIObjects[{1}].Target as GameObject;\t\t";

    //Go标识符
    private const string isGo = "GameObject";

    #endregion

    public void OnEnable()
    {
        m_SerializedProperty = serializedObject.FindProperty("UIObjects");
        //需要序列化的列表 序列化列表的指针
        m_ReorderableList = new ReorderableList(serializedObject, m_SerializedProperty, false, true, false, false);

        m_ReorderableList.drawHeaderCallback = OnDrawHeader;
        m_ReorderableList.drawElementCallback = OnDrawElement;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        m_ReorderableList.DoLayoutList();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("生成", GUILayout.Width(100), GUILayout.Height(30)))
            AutoInject();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = m_ReorderableList.serializedProperty.GetArrayElementAtIndex(index);
        SerializedProperty target = element.FindPropertyRelative("m_target");
        SerializedProperty component = element.FindPropertyRelative("m_component");
        SerializedProperty name = element.FindPropertyRelative("m_name");

        OnDrawTarget(target, rect);

        OnDrawPopup(target, component, rect);

        OnDrawText(name, rect);
    }

    private void OnDrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, $"组件预览   --组件与脚本都是自动生成的 请勿拖拽或创建");
    }

    private void OnAddElement(ReorderableList list)
    {
        list.serializedProperty.arraySize++;
        list.index = list.serializedProperty.arraySize - 1;
        SerializedProperty item = list.serializedProperty.GetArrayElementAtIndex(list.index);
        Debug.Log("item : " + item);

        item.FindPropertyRelative("m_target").objectReferenceValue = null;
        item.FindPropertyRelative("m_name").stringValue = string.Empty;
        item.FindPropertyRelative("m_component").objectReferenceValue = null;
    }

    #region 绘制视图

    private void OnDrawTarget(SerializedProperty target, Rect rect)
    {
        Rect objectRect = rect;
        objectRect.y += 2;
        objectRect.height = 20;
        objectRect.width *= 0.3f;
        target.objectReferenceValue =
            EditorGUI.ObjectField(objectRect, target.objectReferenceValue, typeof(Object), true);
    }

    private void OnDrawPopup(SerializedProperty target, SerializedProperty component, Rect rect)
    {
        Rect objectRect = rect;
        objectRect.y += 2;
        objectRect.height = 20;
        objectRect.width *= 0.3f;
        objectRect.x += rect.width * 0.3f + 5;
        string[] contents = new string[0] { };
        Component[] components = null;
        int popupIndex = 0;

        if (target.objectReferenceValue != null)
        {
            GameObject go = target.objectReferenceValue as GameObject;
            components = go.GetComponents<Component>();
            List<string> list = new List<string>();
            foreach (var item in components)
            {
                list.Add(item.GetType().Name);
            }

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == component.objectReferenceValue)
                {
                    popupIndex = i;
                    break;
                }
            }

            contents = list.ToArray();
        }

        popupIndex = EditorGUI.Popup(objectRect, popupIndex, contents);
        component.objectReferenceValue = components == null ? null : components[popupIndex];
    }

    private void OnDrawText(SerializedProperty name, Rect rect)
    {
        Rect objectRect = rect;
        objectRect.y += 2;
        objectRect.height = 20;
        objectRect.x += (rect.width * 0.3f + 5) * 2;
        objectRect.width = (rect.width - objectRect.x) + 10;
        name.stringValue = EditorGUI.TextField(objectRect, name.stringValue);
    }

    #endregion

    private void AutoInject()
    {
        List<GameObject> childs = new List<GameObject>();
        AddSerializeUIObject(Selection.activeGameObject, childs);
        childs.Clear();
        serializedObject.ApplyModifiedProperties();
    }

    private static void GetAllChild(GameObject go, List<GameObject> childs)
    {
        for (int i = 0; i < go.transform.childCount; i++)
        {
            Transform temp = go.transform.GetChild(i);

            if (temp.name.StartsWith("m_"))
            {
                childs.Add(temp.gameObject);
            }

            if (temp.childCount > 0)
            {
                GetAllChild(temp.gameObject, childs);
            }
        }
    }

    private void AddSerializeUIObject(GameObject curGo, List<GameObject> childs)
    {
        GetAllChild(curGo, childs);
        SerializedProperty elements = m_SerializedProperty;
        elements.ClearArray();
        int index = elements.arraySize;
        string componentName;
        StringBuilder fieldContent = new StringBuilder();
        StringBuilder methodContent = new StringBuilder();

        for (int i = 0; i < childs.Count; i++)
        {
            componentName = GetComponent(childs[i]);
            if (componentName == "error")
            {
                Debug.LogError($"生成失败 请检查 <b><color=red>{childs[i].name}</color></b> 的命名否合法?", childs[i]);
                return;
            }

            if (componentName != isGo)
            {
                if (!childs[i]?.GetComponent(componentName))
                {
                    Debug.LogError($"生成失败 请检查 <b><color=red>{childs[i].name}</color></b> 是否挂载了对应组件?", childs[i]);
                    return;
                }
            }

            string childsGoName = GetGoName(childs[i].name);
            elements.InsertArrayElementAtIndex(elements.arraySize);
            SerializedProperty item = elements.GetArrayElementAtIndex(elements.arraySize - 1);
            item.FindPropertyRelative("m_target").objectReferenceValue = childs[i];
            item.FindPropertyRelative("m_name").stringValue = childsGoName;
            item.FindPropertyRelative("m_component").objectReferenceValue = childs[i].GetComponent(componentName);

            string tempFieldStr = FieldTemplate.Replace("{0}", componentName);
            tempFieldStr = tempFieldStr.Replace("{1}", childsGoName);
            fieldContent.AppendLine("    " + tempFieldStr);

            if (componentName != isGo)
            {
                string tempMethodStr = MethodTemplate.Replace("{0}", childsGoName);
                tempMethodStr = tempMethodStr.Replace("{1}", i.ToString());
                tempMethodStr = tempMethodStr.Replace("{2}", componentName);
                methodContent.AppendLine("        " + tempMethodStr);
            }
            else
            {
                string tempMethodStr = MethodTemplateGo.Replace("{0}", childsGoName);
                tempMethodStr = tempMethodStr.Replace("{1}", i.ToString());
                tempMethodStr = tempMethodStr.Replace("{2}", componentName);
                methodContent.AppendLine("        " + tempMethodStr);
            }

        }

        //强制更新当前的可配置集合
        m_ReorderableList.serializedProperty = elements;
        m_ReorderableList.index = index;

        CreateCurrentCsharpScript(curGo, fieldContent, methodContent);
        AssetDatabase.Refresh();
    }

    private static string GetComponent(GameObject go)
    {
        string goCpm = go.gameObject.name.Split('_')[1].ToUpper();
        string ctName;

        #region 寻找对应组件

        try
        {
            ComponentType ctType = (ComponentType)Enum.Parse(typeof(ComponentType), goCpm);
            CheckNameValid(ctType, out ctName, go);
        }
        catch (ArgumentException e)
        {
            ComponentType ctType = ComponentType.ERROR;
            CheckNameValid(ctType, out ctName, go);
        }

        return ctName;

        #endregion
    }

    private static void CheckNameValid(ComponentType ctType, out string ctName, GameObject go)
    {
        switch (ctType)
        {
            case ComponentType.GO:
                ctName = "GameObject";
                break;
            case ComponentType.TRANS:
                ctName = "RectTransform";
                break;
            case ComponentType.CVS:
                ctName = "Canvas";
                break;
            case ComponentType.IMG:
                ctName = "Image";
                break;
            case ComponentType.TXT:
                ctName = "Text";
                break;
            case ComponentType.BTN:
                ctName = "Button";
                break;
            case ComponentType.TOG:
                ctName = "Toggle";
                break;
            case ComponentType.TOGGROUP:
                ctName = "ToggleGroup";
                break;
            case ComponentType.INPT:
                ctName = "InputField";
                break;
            case ComponentType.SCRL:
                ctName = "Scrollbar";
                break;
            case ComponentType.SLDR:
                ctName = "Slider";
                break;
            case ComponentType.DRPDN:
                ctName = "Dropdown";
                break;
            case ComponentType.GRD:
                ctName = "Grid";
                break;
            case ComponentType.SCROLLRECT:
                ctName = "ScrollRect";
                break;
            case ComponentType.ENS:
                ctName = "EnhancedScroller";
                break;
            case ComponentType.SCROLLRECTEX:
                ctName = "ScrollRectExtra";
                break;
            case ComponentType.LOOPLIST:
                ctName = "LoopListView2";
                break;
            case ComponentType.LOOPGRID:
                ctName = "LoopGridView";
                break;
            case ComponentType.LOOPSTAG:
                ctName = "LoopStaggeredGridView";
                break;
            case ComponentType.RAWIMG:
                ctName = "RawImage";
                break;
            case ComponentType.MASK:
                ctName = "Mask";
                break;
            case ComponentType.GRDLAYOUTGROUP:
                ctName = "GridLayoutGroup";
                break;
            case ComponentType.HLAYOUTGROUP:
                ctName = "HorizontalLayoutGroup";
                break;
            case ComponentType.VLAYOUTGROUP:
                ctName = "VerticalLayoutGroup";
                break;
            case ComponentType.CONTENTSIZEFITTER:
                ctName = "ContentSizeFitter";
                break;
            case ComponentType.SELECTABLE:
                ctName = "Selectable";
                break;
            case ComponentType.RV:
                ctName = "RecycleView";
                break;
            case ComponentType.ERROR:
                ctName = "error";
                break;
            default:
                ctName = "error";
                UnityEngine.Debug.LogError($"出现了未发现的BUG，请联系作者或自己排查一下 : {go.name} ", go);
                break;
        }
    }


    private static string GetGoName(string goName)
    {
        string[] goNameArray = goName.Split("_");
        StringBuilder sb = new StringBuilder();
        foreach (string name in goNameArray)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (name == "m")
                    continue;

                string newCharName = char.ToUpper(name[0]) + name.Substring(1);
                sb.Append(newCharName);
            }
        }
        return sb.ToString();
    }

    private static void CreateCurrentCsharpScript(GameObject go, StringBuilder fieldSb, StringBuilder methodSb)
    {
        string goName = go.name;
        string lastFourChars = goName.Substring(goName.Length - 4);
        string viewTemplateContent;
        string className = go.name + "Data";

        //当前的Go的最后四位是Item
        if (lastFourChars == "Item" || lastFourChars == "item")
            viewTemplateContent = File.ReadAllText(ItemTemplatePath, Encoding.UTF8);
        else
            viewTemplateContent = File.ReadAllText(ViewTemplatePath, Encoding.UTF8);

        string viewPath = ViewDir + "/" + go.name + "Data.cs";

        //view层脚本
        using (StreamWriter sw = new StreamWriter(viewPath))
        {
            string content = viewTemplateContent;
            content = content.Replace("#CLASSNAME#", className);
            content = content.Replace("#FIELD_BIND#", fieldSb.ToString());
            content = content.Replace("#METHOD_BIND#", methodSb.ToString());
            sw.Write(content);
            sw.Close();
        }

        fieldSb.Clear();
        methodSb.Clear();
        Debug.Log("<b><color=green>生成成功 ：</color><color=red>请</color><color=orange>开</color><color=yellow>始</color><color=green>编</color><color=cyan>写</color><color=blue>代</color><color=purple>码</color></b>");
    }

    private enum ComponentType : sbyte
    {
        /// <summary>
        /// GameObject 元素自身
        /// </summary>
        GO,

        /// <summary>
        /// RectTransform，UI 元素的变换组件
        /// </summary>
        TRANS,

        /// <summary>
        /// Canvas，画布，用于控制 UI 的渲染顺序和层级关系
        /// </summary>
        CVS,

        /// <summary>
        /// Image，图片显示组件
        /// </summary>
        IMG,

        /// <summary>
        /// Text，文本显示组件
        /// </summary>
        TXT,

        /// <summary>
        /// Button，按钮组件
        /// </summary>
        BTN,

        /// <summary>
        /// Toggle，单选框或复选框组件
        /// </summary>
        TOG,

        /// <summary>
        /// ToggleGroup 用于管理Toggle状态
        /// </summary>
        TOGGROUP,

        /// <summary>
        /// InputField，输入框组件
        /// </summary>
        INPT,

        /// <summary>
        /// Scrollbar，滚动条组件
        /// </summary>
        SCRL,

        /// <summary>
        /// Slider，滑动条组件
        /// </summary>
        SLDR,

        /// <summary>
        /// Dropdown，下拉框组件
        /// </summary>
        DRPDN,

        /// <summary>
        /// Grid，网格组件
        /// </summary>
        GRD,

        /// <summary>
        /// 滚动视图组件，包含 ScrollRect 和 Scrollbar 组件
        /// </summary>
        SCROLLRECT,

        /// <summary>
        /// 滚动视图组件，ScrollRectExtra
        /// </summary>
        SCROLLRECTEX,

        /// <summary>
        /// 滚动视图组件，EnhancedScroller
        /// </summary>
        ENS,

        /// <summary>
        /// 滚动视图组件，LoopListView2
        /// </summary>
        LOOPLIST,

        /// <summary>
        /// 滚动视图组件，LoopGridView
        /// </summary>
        LOOPGRID,

        /// <summary>
        /// 滚动视图组件，LoopStaggeredGridView
        /// </summary>
        LOOPSTAG,

        /// <summary>
        /// RawImage，未经过处理的图片显示组件
        /// </summary>
        RAWIMG,

        /// <summary>
        /// Mask，遮罩组件，用于裁剪 UI 元素的显示范围
        /// </summary>
        MASK,

        /// <summary>
        /// 布局组件，包含 HorizontalLayoutGroup 和 VerticalLayoutGroup
        /// </summary>
        GRDLAYOUTGROUP,

        /// <summary>
        /// 布局组件 HorizontalLayoutGroup
        /// </summary>
        HLAYOUTGROUP,

        /// <summary>
        /// 布局组件 VerticalLayoutGroup
        /// </summary>
        VLAYOUTGROUP,

        /// <summary>
        /// 内容自适应组件，用于根据子元素的大小调整自身的大小
        /// </summary>
        CONTENTSIZEFITTER,

        /// <summary>
        /// 可选中组件，是所有可选中 UI 组件的基类
        /// </summary>
        SELECTABLE,

        /// <summary>
        /// RecycleView 无限滑动列表
        /// </summary>
        RV,

        /// <summary>
        /// 未知错误
        /// </summary>
        ERROR


        // trans		 		: RectTransform，	     UI 元素的变换组件
        // cvs 		 		    : Canvas，			     画布，用于控制 UI 的渲染顺序和层级关系
        // img 	   	 	     	: Image，			     图片显示组件
        // txt 		 	    	: Text，				 文本显示组件
        // btn 		 		    : Button，				 按钮组件
        // tog 		 		    : Toggle，				 单选框或复选框组件
        // togGroup 	 		:  ToggleGroup 			 用于管理Toggle状态
        // inpt 		 		: InputField，		     输入框组件
        // scrl 		 		: Scrollbar，			 滚动条组件
        // sldr 		 		: Slider，				 滑动条组件
        // drpdn 		 		: Dropdown，     	 	 下拉框组件
        // grd 		 		    : Grid，				 网格组件
        // scrollRect   		: ScrollRect             滚动视图组件，包含 ScrollRect 和 Scrollbar 组件
        // rawImg       		: RawImage，		     未经过处理的图片显示组件
        // mask 		 		: Mask，				 遮罩组件，用于裁剪 UI 元素的显示范围
        // grdLayoutGroup 		: GridLayoutGroup，      布局组件，包含 HorizontalLayoutGroup 和 VerticalLayoutGroup
        // hLayoutGroup   		: HorizontalLayoutGroup，布局组件 横向滑动
        // vLayoutGroup         : VerticalLayoutGroup，  布局组件 纵向滑动
        // contentSizeFitter    : ContentSizeFitter，    内容自适应组件，用于根据子元素的大小调整自身的大小
        // selectable 			: Selectable，           可选中组件，是所有可选中 UI 组件的基类
        // rv 					: RecycleView，          无限滑动列表
    }
}