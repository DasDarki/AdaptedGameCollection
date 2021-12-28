using System.IO;
using Tomlet;

namespace AdaptedGameCollection.Server;

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
    /// To which hostname the server should be bound to.
    /// </summary>
    internal string BindingHost { get; set; } = "127.0.0.1";

    /// <summary>
    /// The port which will be used for the server.
    /// </summary>
    internal int Port { get; set; } = 29563;

    /// <summary>
    /// Loads the configuration. If no config is existing, a new one is created.
    /// </summary>
    /// <returns></returns>
    internal static Config? Load()
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