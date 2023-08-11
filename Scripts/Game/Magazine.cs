using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine : MonoBehaviour
{
    /// <summary>
    /// 子弹数量
    /// </summary>
    private int BulletCount = 6;

    public void SetBulletCount(int count)
    {
        BulletCount = count;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Backet")
        {
            if (transform.parent != null && transform.parent.GetComponentInChildren<HandManger>() != null)
            {
                Destroy(gameObject);
                transform.parent.GetComponentInChildren<HandManger>().Catch(false);
                //加子弹
                AmmoManager.Instance.UpdateBullet(BulletCount);
            }
        }
    }
}
