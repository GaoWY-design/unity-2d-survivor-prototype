using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Joystick Settings")]
    public DynamicJoystick joystick;
    public RectTransform joystickBackground;

    [Header("Movement Settings")]
    public float moveSpeed = 20f;
    private Vector3 movement;
    private Rigidbody2D rb;

    // 记录最后一次朝向，防止原地打滚
    private Vector2 lastMoveDirection = Vector2.right;

    [Header("Boundary Settings")]
    public bool useBoundary = true;
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    [Header("Roll Settings")]
    // 🔥 老师修改：速度加到 80！
    public float rollSpeed = 80f;
    // 🔥 老师修改：时间加到 0.4秒，这样能滑行得更久、更远
    public float rollDuration = 0.4f;
    public float rollCooldown = 1f;
    private bool isRolling = false;
    private float rollCooldownTimer = 0f;

    [Header("Joystick Roll Detection")]
    public float rollThreshold = 0.8f;
    public float rollInputSpeed = 2f;
    private Vector2 lastJoystickInput = Vector2.zero;
    private float lastInputTime = 0f;

    [Header("Double Tap Roll")]
    public bool enableDoubleTap = true;
    public float doubleTapTime = 0.3f;
    private float lastTapTime = -1f;

    [Header("Skill Select Settings")]
    public GameObject skillSelectPanel;
    public float longPressTime = 0.5f;
    public Vector3 skillPanelOffset = Vector3.zero;

    private bool isPressingJoystick = false;
    private float joystickPressStartTime = 0f;
    private bool skillPanelShown = false;

    [Header("Health Settings")]
    public float health = 100f;
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthSlider;

    [Header("Animation Settings")]
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (skillSelectPanel != null) skillSelectPanel.SetActive(false);
    }

    void Update()
    {
        if (rollCooldownTimer > 0) rollCooldownTimer -= Time.deltaTime;

        if (isRolling) return;

        HandleMovement();
        HandleJoystickLongPress();
        HandleJoystickRollDetection();

        if (Input.GetKeyDown(KeyCode.LeftShift)) StartRoll();
    }

    void HandleMovement()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

        if (joystick != null)
        {
            if (Mathf.Abs(joystick.Horizontal) > 0.1f || Mathf.Abs(joystick.Vertical) > 0.1f)
            {
                moveX = joystick.Horizontal;
                moveY = joystick.Vertical;
            }
        }

        movement = new Vector3(moveX, moveY, 0f).normalized;

        if (movement.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = movement; // 记录方向

            if (rb != null)
            {
                Vector2 targetPos = rb.position + (Vector2)movement * moveSpeed * Time.deltaTime;
                rb.MovePosition(targetPos);
            }
            else
            {
                transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
            }

            if (animator != null) animator.SetBool("IsRun", true);
        }
        else
        {
            if (rb != null) rb.velocity = Vector2.zero;
            if (animator != null) animator.SetBool("IsRun", false);
        }

        ClampPositionToBoundary();
    }

    void ClampPositionToBoundary()
    {
        if (!useBoundary) return;
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    void HandleJoystickLongPress()
    {
        if (joystick == null) return;
        Vector2 currentInput = new Vector2(joystick.Horizontal, joystick.Vertical);

        if (currentInput.magnitude > 0.1f)
        {
            if (!isPressingJoystick)
            {
                isPressingJoystick = true;
                joystickPressStartTime = Time.time;
                skillPanelShown = false;
            }
            else
            {
                float pressDuration = Time.time - joystickPressStartTime;
                if (pressDuration >= longPressTime && !skillPanelShown)
                {
                    ShowSkillSelectPanel();
                    skillPanelShown = true;
                }
                if (skillPanelShown) UpdatePanelPosition();
            }
        }
        else
        {
            isPressingJoystick = false;
            joystickPressStartTime = 0f;
        }
    }

    void ShowSkillSelectPanel()
    {
        if (skillSelectPanel != null)
        {
            skillSelectPanel.SetActive(true);
            UpdatePanelPosition();
        }
    }

    void UpdatePanelPosition()
    {
        if (skillSelectPanel == null) return;
        Vector3 targetPos = joystickBackground != null ? joystickBackground.position : Input.mousePosition;
        skillSelectPanel.transform.position = targetPos + skillPanelOffset;
    }

    public void HideSkillSelectPanel()
    {
        if (skillSelectPanel != null) skillSelectPanel.SetActive(false);
    }

    void HandleJoystickRollDetection()
    {
        if (joystick == null) return;
        Vector2 currentInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        float currentMagnitude = currentInput.magnitude;

        if (currentMagnitude >= rollThreshold)
        {
            float deltaTime = Time.time - lastInputTime;
            if (deltaTime > 0)
            {
                Vector2 inputDelta = currentInput - lastJoystickInput;
                float inputSpeed = inputDelta.magnitude / deltaTime;
                if (inputSpeed >= rollInputSpeed && !isRolling && rollCooldownTimer <= 0)
                {
                    Vector3 rollDir = new Vector3(currentInput.x, currentInput.y, 0).normalized;
                    StartCoroutine(RollRoutineCustom(rollDir));
                }
            }
        }

        if (enableDoubleTap)
        {
            if (lastJoystickInput.magnitude < 0.1f && currentMagnitude > 0.1f)
            {
                float timeSinceLastTap = Time.time - lastTapTime;
                if (timeSinceLastTap <= doubleTapTime)
                {
                    if (!isRolling && rollCooldownTimer <= 0)
                    {
                        StartRoll();
                    }
                    lastTapTime = -1f;
                }
                else lastTapTime = Time.time;
            }
        }
        lastJoystickInput = currentInput;
        lastInputTime = Time.time;
    }

    public void StartRoll()
    {
        if (!isRolling && rollCooldownTimer <= 0)
        {
            // 如果有输入，用输入方向；否则用最后一次的朝向
            Vector3 finalDirection = movement.sqrMagnitude > 0.01f ? movement : (Vector3)lastMoveDirection;
            finalDirection.Normalize();
            StartCoroutine(RollRoutineCustom(finalDirection));
        }
    }

    public void StartRollWithDir(Vector3 dir)
    {
        if (!isRolling && rollCooldownTimer <= 0)
            StartCoroutine(RollRoutineCustom(dir));
    }

    IEnumerator RollRoutineCustom(Vector3 rollDir)
    {
        isRolling = true;
        rollCooldownTimer = rollCooldown;

        if (animator != null) animator.SetTrigger("Roll");

        if (rollDir == Vector3.zero) rollDir = Vector3.right;
        rollDir.Normalize();

        float startTime = Time.time;
        while (Time.time < startTime + rollDuration)
        {
            // 移动位置
            if (rb != null)
            {
                Vector2 targetPos = rb.position + (Vector2)rollDir * rollSpeed * Time.deltaTime;
                rb.MovePosition(targetPos);
            }
            else
            {
                transform.Translate(rollDir * rollSpeed * Time.deltaTime, Space.World);
            }

            ClampPositionToBoundary();
            yield return null;
        }

        isRolling = false;
    }

    public void TakeDamage(float damage)
    {
        if (isRolling) return;
        health -= damage;
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        if (healthSlider != null) healthSlider.value = currentHealth;
        if (health <= 0f) Die();
    }

    void Die()
    {
        if (healthSlider != null) healthSlider.value = 0f;
        Destroy(gameObject);
        SceneManager.LoadScene("MainScene");
    }
}