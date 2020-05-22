using System.Collections.Generic;
using Nakama;

public static class GlobalModel {
	public static IApiAccount Me { get; set; }
    public static IUserPresence Opponent { get; set; }
    public static bool OppenentReady { get; set; } = false;

    public static int MyCharacter { get; private set; } = -1;
    public static int OppenentCharacter => (MyCharacter + 1) % 2;

    public const int CHARACTER_CHICK = 0;
    public const int CHARACTER_CAT = 1;

    public static void SetMyCharacter(int charCode) => MyCharacter = charCode;

    public static List<(float, float)> GeneratedSplinePoints; //Ground control points
    public static bool GameFinished = false;
    public static int RecievedWinner = -1;

    public static int CharacterId(string name) {
        switch (name) {
            case "Chick":
                return CHARACTER_CHICK;
            case "Cat":
                return CHARACTER_CAT;
            default:
                return -1;
        }
    }

    public static void ResetGameFlags() {
        GeneratedSplinePoints = null;
        Opponent = null;
        GameFinished = false;
        OppenentReady = false;
        RecievedWinner = -1;
        MyCharacter = -1;
    }
}