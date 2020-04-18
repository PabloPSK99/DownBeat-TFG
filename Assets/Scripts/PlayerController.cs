using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerControls controls;
    public Node currentNode;
    public Transform center;
    Node targetNode;
    Vector2 move;
    public float minStickMovement;
    public float moveDuration;
    private bool canMove = true;

    // Start is called before the first frame update
    private void Awake()
    {
        controls = new PlayerControls();

        controls.Fight.Ataque.performed += context => Attack();
        controls.Fight.Move.performed += context => move = context.ReadValue<Vector2>();
        controls.Fight.Move.canceled += context => move = Vector2.zero;
    }

    private void Update()
    {
        if(canMove && (Mathf.Abs(move.x) > minStickMovement || Mathf.Abs(move.y) > minStickMovement))
        {
            Move();
        }
    }

    private void Attack()
    {
        print("illo");
    }

    private void Move()
    {
        if(Mathf.Abs(move.x) > Mathf.Abs(move.y)) //Horizontal movement
        {
            if(move.x < 0)  //Left
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


        if(targetNode != null)
        {
            StartCoroutine(MoveToTarget());
        }
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
}
