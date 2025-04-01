# ParticleDemo

## About
ParticleDemo is a CounterStrikeSharp plugin for CS2 that demonstrates spawning particle effects in various positions (in front, at feet, above, circle, line). It’s an educational tool for plugin developers.

## Features
- Spawn particles in five different ways: in front, at feet, above, in a circle, or along a line.
- Configurable via a JSON file (`particle_config.json`).
- Reload config in-game with a command—no server restart needed for most changes.
- Debug mode for detailed logs and in-game feedback.
- Simple, commented code to help beginners understand particle spawning.

## Requirements
- **Counter-Strike 2 Server**: A running CS2 server.
- **Metamod:Source**: Installed on your server for plugin support. Download from [sourcemm.net](https://www.sourcemm.net/).
- **CounterStrikeSharp**: The plugin framework installed on your server. Get it from [CounterStrikeSharp GitHub](https://github.com/roflmuffin/CounterStrikeSharp).
- **.NET SDK**: Required to build the plugin (e.g., .NET 8.0).

## Installation
1. **Clone or Download**:
   - Clone this repository or download the ZIP:
     ```bash
     git clone https://github.com/<your-username>/ParticleDemo.git
     ```
   - Or download from the Releases tab (if you add releases later).

2. **Build the Plugin**:
   - Open a terminal in the `ParticleDemo` folder.
   - Run:
     ```bash
     dotnet build
     ```
   - Find the compiled DLL in `bin/Debug/net8.0/ParticleDemo.dll`.

3. **Deploy to Server**:
   - Copy `ParticleDemo.dll` to your CS2 server’s plugin folder:
     - Typically: `csgo/addons/counterstrikesharp/plugins/ParticleDemo/`.
   - Ensure the folder is named `ParticleDemo` to match the plugin’s expected structure.

4. **Config Setup**:
   - On first load, the plugin creates `particle_config.json` in the plugin folder.
   - Edit it to customize (see Configuration section below).

5. **Start Server**:
   - Launch your CS2 server. The plugin will load automatically.

## Configuration
The plugin uses `particle_config.json` for settings. It’s created with defaults on first load. Here’s the structure:

```json
{
  "ParticleFile": "particles/explosions_fx/explosion_c4_short.vpcf",
  "SpawnMethod": 1, // Choose: 1=In Front, 2=At Feet, 3=Above, 4=Circle, 5=Line
  "Debug": true // Set to false to disable extra logs and chat messages
}
```

### Options Explained
- **`ParticleFile`**:
  - Path to the particle file (`.vpcf`) to spawn.
  - Default: `particles/explosions_fx/explosion_c4_short.vpcf` (a built-in CS2 explosion effect).
  - Change to any valid particle file in your CS2 installation.

- **`SpawnMethod`**:
  - Integer selecting where the particle spawns:
    - `1`: 100 units in front of the player.
    - `2`: At the player’s exact position (feet).
    - `3`: 50 units above the player.
    - `4`: Circle around the player (100-unit radius, 5 particles).
    - `5`: Line in front (100 units, 5 particles).
  - Default: `1`.

- **`Debug`**:
  - Boolean (`true`/`false`) enabling extra console logs and in-game chat messages.
  - Default: `true` (recommended for testing).

### Notes
- Changing `ParticleFile` requires a map restart or server reload to precache the new file.
- `SpawnMethod` and `Debug` changes apply instantly after reloading (see Commands).

## Commands
- **`!pcfire`**:
  - Spawns the configured particle using the selected `SpawnMethod`.
  - Usage: Type `!pcfire` in chat or `css_pcfire` in console.
  - Example: Spawns an explosion 100 units in front if `SpawnMethod` is 1.

- **`!pcreload`**:
  - Reloads `particle_config.json` without restarting the server.
  - Usage: Type `!pcreload` in chat or `css_pcreload` in console.
  - Note: New particle files need a map restart to take effect.

## Usage Example
1. Edit `particle_config.json`:
   ```json
   {
     "ParticleFile": "particles/explosions_fx/explosion_c4_short.vpcf",
     "SpawnMethod": 3,
     "Debug": true
   }
   ```

2. In-game:
   - Type `!pcreload` to load the updated config.
   - Type `!pcfire` to spawn the particle 50 units above you.
   - Check console for logs like `[ParticleDemo] Spawned at (x, y, z)`.

3. See chat messages (with `Debug: true`):
   - “Particle spawned!” on success.
   - “Failed to spawn particle!” if something goes wrong.

## Development
### Building Locally
- Open the project in VS Code or any C# IDE.
- Run:
  ```bash
  dotnet build
  ```
- Output DLL is in `bin/Debug/net8.0/`.

### Modifying
- Edit `ParticleDemo.cs` to tweak spawn distances, add new methods, or change behavior.
- Comments in the code explain each function (e.g., `SpawnInFront`, `SpawnCircleAroundPlayer`).

### Debugging
- Set `Debug: true` in the config.
- Watch server console for logs.
- Check in-game chat for immediate feedback.

## Known Limitations
- **Particle Precache**: Changing `ParticleFile` requires a map restart or server reload to precache properly.
- **Single Particle**: Only one particle type can be configured at a time.
- **Hardcoded Values**: Distances (e.g., 100 units) and counts (e.g., 5 particles) are fixed in code—edit the source to adjust.

## Contributing
Feel free to fork this repo, make improvements, and submit pull requests! Suggestions:
- Add configurable distances or particle counts.
- Support multiple particle files.
- Improve error handling.

## Credits
- **Author**: qazlll456, with assistance from xAI’s Grok.
- **Framework**: CounterStrikeSharp team.

## Thanks
Big thanks to the CounterStrikeSharp Discord community for their guidance and debugging help! Special shoutouts to:
- **AquaVadis**: Main mentor who taught me the ropes.
- **fifty Shucks**: For valuable support and insights.
- **exkludera**: For assisting with troubleshooting.
This plugin wouldn’t be here without their patience and expertise!

## License
This project is open-source under the [MIT License](LICENSE). Feel free to use, modify, and distribute it.

## Donate
If you enjoy this plugin or find it useful, consider supporting me with a donation! Every bit helps me keep developing.
- **PayPal**: [Donate Here](https://www.paypal.com/paypalme/qazlll456)
- **Ko-fi**: [Support on Ko-fi](https://ko-fi.com/qazlll456)
- **Patreon**: [Become a Patron](https://www.patreon.com/c/qazlll456)
- **Streamlabs**: [Tip via Streamlabs](https://streamlabs.com/BKCqazlll456/tip)