using Nakama;

public static class GlobalModel {
	public static IApiAccount Me { get; set; }
    public static IUserPresence Opponent { get; set; }
    public static bool OppenentReady { get; set; } = false;

    public static int MyCharacter { get; private set; } = 0;

    public const int CHARACTER_CHICK = 0;
    public const int CHARACTER_CAT = 1;

    public static void SetMyCharacter(int charCode) => MyCharacter = charCode;
}