using DiscordRPC.Message;

// Events
using static Program;

using System.Drawing;
using Console = Colorful.Console;

public static class Events
{
    // Clear the presence and dispose if client lost connection, etc
    public static void Deinitialize()
    {
        _Client.ClearPresence();
        _Client.Dispose();
    }

    // Various messages for various events
    public static void OnReady(object sender, ReadyMessage e)
    {
        Console.WriteLine($"Received Ready from user => {e.User.Username}", Color.LimeGreen);
        Console.WriteLine($"RPC version => {e.Version}\n", Color.LimeGreen);
    }

    public static void OnConnectionEstablished(object sender, ConnectionEstablishedMessage e)
    {
        Console.WriteLine("Pipe connection established successfully", Color.LimeGreen);
    }

    public static void OnPresenceUpdate(object sender, PresenceMessage e)
    {
        Console.WriteLine($"Received Update => {e.Presence}", Color.LimeGreen);
    }

    public static void OnError(object sender, ErrorMessage e)
    {
        Console.WriteLine($"An error occured => ({e.Code}) {e.Message}", Color.Red);
        Deinitialize();
    }

    public static void OnClose(object sender, CloseMessage e)
    {
        Console.WriteLine($"Lost connection with client => {e.Reason}", Color.Red);
        Deinitialize();
    }

    public static void OnConnectionFailed(object sender, ConnectionFailedMessage e)
    {
        Console.WriteLine("Pipe connection failed", Color.Red);
        Deinitialize();
    }
}
