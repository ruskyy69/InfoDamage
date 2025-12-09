using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.GameEvents;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Plugins;

namespace InfoDamage;

[PluginMetadata(
    Id = "InfoDamage",
    Version = "1.0.1",
    Name = "Simple InfoDamage Alert",
    Author = "rusky",
    Description = "Simple InfoDamage using alerts"
)]
public class InfoDamage : BasePlugin
{
    private readonly ILogger _logger;

    private IOptionsMonitor<ConfigModel> _configMonitor = null!;
    private ConfigModel _config = new();

    public InfoDamage(ISwiftlyCore core) : base(core)
    {
        _logger = Core.LoggerFactory.CreateLogger<InfoDamage>();
    }

    public override void Load(bool hotReload)
    {
        try
        {
            const string fileName = "config.jsonc";
            const string section = "ConfigModel";

            // Load config
            Core.Configuration.InitializeJsonWithModel<ConfigModel>(fileName, section);
            Core.Configuration.Configure(cfg => cfg.AddJsonFile(fileName, false, true));

            // DI container
            var services = new ServiceCollection();
            services.AddSwiftly(Core)
                    .AddOptionsWithValidateOnStart<ConfigModel>()
                    .BindConfiguration(section);

            var provider = services.BuildServiceProvider();

            _configMonitor = provider.GetRequiredService<IOptionsMonitor<ConfigModel>>();
            _config = _configMonitor.CurrentValue ?? new ConfigModel();

            _configMonitor.OnChange(cfg =>
            {
                _config = cfg;
                _logger.LogInformation("[InfoDamage] Configuration reloaded.");
            });

            _logger.LogInformation("[InfoDamage] Loaded successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[InfoDamage] Failed to load plugin.");
        }
    }

    [GameEventHandler(HookMode.Post)]
    public HookResult OnPlayerHurt(EventPlayerHurt hurt)
    {
        try
        {
            var attacker = Core.PlayerManager.GetPlayer(hurt.Attacker);
            var victim   = Core.PlayerManager.GetPlayer(hurt.UserId);

            if (attacker == null || !_config.Enabled)
                return HookResult.Continue;

            string victimName = victim?.Controller.PlayerName ?? "Unknown";
            int dmgHealth = hurt.DmgHealth;
            int dmgArmor  = hurt.DmgArmor;

            // Permission check
            if (!string.IsNullOrWhiteSpace(_config.Permissions))
            {
                if (!Core.Permission.PlayerHasPermission(attacker.SteamID, _config.Permissions))
                    return HookResult.Continue;
            }

            string msg = BuildMessage(victimName, dmgHealth, dmgArmor);
            if (!string.IsNullOrWhiteSpace(msg))
                attacker.SendAlertAsync(msg);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[InfoDamage] Error in OnPlayerHurt");
        }

        return HookResult.Continue;
    }

    private string BuildMessage(string victim, int hp, int armor)
    {
        if (_config.ShowVictim)
        {
            return _config.ShowDamage switch
            {
                1 => $"[{victim}]\n{Core.Localizer["health"]}: -{hp}",
                2 => $"[{victim}]\n{Core.Localizer["armor"]}: -{armor}",
                3 => $"[{victim}]\n{Core.Localizer["damage"]}: -{hp} | {Core.Localizer["armor"]}: -{armor}",
                _ => ""
            };
        }
        else
        {
            return _config.ShowDamage switch
            {
                1 => $"{Core.Localizer["health"]}: -{hp}",
                2 => $"{Core.Localizer["armor"]}: -{armor}",
                3 => $"{Core.Localizer["damage"]}: -{hp} | {Core.Localizer["armor"]}: -{armor}",
                _ => ""
            };
        }
      }


    public override void Unload()
    {
        _logger.LogInformation("[InfoDamage] Plugin unloaded.");
    }
}

