using UnityEditor;
using UnityEngine;

public class Witch : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] Transform player;
    [SerializeField] GameObject Skeleton;

    [Header("Range")]
    [SerializeField] float AttackDistance = 7f;
    [SerializeField] CircleCollider2D SpawnRange;

    [SerializeField] float numberOfSkelton = 4f;
   

    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>().transform;
    }

    void Update()
    {
        float Distance = Vector2.Distance(this.transform.position, player.position);
        if(Distance <= AttackDistance)
        {
            SpawmSkeleton();
        }
        else
        {

        }
    }

    void SpawmSkeleton()
    {
        
    }
}
