using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Watch : MonoBehaviour
{
    private Transform target;
    private bool isFind = false;
    private Text txt_Hp;

    private void Awake()
    {
        txt_Hp = GetComponentInChildren<Text>();
        EventCenter.AddListener<int>(EventDefine.UpdateHpUI, UpdateHP);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<int>(EventDefine.UpdateHpUI, UpdateHP);
    }
    private void FixedUpdate()
    {
        if (GameObject.FindGameObjectWithTag("WatchTarget") != null && isFind == false)
        {
            target = GameObject.FindGameObjectWithTag("WatchTarget").transform;
            isFind = true;
        }
        if (target != null)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
    /// <summary>
    /// 更新HP显示
    /// </summary>
    /// <param name="hp"></param>
    private void UpdateHP(int hp)
    {
        txt_Hp.text = hp.ToString();
    }
}
