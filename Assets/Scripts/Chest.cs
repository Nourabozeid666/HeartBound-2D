using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour,Iinteractable
{
    public bool IsOpened { get; private set; }
    public string chestID { get; private set; }
    public List<GameObject> chestList = new List<GameObject>();
    public Sprite openedSprite;
    void Start()
    {
        chestID ??= GlobalHelper.GenerateGlobalID(gameObject);
    }
    public void interact()
    {
        if (!canInteract())
            return;
        OpenChest();
    }

    public bool canInteract()
    {
        return !IsOpened;
    }

    public void OpenChest()
    {
        setOpened();
        if (chestList.Count > 0)
        {
            GameObject dropped = Instantiate(chestList[0], transform.position + Vector3.down, Quaternion.identity);
        }
    }
     
    public void setOpened()
    {
        if (IsOpened) return;              
        IsOpened = true;                    
         GetComponent<SpriteRenderer>().sprite = openedSprite;
    }
}
