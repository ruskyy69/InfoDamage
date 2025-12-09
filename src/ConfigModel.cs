namespace InfoDamage;

public class ConfigModel
{
    public bool Enabled { get; set; } = true;
    /// <summary>
    /// Show victim's name
    /// </summary>
    public bool ShowVictim { get; set; } = true;
    /// <summary>
    /// Show damage dealt
    /// 1: Health
    /// 2: Armor
    /// 3: Both
    /// </summary>
    public int ShowDamage { get; set; } = 3;
    /// <summary>
    /// What type of permission is required to see damage info
    /// "" = Everyone
    /// </summary>
    public string Permissions { get; set; } = "";
}