using System;
using System.Windows.Forms;
using AdaptedGameCollection.Game.Network;
using AdaptedGameCollection.Logging;
using AdaptedGameCollection.Protocol;
using LiteNetwork;
using LiteNetwork.Client;

namespace AdaptedGameCollection.Game;

/// <summary>
/// The main entry point for the application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// The current config instance loaded.
    /// </summary>
    internal static Config Config { get; private set; } = null!;
    
    /// <summary>
    /// The current main form instance.
    /// </summary>
    internal static MainForm Form { get; private set; }

    private static readonly Logger Logger = LoggerFactory.Create<NetworkClient>();
    
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        var config = Config.Load();
        if (config == null)
        {
            MessageBox.Show("Die Konfigurationsdatei ist korrupt!", "Fehler!", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }
        
        Config = config;
        LoggerFactory.IsDebug = Config.IsDebug;
        if (LoggerFactory.IsDebug) LoggerFactory.OpenConsole();
        PacketRegistry.Initialize();
        
        if (!ConnectToServer())
        {
            MessageBox.Show("Verbindung zum Server ist fehlgeschlagen!", "Fehler!", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }
        
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(Form = new MainForm());
    }

    private static bool ConnectToServer()
    {
        try
        {
            var client = new NetworkClient(new LiteClientOptions
            {
                Host = Config.Host,
                Port = Config.Port,
                ReceiveStrategy = ReceiveStrategyType.Queued
            });
            client.ConnectAsync().GetAwaiter().GetResult();
            return client.Socket.Connected;
        }
        catch(Exception ex)
        {
            Logger.Error(ex, "An error occurred while connecting to the server!");
            return false;
        }
    }
}