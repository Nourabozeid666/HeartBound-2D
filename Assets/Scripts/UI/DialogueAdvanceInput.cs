using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueAdvanceInput : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference advanceAction;   // drag your action here
    [Header("Dialogue")]
    public NPC npc;                              // drag your NPC here

    void OnEnable()
    {
        if (advanceAction != null)
        {
            advanceAction.action.performed += OnAdvance;
            advanceAction.action.Enable();      // start listening
        }
    }

    void OnDisable()
    {
        if (advanceAction != null)
        {
            advanceAction.action.performed -= OnAdvance;
            advanceAction.action.Disable();     // stop listening
        }
    }

    void OnAdvance(InputAction.CallbackContext _)
    {
        if (npc != null) npc.NextLine();
    }
}
