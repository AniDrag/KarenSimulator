using UnityEngine;


[CreateAssetMenu(fileName = "Keycodes", menuName = "Tools/new KeyCode set")]

public class KeyBinds : ScriptableObject
{
    [Header("Player Interactions")]
    public KeyCode interact = KeyCode.E;
    public KeyCode showStats = KeyCode.Tab;
    public KeyCode menu = KeyCode.Escape;

    [Header("Player Movement")]
    public KeyCode sprintHold = KeyCode.LeftShift;
    public KeyCode sprintToggle = KeyCode.LeftShift; // only one sprint is fesable
    public KeyCode crouchToggle = KeyCode.C;
    public KeyCode crouchHold = KeyCode.LeftControl;
    public KeyCode walkToggle = KeyCode.CapsLock;
    public KeyCode jump = KeyCode.Space;

    [Header("Player Combat")]
    public KeyCode useItem = KeyCode.Mouse0;
    public KeyCode aim = KeyCode.Mouse1;
    public void ResetKeyBinds()
    {
        interact = KeyCode.E;
        menu = KeyCode.Escape;

        sprintHold = KeyCode.LeftShift;
        sprintToggle = KeyCode.LeftShift;
        crouchToggle = KeyCode.C;
        crouchHold = KeyCode.LeftControl;
        walkToggle = KeyCode.CapsLock;
        jump = KeyCode.Space;

        useItem = KeyCode.Mouse0;
        aim = KeyCode.Mouse1;

        Debug.Log("Key bindings reset to default values.");
    }


  
}