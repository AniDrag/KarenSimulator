using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager instance;
    private void Awake()
    {
      instance = this;
    }


    public Transform playerOrientation;
    public Transform player;
    public PlayerMovemant playerMovemant;
    public PlayerInputs playerInputs;






    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
