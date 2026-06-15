using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // 子弹的速度
    public float damage = 100f; // 子弹的伤害
    public float lifetime = 3f; // 子弹的生命周期
    private Vector2 direction;
    Rigidbody2D rb;
    private void Start()
    {
        // 在一定时间后销毁子弹
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
        rb.velocity = direction * speed;

        // 可选：视觉上让子弹朝向运动方向（根据 sprite 朝向做偏移）
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    void Update()
    {
        // 保证速度恒定
        rb.velocity = direction * speed;
    }
    // 设置子弹的方向
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查子弹是否与敌人发生碰撞
        if (other.CompareTag("Player"))
        {
            // 处理敌人受伤逻辑（可以用伤害、减血等）
            PlayerMovement pw= GameManager.Instance.player .GetComponent<PlayerMovement>();
            if (pw != null)
            {
                pw.TakeDamage(damage); // 假设敌人有 TakeDamage 方法
            }
            
            // 销毁子弹
            Destroy(gameObject);
        }
        if (other.transform.tag == "12")
        {


            // 销毁子弹
            Destroy(gameObject);
        }
    }

   
}
