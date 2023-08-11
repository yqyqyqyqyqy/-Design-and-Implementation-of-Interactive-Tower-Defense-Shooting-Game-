using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;
using VRTK;

public enum GrabObjectType
{
    None,
    Other,
    Book,
    Pistol,
    Belt,
    Magazine,
    Bomb,
}
public enum HandAnimStateType
{
    None,
    Pistol,
}
public class HandManger : MonoBehaviour
{
    /// <summary>
    /// 监听抓取按键按下的事件码
    /// </summary>
    public EventDefine GrabEvent;
    public EventDefine ShotEvent;
    public EventDefine UseBombEvent;
    public EventDefine UsePistolEvent;

    public GameObject go_Bomb;
    public float m_ThrowMulitiple = 1f;

    private Animator m_Anim;
    /// <summary>
    /// 是否可以抓取
    /// </summary>
    private bool m_IsCanGrab = false;
    /// <summary>
    /// 当前抓取的物体
    /// </summary>
    public GameObject m_GrabObj = null;
    public GrabObjectType m_GrabObjectType = GrabObjectType.None;

    public StateModel[] m_StateModels;

    [System.Serializable]
    public class StateModel
    {
        public HandAnimStateType StateType;
        public GameObject go;
    }

    /// <summary>
    /// 判断手是否触碰到可以抓取的物体
    /// </summary>
    private bool m_IsTrigger = false;
    /// <summary>
    /// 是否使用手枪
    /// </summary>
    private bool m_IsUsePistol = false;
    /// <summary>
    /// 是否使用炸弹
    /// </summary>
    private bool m_IsUseBomb = false;
    private VRTK_ControllerEvents controllerEvents;

    private void Awake()
    {
        m_Anim = GetComponent<Animator>();
        controllerEvents = GetComponentInParent<VRTK_ControllerEvents>();
        EventCenter.AddListener<bool>(GrabEvent, IsCanGrab);
        EventCenter.AddListener(ShotEvent, Shot);
        EventCenter.AddListener(UseBombEvent, UseBomb);
        EventCenter.AddListener(UsePistolEvent, UsePistol);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<bool>(GrabEvent, IsCanGrab);
        EventCenter.RemoveListener(ShotEvent, Shot);
        EventCenter.RemoveListener(UseBombEvent, UseBomb);
        EventCenter.RemoveListener(UsePistolEvent, UsePistol);
    }
    /// <summary>
    /// 射击
    /// </summary>
    private void Shot()
    {
        if (m_Anim.GetInteger("State") != (int)HandAnimStateType.Pistol) return;

        m_Anim.SetTrigger("Shot");
    }
    /// <summary>
    /// 是否可以抓取物体的监听方法
    /// </summary>
    /// <param name="value"></param>
    private void IsCanGrab(bool value)
    {
        if (value == false)
        {
            if (m_GrabObj != null && m_GrabObjectType == GrabObjectType.Bomb)
            {
                //代表拿的是炸弹
                ThrowBomb();
            }
        }
        //释放抓取的物体
        if (value)
        {
            if (m_GrabObj != null)
            {
                if (m_GrabObjectType == GrabObjectType.Other)
                {
                    m_GrabObj.transform.parent = null;
                    m_GrabObj = null;
                    m_GrabObjectType = GrabObjectType.None;
                }
                else if (m_GrabObjectType == GrabObjectType.Book)
                {
                    if (m_GrabObj.GetComponent<Book>().m_IsTrigger)
                    {
                        m_GrabObj.GetComponent<Book>().Put();
                    }
                    else
                    {
                        m_GrabObj.transform.parent = null;
                        m_GrabObj.transform.position = m_GrabObj.GetComponent<Book>().m_StratPos;
                        m_GrabObj.transform.rotation = m_GrabObj.GetComponent<Book>().m_StartRot;
                    }

                    m_GrabObj = null;
                    m_GrabObjectType = GrabObjectType.None;
                }
                else if (m_GrabObjectType == GrabObjectType.Belt || m_GrabObjectType == GrabObjectType.Magazine)
                {
                    m_GrabObj.transform.parent = null;
                    m_GrabObj.GetComponent<Rigidbody>().useGravity = true;
                    m_GrabObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    m_GrabObj = null;
                    m_GrabObjectType = GrabObjectType.None;
                }
                return;
            }
        }
        if (m_GrabObj == null)
            m_Anim.SetBool("Catch", value);
        m_IsCanGrab = value;

        PistolOrBombChangeHand();
    }
    /// <summary>
    /// 投掷炸弹
    /// </summary>
    private void ThrowBomb()
    {
        //更新炸弹数量
        AmmoManager.Instance.UpdateBomb();

        m_GrabObj.transform.parent = null;
        m_GrabObj.AddComponent<Rigidbody>();
        m_Anim.SetBool("Catch", false);
        m_GrabObjectType = GrabObjectType.None;

        Vector3 velocity = controllerEvents.GetVelocity();
        Vector3 angularVelocity = controllerEvents.GetAngularVelocity();

        m_GrabObj.GetComponent<Rigidbody>().velocity = transform.parent.parent.TransformDirection(velocity) * m_ThrowMulitiple;
        m_GrabObj.GetComponent<Rigidbody>().angularVelocity = transform.parent.parent.TransformDirection(angularVelocity);
        m_GrabObj.GetComponent<Bomb>().IsThrow = true;
        m_GrabObj = null;
        m_IsUseBomb = false;

        UsePistol();
    }
    /// <summary>
    /// 手枪换手
    /// </summary>
    private void PistolOrBombChangeHand()
    {
        //1.满足当前手没有抓取任何物体
        //2.当前手没有触碰到任何可以抓取的物体
        //3.另外一只手要保证拿着枪
        if (m_GrabObj == null && m_IsTrigger == false && m_IsCanGrab == false)
        {
            HandManger[] handMangers = GameObject.FindObjectsOfType<HandManger>();
            foreach (var handManger in handMangers)
            {
                if (handManger != this)
                {
                    if (handManger.m_IsUsePistol)
                    {
                        UsePistol();
                        handManger.UnUsePistol();
                        m_StateModels[0].go.GetComponent<Pistol>().m_CurrentBulletCount =
                            handManger.m_StateModels[0].go.GetComponent<Pistol>().m_CurrentBulletCount;
                    }
                    //炸弹换手
                    if (handManger.m_IsUseBomb)
                    {
                        handManger.UnUseBomb();
                        UseBomb();
                    }
                }
            }
        }
    }
    /// <summary>
    /// 抓取
    /// 作用：一只手拿另外一种手的物体的一些逻辑处理
    /// </summary>
    /// <param name="value"></param>
    public void Catch(bool value)
    {
        if (m_GrabObj != null)
        {
            m_GrabObj = null;
            m_GrabObjectType = GrabObjectType.None;
        }
        m_Anim.SetBool("Catch", value);
    }
    /// <summary>
    /// 使用炸弹
    /// </summary>
    private void UseBomb()
    {
        if (AmmoManager.Instance.IsHasBomb() == false) return;

        //判断当前右手是否拿着物品，如果拿着则卸掉
        if (m_GrabObj != null)
        {
            if (m_GrabObjectType == GrabObjectType.Pistol)
            {
                UnUsePistol();
            }
            else if (m_GrabObjectType == GrabObjectType.Belt || m_GrabObjectType == GrabObjectType.Magazine)
            {
                m_GrabObj.transform.parent = null;
                m_GrabObj.GetComponent<Rigidbody>().useGravity = true;
                m_GrabObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                m_GrabObj = null;
                m_GrabObjectType = GrabObjectType.None;
            }
            else if (m_GrabObjectType == GrabObjectType.Bomb)
            {
                return;
            }
        }
        Transform target = transform.parent.Find("BombTarget");
        GameObject bomb = Instantiate(go_Bomb, transform.parent);
        bomb.transform.localPosition = target.localPosition;
        bomb.transform.localRotation = target.localRotation;
        bomb.transform.localScale = target.localScale;

        m_GrabObj = bomb;
        m_GrabObjectType = GrabObjectType.Bomb;
        m_Anim.SetBool("Catch", true);
        m_IsUseBomb = true;
    }
    /// <summary>
    /// 卸载炸弹
    /// </summary>
    public void UnUseBomb()
    {
        m_IsUseBomb = false;
        Destroy(m_GrabObj);
        m_GrabObj = null;
        m_GrabObjectType = GrabObjectType.None;
        m_Anim.SetBool("Catch", false);
    }
    /// <summary>
    /// 使用手枪
    /// </summary>
    private void UsePistol()
    {
        if (m_GrabObj != null)
        {
            if (m_GrabObjectType == GrabObjectType.Belt || m_GrabObjectType == GrabObjectType.Magazine)
            {
                m_GrabObj.transform.parent = null;
                m_GrabObj.GetComponent<Rigidbody>().useGravity = true;
                m_GrabObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                m_GrabObj = null;
                m_GrabObjectType = GrabObjectType.None;
            }
            else if (m_GrabObjectType == GrabObjectType.Bomb)
            {
                UnUseBomb();
            }
            else if (m_GrabObjectType == GrabObjectType.Pistol)
            {
                return;
            }
        }
        m_IsUsePistol = true;
        m_Anim.SetBool("Catch", false);
        m_GrabObjectType = GrabObjectType.Pistol;
        m_GrabObj = m_StateModels[0].go;
        //切换成拿枪的动画
        //显示手枪
        TurnState(HandAnimStateType.Pistol);
    }
    /// <summary>
    /// 卸下手枪
    /// </summary>
    public void UnUsePistol()
    {
        m_IsUsePistol = false;
        m_Anim.SetBool("Catch", false);
        m_GrabObjectType = GrabObjectType.None;
        m_GrabObj = null;
        TurnState(HandAnimStateType.None);
    }
    private void TurnState(HandAnimStateType stateType)
    {
        m_Anim.SetInteger("State", (int)stateType);
        foreach (var item in m_StateModels)
        {
            if (item.StateType == stateType && item.go.activeSelf == false)
            {
                item.go.SetActive(true);
            }
            else if (item.go.activeSelf)
            {
                item.go.SetActive(false);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Others" || other.tag == "Book" || other.tag == "Pistol" || other.tag == "Belt" || other.tag == "Magazine")
        {
            m_IsTrigger = true;
        }
        if (other.GetComponent<Highlighter>() != null)
        {
            other.GetComponent<Highlighter>().On(Color.red);
        }
        if (other.tag == "Others" && m_IsCanGrab && m_GrabObj == null)
        {
            ProcessGrab(other);
            m_GrabObjectType = GrabObjectType.Other;
        }
        else if (other.tag == "Book" && m_IsCanGrab && m_GrabObj == null)
        {
            ProcessGrab(other);
            m_GrabObjectType = GrabObjectType.Book;
        }
        else if (other.tag == "Pistol" && m_IsCanGrab && m_GrabObj == null)
        {
            EventCenter.Broadcast(EventDefine.WearPistol);
            Destroy(other.gameObject);
            UsePistol();
        }
        else if (other.tag == "Belt" && m_IsCanGrab && m_GrabObj == null)
        {
            ProcessGrab(other);
            other.GetComponent<Rigidbody>().useGravity = false;
            other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            m_GrabObjectType = GrabObjectType.Belt;
        }
        else if (other.tag == "Magazine" && m_IsCanGrab && m_GrabObj == null)
        {
            ProcessGrab(other);
            other.GetComponent<Rigidbody>().useGravity = false;
            other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            m_GrabObjectType = GrabObjectType.Magazine;
        }
    }
    /// <summary>
    /// 处理抓取
    /// </summary>
    /// <param name="other"></param>
    private void ProcessGrab(Collider other)
    {
        //一只手拿另外一种手的物体的一些逻辑处理
        if (other.transform.parent != null)
        {
            if (other.transform.parent.tag == "ControllerRight" || other.transform.parent.tag == "ControllerLeft")
            {
                other.transform.parent.GetComponentInChildren<HandManger>().Catch(false);
            }
        }
        Catch(true);
        other.gameObject.transform.parent = transform.parent;
        m_GrabObj = other.gameObject;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Highlighter>() != null)
        {
            other.GetComponent<Highlighter>().Off();
        }
        m_IsTrigger = false;
    }
}
