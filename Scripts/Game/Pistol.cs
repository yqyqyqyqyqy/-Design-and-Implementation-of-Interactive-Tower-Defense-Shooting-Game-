using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pistol : MonoBehaviour
{
    public EventDefine ShotEvent;
    public EventDefine ReloadMagazineEvent;

    public Transform m_StartPos;
    public GameObject go_Point;
    public GameObject effect_HitOtherMask;
    public GameObject effect_HitOther;
    public GameObject effect_Fire;
    public GameObject effect_Blood;
    public GameObject go_Magazine;
    public AudioClip audio_Shot;

    private LineRenderer m_LineRenderer;
    private Animator m_Anim;
    private RaycastHit m_Hit;
    public int m_CurrentBulletCount = 6;
    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_LineRenderer = GetComponent<LineRenderer>();
        m_Anim = GetComponent<Animator>();
        EventCenter.AddListener(ShotEvent, Shot);
        EventCenter.AddListener(ReloadMagazineEvent, ReloadMagazine);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(ShotEvent, Shot);
        EventCenter.RemoveListener(ReloadMagazineEvent, ReloadMagazine);
    }
    /// <summary>
    /// 换弹夹
    /// </summary>
    private void ReloadMagazine()
    {
        //代表是Main场景 则忽略
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        //如果手枪是隐藏的，则忽略
        if (gameObject.activeSelf == false) return;
        //如果当前正在播放开火的动画，则忽略
        if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Fire")) return;
        //如果当前正在播放换弹夹的动画，则忽略
        if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Reload")) return;

        if (GameObject.FindObjectOfType<RadialMenuManager>() != null)
            if (GameObject.FindObjectOfType<RadialMenuManager>().transform.localScale != Vector3.zero)
                return;

        int temp = m_CurrentBulletCount;
        m_CurrentBulletCount = AmmoManager.Instance.ReloadMagazine();
        if (m_CurrentBulletCount != 0)
        {
            m_Anim.SetTrigger("Reload");
            GameObject go = Instantiate(go_Magazine, transform.Find("Magazine").position, transform.Find("Magazine").rotation);
            go.GetComponent<Magazine>().SetBulletCount(temp);
        }
    }
    /// <summary>
    /// 射击
    /// </summary>
    private void Shot()
    {
        //代表是Main场景 则忽略
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        //如果手枪是隐藏的，则忽略
        if (gameObject.activeSelf == false) return;
        //如果当前正在播放开火的动画，则忽略
        if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Fire")) return;
        //如果当前正在播放换弹夹的动画，则忽略
        if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Reload")) return;

        if (m_CurrentBulletCount <= 0) return;
        m_CurrentBulletCount--;
        //播放射击动画
        m_Anim.SetTrigger("Shot");

        if (m_AudioSource.isPlaying == false)
        {
            m_AudioSource.clip = audio_Shot;
            m_AudioSource.Play();
        }
        Destroy(Instantiate(effect_Fire, m_StartPos.position, m_StartPos.rotation), 1.5f);

        if (m_Hit.collider != null)
        {
            //是否是僵尸
            if (m_Hit.collider.tag == "Zombie")
            {
                if (m_Hit.transform.GetComponent<BodyPartDrop>() != null)
                {
                    m_Hit.transform.GetComponent<BodyPartDrop>().Hit();
                }
                if (m_Hit.transform.GetComponent<ZombieHit>() != null)
                {
                    m_Hit.transform.GetComponent<ZombieHit>().Hit();
                }
                //实例化血的特效，1.5秒之后销毁
                Destroy(Instantiate(effect_Blood, m_Hit.point, Quaternion.LookRotation(m_Hit.normal)), 2f);
            }
            else
            {
                GameObject mask = Instantiate(effect_HitOtherMask, m_Hit.point, Quaternion.LookRotation(m_Hit.normal));
                mask.transform.parent = m_Hit.transform;
                Destroy(Instantiate(effect_HitOther, m_Hit.point, Quaternion.LookRotation(m_Hit.normal)), 2);
            }
        }
    }
    /// <summary>
    /// 画线
    /// </summary>
    private void DrawLine(Vector3 startPos, Vector3 endPos, Color color)
    {
        m_LineRenderer.positionCount = 2;
        m_LineRenderer.SetPosition(0, startPos);
        m_LineRenderer.SetPosition(1, endPos);
        m_LineRenderer.startWidth = 0.001f;
        m_LineRenderer.endWidth = 0.001f;
        m_LineRenderer.material.color = color;
    }
    private void FixedUpdate()
    {
        if (Physics.Raycast(m_StartPos.position, m_StartPos.forward, out m_Hit, 100000, 1 << 0 | 1 << 2))
        {
            DrawLine(m_StartPos.position, m_Hit.point, Color.green);
            go_Point.SetActive(true);
            go_Point.transform.position = m_Hit.point;
        }
        else
        {
            DrawLine(m_StartPos.position, m_StartPos.forward * 100000, Color.red);
            go_Point.SetActive(false);
        }
    }
}
