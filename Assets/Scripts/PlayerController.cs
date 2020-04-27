using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    PlayerControls controls;
    public MainCamera mainCamera;
    public Node currentNode;
    public Transform center;
    public Rhythm rhythm;
    public float successChanceThreshold;
    Node targetNode;
    Vector2 move;
    public float minStickMovement;
    public float moveDuration;
    public float successChance;
    private bool canMove = true;
    private bool moveInput = false;

    public EnemyController enemy;

    [Header("Stats")]
    public int damage;
    public int maxHealth;
    private int health;

    [Header("Actions")]
    public Action currentAction;
    public bool charged;
    private Bullet[] chamber;

    [Header("UI")]
    public Image healthBar;
    public Image damageBar;
    public float healthBarTime;
    public float damageBarTime;
    public float damageBarDelay;
    public Image[] bulletsUI;
    public Color plusBulletColor;
    public Color burntBulletColor;
    public Color burntPlusBulletColor;
    public float burnBulletTime;
    private readonly float maxFillAmount = 0.2f;

    [Header("VFX")]
    public float shotCameraShake;
    public float plusShotCameraShake;

    // Start is called before the first frame update
    private void Awake()
    {
        health = maxHealth;
        chamber = new Bullet[] { Bullet.One, Bullet.One, Bullet.One, Bullet.Plus };
        //chamber = new Bullet[] { Bullet.Empty, Bullet.Empty, Bullet.Empty, Bullet.Empty };
        UpdateAmmoUI();
        currentAction = Action.None;

        controls = new PlayerControls();
        controls.Fight.Attack.started += context => ChargeAttack();
        controls.Fight.Attack.canceled += context => Attack();

        controls.Fight.Tech.started += context => ChargeTech();
        controls.Fight.Tech.canceled += context => Tech();

        controls.Fight.Move.performed += context => move = context.ReadValue<Vector2>();
        controls.Fight.Move.canceled += context => move = Vector2.zero;
        controls.Fight.Move.canceled += context => moveInput = false;

        GetDamage(90f);
    }

    private void Update()
    {
        if (Mathf.Abs(move.x) > minStickMovement || Mathf.Abs(move.y) > minStickMovement)
        {


            if (!moveInput)
            {
                Move();
            }
        }
    }




    private void ChargeTech()
    {
        if ((!rhythm.beatLocked || (rhythm.beatLocked && currentAction == Action.Move)) && CheckSuccess())
        {
            if (move.y > minStickMovement && currentNode.forward == null)
            {
                currentAction = Action.Twirl;
            }
            else if (move.y < -minStickMovement && currentNode.back == null)
            {
                currentAction = Action.Flash;
            }
            else if (Mathf.Abs(move.y) < minStickMovement)
            {
                currentAction = Action.Reload;
                StartCoroutine(WaitForTechCorrection(1.2f));
            }
            charged = true;
        }
        rhythm.LockTwoBeats();
    }

    private void Tech()
    {

        if (!rhythm.beatLocked)
        {
            if (currentAction == Action.Reload)  //Recarga
            {
                if (successChance == 100f) //Crítico
                {
                    Reload(4);
                }
                else
                {
                    Reload(Mathf.FloorToInt(successChance) / 30);
                }
                print("reload");
            }
            else if(currentAction == Action.Twirl)
            {

            }
            
        }
        else
        {
            if (rhythm.beatLocked) print("LOCKED");
            else print("Faaaaaaaaaaaaaaaaaaaaaaaail");
        }
        charged = false;
        currentAction = Action.None;
    }

    private void Move()
    {
        moveInput = true;
        if (currentAction == Action.None)
        {
            currentAction = Action.Move;
            StartCoroutine(ScheduleFinish(rhythm.bpm / 60f));
        }
        if (!rhythm.beatLocked && CheckSuccess())
        {
            if (Mathf.Abs(move.x) > Mathf.Abs(move.y)) //Horizontal movement
            {
                if (move.x < 0)  //Left
                {
                    targetNode = currentNode.left;
                }
                else            //Right
                {
                    targetNode = currentNode.right;
                }
            }
            else //Vertical movement
            {
                if (move.y > 0) //Forward
                {
                    targetNode = currentNode.forward;
                }
                else            //Back
                {
                    targetNode = currentNode.back;
                }
            }


            if (targetNode != null && canMove)
            {
                if(currentAction == Action.Move)
                {
                    currentAction = Action.None;
                    StartCoroutine(MoveToTarget());
                }
            }
        }
        else
        {
            if (rhythm.beatLocked) print("LOCKED");
            else print("Faaaaaaaaaaaaaaaaaaaaaaaail");
        }
        rhythm.LockThisBeat();

    }

    IEnumerator MoveToTarget()
    {
        canMove = false;
        float rotation = 0;
        float lastRotation = transform.rotation.eulerAngles.y;
        if (targetNode == currentNode.left)
        {
            rotation = 1/6f;
            mainCamera.Rotate(rotation, moveDuration);
        }
        else if (targetNode == currentNode.right)
        {
            rotation = -1/6f;
            mainCamera.Rotate(rotation, moveDuration);
        }
        else
        {
            iTween.MoveTo(gameObject, targetNode.transform.position, moveDuration);
        }
        iTween.RotateBy(transform.parent.gameObject, Vector3.up * rotation, moveDuration);
        currentNode = targetNode;
        yield return new WaitForSeconds(moveDuration);
        transform.position = targetNode.transform.position;
        transform.rotation = Quaternion.Euler(0, lastRotation + rotation * 360, 0);
        canMove = true;
    }

    private void OnEnable()
    {
        controls.Fight.Enable();
    }

    private void OnDisable()
    {
        controls.Fight.Disable();
    }

    private bool CheckSuccess()
    {
        if (rhythm.normalized < 0.5f) //Si es tiempo 
        {
            successChance = Mathf.Min(100, (0.5f - rhythm.normalized) * 2 * (100 + successChanceThreshold));
        }
        else                   //Si es contratiempo
        {
            successChance = Mathf.Min(100, (rhythm.normalized - 0.5f) * 2 * (100 + successChanceThreshold));
        }


        if (successChance < 70)
        {
            float rng = Random.value * 100;
            return successChance >= rng;
        }
        return true;
    }


    

    IEnumerator WaitForTechCorrection(float time)
    {
        float elapsed = 0;
        print("START: " + currentAction + "--------------");
        while (currentAction == Action.Reload && elapsed <= time)
        {
            if (move.y > minStickMovement && currentNode.forward == null)
            {
                print("while: " + currentAction + "   twirlcondition");
                currentAction = Action.Twirl;
            }
            else if (move.y < -minStickMovement && currentNode.back == null)
            {
                print("while: " + currentAction + "   flashcondition");
                currentAction = Action.Flash;
            }
            elapsed += Time.deltaTime;
            print(elapsed);
            yield return null;
        }
        print("FINISH: " + currentAction + "--------------");

    }

    IEnumerator ScheduleFinish(float time)
    {
        float elapsed = 0;
        Action lastAction = currentAction;
        while (lastAction == currentAction && elapsed <= time)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        if(lastAction == currentAction) currentAction = Action.None;
    }




    //-------------------------------------------ATTACK----------------------------------------------

    private void ChargeAttack()
    {
        if (!rhythm.beatLocked && CheckSuccess())
        {
            currentAction = Action.Attack;
            charged = true;
        }
        rhythm.LockTwoBeats();
    }

    private void Attack()
    {

        if (!rhythm.beatLocked && currentAction == Action.Attack && charged == true)
        {
            CheckSuccess();
            if (currentNode.forward == null)  //Ataque Melee
            {
                if (successChance == 100f) //Crítico
                {
                    enemy.GetAttack(damage * 1.5f);
                    print("CRÍTICO");
                    FillChamberSlot(false);
                }
                else
                {
                    enemy.GetAttack(damage * successChance * 0.01f);
                }
            }
            else                          //Ataque a distancia
            {
                if (successChance == 100f) //Crítico
                {
                    enemy.GetAttack(Shoot() * 1.5f);
                    print("CRÍTICO");
                }
                else
                {
                    enemy.GetAttack(Shoot() * successChance * 0.01f);
                }
            }
        }
        else
        {
            if (rhythm.beatLocked) print("LOCKED");
            else print("Faaaaaaaaaaaaaaaaaaaaaaaail");
        }
        charged = false;
        currentAction = Action.None;
    }

    private float Shoot()
    {
        float bulletDamage = 0;
        for (int i = 0; i < 4; i++)
        {
            if (chamber[i] != Bullet.Empty)
            {
                bulletDamage = damage;
                if (chamber[i] == Bullet.Plus)
                {
                    iTween.ShakePosition(mainCamera.gameObject, Vector3.one * plusShotCameraShake * successChance / 100, 0.15f);
                    bulletDamage *= 2;
                    Heal(20f);
                }
                else
                {
                    iTween.ShakePosition(mainCamera.gameObject, Vector3.one * shotCameraShake * successChance / 100, 0.1f);
                }
                StartCoroutine(BurnBullet(i));
                chamber[i] = Bullet.Empty;
                break;
            }
        }
        return bulletDamage;
    }





    //--------------------------------------------AMMO-----------------------------------------------

    private void FillChamberSlot(bool isPlus)
    {
        for (int i = 3; i >= 0; i--)
        {
            if (chamber[i] == Bullet.Empty)
            {
                chamber[i] = isPlus ? Bullet.Plus : Bullet.One;
                break;
            }
        }
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        for (int i = 0; i < 4; i++)
        {
            if (chamber[i] == Bullet.Empty)
            {
                bulletsUI[i].color = Color.clear; ;
            }
            else if (chamber[i] == Bullet.One)
            {
                bulletsUI[i].color = Color.white;
            }
            else
            {
                bulletsUI[i].color = plusBulletColor;
            }
        }

    }

    private void Reload(int bullets)
    {
        StartCoroutine(ReloadCoroutine(bullets));
    }

    IEnumerator ReloadCoroutine(int bullets)
    {
        int emptySlots = 0;
        for (int i = 0; i < 4; i++)
        {
            if (chamber[i] == Bullet.Empty)
            {
                emptySlots++;
            }
        }
        for (int i = emptySlots - 1; i >= 0; i--)
        {
            if (i == 3)
            {
                chamber[i] = Bullet.Plus;
                bulletsUI[i].color = plusBulletColor;
            }
            else
            {
                chamber[i] = Bullet.One;
                bulletsUI[i].color = Color.white;
            }
            iTween.PunchScale(bulletsUI[i].gameObject, new Vector3(0.1f, -0.1f, 0), 1f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator BurnBullet(int slot)
    {
        Vector3 startScale = bulletsUI[slot].transform.localScale;
        float elapsed = 0;
        Color startColor = bulletsUI[slot].color;
        Color targetColor = chamber[slot] == Bullet.Plus ? burntPlusBulletColor : burntBulletColor;
        iTween.ScaleTo(bulletsUI[slot].gameObject, iTween.Hash(
            "scale", new Vector3(1.2f, 0.6f, 0.6f),
            "time", burnBulletTime * 1.5f,
            "easetype", iTween.EaseType.easeOutCubic
            )
        );
        while (elapsed < burnBulletTime)
        {
            targetColor = new Color(targetColor.r, targetColor.g, targetColor.b, targetColor.a * (1 - elapsed/burnBulletTime));
            bulletsUI[slot].color = ((1f - elapsed / burnBulletTime) * startColor) + (elapsed / burnBulletTime * targetColor);
            elapsed += Time.deltaTime;
            yield return null;
        }
        bulletsUI[slot].color = Color.clear;
        bulletsUI[slot].transform.localScale = startScale;
    }

    //-------------------------------------------HEALTH----------------------------------------------

    public void GetAttack(float damage)
    {
        GetDamage(damage);
    }

    public void GetDamage(float damage)
    {
        health = Mathf.Max(health - Mathf.RoundToInt(damage), 0);
        StartCoroutine(UpdateHealthBar());
    }

    public void Heal(float healing)
    {
        health = Mathf.Min(health + Mathf.RoundToInt(healing), maxHealth);
        StartCoroutine(UpdateHealthBar());
    }

    IEnumerator UpdateHealthBar()
    {
        float scale = 0.2f * health / maxHealth;
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", healthBar.fillAmount,
            "to", scale,
            "time", healthBarTime,
            "onupdate", "OnHealthBarUpdate",
            "easetype", iTween.EaseType.easeOutCirc
            )
        );
        yield return new WaitForSeconds(damageBarDelay);
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", damageBar.fillAmount,
            "to", scale,
            "time", damageBarTime,
            "onupdate", "OnDamageBarUpdate",
            "easetype", iTween.EaseType.linear
            )
        );
        yield return null;
    }

    private void OnHealthBarUpdate(float newValue)
    {
        healthBar.fillAmount = newValue;
    }

    private void OnDamageBarUpdate(float newValue)
    {
        damageBar.fillAmount = newValue;
    }
}
