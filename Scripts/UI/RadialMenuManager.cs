using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuManager : MonoBehaviour
{
    private bool m_IsWearBelt = false;
    private bool m_IsWearPistol = false;

    private void Awake()
    {
        gameObject.SetActive(false);
        EventCenter.AddListener(EventDefine.WearBelt, WearBelt);
        EventCenter.AddListener(EventDefine.WearPistol, WearPistol);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.WearBelt, WearBelt);
        EventCenter.RemoveListener(EventDefine.WearPistol, WearPistol);
    }
    /// <summary>
    /// 腰带穿戴上的监听方法
    /// </summary>
    private void WearBelt()
    {
        m_IsWearBelt = true;
        if (m_IsWearBelt && m_IsWearPistol)
        {
            gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 枪拿起的监听方法
    /// </summary>
    private void WearPistol()
    {
        m_IsWearPistol = true;
        if (m_IsWearBelt && m_IsWearPistol)
        {
            gameObject.SetActive(true);
        }
    }
    public void OnUsePistolClick()
    {
        HandManger handManger = transform.parent.parent.parent.GetComponentInChildren<HandManger>();
        if (handManger.m_GrabObj != null && handManger.m_GrabObjectType == GrabObjectType.Pistol)
        {
            handManger.UnUsePistol();
        }
        if (handManger.m_GrabObj != null && handManger.m_GrabObjectType == GrabObjectType.Bomb)
        {
            handManger.UnUseBomb();
        }
        EventCenter.Broadcast(EventDefine.UsePistol);
        EventCenter.Broadcast(EventDefine.IsStartGestureRecognition, false);
    }
    public void OnUseBombClick()
    {
        HandManger handManger = transform.parent.parent.parent.GetComponentInChildren<HandManger>();
        if (handManger.m_GrabObj != null && handManger.m_GrabObjectType == GrabObjectType.Pistol)
        {
            handManger.UnUsePistol();
        }
        if (handManger.m_GrabObj != null && handManger.m_GrabObjectType == GrabObjectType.Bomb)
        {
            handManger.UnUseBomb();
        }
        EventCenter.Broadcast(EventDefine.UseBomb);
        EventCenter.Broadcast(EventDefine.IsStartGestureRecognition, false);
    }
    public void OnUseGestureClick()
    {
        HandManger[] handMangers = GameObject.FindObjectsOfType<HandManger>();
        foreach (var handManger in handMangers)
        {
            if (handManger.m_GrabObj != null && handManger.m_GrabObjectType == GrabObjectType.Pistol)
            {
                handManger.UnUsePistol();
            }
            if (handManger.m_GrabObj != null && handManger.m_GrabObjectType == GrabObjectType.Bomb)
            {
                handManger.UnUseBomb();
            }
        }
        EventCenter.Broadcast(EventDefine.IsStartGestureRecognition, true);
    }
}
