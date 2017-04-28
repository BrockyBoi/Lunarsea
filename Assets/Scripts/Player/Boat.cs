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
    float missileForce = 750;
    BoxCollider2D boxCollider;
    CircleCollider2D[] circleColliders;
    public bool moonOut = false;

    public GameObject moonPrefab;

    bool dead;

    Rigidbody2D rb2d;
    int health;
    int maxHealth;

    Animator anim;

    List<Collider2D> colliders;

    bool tutorialMode;
    bool finishedLevel;

    float nextDamageTime;
    float invulTime = .3f;

    float extraSpeed;

    [SerializeField]
    CircleCollider2D coinMagnet;

    bool tookDamage;

    bool sailingIn;

    bool upgrading = true;

    GameObject moonItem;

    public delegate void OnFinishedSailingIn();
    public event OnFinishedSailingIn onFinishedSailingIn;

    public delegate void OnBoatDeath();
    public event OnBoatDeath onBoatDeath;
    #endregion

    void OnEnable()
    {
    }

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
        MillileSpawner.controller.onWavesCleared += SailOffScreen;

        TutorialController.controller.onFinishTutorial += FinishTutorial;

        SinkKill.controller.BoatDied += Die;
        Urdu.sideUrdu.BoatDied += Die;

        TutorialController.controller.onStartTutorial += StartTutoiral;

        UpgradeController.controller.notUpgrading += SailIn;

        if (PlayerInfo.controller.ResetSaveFile || PlayerInfo.controller.CheckIfFirstTime())
        {
            maxHealth = health = 1;
            MainCanvas.controller.HealthChange();
        }

        moonItem = Instantiate(moonPrefab, transform.position, Quaternion.identity) as GameObject;
        moonItem.SetActive(false);
    }

    void OnDisable()
    {
        MillileSpawner.controller.onWavesCleared -= SailOffScreen;

        TutorialController.controller.onFinishTutorial -= FinishTutorial;

        SinkKill.controller.BoatDied -= Die;
        Urdu.sideUrdu.BoatDied -= Die;

        TutorialController.controller.onStartTutorial -= StartTutoiral;

        UpgradeController.controller.notUpgrading -= SailIn;
    }

    void Update()
    {
        if (dead || upgrading)
            return;

#if UNITY_STANDALONE || UNITY_WEBPLAYER 
        float horizontal = Input.GetAxis("Horizontal");

#elif UNITY_IOS || UNITY_ANDROID
                float horizontal = Input.acceleration.x  * 3;
                if(horizontal > Time.deltaTime)
                    horizontal = Time.deltaTime;
                if (horizontal < -Time.deltaTime)
                    horizontal = -Time.deltaTime;
#endif

        if (sailingIn)
        {
            horizontal = 0;
        }


        if (tutorialMode && TutorialController.controller.CheckIfOnStage(TutorialController.TutorialStage.MOVEMENT) && horizontal != 0)
            TutorialController.controller.SetStage(TutorialController.TutorialStage.SPAWN_MOON);

        if (Input.GetMouseButtonDown(0) && !moonOut && !sailingIn)
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
        float midPoint = Camera.main.ViewportToWorldPoint(new Vector2(.5f, .5f)).x;

        if (!finishedLevel && transform.position.x > rightSide.x - 1)
        {
            transform.position = new Vector3(rightSide.x - 1, transform.position.y);
        }
        if (!sailingIn && transform.position.x < leftSide.x + 1)
        {
            transform.position = new Vector3(leftSide.x + 1, transform.position.y);
        }

        if (finishedLevel && transform.position.x > rightSide.x - 1)
        {
            MainCanvas.controller.FinishLevel();
        }

        if (extraSpeed > 0 && !finishedLevel && Mathf.Abs(transform.position.x - midPoint) <= .5f)
        {
            FinishedSailingIn();
        }
    }

    void FixedUpdate()
    {
       // SelfRight();
    }

    void SelfRight()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0)), Time.deltaTime * uprightConstant);
    }
    #endregion

    #region Input
    void Movement(float h)
    {
        if ((h == 0 && extraSpeed == 0))
            return;

        float speed = ((hSpeed * h) + extraSpeed) * Time.deltaTime;

        Vector3 vec = Vector2.MoveTowards(transform.position, transform.position + Vector3.right * speed, 10 * Time.deltaTime);
        vec.z = -10;

        transform.position = vec;
    }

    void CreateMoon()
    {
        moonOut = true;
        moonItem.transform.position = transform.position;
        moonItem.SetActive(true);
        moonItem.GetComponent<Moon>().GiveVector(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy Boat"))
        {
            TakeDamage();
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

        tookDamage = true;
        nextDamageTime = Time.time + invulTime;
        health--;
        MainCanvas.controller.HealthChange();
        anim.SetTrigger("hit");
        SetInvulAnimation();
        if (health == 0)
            Die();
    }

    void SetInvulAnimation()
    {
        anim.SetBool("invulnerable", true);
        Invoke("DisableInvulAnimation", invulTime);
    }

    void DisableInvulAnimation()
    {
        anim.SetBool("invulnerable", false);
    }

    public bool CheckIfTookDamage()
    {
        return tookDamage;
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

    void Die()
    {
        health = 0;
        dead = true;
        UpdateColliders();

        onBoatDeath();
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
        maxHealth = value + 2;
        health = maxHealth;
        MainCanvas.controller.HealthChange();
    }

    public void UpdateMagnetSize(int value)
    {
        if (value == 0)
            coinMagnet.enabled = false;
        else coinMagnet.enabled = true;
        coinMagnet.radius = 2 + .25f * value;
    }

    public void UpdateInvulTime(float time)
    {
        invulTime = .5f + time * .25f;
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

    void StartTutoiral()
    {
        tutorialMode = true;
    }

    void FinishTutorial()
    {
        tutorialMode = false;
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

    #region Sailing
    public void SailIn()
    {
        AudioController.controller.PlayFX(AudioController.controller.mastUnfurl);
        extraSpeed = 7.5f;
        sailingIn = true;
        upgrading = false;
    }
    public void SailOffScreen()
    {
        if (GameModeController.controller.CheckCurrentMode(GameModeController.Mode.Endless))
            return;

        extraSpeed = 7.5f;
        finishedLevel = true;

    }

    void FinishedSailingIn()
    {
        sailingIn = false;
        extraSpeed = 0;
        onFinishedSailingIn();
    }
    #endregion
}