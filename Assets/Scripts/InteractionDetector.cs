using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private Iinteractable iinteractableInRange = null;
    public GameObject interactionIcon;
    private void Start()
    {
        interactionIcon.SetActive(false);
    }

    public void OnInteraction(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            iinteractableInRange?.interact();
            interactionIcon.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Iinteractable iinteractable) && iinteractable.canInteract())
        {
            interactionIcon.SetActive(true);
            iinteractableInRange = iinteractable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Iinteractable iinteractable) && iinteractableInRange == iinteractable)
        {
            interactionIcon.SetActive(false);
            iinteractableInRange = null;
        }
    }
}
