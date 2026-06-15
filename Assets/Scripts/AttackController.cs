using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Camera mainCamera;  // 主摄像机

    void Update()
    {
      
#if UNITY_STANDALONE || UNITY_EDITOR
        // 电脑端控制（鼠标点击）
        HandlePCAttack();
#elif UNITY_IOS || UNITY_ANDROID
            // 移动端控制（触摸输入）
            HandleMobileAttack();
#endif
    }

    // 电脑端使用鼠标点击进行攻击
    void HandlePCAttack()
    {
        if (Input.GetMouseButtonDown(0))  // 鼠标左键点击
        {
         
            // 获取鼠标点击的位置
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);


            // 如果点击的位置有物体（比如敌人）可以攻击
            if (hit.collider != null&&Physics2D.Raycast(ray.origin, ray.direction))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Attack(hit.collider.gameObject);
                }
            }
           
        }
    }

    // 移动端使用触摸进行攻击
    void HandleMobileAttack()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);  // 获取第一个触摸点

            if (touch.phase == TouchPhase.Began)
            {
                // 将触摸位置转换为世界坐标
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;

                // 如果点击的位置有物体（比如敌人）可以攻击
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        Attack(hit.collider.gameObject);
                    }
                }
            }
        }
    }

    // 攻击逻辑
    void Attack(GameObject enemy)
    {
        // 在此实现攻击敌人的逻辑
        Debug.Log("攻击了敌人: " + enemy.name);
        Destroy(enemy);
    }
}
