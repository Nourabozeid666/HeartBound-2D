using System;
using UnityEngine;

public class gateAnim : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] string openTrigger = "Open";

 
    public event Action OnOpened;

    void Awake()
    {
        if (!anim) anim = GetComponentInChildren<Animator>();
    }

   
    public void Open()
    {
        if (anim && !string.IsNullOrEmpty(openTrigger))
            anim.SetTrigger(openTrigger);
        else
           
            OnOpened?.Invoke();
            
    }
    public void _AnimEvent_OnOpenFinished()
    {
        OnOpened?.Invoke();
    }
}
