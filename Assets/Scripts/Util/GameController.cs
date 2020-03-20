using System;
using System.Collections.Generic;
using Nakama;
using UnityEngine;

public static class GameController
{
    public const int ACTION_JUMP = 1;

    public static Queue<IMatchState> recievedActions = new Queue<IMatchState>();

    public static Action<IMatchState> RecieveState = newState => {
        var enc = System.Text.Encoding.UTF8;
        var content = enc.GetString(newState.State);

        switch (newState.OpCode) {
            case ACTION_JUMP:
                recievedActions.Enqueue(newState);
                Debug.Log("Jump!!!");
                break;
            case 101:
                Debug.Log("A custom opcode.");
                break;
            default:
                Debug.LogFormat("User '{0}'' sent '{1}'", newState.UserPresence.Username, content);
                break;
        }
    };

    public static void SendState(int opCode, string state) {
        if (ServerConnection.Instance.Socket == null || ServerConnection.Instance.matchmaker.Match == null)
            return;
        ServerConnection.Instance.Socket.SendMatchStateAsync(
            ServerConnection.Instance.matchmaker.Match.Id,
            opCode,
            state
        );
    }

    

}
