using System;
using Nakama;
using UnityEngine;

public static class GameController
{
    public static Action<IMatchState> recievedState = newState => {
        var enc = System.Text.Encoding.UTF8;
        var content = enc.GetString(newState.State);

        switch (newState.OpCode) {
            case 101:
                Debug.Log("A custom opcode.");
                break;
            default:
                Debug.LogFormat("User '{0}'' sent '{1}'", newState.UserPresence.Username, content);
                break;
        }
    };

    
}
