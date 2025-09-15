using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] List <GameObject> weapons = new List<GameObject>();
    private void Start()
    {
        int i = Random.Range(0, weapons.Count);
        weapons[i].SetActive(true);
    }
}
