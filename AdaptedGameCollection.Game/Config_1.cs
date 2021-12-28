using System.IO;
using Tomlet;

namespace AdaptedGameCollection.Game;

/// <summary>
/// The config model which is used to generate a configuration TOML file to define softly some behaviours of AGC.
/// </summary>
internal class Config
{
    /// <summary>
    /// Whether AGC is in debug mode or not.
    /// </summary>
    internal bool IsDebug { get; set; } = false;
    
    /// <summary>
    /// The host ip of the server the client should connect to.
    /// </summary>
    internal string Host { get; set; } = "127.0.0.1";

    /// <summary>
    /// The port which is used by the server.
    /// </summary>
    internal int Port { get; set; } = 29563;

    /// <summary>
    /// Loads the configuration. If no config is existing, a new one is created.
    /// </summary>
    /// <returns></returns>
    internal static Config Load()
    {
        try
        {
            string file = Path.Combine(Directory.GetCurrentDirectory(), "config.toml");
            if (File.Exists(file))
            {
                return TomletMain.To<Config>(File.ReadAllText(file));
            }

            Config config = new Config();
            File.WriteAllText(file, TomletMain.TomlStringFrom(config));
            return config;
        }
        catch
        {
            return null;
        }
    }
}