using Pathfinding;
using UnityEngine;

public class EnemyGfx : MonoBehaviour
{
    [SerializeField] AIPath path; 
    void Update()
    {
        if(gameObject.tag == "Demon")
            transform.localScale = new Vector2(-Mathf.Sign(path.desiredVelocity.x), 1);
        else
            transform.localScale = new Vector2(Mathf.Sign(path.desiredVelocity.x), 1);
    }
}