# InfoDamage – Simple Damage Indicator for SwiftlyS2

A lightweight SwiftlyS2 plugin that shows damage information to players using the Alert UI system.  
Supports permissions, configurable visibility, and automatic config reload.

---

## ✨ Features

- Shows damage dealt (HP, Armor, or both)
- Optionally displays the victim’s name
- Permission-based visibility
- Uses the clean **Alert UI** system
- Auto-reload when the config file changes
- Simple configuration and lightweight performance

---

## 📦 Installation

1. Go to the **Releases** page.
2. Download the latest **InfoDamage.zip**.
3. Place `InfoDamage` inside the folder.
4. Restart your server.

---

## ⚙️ Configuration (`config.jsonc`)

```jsonc
{
  "ConfigModel": {
    "Enabled": true,
    "ShowVictim": true,
    "ShowDamage": 3,   // 1 = HP, 2 = Armor, 3 = Both
    "Permissions": ""  // "" = everyone; or e.g. "plugin.vip"
  }
}


