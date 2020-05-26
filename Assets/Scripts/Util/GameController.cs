using System;
using System.Collections.Generic;
using Nakama;
using Nakama.TinyJson;
using UnityEngine;

public static class GameController {
    public const int ACTION_JUMP = 1;
    public const int ACTION_PICKED_CHARACTER = 2;
    public const int ACTION_READY = 3;
    public const int ACTION_GROUND_GENERATED = 4;
    public const int ACTION_POSITION_UPDATE = 5;
    public const int ACTION_GAME_OVER = 6;
    public const int ACTION_OBSTACLES_GENERATED = 7;

    public static Queue<(float, float)> recievedJumps = new Queue<(float,float)>();
    public static Queue<(float, float)> recievedPositions = new Queue<(float, float)>();

    public static Action<IMatchState> RecieveState = newState => {
        string content = "";
        if (newState.State != null) {
            var enc = System.Text.Encoding.UTF8;
            content = enc.GetString(newState.State);
        }

        switch (newState.OpCode) {
            case ACTION_JUMP:
                recievedJumps.Enqueue(content.FromJson<(float, float)>());
                Debug.Log("Jump!!!");
                break;
            case ACTION_PICKED_CHARACTER:
                GlobalModel.SetMyCharacter((int.Parse(content) + 1) % 2);
                break;
            case ACTION_READY:
                GlobalModel.OppenentReady = true;
                break;
            case ACTION_GROUND_GENERATED:
                GlobalModel.GeneratedSplinePoints = content.FromJson<List<(float, float)>>();
                break;
            case ACTION_POSITION_UPDATE:
                recievedPositions.Enqueue(content.FromJson<(float, float)>());
                break;
            case ACTION_GAME_OVER:
                GlobalModel.RecievedWinner = int.Parse(content);
                break;
            case ACTION_OBSTACLES_GENERATED:
                GlobalModel.GeneratedObstaclePositions = content.FromJson<List<(float, int)>>();
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
