using DiscordRPC.Message;
using System;

using static Program;

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
        Console.WriteLine($"Received Ready from user => {e.User.Username}");
        Console.WriteLine($"RPC version => {e.Version}");
    }

    public static void OnClose(object sender, CloseMessage e)
    {
        Console.WriteLine($"Lost connection with client => {e.Reason}");
        Deinitialize();
    }

    public static void OnError(object sender, ErrorMessage e)
    {
        Console.WriteLine($"A fatal error occured => ({e.Code}) {e.Message}");
        Deinitialize();
    }

    public static void OnConnectionEstablished(object sender, ConnectionEstablishedMessage e)
    {
        Console.WriteLine("Pipe connection established successfully");
    }

    public static void OnConnectionFailed(object sender, ConnectionFailedMessage e)
    {
        Console.WriteLine($"Pipe connection failed");
        Deinitialize();
    }

    public static void OnPresenceUpdate(object sender, PresenceMessage e)
    {
        Console.WriteLine($"Received Update => {e.Presence}");
    }
}
