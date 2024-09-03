using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Items { Nothing, Coin, Key, LAST}

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    [HideInInspector] public float PlayerSpeed;
    

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        MazeGenerator.OnMazeGenerated += MoveToFirstMazeCell;
    }


    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 velocity = new Vector2(horizontal, vertical).normalized;

        _rb.AddForce(velocity * PlayerSpeed * Time.deltaTime*10,ForceMode2D.Force);

        if(horizontal!=0) _spriteRenderer.flipX = Mathf.Sign(horizontal) == -1 ? true : false;


    }

    void MoveToFirstMazeCell()
    {
        transform.position = MazeUtilities.PositionToMaze(MazeUtilities.FirstCell);
    }


}
