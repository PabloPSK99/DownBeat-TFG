using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    PlayerControls controls;
    public MainCamera mainCamera;
    public Node currentNode;
    public Node initialNode;
    public Transform center;
    public Rhythm rhythm;
    public UIController UIController;
    public float successChanceThreshold;
    Node targetNode;
    Vector2 move;
    public float minStickMovement;
    public float moveDuration;
    public float successChance;
    private bool canMove = true;
    private bool moveInput = false;
    private Animator camAnimator;
    public Animator charAnimator;
    public Effects effects;
    public EnemyController enemy;

    [Header("Stats")]
    public int damage;
    public int maxHealth;
    private int health;
    public bool offBeat;

    [Header("Actions")]
    public Action currentAction;
    public bool charged;
    public bool nearlyLoaded;
    public float jumpHeight;
    private Bullet[] chamber;
    public float damageToBlock;
    public float maxBlockableRatio;
    private IEnumerator load;

    [Header("UI")]
    public Image healthBar;
    public Image damageBar;
    public float healthBarTime;
    public float damageBarTime;
    public float damageBarDelay;
    public Image[] bulletsUI;
    public Transform bulletsBackground;
    public Color plusBulletColor;
    public Color burntBulletColor;
    public Color burntPlusBulletColor;
    public Color warningBulletColor;
    public float burnBulletTime;
    private readonly float maxFillAmount = 0.2f;

    [Header("VFX")]
    public float shotCameraShake;
    public float plusShotCameraShake;
    public float damageCameraShake;
    public float offBeatShake;



    private void Awake()
    {
        camAnimator = GetComponent<Animator>();
        health = maxHealth;
        chamber = new Bullet[] { Bullet.One, Bullet.One, Bullet.One, Bullet.Plus };
        for (int i = 0; i < 4; i++)
        {
            //Shoot();
        }
        currentAction = Action.None;
        load = Load();

        controls = new PlayerControls();

        controls.Fight.Attack.started += context => ChargeAttack();
        controls.Fight.Attack.canceled += context => Attack();

        controls.Fight.Block.started += context => ChargeBlock();
        controls.Fight.Block.canceled += context => Block();

        controls.Fight.Tech.started += context => ChargeTech();
        controls.Fight.Tech.canceled += context => Tech();

        controls.Fight.Cut.started += context => Cut();

        controls.Fight.Move.performed += context => move = context.ReadValue<Vector2>();
        controls.Fight.Move.canceled += context => move = Vector2.zero;
        controls.Fight.Move.canceled += context => moveInput = false;

        controls.Fight.Debug1.started += context => Debug1();
        controls.Fight.Debug2.started += context => Debug2();
        controls.Fight.Debug3.started += context => Debug3();
        controls.Fight.Debug4.started += context => Debug4();
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


    #region Attack
    //-------------------------------------------ATTACK----------------------------------------------

    private void ChargeAttack()
    {
        if (!rhythm.beatLocked && CheckSuccess() && rhythm.IsDownBeat())
        {
            currentAction = Action.Attack;
            StartLoading();
            charged = true;
            if (currentNode.forward == null)  //Ataque Melee
            {
                UIController.PopUpChance(successChance, Action.Attack);
                SetTrigger("chargePunch");
            }
            else
            {
                if (chamber[3] != Bullet.Empty)
                {
                    UIController.PopUpChance(successChance, Action.Attack);
                    SetTrigger("chargeShot");
                }
                else
                {
                    UIController.PopUpText("OUT OF AMMO!");
                    StartCoroutine(OutOfAmmo());
                    SetTrigger("outOfAmmo");
                }
            }
        }
        else
        {
            UIController.PopUpText("FAIL!");
        }

        rhythm.LockTwoBeats();
        rhythm.ScheduleDischarge(3);
    }

    private void Attack()
    {

        if (!rhythm.beatLocked && currentAction == Action.Attack && charged == true && rhythm.IsDownBeat())
        {
            CheckSuccess();
            SetTrigger("release");
            UIController.PopUpChance(successChance, Action.Attack);
            if (currentNode.forward == null)  //Ataque Melee
            {
                if (successChance == 100f) //Crítico
                {
                    CameraShake(plusShotCameraShake, 0.25f);
                    enemy.GetAttack(damage * 1.5f);
                    FillChamberSlot(false);
                }
                else
                {
                    enemy.GetAttack(damage * successChance / 100);
                    CameraShake(shotCameraShake * successChance / 100, 0.15f);
                }
            }
            else                          //Ataque a distancia
            {
                float shootDamage = Shoot();
                if(shootDamage != 0)
                {
                    if (successChance == 100f) //Crítico
                    {
                        enemy.GetAttack(shootDamage * 1.5f);
                    }
                    else
                    {
                        enemy.GetAttack(shootDamage * successChance / 100);
                    }
                }
            }
        }
        else
        {
            if (currentAction == Action.Attack)
            {
                UIController.PopUpText("FAIL!");
                currentAction = Action.None;
                ResetLoading();
                SetTrigger("fail");
                effects.Fail();
            }
        }
        charged = false;
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
                    if(successChance == 100)
                    {
                        CameraShake(plusShotCameraShake * 1.5f, 0.25f);
                    }
                    else
                    {
                        CameraShake(plusShotCameraShake * successChance / 100, 0.25f);
                    }
                    effects.Bullet(true);
                    bulletDamage *= 2;
                    Heal(20f);
                }
                else
                {
                    effects.Bullet(false);
                    CameraShake(shotCameraShake * successChance / 100, 0.15f);
                }
                StartCoroutine(BurnBullet(i));
                chamber[i] = Bullet.Empty;
                break;
            }
        }
        return bulletDamage;
    }

#endregion

    #region Block
    //-------------------------------------------BLOCK-----------------------------------------------

    private void ChargeBlock()
    {
        if (!rhythm.beatLocked && CheckSuccess())
        {
            SetTrigger("chargeBlock");
            UIController.PopUpChance(successChance, Action.Block);
            currentAction = Action.Block;
            charged = true;
            StartLoading();
            effects.Block(rhythm.IsDownBeat(), true);
        }
        else
        {
            UIController.PopUpText("FAIL!");
        }
        rhythm.LockThisBeat();
        rhythm.ScheduleDischarge(3);
    }

    private void Block()
    {
        if (!rhythm.beatLocked && currentAction == Action.Block && charged == true && rhythm.IsDownBeat())
        {
            CheckSuccess();
            UIController.PopUpChance(successChance, Action.Block);
            StartCoroutine(WaitForDamage(0.2f));
        }
        else
        {
            if (currentAction == Action.Block)
            {
                SetTrigger("fail");
                UIController.PopUpText("FAIL!");
                currentAction = Action.None;
                ResetLoading();
            }
            charged = false;
        }
    }

    IEnumerator WaitForDamage(float time)
    {
        float elapsed = 0;
        while (elapsed <= time)
        {
            if (damageToBlock != 0)
            {
                if (successChance == 100f) //Crítico
                {
                    enemy.OffBeat();
                    SetTrigger("parry");
                    UIController.PopUpNumber(damageToBlock, NumberType.Block, true);
                }
                else
                {
                    SetTrigger("block");
                    UIController.PopUpNumber(damageToBlock * maxBlockableRatio * successChance / 100, NumberType.Block, false);
                    GetDamage(damageToBlock * (1 - maxBlockableRatio * successChance / 100));
                }
                break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (damageToBlock == 0)
        {
            SetTrigger("fail");
        }
        else
        {
            damageToBlock = 0;
        }
        charged = false;
        currentAction = Action.None;
        ResetLoading();
    }
#endregion

    #region Techniques
    //-----------------------------------------TECHNIQUES--------------------------------------------

    private void ChargeTech()
    {
        if ((!rhythm.beatLocked || (rhythm.beatLocked && currentAction == Action.Move)) && CheckSuccess() && !rhythm.IsDownBeat())
        {
            UIController.PopUpChance(successChance, Action.Tech);
            if (move.y > minStickMovement && currentNode.forward == null)
            {
                currentAction = Action.Twirl;
                SetTrigger("chargeTwirl");
            }
            else if (move.y < -minStickMovement && currentNode.back == null)
            {
                currentAction = Action.Flash;
                camAnimator.SetTrigger("Charging Flash");
                SetTrigger("chargeFlash");
            }
            else if (Mathf.Abs(move.y) < minStickMovement)
            {
                currentAction = Action.Reload;
                SetTrigger("reload");
                effects.Reload();
                StartCoroutine(WaitForTechCorrection(0.2f));
            }
            StartLoading();
            charged = true;
        }
        else
        {
            UIController.PopUpText("FAIL!");
        }
        rhythm.LockTwoBeats();
        rhythm.ScheduleDischarge(3);
    }

    private void Tech()
    {

        if (!rhythm.beatLocked && !rhythm.IsDownBeat() && charged == true)
        {
            CheckSuccess();
            SetTrigger("release");
            if (currentAction == Action.Reload)  //Recarga
            {
                UIController.PopUpChance(successChance, Action.Tech);
                if (successChance == 100f) //Crítico
                {
                    Reload(4);
                }
                else
                {
                    Reload(Mathf.FloorToInt(successChance) / 30);
                }
            }
            else if (currentAction == Action.Twirl) //Pirueta
            {
                UIController.PopUpChance(successChance, Action.Tech);
                if (successChance == 100f) //Crítico
                {
                    FillChamberSlot(true);
                    CameraShake(plusShotCameraShake * 1.5f, 0.25f);
                }
                else
                {
                    CameraShake(plusShotCameraShake * successChance / 100, 0.15f);
                }
                enemy.GetTech(damage * successChance / 100);

                targetNode = currentNode.GetLastNodeOnThisAxis();
                StartCoroutine(TwirlToTarget());
            }
            else if (currentAction == Action.Flash) //Destello
            {
                UIController.PopUpChance(successChance, Action.Tech);
                if (successChance == 100f) //Crítico
                {
                    FillChamberSlot(true);
                    CameraShake(plusShotCameraShake * 1.5f, 0.25f);
                }
                else
                {
                    CameraShake(plusShotCameraShake * successChance / 100, 0.15f);
                }
                enemy.GetTech(damage * successChance / 100);
                targetNode = currentNode.GetFirstNodeOnOppositeAxis();
                StartCoroutine(FlashToTarget());
            }
        }
        else
        {
            if (currentAction == Action.Reload || currentAction == Action.Twirl || currentAction == Action.Flash)
            {
                SetTrigger("fail");
                camAnimator.ResetTrigger("Charging Flash");
                camAnimator.SetTrigger("Fail");
                UIController.PopUpText("FAIL!");
                effects.Fail();
                currentAction = Action.None;
                ResetLoading();
            }
        }
        charged = false;
    }




    //--------------------------------------------AMMO-----------------------------------------------

    private void FillChamberSlot(bool isPlus)
    {
        for (int i = 3; i >= 0; i--)
        {
            if (chamber[i] == Bullet.Empty)
            {
                bulletsUI[i].transform.localScale = Vector3.one * 0.6f;
                if (isPlus)
                {
                    chamber[i] = Bullet.Plus;
                    bulletsUI[i].color = plusBulletColor;
                }
                else
                {
                    chamber[i] = Bullet.One;
                    bulletsUI[i].color = Color.white;
                }
                break;
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
                bulletsUI[i].transform.localScale = Vector3.one * 0.6f;
                emptySlots++;
            }
        }
        for (int i = emptySlots - 1; i >= 0 && bullets > 0; i--)
        {
            if (i == 3 && bullets == 4)
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
            bullets--;
        }
    }

    IEnumerator OutOfAmmo()
    {
        Image[] backgrounds = bulletsBackground.GetComponentsInChildren<Image>();
        for (int i = backgrounds.Length-1; i >= 0; i--)
        {
            backgrounds[i].transform.localScale = Vector3.one * 0.6f;
            backgrounds[i].color = warningBulletColor;
            iTween.PunchScale(backgrounds[i].gameObject, new Vector3(0.1f, -0.1f, 0), 1f);
            yield return new WaitForSeconds(0.1f);
        }

        for (int i = backgrounds.Length - 1; i >= 0; i--)
        {
            backgrounds[i].color = Color.black / 2f;
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
            "oncomplete", "onBurnBulletComplete",
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
        yield return null;
    }
    #endregion

    #region Cut
    //--------------------------------------------CUT------------------------------------------------

    private void Cut()
    {
        if (!rhythm.beatLocked)
        {
            CheckSuccess();
            if (successChance == 100f && (enemy.currentAction == Action.Block || enemy.currentAction == Action.Tech))
            {
                UIController.PopUpChance(successChance, Action.Cut);
                enemy.OffBeat();
            }
            else
            {
                UIController.PopUpText("FAIL!");
                OffBeat();
            }
        }
    }

    public void OffBeat()
    {
        SetTrigger("offBeat");
        currentAction = Action.None;
        ResetLoading();
        charged = false;
        offBeat = true;
        rhythm.OffBeat(true);
        CameraShake(offBeatShake, 0.35f);

        if (currentNode.forward == null)
        {
            camAnimator.SetTrigger("Inner");
        }
        else if(currentNode.back == null)
        {
            camAnimator.SetTrigger("External");
        }
        else
        {
            camAnimator.SetTrigger("Middle");
        }

        StartCoroutine(WaitForRecover());
    }

    public void RecoverBeat()
    {
        offBeat = false;
        rhythm.OffBeat(false);
    }

    #endregion

    #region Health
    //-------------------------------------------HEALTH----------------------------------------------

    public void GetAttack(float damage)
    {
        if(currentAction == Action.Block)
        {
            damageToBlock = damage;
        }
        else 
        {
            if(currentAction == Action.Reload || currentAction == Action.Twirl || currentAction == Action.Flash)
            {
                OffBeat();
            }
            GetDamage(damage);
        }
    }

    public void GetDamage(float damage)
    {
        CameraShake(damageCameraShake * damage, 0.15f);
        UIController.PopUpNumber(damage, NumberType.Damage, damageToBlock > enemy.damage);
        health = Mathf.Max(health - Mathf.RoundToInt(damage), 0);
        StartCoroutine(UpdateHealthBar());
        enemy.CancelSpearCombo();
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

    #endregion

    #region Movement
    //------------------------------------------MOVEMENT---------------------------------------------
    private void Move()
    {
        moveInput = true;
        if (currentAction == Action.None)
        {
            currentAction = Action.Move;
            StartCoroutine(ScheduleFinish(rhythm.bpm / 60f));
        }
        if (!rhythm.beatLocked)
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
                if (currentAction == Action.Move)
                {
                    if (CheckMovementSuccess())
                    {
                        StartCoroutine(MoveToTarget());
                        UIController.PopUpChance(successChance, Action.Move);
                    }
                    else
                    {
                        UIController.PopUpText("FAIL!");
                    }
                }
                else if (currentAction == Action.Block && CheckSuccess())
                {
                    if (CheckSuccess())
                    {
                        StartCoroutine(MoveToTarget());
                        UIController.PopUpChance(successChance, Action.Move);
                    }
                    else
                    {
                        UIController.PopUpText("FAIL!");
                    }
                }
                currentAction = Action.None;
                ResetLoading();
            }
        }
        else
        {
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
            SetTrigger("left");
            rotation = 1 / 6f;
            mainCamera.Rotate(rotation, moveDuration);
        }
        else if (targetNode == currentNode.right)
        {
            SetTrigger("right");
            rotation = -1 / 6f;
            mainCamera.Rotate(rotation, moveDuration);
        }
        else
        {
            if(targetNode == currentNode.forward)
            {
                SetTrigger("forward");
            }
            else
            {
                SetTrigger("back");
            }
            int nodeIndex = targetNode.GetIndexInAxis();
            switch (nodeIndex)
            {
                case 0:
                    camAnimator.SetTrigger("Inner");
                    break;
                case 1:
                    camAnimator.SetTrigger("Middle");
                    break;
                case 2:
                    camAnimator.SetTrigger("External");
                    break;
                default:
                    break;
            }
            


            iTween.MoveTo(gameObject, targetNode.transform.position, moveDuration);
        }
        iTween.RotateBy(transform.parent.gameObject, Vector3.up * rotation, moveDuration);
        currentNode = targetNode;
        yield return new WaitForSeconds(moveDuration);
        transform.position = targetNode.transform.position;
        transform.rotation = Quaternion.Euler(0, lastRotation + rotation * 360, 0);
        canMove = true;
    }

    IEnumerator TwirlToTarget()
    {
        canMove = false;
        iTween.MoveTo(gameObject, iTween.Hash(
            "path", new Vector3[2] { (targetNode.transform.position + transform.position) / 2 + Vector3.up * jumpHeight, targetNode.transform.position },
            "time", moveDuration,
            "easetype", iTween.EaseType.easeOutQuad
            )
        );
        currentNode = targetNode;
        camAnimator.SetTrigger("External");
        yield return new WaitForSeconds(moveDuration);
        transform.position = targetNode.transform.position;
        canMove = true;
    }


    IEnumerator FlashToTarget()
    {
        canMove = false;
        camAnimator.SetTrigger("Flash");
        iTween.MoveTo(gameObject, targetNode.transform.position, moveDuration);
        currentNode = targetNode;
        mainCamera.Rotate(0.5f, moveDuration, iTween.EaseType.easeInOutCubic);
        yield return new WaitForSeconds(moveDuration);
        iTween.RotateBy(gameObject, Vector3.up * 0.5f, moveDuration);
        transform.position = targetNode.transform.position;
        canMove = true;
    }


    #endregion

    #region Success
    //------------------------------------------SUCCESS----------------------------------------------

    private bool CheckMovementSuccess()
    {
        if (rhythm.IsDownBeat()) //Si es tiempo 
        {
            successChance = Mathf.Min(100, (0.5f - rhythm.normalized) * 2 * (100 + successChanceThreshold));
        }
        else                   //Si es contratiempo
        {
            successChance = Mathf.Min(100, (rhythm.normalized - 0.5f) * 2 * (100 + successChanceThreshold));
        }
        if (offBeat)
        {
            if (successChance == 100)
            {
                return true;
            }
            return false;
        }
        if (successChance < 90)
        {
            if (successChance < 50)
            {
                return false;
            }
            float rng = Random.value * 120;
            return successChance >= rng;
        }
        return true;
    }

    private bool CheckSuccess()
    {
        if (rhythm.IsDownBeat()) //Si es tiempo 
        {
            successChance = Mathf.Min(100, (0.5f - rhythm.normalized) * 2 * (100 + successChanceThreshold));
        }
        else                   //Si es contratiempo
        {
            successChance = Mathf.Min(100, (rhythm.normalized - 0.5f) * 2 * (100 + successChanceThreshold));
        }
        if (offBeat)
        {
            if (successChance == 100)
            {
                return true;
            }
            return false;
        }
        if (successChance < 50)
        {
            float rng = Random.value * 100;
            return successChance >= rng;
        }
        return true;
    }

    IEnumerator WaitForRecover()
    {
        yield return new WaitForSeconds(0.5f);
        successChance = 0;
        while (offBeat)
        {
            if(successChance == 100f)
            {
                RecoverBeat();
                break;
            }
            yield return null;
        }
    }

    #endregion

    #region Control & Scheduling
    //------------------------------------CONTROL & SCHEDULING---------------------------------------

    public void EnableFightControls()
    {
        controls.Fight.Enable();
    }

    public void DisableFightControls()
    {
        controls.Fight.Disable();
    }

    public void ResetAll()
    {
        currentNode = initialNode;
        camAnimator.SetTrigger("External");
        transform.parent.localRotation = Quaternion.identity;
        transform.localRotation = Quaternion.identity;
        transform.position = currentNode.transform.position;
        mainCamera.transform.localRotation = Quaternion.identity;
        enemy.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        enemy.currentNode = currentNode.GetFirstNodeOnThisAxis();
    }

    public void CameraShake(float intensity, float duration)
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        if(iTween.Count("shake") == 0)
        {
            iTween.ShakePosition(mainCamera.gameObject, iTween.Hash(
                "name", "shake",
                "amount", Vector3.one * intensity,
                "time", duration,
                "oncomplete", "OnCompleteCameraShake",
                "oncompleteparams", cameraPosition
                )
            );
        }
    }

    private void OnCompleteCameraShake(Vector3 cameraPosition)
    {
        mainCamera.transform.position = cameraPosition;
    }


    public void SetTrigger(string trigger)
    {
        foreach (AnimatorControllerParameter parameter in charAnimator.parameters)
        {
            charAnimator.ResetTrigger(parameter.name);
        }
        charAnimator.SetTrigger(trigger);
    }

    IEnumerator WaitForTechCorrection(float time)
    {
        float elapsed = 0;
        //print("START: " + currentAction + "--------------");
        while (currentAction == Action.Reload && elapsed <= time)
        {
            if (move.y > minStickMovement && currentNode.forward == null)
            {
                //print("while: " + currentAction + "   twirlcondition");
                currentAction = Action.Twirl;
            }
            else if (move.y < -minStickMovement && currentNode.back == null)
            {
                //print("while: " + currentAction + "   flashcondition");
                currentAction = Action.Flash;
                camAnimator.SetTrigger("Charging Flash");
                SetTrigger("chargeFlash");
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        //print("FINISH: " + currentAction + "--------------");

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
        if (lastAction == currentAction)
        {
            currentAction = Action.None;
            SetTrigger("fail");
            ResetLoading();
        }
    }

    private void StartLoading()
    {
        ResetLoading();
        load = Load();
        StartCoroutine(load);
    }

    private void ResetLoading()
    {
        nearlyLoaded = false;
        if (load != null)
        {
            StopCoroutine(load);
        }
    }

    IEnumerator Load()
    {
        yield return new WaitForSeconds(1.4f);
        nearlyLoaded = true;
    }

    public void SetIntroTrigger()
    {
        camAnimator.SetTrigger("Start");
    }

    void Debug1()
    {
        Debug.Break();
    }

    void Debug2()
    {
        Heal(maxHealth);
    }

    void Debug3()
    {
        ResetAll();
    }

    void Debug4()
    {
        enemy.NextPhase();
    }
    #endregion
}
