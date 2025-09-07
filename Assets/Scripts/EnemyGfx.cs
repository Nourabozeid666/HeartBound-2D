using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyGfx : MonoBehaviour
{
    [SerializeField] AIPath path; 
    void Update()
    {
        transform.localScale = new Vector2(-Mathf.Sign(path.desiredVelocity.x), 1);
    }
}