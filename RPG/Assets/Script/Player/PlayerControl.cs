using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public int Turn;

    public PlayerControl instance;


    // Start is called before the first frame update

    void Awake()
    {
        instance = this;
        Turn = 0;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void PlayerMove()
    {

    }
}
