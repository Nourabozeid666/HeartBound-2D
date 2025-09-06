using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float EnemySpeed = 5;

    Rigidbody2D Enemyrb;
    ChasingController chasePlayer;
    Vector2 targetDir;
    Animator Enemyanim;
    void Awake()
    {
        Enemyrb = GetComponent<Rigidbody2D>();
        chasePlayer = GetComponent<ChasingController>();
        Enemyanim = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        UpdateDir();
        SetVelocity();
    }

    void UpdateDir()
    {
        if (chasePlayer.AwareOfPlayer)
            targetDir = chasePlayer.DirToPlayer;
        else
            targetDir = Vector2.zero;
    }
    void SetVelocity()
    {
        bool MovingHorizintal = Mathf.Abs(targetDir.x) > Mathf.Epsilon;
        Enemyanim.SetBool("isWalking", MovingHorizintal);
        if (targetDir == Vector2.zero)
            Enemyrb.linearVelocity = Vector2.zero;
        else
        {
            Enemyrb.linearVelocity = targetDir.normalized * EnemySpeed;

            transform.localScale = new Vector2(-Mathf.Sign(targetDir.x), 1);
        }
           
    }
}
