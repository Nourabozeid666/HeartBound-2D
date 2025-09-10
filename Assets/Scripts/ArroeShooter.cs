using UnityEngine;

public class ArroeShooter : MonoBehaviour
{
    public BowEnemy owner;
    public void Anim_Shoot() { owner?.Anim_Shoot(); }
}
