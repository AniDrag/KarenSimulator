using UnityEngine;


[CreateAssetMenu(fileName = "Keycodes", menuName = "Tools/KeyCode Set")]

public class KeyBinds : ScriptableObject
{
    [Header("Player Interactions")]
    public KeyCode interact = KeyCode.F;
    public KeyCode screenSwitch = KeyCode.Tab;
    public KeyCode menu = KeyCode.Escape;

    [Header("Player Movement")]
    public KeyCode sprintHold = KeyCode.LeftShift;
    public KeyCode crouchToggle = KeyCode.C;
    public KeyCode jump = KeyCode.Space;

    [Header("Player Combat")]
    public KeyCode fire = KeyCode.Mouse0;
    public KeyCode aim = KeyCode.Mouse1;
    public void ResetKeyBinds()
    {
        interact = KeyCode.F;
        screenSwitch = KeyCode.Tab;
        menu = KeyCode.Escape;

        sprintHold = KeyCode.LeftShift;
        crouchToggle = KeyCode.C;
        jump = KeyCode.Space;

        fire = KeyCode.Mouse0;
        aim = KeyCode.Mouse1;

        Debug.Log("Key bindings reset to default values.");
    }


  
}