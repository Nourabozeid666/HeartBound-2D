using UnityEngine;

public class ChasingController : MonoBehaviour
{
   
    public bool AwareOfPlayer { get; private set; }
    public Vector2 DirToPlayer { get; private set; }


    [SerializeField] float MaxDistance = 10;

    Transform Player;

    private void Awake()
    {
        Player = FindAnyObjectByType<PlayerMovement>().transform;
    }
    void Update()
    {
        Vector2 DistanceinBetween = Player.position - transform.position;
        DirToPlayer = DistanceinBetween.normalized;
        if (DistanceinBetween.magnitude <= MaxDistance)
            AwareOfPlayer = true;
        else
            AwareOfPlayer = false;
    }
}
