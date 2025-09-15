using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun settings")]
    [SerializeField] float offset=-90;
    [SerializeField] Transform shotPoint;
    [SerializeField] GameObject gun;

    [Header("Attack_1")]
    [SerializeField] GameObject weakBullets;
    [SerializeField] float timeBetweenStrongShoots = 0.9f;
    public float strongShotsTime
    {
        get => timeBetweenStrongShoots;
        set => timeBetweenStrongShoots = value;
    }
    [SerializeField] float reloadTime = 0.75f;
    public float reloadtime
    {
        get => reloadTime;
        set => reloadTime = value;
    }
    [SerializeField] float timeBetweenSmallerShoots = 0.09f;
    [SerializeField] int bulletsInMag = 0;
    int magazineSize = 12;
    bool reloading = false;
    bool canFireWeak = true;

    [Header("Attack_2")]
    [SerializeField] GameObject strongBullets;
    bool canFireStrong = true;

    enum lockShooting { none, strong, weak}
    lockShooting isLocked = lockShooting.none;

    void Awake() {
        bulletsInMag = magazineSize; 
    }
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
        if (!canFireWeak || !gun.activeInHierarchy || reloading) 
            return;
        if(isLocked != lockShooting.none)
            return;
        if (gun.activeInHierarchy)
        {
            if (bulletsInMag <= magazineSize && bulletsInMag != 0)
            {
                Instantiate(weakBullets, shotPoint.position, shotPoint.rotation);
                bulletsInMag--;
                StartCoroutine(WeakCooldown());
            }
            else if(bulletsInMag == 0)
            {
                StartCoroutine(AutomaticReload());
            }
        }
    }
    public void Attack_2()
    {
        if (!gun.activeInHierarchy || !canFireStrong )
            return;
        if (isLocked != lockShooting.none)
            return;
        if (gun.activeInHierarchy)
        {
            Instantiate(strongBullets, shotPoint.position, shotPoint.rotation);
            StartCoroutine(StrongCooldown());
        }
    }

    IEnumerator StrongCooldown()
    {
        canFireStrong = false;
        isLocked = lockShooting.strong;
        yield return new WaitForSeconds(timeBetweenStrongShoots);
        canFireStrong = true;
        isLocked = lockShooting.none;
    }
    IEnumerator WeakCooldown()
    {
        isLocked = lockShooting.weak;
        canFireWeak = false;
        yield return new WaitForSeconds(timeBetweenSmallerShoots);
        canFireWeak = true;
        isLocked = lockShooting.none;
    }
    IEnumerator AutomaticReload()
    {
        if (reloading) yield break;
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        bulletsInMag = magazineSize;
        reloading = false;
    }

}
