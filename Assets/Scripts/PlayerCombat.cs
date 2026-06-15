using System.Collections.Generic; // 必须引用这个，用于列表操作
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // 引用UI

public class PlayerCombat : MonoBehaviour
{
    // 枚举定义
    public enum SkillMode { Range, Melee }

    [Header("Skill Mode")]
    public SkillMode currentMode = SkillMode.Range;

    [Header("Range Attack Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public int bulletCount = 4;
    public float spreadAngle = 15f;

    [Header("Range Skill (360 Circle) Settings")]
    public GameObject circleBulletPrefab;
    public int circleBulletCount = 12;
    public float circleBulletSpeed = 8f;

    [Header("Melee Attack Settings")]
    public GameObject meleeAttackPrefab;
    public float meleeCooldown = 1f;
    private float meleeTimer = 0f;

    public float meleeSpawnOffset = 1.0f;
    private Vector2 lastMoveDirection = Vector2.right;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        UpdateFacingDirection();

        if (meleeTimer > 0) meleeTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            // 🔥 使用修复后的UI检测方法
            if (IsPointerOverBlockingUI())
            {
                // Debug.Log("点击到了技能按钮，不攻击");
                return;
            }

            if (currentMode == SkillMode.Range)
            {
                FireShotgunBullets();
            }
            else if (currentMode == SkillMode.Melee)
            {
                ExecuteMeleeAttack();
            }
        }
    }

    void UpdateFacingDirection()
    {
        Vector2 input = Vector2.zero;
        if (playerMovement != null && playerMovement.joystick != null)
        {
            input = new Vector2(playerMovement.joystick.Horizontal, playerMovement.joystick.Vertical);
        }

        if (input.magnitude < 0.1f)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
        }

        if (input.magnitude > 0.1f)
        {
            lastMoveDirection = input.normalized;
        }
    }

    // 🔥 核心修改：智能判断点击的是不是“阻挡型UI” 🔥
    bool IsPointerOverBlockingUI()
    {
        // 1. 如果鼠标/手指没点任何UI，直接返回 false（允许攻击）
        if (EventSystem.current == null) return false;
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.touchCount > 0)
            {
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    return false;
            }
            else return false;
        }

        // 2. 发射一条射线，看看点到了哪些 UI
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        // 兼容鼠标和触摸
        if (Input.touchCount > 0) eventData.position = Input.GetTouch(0).position;
        else eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // 3. 遍历点到的所有东西
        foreach (RaycastResult result in results)
        {
            string objName = result.gameObject.name;

            // ⚠️ 关键逻辑：
            // 如果点到的东西名字包含 "Joystick" 或者 "Background" (摇杆背景)
            // 我们就认为它“不是”阻挡攻击的 UI，直接忽略它，继续检查下一个
            if (objName.Contains("Joystick") || objName.Contains("Background") || objName.Contains("Handle"))
            {
                continue;
            }

            // 如果点到了除了摇杆以外的其他UI（比如 Button, Image, Panel）
            // 那就是真的挡住了，返回 true（阻止攻击）
            return true;
        }

        // 如果循环结束了，只点到了摇杆或者什么都没点到，那就允许攻击
        return false;
    }

    void FireShotgunBullets()
    {
        if (bulletPrefab == null) return;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        Vector2 fireDirection = (mousePosition - transform.position).normalized;
        float baseAngle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;

        for (int i = 0; i < bulletCount; i++)
        {
            float offset = (i - (bulletCount - 1) / 2f) * spreadAngle;
            Quaternion rotation = Quaternion.Euler(0, 0, baseAngle + offset);
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);

            Bullet bScript = bullet.GetComponent<Bullet>();
            Vector2 dir = rotation * Vector2.right;

            if (bScript != null) { bScript.SetDirection(dir); bScript.speed = bulletSpeed; }
            else { Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>(); if (rb != null) rb.velocity = dir * bulletSpeed; }
        }
    }

    void ExecuteMeleeAttack()
    {
        if (meleeTimer > 0) return;
        if (meleeAttackPrefab == null) return;

        float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Vector3 spawnPos = transform.position + (Vector3)(lastMoveDirection * meleeSpawnOffset);

        GameObject effect = Instantiate(meleeAttackPrefab, spawnPos, rotation);
        if (effect != null) meleeTimer = meleeCooldown;
    }

    void ExecuteCircleAttack()
    {
        if (circleBulletPrefab == null) return;
        float angleStep = 360f / circleBulletCount;
        float currentAngle = 0f;
        for (int i = 0; i < circleBulletCount; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            GameObject bullet = Instantiate(circleBulletPrefab, transform.position, rotation);
            Vector2 dir = rotation * Vector2.right;
            Bullet bScript = bullet.GetComponent<Bullet>();
            if (bScript != null) { bScript.SetDirection(dir); bScript.speed = circleBulletSpeed; }
            else { Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>(); if (rb != null) rb.velocity = dir * circleBulletSpeed; }
            currentAngle += angleStep;
        }
    }

    public void Button_UseMeleeSkill()
    {
        currentMode = SkillMode.Range;
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null) pm.HideSkillSelectPanel();
        ExecuteMeleeAttack();
    }

    public void Button_UseRangeCircleSkill()
    {
        currentMode = SkillMode.Range;
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null) pm.HideSkillSelectPanel();
        ExecuteCircleAttack();
    }

    public void SwitchToRangeMode()
    {
        currentMode = SkillMode.Range;
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null) pm.HideSkillSelectPanel();
    }
}