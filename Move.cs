using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private Rigidbody2D _rb2d;
    private float x;
    public float speedWalk = 2f;

    // Start is called before the first frame update
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>(); //初始化
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        Vector2 dir = new Vector2(x, 0);
        Walk(dir);

    }
    private void FixedUpdate()  
    {
        //if (jumping)
        //{
            //_rb2d.velocity = new Vector2(_rb2d.velocity.x, jumpSpeed);   //改速度
            //_rb2d.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);  //加impulse力
          //  jumping = false;
        //}
    }

    void Walk(Vector2 dir)
    {
        _rb2d.velocity = dir * speedWalk;   
    }
}
