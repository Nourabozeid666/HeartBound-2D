using UnityEngine;

public class GunIconDisappear : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            Destroy(this.gameObject);
            var gunTr = collision.transform.Find("Gun");
            gunTr.gameObject.SetActive(true);
        }
    }
}
