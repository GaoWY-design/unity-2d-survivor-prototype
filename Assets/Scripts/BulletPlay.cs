using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPlay : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f; // 3秒后自动销毁

    void Start()
    {
        // 确保子弹生成后 3 秒自动消失
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // 让子弹朝自己的右方（即前方）飞行
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    // 当子弹进入触发区域时调用
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果碰到的物体标签是 "Enemy"
        if (collision.CompareTag("Enemy"))
        {
            // 在这里处理敌人受伤逻辑，例如：
             collision.GetComponent<Enemy>().TakeDamage(50);

            Debug.Log("击中敌人！");

            // 销毁子弹
            Destroy(gameObject);
        }

        //// 可选：碰到墙壁等静态物体也销毁
        //if (collision.CompareTag("Wall"))
        //{
        //    Destroy(gameObject);
        //}
    }
}
