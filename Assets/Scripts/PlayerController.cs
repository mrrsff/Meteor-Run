using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    GameObject prefab, player;
    Rigidbody rb;
    [SerializeField] float speed, jumpForce, shield;
    bool isGrounded;
    Animator anim;
    float stamina, hp;
    [SerializeField] Slider staminaBar, hpBar;
    [SerializeField] float healthRegenRate, staminaRegenRate, healthRegenTime, staminaRegenTime, regenSpeed;
    float staminaRegenMultiplier = 1, speedMultiplier = 1,healthRegenMultiplier = 1;
    public float score;
    public static event System.Action<float> OnScoreChange;
    public static event System.Action OnPlayerDeath;
    [SerializeField] public List<GameObject> prefabs;
    Coroutine powerUpHPRegen, powerUpSpeed, powerUpStaminaRegen;
    void Start() {
        stamina = 100;
        hp = 100;
        if(anim = GetComponentInChildren<Animator>()) {
            Destroy(anim.gameObject);
        }
        var count = prefabs.Count;
        var index = Random.Range(0, count);
        prefab = prefabs[index];
        player = Instantiate(prefab, transform.position, transform.rotation);
        player.transform.SetParent(transform);
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(RegenStamina());
        StartCoroutine(RegenHealth());
        MeteroiteController.OnWaveChange += OnWaveChange;
        PowerUpController.OnPowerUpCollected += OnPowerUpCollected;
    }
    private void OnPowerUpCollected(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.HPRegen:
                if(powerUpHPRegen != null) StopCoroutine(powerUpHPRegen);
                powerUpHPRegen = StartCoroutine(PowerUpHPRegen());
                break;
            case PowerUpType.Shield:
                shield += 20;
                break;
            case PowerUpType.Speed:
                if(powerUpSpeed != null) StopCoroutine(powerUpSpeed);
                powerUpSpeed = StartCoroutine(PowerUpSpeed());
                break;
            case PowerUpType.StaminaRegen:
                if(powerUpStaminaRegen != null) StopCoroutine(powerUpStaminaRegen);
                powerUpStaminaRegen = StartCoroutine(PowerUpStaminaRegen());
                break;
            default:
                break;
        }
    }
    IEnumerator PowerUpHPRegen()
    {
        healthRegenMultiplier *= 2;
        yield return new WaitForSeconds(10f);
        healthRegenMultiplier /= 2;
    }
    IEnumerator PowerUpSpeed()
    {
        speedMultiplier *= 2;
        yield return new WaitForSeconds(10f);
        speedMultiplier /= 2;
    }
    IEnumerator PowerUpStaminaRegen()
    {
        staminaRegenMultiplier *= 2;
        yield return new WaitForSeconds(10f);
        staminaRegenMultiplier /= 2;
    }

    private void OnWaveChange(int wave)
    {
        speed += 0.5f;
        healthRegenRate += 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if(hp <= 0) return;

        if(anim == null) anim = GetComponentInChildren<Animator>();

        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");
    
        if (x != 0 || y != 0) {
            // move the player with physics
            if(Input.GetKey(KeyCode.LeftShift) && stamina > 4 * Time.deltaTime)
            {
                anim.SetBool("isRunning", true);
                anim.SetBool("isWalking", false);
                stamina -= 4 * Time.deltaTime;
                rb.MovePosition(transform.position + new Vector3(x, 0, y) * speed * 3 * Time.deltaTime * speedMultiplier);
            }
            else
            {
                anim.SetBool("isRunning", false);
                anim.SetBool("isWalking", true);
                rb.MovePosition(transform.position + new Vector3(x, 0, y) * speed * Time.deltaTime * speedMultiplier);
            }
            var rot = transform.rotation;
            var target = Quaternion.LookRotation(new Vector3(x, 0, y));
            transform.rotation = Quaternion.Slerp(rot, target, Time.deltaTime * 10);
        }
        else{
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            rb.AddForce(Vector3.up * jumpForce * speedMultiplier, ForceMode.Impulse);
            stamina -= 10;
            anim.SetTrigger("jump");
        }
        staminaBar.value = stamina;
        hpBar.value = hp;
    }
    public bool IsGrounded() => isGrounded;
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Ground")) {
            isGrounded = true;
            anim.SetBool("isGrounded", true);
        }
    }
    private void OnCollisionExit(Collision other) {
        if (other.gameObject.CompareTag("Ground")) {
            isGrounded = false;
            anim.SetBool("isGrounded", false);
        }
    }
    IEnumerator RegenStamina() {
        while (true) {
            if(anim.GetBool("isRunning") == false)
            {
                if(stamina < 10)
                {
                    yield return new WaitForSeconds(staminaRegenTime * 3f / regenSpeed);
                    stamina = 10;
                }
                if(stamina < 100) stamina = Mathf.Min((stamina + (staminaRegenRate * staminaRegenMultiplier / regenSpeed)), 100);
            }
            yield return new WaitForSeconds(staminaRegenTime / regenSpeed);
        }
    }
    IEnumerator RegenHealth(){
        while (true)
        {
            if (hp < 100) hp = Mathf.Min((hp + (healthRegenRate * healthRegenMultiplier / regenSpeed)) , 100);
            yield return new WaitForSeconds(healthRegenTime / regenSpeed);
        }
    }
    public void TakeDamage(float damage)
    {
        shield -= damage;
        if(shield <= 0)
        {
            hp += shield;
            shield = 0;
        }
        hp -= damage;
        if(hp <= 0)
        {
            OnPlayerDeath?.Invoke();
            MeteroiteController.OnWaveChange -= OnWaveChange;
            PowerUpController.OnPowerUpCollected -= OnPowerUpCollected;
            speed = 0;
            hp = 0;
            hpBar.value = hp;
            var random = Random.Range(0, 2);
            anim.SetInteger("dieType", random);
            anim.SetTrigger("die");
            StopAllCoroutines();
        }
    }
    public void AddScore(float score)
    {
        this.score += score / 5;
        OnScoreChange?.Invoke(this.score);
    }
}
