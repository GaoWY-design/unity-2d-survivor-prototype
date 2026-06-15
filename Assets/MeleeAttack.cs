using System.Collections;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("攻击设置")]
    public float damage = 99999f;   // 伤害值（秒杀）
    public float duration = 0.3f;   // 持续时间（突刺通常比较快，建议改短一点，比如0.3）
    public float moveSpeed = 5f;    // 突刺时特效向前飞行的速度

    // 不需要旋转速度了
    // public float rotateSpeed = 180f; 

    private float timer = 0f;
    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;

        // 自动销毁
        Destroy(gameObject, duration);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 1. 【突刺动作】让特效沿着它自己的“前方”飞一小段距离
        // transform.right 在 2D 中通常代表物体的红色轴向（前方）
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        // 2. 【淡出效果】逐渐变透明
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(1f, 0f, timer / duration);
            sr.color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 只有碰到标签是 "Enemy" 的物体才生效
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                // 秒杀逻辑
                enemy.TakeDamage(damage);
                // Debug.Log("💀 突刺秒杀了敌人: " + collision.name);
            }
        }
    }
}