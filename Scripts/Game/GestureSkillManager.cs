using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edwon.VR;
using Edwon.VR.Gesture;

public class GestureSkillManager
{
    /// <summary>
    /// 手势名与技能名的字典
    /// </summary>
    private static Dictionary<string, string> m_GestureSkillDic = new Dictionary<string, string>();
    private static VRGestureSettings gestureSettings;
    private static string SkillName = "Skill";

    static GestureSkillManager()
    {
        gestureSettings = Utils.GetGestureSettings();
        m_GestureSkillDic = GetGestureSkillDic();
    }
    /// <summary>
    /// 获取手势名与技能名之间的关系
    /// </summary>
    /// <returns></returns>
    private static Dictionary<string, string> GetGestureSkillDic()
    {
        Dictionary<string, string> gestureSkillDic = new Dictionary<string, string>();
        //规则：手势名-技能名；手势名-技能名
        if (PlayerPrefs.HasKey("GestureSkill"))
        {
            string gestureSkill = PlayerPrefs.GetString("GestureSkill");
            string[] arr = gestureSkill.Split(';');
            foreach (var item in arr)
            {
                string[] tempArr = item.Split('-');
                gestureSkillDic.Add(tempArr[0], tempArr[1]);
            }
        }
        else
        {
            for (int i = 0; i < gestureSettings.gestureBank.Count; i++)
            {
                gestureSkillDic.Add(gestureSettings.gestureBank[i].name, SkillName + (i + 1).ToString());
            }
            SaveGestureSkillDic(gestureSkillDic);
        }
        return gestureSkillDic;
    }
    /// <summary>
    /// 保存手势与技能之间的关系
    /// </summary>
    private static void SaveGestureSkillDic(Dictionary<string, string> dic)
    {
        string temp = "";
        int index = 0;
        foreach (var item in dic)
        {
            //规则：手势名-技能名；手势名-技能名
            temp += item.Key + "-" + item.Value;
            index++;
            if (index != dic.Count)
                temp += ";";
        }
        PlayerPrefs.SetString("GestureSkill", temp);
    }
    /// <summary>
    /// 通过手势名获取技能名
    /// </summary>
    public static string GetSkillNameByGestureName(string gestureName)
    {
        if (m_GestureSkillDic.ContainsKey(gestureName))
        {
            return m_GestureSkillDic[gestureName];
        }
        return null;
    }
    /// <summary>
    /// 更换手势与技能之间的关系（更换技能）
    /// </summary>
    /// <param name="gestureName"></param>
    /// <param name="newSkillName"></param>
    public static void ChangeSkill(string gestureName, string newSkillName)
    {
        if (m_GestureSkillDic.ContainsKey(gestureName))
        {
            m_GestureSkillDic[gestureName] = newSkillName;
            SaveGestureSkillDic(m_GestureSkillDic);
        }
    }
    /// <summary>
    /// 通过手势名获取技能图片
    /// </summary>
    /// <param name="gestureName"></param>
    public static Sprite GetSkilSpriteByGestureName(string gestureName)
    {
        if (m_GestureSkillDic.ContainsKey(gestureName))
        {
            return ResourcesManager.LoadSprite(m_GestureSkillDic[gestureName]);
        }
        return null;
    }
}
