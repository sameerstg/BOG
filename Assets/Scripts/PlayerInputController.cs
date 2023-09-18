using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public InputControls inputActions;
    public InputControls.PlayerActions playerActions;
    public Player player;
    public PhotonView photonView;
    private void Awake()
    {
        photonView =GetComponent<PhotonView>();
        inputActions = new();
        playerActions = inputActions.Player;
        player = GetComponent<Player>();
    }
    private void OnEnable()
    {
        playerActions.Enable();
        playerActions.Movement.started += Move;
        playerActions.Movement.canceled += Move;
        playerActions.Jump.started += Jump;
        playerActions.Action.started += Fire;
    }

    private void Fire(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine)
            return;

        player.Fire(context);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine)
            return;
        player.Jump(context);
    }

    private void Move(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine)
            return;
        player.Move(context);
    }

    private void OnDisable()
    {
        playerActions.Disable();
        playerActions.Movement.started -= Move;
        playerActions.Movement.canceled -= Move;
        playerActions.Jump.started -= Jump;
        playerActions.Action.started -= Fire;
    }
}
