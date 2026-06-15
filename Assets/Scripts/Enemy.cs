using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3f;  // 敌人移动速度
    public Transform player;      // 主角的 Transform
    private Vector3 targetPosition;  // 目标位置（主角的位置）
    public float health = 100f;

    public float fireRate = 10f; // 射击间隔
    private float nextFireTime = 0f;

    public Transform firePoint; // 发射点

    public GameObject coin;
    void Awake()
    {
        // 如果没有指定主角的位置，尝试找到主角对象
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
    }
    private void Start()
    {
        firePoint=transform.GetChild(1);
    }

    void Update()
    {
        // 获取主角的位置
        if (player!=null)
        {
            targetPosition = player.position;
        }


        // 计算敌人和主角之间的方向
        Vector3 direction = (targetPosition - transform.position).normalized;

        // 计算敌人与主角的左右关系
        Vector3 toPlayer = targetPosition - transform.position;  // 从敌人到主角的向量
        Vector3 right = player.right;  // 主角的右边方向（在2D中，通常使用transform.right）

        // 计算叉积，判断左右
        float crossProduct = Vector3.Cross(toPlayer, right).z;

        if (crossProduct > 0)
        {
            // 敌人在主角的左边
            transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (crossProduct < 0)
        {
            // 敌人在主角的右边
            transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            // 敌人正前方或背后（与主角在同一条直线）
            Debug.Log("Enemy is directly in front or behind the player.");
        }
        // 计算敌人和主角的距离
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance>1.5f)
        {
            // 移动敌人朝着主角的位置
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        }
        // 按下空格键发射子弹
        if (Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + fireRate; // 设置下一次发射的时间
        }

        void FireBullet()
        {
            // 实例化一个子弹并设置发射方向
            GameObject go = Instantiate(GameManager.Instance.goEnemyBullent, firePoint.position, firePoint.rotation);

            // 如果你想要将子弹朝着敌人发射，可以在子弹类中添加方向设置。
            var b = go.GetComponent<Bullet>();
            if (b != null)
            {
                Vector2 dir = (player.position - firePoint.position);
                b.SetDirection(dir.normalized);
            }
        }

    }
    // 处理敌人受到伤害
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    // 敌人死亡
    void Die()
    {
        if (coin != null)
        {
            GameObject c = Instantiate(coin, transform.position, Quaternion.identity);
            // 如果 coin 上有 Rigidbody2D，则给它一个随机的抛掷力
            Rigidbody2D rb = c.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 force = new Vector2(Random.Range(-0.8f, 0.8f), 1f).normalized * Random.Range(150f, 250f);
                rb.AddForce(force);
            }
        }

        // 例如播放死亡动画或销毁敌人
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag=="Player")
        {
            TakeDamage(120);
        }
    }
}

