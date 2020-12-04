using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCollider : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent call;
    public Character player;

    private Collider2D self;

    void Start()
    {
        self = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == gameObject.layer)
        {
            Physics2D.IgnoreCollision(other, self);
        }
        else
        {
            player.m_nextHit = other;
            call.Invoke();
        }
    }
}
