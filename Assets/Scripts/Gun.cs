using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] float offset=-90;
    [SerializeField] GameObject weakBullets;
    [SerializeField] GameObject strongBullets;
    [SerializeField] Transform shotPoint;
    [SerializeField] GameObject gun;
    [SerializeField] float timeBetweenStrongShoots;
    [SerializeField] float timeBetweenSmallerShoots = 0.09f;
    [SerializeField] float reloadTime = 0.75f;
    [SerializeField] int bulletsNumber = 0;
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
    public void Attack_1()
    {
        if (gun.activeInHierarchy)
        {
            if (bulletsNumber <= 12)
            {
                Instantiate(weakBullets, shotPoint.position, shotPoint.rotation);
                bulletsNumber++;
                StartCoroutine(Attack_1Cooldown());
            }
            else
            {
                bulletsNumber = 0;
                StartCoroutine(Attack_1Cooldown());
            }
        }
    }
    public void Attack_2()
    {
        if (gun.activeInHierarchy)
        {
            Instantiate(strongBullets, shotPoint.position, shotPoint.rotation);
            StartCoroutine(StrongAttackCooldown());
        }
    }

    IEnumerator StrongAttackCooldown()
    {
        yield return new WaitForSeconds(timeBetweenStrongShoots);
    }
    IEnumerator Attack_1Cooldown()
    {
        yield return new WaitForSeconds(timeBetweenStrongShoots);
    }
    IEnumerator Attack_1Inbetween()
    {
        yield return new WaitForSeconds(timeBetweenSmallerShoots);
    }

}
