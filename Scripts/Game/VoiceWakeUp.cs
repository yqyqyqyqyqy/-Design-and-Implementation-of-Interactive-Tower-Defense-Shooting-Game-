using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceWakeUp : MonoBehaviour
{
    /// <summary>
    /// 可信度
    /// </summary>
    public ConfidenceLevel m_ConfidenceLevel = ConfidenceLevel.Medium;
    /// <summary>
    /// 关键词数组
    /// </summary>
    public string[] keyWordArr;
    /// <summary>
    /// 短语识别器
    /// </summary>
    private PhraseRecognizer m_PhraseRecognizer;

    private void Awake()
    {
        m_PhraseRecognizer = new KeywordRecognizer(keyWordArr, m_ConfidenceLevel);
        //识别方法的注册
        m_PhraseRecognizer.OnPhraseRecognized += M_PhraseRecognizer_OnPhraseRecognized;
        m_PhraseRecognizer.Start();
    }
    private void OnDestroy()
    {
        m_PhraseRecognizer.Dispose();
    }
    private void M_PhraseRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        print(args.text);
        if (args.text == keyWordArr[0])//开始游戏
        {
            EventCenter.Broadcast(EventDefine.StartGame);
        }
        else if (args.text == keyWordArr[1])//退出游戏
        {
            Application.Quit();
        }
        else if (args.text == keyWordArr[2])//换弹夹
        {
            EventCenter.Broadcast(EventDefine.LeftReloadMagazine);
            EventCenter.Broadcast(EventDefine.RightReloadMagazine);
        }
        else if (args.text == keyWordArr[3])//换手枪
        {
            if (GetComponentInChildren<RadialMenuManager>() != null)
                GetComponentInChildren<RadialMenuManager>().OnUsePistolClick();
        }
        else if (args.text == keyWordArr[4])//使用炸弹
        {
            if (GetComponentInChildren<RadialMenuManager>() != null)
                GetComponentInChildren<RadialMenuManager>().OnUseBombClick();
        }
        else if (args.text == keyWordArr[5])//手势识别 
        {
            if (GetComponentInChildren<RadialMenuManager>() != null)
                GetComponentInChildren<RadialMenuManager>().OnUseGestureClick();
        }
    }
}
