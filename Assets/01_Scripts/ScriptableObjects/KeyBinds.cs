using UnityEngine;


[CreateAssetMenu(fileName = "Keycodes", menuName = "Tools/new KeyCode set")]

public class KeyBinds : ScriptableObject
{
    [Header("Player Interactions")]
    public KeyCode interact = KeyCode.E;
    public KeyCode showStats = KeyCode.Tab;
    public KeyCode menu = KeyCode.Escape;

    [Header("Player Movement")]
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode crouch = KeyCode.C;
    public KeyCode jump = KeyCode.Space;

    [Header("Player Combat")]
    public KeyCode useItem = KeyCode.Mouse0;
    public KeyCode aim = KeyCode.Mouse1;
    public void ResetKeyBinds()
    {
        interact = KeyCode.F;
        menu = KeyCode.Escape;

        sprint = KeyCode.LeftShift;
        crouch = KeyCode.C;
        jump = KeyCode.Space;

        useItem = KeyCode.Mouse0;
        aim = KeyCode.Mouse1;

        Debug.Log("Key bindings reset to default values.");
    }


  
}