using System;
using System.Collections.Generic;
using Nakama;
using Nakama.TinyJson;
using UnityEngine;

public static class GameController {
    public const int ACTION_JUMP = 1;
    public const int ACTION_PICKED_CHARACTER = 2;
    public const int ACTION_READY = 3;

    public static Queue<IMatchState> recievedActions = new Queue<IMatchState>();

    public static Action<IMatchState> RecieveState = newState => {
        string content = "";
        if (newState.State != null) {
            var enc = System.Text.Encoding.UTF8;
            content = enc.GetString(newState.State);
        }

        switch (newState.OpCode) {
            case ACTION_JUMP:
                recievedActions.Enqueue(newState);
                Debug.Log("Jump!!!");
                break;
            case ACTION_PICKED_CHARACTER:
                GlobalModel.SetMyCharacter((int.Parse(content) + 1) % 2);
                break;
            case ACTION_READY:
                GlobalModel.OppenentReady = true;
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
