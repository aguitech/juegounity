using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float laneDistance = 3f;
    public float laneChangeSpeed = 12f;
    public float jumpForce = 10f;
    public float gravity = 25f;

    [Header("State")]
    public int currentLane = 1; // 0=izquierda, 1=centro, 2=derecha
    public bool isJumping = false;
    public bool isGrounded = true;
    public bool isSliding = false;
    public bool isDead = false;

    [Header("References")]
    public Rigidbody rb;
    public GameObject slideCollider;
    public GameManager gm;
    public TrailRenderer trail;
    public ParticleSystem speedParticles;

    private Vector3 targetPosition;
    private float verticalVelocity = 0f;
    private Animator animator;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
    }

    void Update()
    {
        if (gm.currentState != GameManager.GameState.Playing) return;

        // Input handling
        HandleInput();

        // Movimiento lateral suave
        Vector3 newPos = transform.position;
        newPos.x = Mathf.MoveTowards(newPos.x, targetPosition.x, laneChangeSpeed * Time.deltaTime);

        // Vertical (salto)
        if (isJumping)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            newPos.y += verticalVelocity * Time.deltaTime;

            if (newPos.y <= 1f)
            {
                newPos.y = 1f;
                isJumping = false;
                isGrounded = true;
                verticalVelocity = 0;
                if (animator != null) animator.SetBool("isJumping", false);
            }
        }

        transform.position = newPos;

        // Animaciones
        if (animator != null)
        {
            animator.SetBool("isRunning", gm.currentState == GameManager.GameState.Playing);
            animator.SetBool("isSliding", isSliding);
            animator.SetBool("isDead", isDead);
        }
    }

    void HandleInput()
    {
        // Teclado (testing desktop)
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            ChangeLane(1);
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            Jump();
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            StartSlide();
        #endif

        // Touch (mobile)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                float touchX = touch.position.x;
                float touchY = touch.position.y;
                float screenMid = Screen.width / 2f;
                float screenLow = Screen.height * 0.3f;

                if (touchY < screenLow)
                {
                    StartSlide();
                }
                else if (touchX < screenMid * 0.5f)
                {
                    ChangeLane(-1);
                }
                else if (touchX > screenMid * 1.5f)
                {
                    ChangeLane(1);
                }
                else
                {
                    Jump();
                }
            }
        }
    }

    public void ChangeLane(int direction)
    {
        if (isSliding) return;
        currentLane = Mathf.Clamp(currentLane + direction, 0, 2);
        float xPos = (currentLane - 1) * laneDistance;
        targetPosition = new Vector3(xPos, transform.position.y, transform.position.z);

        if (animator != null) animator.SetTrigger("dodge");
    }

    public void Jump()
    {
        if (!isGrounded || isJumping || isSliding) return;
        isJumping = true;
        isGrounded = false;
        verticalVelocity = jumpForce;
        if (animator != null) animator.SetBool("isJumping", true);
    }

    public void StartSlide()
    {
        if (isJumping || isSliding || !isGrounded) return;
        isSliding = true;
        if (slideCollider != null) slideCollider.SetActive(true);
        Invoke(nameof(StopSlide), 0.6f);
        if (animator != null) animator.SetTrigger("slide");
    }

    public void StopSlide()
    {
        isSliding = false;
        if (slideCollider != null) slideCollider.SetActive(false);
    }

    public void StartRunning()
    {
        isDead = false;
        if (trail != null) trail.emitting = true;
        if (speedParticles != null) speedParticles.Play();
    }

    public void ResetPlayer()
    {
        transform.position = new Vector3(0, 1f, 0);
        targetPosition = transform.position;
        currentLane = 1;
        isJumping = false;
        isGrounded = true;
        isSliding = false;
        isDead = false;
        verticalVelocity = 0;
        if (slideCollider != null) slideCollider.SetActive(false);
    }

    public void Die()
    {
        isDead = true;
        if (trail != null) trail.emitting = false;
        if (speedParticles != null) speedParticles.Stop();
        if (animator != null) animator.SetBool("isDead", true);

        // Volar hacia atrás con efecto dramático
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            rb.AddForce(Vector3.back * 3f, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && !isDead)
        {
            if (gm != null) gm.GameOver();
            Die();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Orb"))
        {
            if (gm != null) gm.AddScore(50);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Obstacle") && !isDead)
        {
            if (gm != null) gm.GameOver();
            Die();
        }
    }
}