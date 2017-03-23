using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    #region variables
    public static Boat player;
    public float hSpeed = 5;
    [SerializeField]
    float uprightConstant = 1.0f;
    bool aiming;
    [SerializeField]
    float missileForce = 500;
    BoxCollider2D boxCollider;
    CircleCollider2D[] circleColliders;
    public bool moonOut = false;

    float power;

    public GameObject moonPrefab;

    //bool thrown;
    bool dead;

    Rigidbody2D rb2d;
    int health;
    int maxHealth;

    Animator anim;

    List<Collider2D> colliders;

    bool tutorialMode;
    bool finishedLevel;

    float nextDamageTime;
    public float invulTime;

    float extraSpeed;

    [SerializeField]
    CircleCollider2D coinMagnet;
    #endregion

    void Awake()
    {
        player = this;
        dead = false;
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        colliders = new List<Collider2D>();
        colliders.Add(GetComponent<BoxCollider2D>());
        colliders.Add(GetComponent<PolygonCollider2D>());
    }

    void Start()
    {
        maxHealth = health = 1;
        MainCanvas.controller.HealthChange();
    }

    void Update()
    {
        if (dead || UpgradeController.controller.CheckIfUpgrading())
            return;

        float horizontal = (Input.GetAxis("Horizontal") * Time.deltaTime) + extraSpeed;

        if (Input.GetMouseButtonDown(0) && !moonOut)
        {
            if (CheckIfAllowed(TutorialController.TutorialStage.SPAWN_MOON))
                CreateMoon();
        }

        Movement(horizontal);
        CheckBoundaries();
    }

    #region Movement Checks
    void CheckBoundaries()
    {
        //Vector3 screenCoor = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        Vector3 leftSide = Camera.main.ViewportToWorldPoint(Vector2.zero);
        Vector3 rightSide = Camera.main.ViewportToWorldPoint(Vector2.right);

        if (!finishedLevel && transform.position.x > rightSide.x - 1)
        {
            transform.position = new Vector3(rightSide.x - 1, transform.position.y);
        }
        if (transform.position.x < leftSide.x + 1)
        {
            transform.position = new Vector3(leftSide.x + 1, transform.position.y);
        }

        if (finishedLevel && transform.position.x > rightSide.x - 1)
        {
            MainCanvas.controller.FinishLevel();
        }
    }

    void FixedUpdate()
    {
        SelfRight();
    }

    void SelfRight()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0)), Time.deltaTime * uprightConstant);
    }
    #endregion

    #region Input
    void Movement(float h)
    {
        if (h == 0)
            return;

        Vector3 vec = Vector2.MoveTowards(transform.position, transform.position + Vector3.right * hSpeed * h, hSpeed + extraSpeed);
        vec.z = -10;

        transform.position = vec;


        if (tutorialMode && TutorialController.controller.CheckIfOnStage(TutorialController.TutorialStage.MOVEMENT))
            TutorialController.controller.SetStage(TutorialController.TutorialStage.SPAWN_MOON);
    }

    void CreateMoon()
    {
        moonOut = true;
        GameObject moon = Instantiate(moonPrefab, transform.position, Quaternion.identity) as GameObject;
        moon.GetComponent<Moon>().GiveVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        AudioController.controller.WaterRise();

        if (tutorialMode && TutorialController.controller.CheckIfOnStage(TutorialController.TutorialStage.SPAWN_MOON))
            TutorialController.controller.SetStage(TutorialController.TutorialStage.RETRACT_MOON);
    }
    #endregion

    #region Collisions/Triggers

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PlatformProjectile"))
        {
            TakeDamage();
            Destroy(other.gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            //AudioController.controller.Gargle();
        }

        if (other.gameObject.CompareTag("Pillar"))
        {
            TakeDamage();
            Destroy(other.transform.parent.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            AudioController.controller.StopGargling();
        }
    }
    #endregion

    #region Health Scripts
    public void TakeMissileDamage()
    {
        if (Time.time < nextDamageTime)
            return;
        rb2d.AddForce(Vector2.left * missileForce);
        TakeDamage();
    }

    public void TakeDamage()
    {
        if (Time.time < nextDamageTime)
            return;

        nextDamageTime = Time.time + invulTime;
        health--;
        MainCanvas.controller.HealthChange();
        anim.SetTrigger("hit");
        if (health == 0)
            Die();

    }

    void UpdateColliders()
    {
        if (health < 1)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                colliders[i].enabled = false;
            }
        }
    }

    public void Die()
    {
        health = 0;
        MainCanvas.controller.HealthChange();
        dead = true;
        UpdateColliders();
        MainCanvas.controller.DeathScreen();
        AudioController.controller.BoatDeath();
        TempGoalController.controller.PlayerDied();
        DeathCounter.controller.PlayerDeath();
        PlayerInfo.controller.Save();
    }

    public void AddHealth()
    {
        health = Mathf.Min(health + 1, maxHealth);
        MainCanvas.controller.HealthChange();
        AudioController.controller.PlayRepairBoat();
    }

    public void UpdateBoatSpeed(int value)
    {
        hSpeed = 5 + value * .75f;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void UpdateMaxHealth(int value)
    {
        maxHealth = value + 1;
        health = maxHealth;
        MainCanvas.controller.HealthChange();
    }

    public void UpdateMagnetSize(int value)
    {
        if(value == 0)
            coinMagnet.enabled = false;
        else coinMagnet.enabled = true;
        coinMagnet.radius = 2 + .25f * value;
    }

    public void UpdateInvulTime(float time)
    {
        invulTime = .3f + time * .15f;
    }

    public int GetHealth()
    {
        return health;
    }
    #endregion

    #region Tutorial Functions

    public bool CheckTutorialMode()
    {
        return tutorialMode;
    }

    public void SetTutorialMode(bool b)
    {
        tutorialMode = b;
    }

    public bool CheckIfAlive()
    {
        if (!dead)
            return true;

        return false;
    }

    bool CheckIfAllowed(TutorialController.TutorialStage t)
    {
        if ((tutorialMode && TutorialController.controller.CheckIfOnStage(t)) || !tutorialMode)
            return true;

        return false;
    }
    #endregion

    public void SailOffScreen()
    {
        if (GameModeController.controller.CheckCurrentMode(GameModeController.Mode.Endless))
            return;

        extraSpeed = 1.5f * Time.deltaTime;
        finishedLevel = true;
    }
}
