using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance;
    /// <summary>
    /// 子弹数量
    /// </summary>
    public int BulletCount;
    /// <summary>
    /// 炸弹数量
    /// </summary>
    public int BombCount;

    private Transform target;
    private Text txt_Bullet;
    private Text txt_Bomb;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        txt_Bullet = transform.Find("Bullet/Text").GetComponent<Text>();
        txt_Bomb = transform.Find("Bomb/Text").GetComponent<Text>();
        target = GameObject.FindGameObjectWithTag("CameraRig").transform;
        EventCenter.AddListener(EventDefine.WearBelt, Show);
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.WearBelt, Show);
    }
    private void FixedUpdate()
    {
        float height = target.GetComponent<CapsuleCollider>().height;
        transform.position = new Vector3(Camera.main.transform.position.x, height, Camera.main.transform.position.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 0);
    }
    /// <summary>
    /// 显示弹药库界面
    /// </summary>
    private void Show()
    {
        gameObject.SetActive(true);
        UpdateBullet(0);
        UpdateBomb(0);
    }
    /// <summary>
    /// 重装弹夹
    /// </summary>
    public int ReloadMagazine()
    {
        if (BulletCount >= 6)
        {
            UpdateBullet(-6);
            return 6;
        }
        else
        {
            int temp = BulletCount;
            BulletCount = 0;
            UpdateBullet(0);
            return temp;
        }
    }
    /// <summary>
    /// 是否有手榴弹
    /// </summary>
    /// <returns></returns>
    public bool IsHasBomb()
    {
        if (BombCount <= 0)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// 更新子弹数量
    /// </summary>
    /// <param name="count"></param>
    public void UpdateBullet(int count)
    {
        BulletCount += count;
        txt_Bullet.text = BulletCount.ToString();
    }
    /// <summary>
    /// 更新手榴弹数量
    /// </summary>
    /// <param name="count"></param>
    public void UpdateBomb(int count = -1)
    {
        BombCount += count;
        txt_Bomb.text = BombCount.ToString();
    }
}
