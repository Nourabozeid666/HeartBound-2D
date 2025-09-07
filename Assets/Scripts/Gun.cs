using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] float offset=-90;
    [SerializeField] GameObject bullets;
    [SerializeField] Transform shotPoint;
    [SerializeField] GameObject gun;
    void Update()
    {
        //“depth”:Calculates how far the gun is in front of the camera along the camera’s view axis.
        float zDist = transform.position.z - Camera.main.transform.position.z;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, zDist));
        //Builds a direction vector from the gun to the mouse. This vector “points” where to aim; its length equals the distance to the cursor.
        Vector3 dir = mouseWorld - transform.position;
        //Converts that direction to a 2D angle (in degrees) around Z. Atan2(y, x) gives the angle vs. the X-axis;
        //Mathf.Atan2 expects (y, x) and returns the angle from the +X axis to the vector (x, y). Swapping them mirrors the angle across the line y = x
        float angle = Mathf.Atan2 (dir.y, dir.x) *Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,angle+offset);
    }
    public void Shot()
    {
        if (gun.activeInHierarchy)
        {
            Instantiate(bullets, shotPoint.position, shotPoint.rotation);
        }
    }
}
