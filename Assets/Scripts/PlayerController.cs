using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerControls controls;
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

    [Header("Actions")]
    public Action currentAction;

    // Start is called before the first frame update
    private void Awake()
    {
        controls = new PlayerControls();

        controls.Fight.Ataque.performed += context => Attack();
        controls.Fight.Move.performed += context => move = context.ReadValue<Vector2>();
        controls.Fight.Move.canceled += context => move = Vector2.zero;
        controls.Fight.Move.canceled += context => moveInput = false;
    }

    private void Update()
    {
        if (!moveInput && (Mathf.Abs(move.x) > minStickMovement || Mathf.Abs(move.y) > minStickMovement))
        {
            Move();
        }
    }

    private void Attack()
    {
        //CheckSuccessMovement();
    }

    private void Move()
    {
        moveInput = true;
        if (!rhythm.beatLocked && CheckSuccessMovement())
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
                StartCoroutine(MoveToTarget());
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
        if (targetNode == currentNode.left)
        {

        }
        else if (targetNode == currentNode.right)
        {

        }
        currentNode = targetNode;
        iTween.MoveTo(this.gameObject, targetNode.transform.position, moveDuration);
        //float degrees =   
        //iTween.RotateTo(this.gameObject, )
        yield return new WaitForSeconds(moveDuration/2);
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

    private bool CheckSuccessMovement()
    {
        if (rhythm.normalized < 0.5f) //Si es tiempo 
        {
            successChance = (0.5f - rhythm.normalized) * 2 * (100 + successChanceThreshold);
        }
        else                   //Si es contratiempo
        {
            successChance = (rhythm.normalized - 0.5f) * 2 * (100 + successChanceThreshold);
        }
        print(successChance);
        if (successChance < 70)
        {
            float rng = Random.value * 100;
            return successChance >= rng;
        }
        return true;
    }
}
