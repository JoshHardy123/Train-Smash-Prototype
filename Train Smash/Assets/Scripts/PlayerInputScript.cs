using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputScript : MonoBehaviour
{
     PathFollower playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PathFollower>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.gameMan.gameStarted)
        {
            if (Input.GetButton("Fire1") && playerMovement.canMove) playerMovement.currentState = PathFollower.TrainStates.Accelerating;
            else if (playerMovement.canMove) playerMovement.currentState = PathFollower.TrainStates.Coasting;
        }
        else
        {
            //if (Input.GetButton("Fire1"))
            //{
            //    //StartLevel
            //    GameManager.gameMan.StartGame();
            //    playerMovement.InitiatePlayer();
            //}
        }
        
    }
}
