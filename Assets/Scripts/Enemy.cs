using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Transform player;
    float health = 20;
    Rigidbody rb;

    [SerializeField] float speed = 2f;
    float attackTimer = 2f;
    [SerializeField] float attackTimerMax;
    [SerializeField] float attackTimerMin;
    float attackTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        attackTimer = Random.Range(attackTimerMin, attackTimerMax);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < 3f)
        {
            if (attackTime == 0) attackTimer = Random.Range(attackTimerMin, attackTimerMax);
            attackTime += Time.deltaTime;
            if (attackTime >= attackTimer)
            {
                attackTime = 0;
                QuestManager.instance.Hurt();
                player.gameObject.GetComponent<PlayerMove>().Hurt();
            }
        }
        else if (Vector3.Distance(transform.position, player.position) < 15f)
        {
            transform.LookAt(player);
            Vector3 move = transform.TransformDirection(Vector3.forward) * speed;
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
            attackTime = 0f;
        }
    }
    public void TakeDamage(float damage)
    {
        if (health > 0)
        {
            health -= damage;
            if (health <= 0)
            {
                QuestManager.instance.KillEnemy();
                Destroy(gameObject);
            }
        }
    }
}
