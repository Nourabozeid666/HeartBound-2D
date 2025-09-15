using UnityEngine;

public class PlayerBootstrap : MonoBehaviour
{
    private void OnEnable()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.RegisterPlayer(gameObject);
    }
}
