using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BookType
{
    StartBook,
    AboutBook,
    GestureBook
}
public class Book : MonoBehaviour
{
    public BookType m_BookType;
    public Vector3 m_StratPos;
    public Quaternion m_StartRot;
    /// <summary>
    /// 判断书本是否触发到书台
    /// </summary>
    public bool m_IsTrigger = false;
    /// <summary>
    /// 触发的书台物体
    /// </summary>
    private GameObject go_StandBook;

    private void Awake()
    {
        m_StratPos = transform.position;
        m_StartRot = transform.rotation;
    }
    private void Update()
    {
        if (transform.parent != null && go_StandBook != null)
        {
            if (transform.parent != go_StandBook.transform)
            {
                IsActiveUI(false);
            }
        }
    }
    /// <summary>
    /// 放置书本
    /// </summary>
    public void Put()
    {
        if (go_StandBook.GetComponentInChildren<Book>() != null)
        {
            go_StandBook.GetComponentInChildren<Book>().Release();
        }
        transform.parent = go_StandBook.transform;
        transform.position = go_StandBook.transform.GetChild(0).position;
        transform.rotation = go_StandBook.transform.GetChild(0).rotation;
        IsActiveUI(true);
    }
    /// <summary>
    /// 书本归为
    /// </summary>
    public void Release()
    {
        transform.parent = null;
        transform.position = m_StratPos;
        transform.rotation = m_StartRot;
        IsActiveUI(false);
    }
    /// <summary>
    /// 是否激活当前书本对应的UI界面
    /// </summary>
    /// <param name="value"></param>
    private void IsActiveUI(bool value)
    {
        switch (m_BookType)
        {
            case BookType.StartBook:
                EventCenter.Broadcast(EventDefine.IsShowStartPanel, value);
                break;
            case BookType.AboutBook:
                EventCenter.Broadcast(EventDefine.IsShowAboutPanel, value);
                break;
            case BookType.GestureBook:
                EventCenter.Broadcast(EventDefine.IsShowGesturePanel, value);
                break;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "BookStand")
        {
            m_IsTrigger = true;
            go_StandBook = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "BookStand")
        {
            m_IsTrigger = false;
            go_StandBook = null;
        }
    }
}
