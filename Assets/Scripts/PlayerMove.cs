using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxspeed;
    Rigidbody2D rigid;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        rigid.AddForce(Vector2.up * v, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxspeed)
            rigid.velocity = new Vector2(maxspeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxspeed * (-1))
            rigid.velocity = new Vector2(maxspeed * (-1), rigid.velocity.y);

        if (rigid.velocity.y > maxspeed)
            rigid.velocity = new Vector2(rigid.velocity.x, maxspeed);
        else if (rigid.velocity.y < maxspeed * (-1))
            rigid.velocity = new Vector2(rigid.velocity.x, maxspeed * (-1));
    }
}
