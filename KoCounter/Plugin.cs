using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using KoCounter.Windows;

namespace KoCounter;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog PluginLog { get; private set; } = null!;
    [PluginService] public static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] public static IFramework Framework { get; private set; } = null!;


    private const string CommandName = "/kocounter";
    private const string CommandName2 = "/koc";

    public static Configuration Configuration { get; set; }

    public readonly WindowSystem WindowSystem = new("KoCounter");
    private static Logic.KoCounter _koCounter;

    private MainWindow MainWindow { get; init; }
    public static KnockoutDisplay KnockoutDisplay { get; set; }
    public static Logic.KoCounter KoCounter { get => _koCounter; set => _koCounter = value; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // you might normally want to embed resources and load them from the manifest stream
        var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

        MainWindow = new MainWindow(this, goatImagePath);
        KnockoutDisplay = new KnockoutDisplay(this, goatImagePath);

        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(KnockoutDisplay);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens knockout counter"
        });
        CommandManager.AddHandler(CommandName2, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens knockout counter"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        _koCounter = new KoCounter.Logic.KoCounter();
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        KnockoutDisplay.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        string[] splitArgs = args.Split(' ');
        switch (splitArgs[0].ToLower())
        {
            case "debug":
                if (splitArgs.Length > 1 && Plugin.Configuration.DebugCommands)
                {
                    switch (splitArgs[1])
                    {
                        case "increment":
                            KoCounter.DebugIncrement();
                            break;
                        case "defeat":
                            KoCounter.DebugDefeat();
                            break;
                        case "reset":
                            KoCounter.DebugNewSession();
                            break;
                    }
                }
                break;
            case "debug defeat":
                break;
            case "debug reset":
                break;
            case "details":
                ToggleConfigUI();
                break;
            case "help":
                string help = "/koc (toggles session display)\r\n" +
                    "/koc details (toggles past sessions and settings)\r\n" +
                    (Plugin.Configuration.DebugCommands ? "/koc debug increment (increments knockout counter)\r\n" +
                    "/koc debug defeat (increments knockout counter)\r\n" +
                    "/koc debug reset (resets session)" : "");
                break;
            default:
                ToggleMainUI();
                break;
        }
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => MainWindow.Toggle();
    public void ToggleMainUI() => KnockoutDisplay.Toggle();
}
