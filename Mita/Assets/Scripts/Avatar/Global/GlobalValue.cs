using System.Collections.Generic;
public class GlobalValue
{
    #region ����
    public static string ChatSceneName = "Chatroom";
    public static string CreateSceneName = "Sapsule";
    public static string ChatPlayer = "chatplayer";
    public static string AvatarPlayer = "avatarplayer";

    public static string Chatroom_f_01_4 = "chatroom_f_01_4";
    public static string Chatroom_m_01_4 = "chatroom_m_01_4";
    #endregion

    #region ����ģ������
    /// <summary>
    /// Ů�Թ�ģ
    /// </summary>
    public static string Avaterfemale = "avaterfemale";//"clo09"

    /// <summary>
    /// ���Թ�ģ
    /// </summary>
    public static string Avatermale = "avatermale";
    #endregion

    #region  ����
    /// <summary>
    /// �� idle ����
    /// </summary>
    public static string ChatClipMaleIdleNames = "ani_idle_male";
    /// <summary>
    /// Ů idle ����
    /// </summary>
    public static string ChatClipFemaleIdleNames = "ani_idle_female";

    //�� ��������
    public static string[] ChatClipMaleNames = new string[6] { "ani_sit01_male", "ani_sit02_male", "ani_sit03_male", "ani_sit04_male", "ani_sit05_male", "ani_sit06_male" };

    //Ů ��������
    public static string[] ChatClipFemaleNames = new string[6] { "ani_sit01_female", "ani_sit02_female", "ani_sit03_female", "ani_sit04_female", "ani_sit05_female", "ani_sit06_female" };

    //�� ��Ӱ POSE ����
    public static string[] ChatClipGroupPhoto = new string[6] { "ani_photo01", "ani_photo02", "ani_photo03", "ani_photo04", "ani_photo05", "ani_photo06" };
    #endregion

    public static string FaceBlendShapeStr = "blendShape1.";

    public static string AvatarMaterialBundle = "avatarmaterial";

    public static string AvatarAnimationBundle = "animation";

    #region  ��װ��λ
    public static string AvatarPart_Head = "head";
    public static string AvatarPart_Body = "BodyCull";
    public static string AvatarPart_Hair = "hair";
    public static string AvatarPart_Cloth = "cloth";
    public static string AvatarPart_Glasses = "glasses";

    public static Dictionary<string, bool> AvatarMeshCombineHideMap = new Dictionary<string, bool> {
        { "Chest01", true},
        { "Hip01", true  },
        { "ArmR01", true },
        { "ArmR02", true },
        { "ArmR03", true },
        { "ArmL01", true },
        { "ArmL02", true },
        { "ArmL03",true  },
        { "LegR01", true },
        { "LegR02", true },
        { "LegR03", true },
        { "LegR04", true },
        { "FootR", true  },
        { "LegL01", true },
        { "LegL02", true },
        { "LegL03", true },
        { "LegL04", true },
        { "FootL", true  },
    };

    #endregion

    public static int IOS_Width = 720;
    public static int Andro_Hight_Width = 920;
    public static int Andro_Low_Width = 720;

    #region Guide

    #endregion
}
