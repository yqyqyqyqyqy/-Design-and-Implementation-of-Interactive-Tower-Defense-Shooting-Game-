using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public float m_WalkSpeed = 0.8f;
    public float m_RunSpeed = 2;
    public float m_DistanceJudge = 1f;
    public float m_HitDealyTime = 2f;

    /// <summary>
    /// 攻击时间间隔
    /// </summary>
    public float m_AttackInterval = 3f;
    public AudioClip audio_Attack;
    public AudioClip audio_Walk;

    private NavMeshAgent m_Agent;
    private Animator m_Anim;
    private Transform m_Target;
    /// <summary>
    /// 是否第一次攻击
    /// </summary>
    private bool m_IsFirstAttack = true;
    private float m_Timer = 0.0f;
    /// <summary>
    /// 是否正在攻击
    /// </summary>
    private bool m_IsAttacking = false;
    /// <summary>
    /// 是否正在受伤中
    /// </summary>
    private bool m_IsHitting = false;
    /// <summary>
    /// 僵尸是否死亡
    /// </summary>
    private bool m_IsDeath = false;
    public bool IsDeath
    {
        get
        {
            return m_IsDeath;
        }
    }
    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_Anim = GetComponent<Animator>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_Target = Camera.main.transform;
        EventCenter.AddListener<Vector3>(EventDefine.BombBrust, BombBrust);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<Vector3>(EventDefine.BombBrust, BombBrust);
    }
    private void Start()
    {
        RandomWalkOrRun();
    }
    private void FixedUpdate()
    {
        if (m_IsDeath) return;
        if (m_IsHitting) return;

        Vector3 tempTargetPos = new Vector3(m_Target.position.x, transform.position.y, m_Target.position.z);

        if (Vector3.Distance(transform.position, tempTargetPos) < m_DistanceJudge)
        {
            if (m_Agent.isStopped == false)
            {
                m_Agent.isStopped = true;
            }

            if (m_IsAttacking == false)
            {
                m_Timer += Time.deltaTime;
                if (m_Timer >= m_AttackInterval)
                {
                    m_Timer = 0.0f;
                    m_IsAttacking = true;
                    Attack();
                }
            }
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("AttackBlendTree") == false)
            {
                if (m_IsFirstAttack)
                {
                    m_DistanceJudge += 0.5f;
                    m_IsFirstAttack = false;
                    m_IsAttacking = true;
                    Attack();
                }
                else
                {
                    m_IsAttacking = false;
                }
            }
        }
        else
        {
            if (m_Agent.isStopped)
            {
                m_DistanceJudge -= 0.5f;
                m_Agent.isStopped = false;
                m_IsFirstAttack = true;
                RandomWalkOrRun();
            }
            m_Agent.SetDestination(Camera.main.transform.position);
        }
    }
    /// <summary>
    /// 炸弹爆炸
    /// </summary>
    /// <param name="brustPos"></param>
    private void BombBrust(Vector3 brustPos)
    {
        if (Vector3.Distance(transform.position, brustPos) < 10.0f)
        {
            BodyPartDrop[] drops = transform.GetComponentsInChildren<BodyPartDrop>();
            foreach (var item in drops)
            {
                item.Hit();
            }
            Death();
        }
    }
    /// <summary>
    /// 死亡
    /// </summary>
    public void Death()
    {
        if (m_IsDeath) return;
        PlayAnim(4, "Death", "DeathValue");
        m_Agent.isStopped = true;
        m_IsDeath = true;
        Destroy(m_Agent);
        EventCenter.Broadcast(EventDefine.ZombieDeath);
    }
    /// <summary>
    /// 左边受伤
    /// </summary>
    public void HitLeft()
    {
        m_Anim.SetTrigger("HitLeft");
        m_Agent.isStopped = true;
        m_Anim.SetTrigger("Idle");
        m_IsHitting = true;
        StartCoroutine(HitDealy());
    }
    /// <summary>
    /// 右边受伤
    /// </summary>
    public void HitRight()
    {
        m_Anim.SetTrigger("HitRight");
        m_Agent.isStopped = true;
        m_Anim.SetTrigger("Idle");
        m_IsHitting = true;
        StartCoroutine(HitDealy());
    }
    /// <summary>
    /// 随机受伤动画
    /// </summary>
    public void Hit()
    {
        PlayAnim(3, "Hit", "HitValue");
        m_Agent.isStopped = true;
        m_Anim.SetTrigger("Idle");
        m_IsHitting = true;
        StartCoroutine(HitDealy());
    }
    IEnumerator HitDealy()
    {
        yield return new WaitForSeconds(m_HitDealyTime);
        m_Agent.isStopped = false;
        m_IsHitting = false;
        RandomWalkOrRun();
    }
    /// <summary>
    /// 攻击
    /// </summary>
    private void Attack()
    {
        if (m_AudioSource.isPlaying == false)
        {
            m_AudioSource.clip = audio_Attack;
            m_AudioSource.Play();
        }
        EventCenter.Broadcast(EventDefine.UpdateHP, -10);
        EventCenter.Broadcast(EventDefine.ScreenBlood);
        Vector3 targetPos = new Vector3(m_Target.position.x, transform.position.y, m_Target.position.z);
        transform.LookAt(targetPos);

        PlayAnim(6, "Attack", "AttackValue");
    }
    /// <summary>
    /// 随机播放跑或者走的动画
    /// </summary>
    private void RandomWalkOrRun()
    {
        int ran = Random.Range(0, 2);
        if (ran == 0)
        {
            //走
            WalkAnim();
            m_Agent.speed = m_WalkSpeed;
        }
        else
        {
            //跑
            RunAnim();
            m_Agent.speed = m_RunSpeed;
        }
    }
    /// <summary>
    /// 走路动画
    /// </summary>
    private void WalkAnim()
    {
        if (m_AudioSource.isPlaying == false)
        {
            m_AudioSource.clip = audio_Walk;
            m_AudioSource.Play();
        }
        PlayAnim(3, "Walk", "WalkValue");
    }
    /// <summary>
    /// 跑的动画
    /// </summary>
    private void RunAnim()
    {
        PlayAnim(2, "Run", "RunValue");
    }
    private void PlayAnim(int clipCount, string triggerName, string floatName)
    {
        float rate = 1.0f / (clipCount - 1);
        m_Anim.SetTrigger(triggerName);
        m_Anim.SetFloat(floatName, rate * Random.Range(0, clipCount));
    }
}
