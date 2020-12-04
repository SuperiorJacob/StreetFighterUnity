using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Animator animControl;
    private SpriteRenderer spriteControl;
    private Rigidbody2D physicsControl;

    [HideInInspector] public bool m_OnGround = true;
    [HideInInspector] public Collider2D m_nextHit;

    private bool m_Jumping = false;
    private bool m_Punching = false;
    private bool m_Walking = false;
    private float punchTimer = 0f;
    private bool dealDamage = false;
    private int damage = 30;
    
    private Quaternion rot = new Quaternion();

    private Vector2 nextMove = Vector2.zero;

    [Header("Settings")]
    [SerializeField] private float jumpHeight = 100;
    [SerializeField] private float moveSpeed = 8;
    [SerializeField] private float maxMoveSpeed = 10;

    public float health = 1000;

    [SerializeField] private UnityEngine.UI.Text hpText;

    [Header("Controls")]
    [SerializeField] private KeyCode jump = KeyCode.Space;
    [SerializeField] private KeyCode left = KeyCode.A;
    [SerializeField] private KeyCode right = KeyCode.D;
    [SerializeField] private KeyCode punch = KeyCode.LeftControl;

    //[Header("HitBoxes")]

    void Start()
    {
        // Change later
        Application.targetFrameRate = 60;
        //

        animControl = GetComponent<Animator>();
        spriteControl = GetComponent<SpriteRenderer>();
        physicsControl = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void Punch(bool left)
    {
        if (!dealDamage) return;

        Character ply;
        if (m_nextHit.TryGetComponent(out ply))
        {
            ply.TakeDamage((int)Random.Range(damage/1.5f, damage * 1.5f));

            dealDamage = false;
        }
    }

    // Update is called once per frame
    void Move()
    {
        if (Input.GetKeyDown(jump) && m_OnGround && !m_Jumping)
        {
            m_Jumping = true;
            nextMove.y += jumpHeight;
        }

        if (Input.GetKey(right))
        {
            if (!m_Jumping) m_Walking = true;

            rot.eulerAngles = new Vector3(0, 0, 0);
            transform.rotation = rot;
            //spriteControl.flipX = false;

            if ((physicsControl.velocity.x + moveSpeed) <= maxMoveSpeed)
                nextMove.x += moveSpeed;
        }
        else if (Input.GetKey(left))
        {
            if (!m_Jumping) m_Walking = true;

            rot.eulerAngles = new Vector3(0, 180, 0);
            transform.rotation = rot;
            //spriteControl.flipX = true;

            if ((physicsControl.velocity.x - moveSpeed) >= -maxMoveSpeed)
                nextMove.x -= moveSpeed;
        }
        else m_Walking = false;

        if (Input.GetKeyDown(punch) && !m_Punching)
        {
            dealDamage = true;
            m_Punching = true;
            punchTimer = Time.time + 0.5f;
        }
        else if (m_Punching && punchTimer < Time.time)
        {
            m_Punching = false;
            punchTimer = 0f;
        }

        physicsControl.AddForce(nextMove);
        nextMove = Vector2.zero;

        animControl.SetBool("jumping", !m_Punching && !m_Punching && m_Jumping);
        animControl.SetBool("walking", !m_Punching && !m_Jumping && m_Walking);
        animControl.SetBool("punching", m_Punching);
    }

    void Update()
    {
        Move();

        hpText.text = "HP: " + health;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_OnGround = true;
        m_Jumping = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        m_OnGround = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        m_OnGround = false;
    }
}
