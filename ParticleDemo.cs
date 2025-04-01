using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace ParticleDemo
{
    // ---- Config class for particle settings
    public class ParticleConfig
    {
        public string ParticleFile { get; set; } = "particles/explosions_fx/explosion_c4_short.vpcf";
        // ---- SpawnMethod: 1=InFront, 2=Feet, 3=Above, 4=Circle, 5=Line
        public int SpawnMethod { get; set; } = 1;
        public bool Debug { get; set; } = true;
    }

    public class ParticleDemo : BasePlugin
    {
        public override string ModuleName => "ParticleDemo";
        public override string ModuleVersion => "1.0.0";
        public override string ModuleAuthor => "qazlll456 with xAI assistance";

        private ParticleConfig Config = new ParticleConfig();

    public override void Load(bool hotReload)
    {
        string configPath = Path.Combine(ModuleDirectory, "particle_config.json");
        if (File.Exists(configPath))
        {
            Config = JsonSerializer.Deserialize<ParticleConfig>(File.ReadAllText(configPath)) ?? Config;
        }
        else
        {
            // ---- Write clean JSON without comments
            File.WriteAllText(configPath, JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true }));
        }
        Server.PrintToConsole("[ParticleDemo] Plugin loaded");

        RegisterListener<Listeners.OnServerPrecacheResources>(manifest => manifest.AddResource(Config.ParticleFile));
        AddCommand("css_pcfire", "Spawn a particle using config settings", OnParticleFire);
        AddCommand("css_pcreload", "Reload the particle config", OnParticleReload);
    }

    public void OnParticleReload(CCSPlayerController? player, CommandInfo command)
    {
        string configPath = Path.Combine(ModuleDirectory, "particle_config.json");
        if (!File.Exists(configPath))
        {
            Server.PrintToConsole("[ParticleDemo] ERROR: Config file not found");
            if (player != null && Config.Debug) player.PrintToChat("Config file not found!");
            return;
        }

        try
        {
            Config = JsonSerializer.Deserialize<ParticleConfig>(File.ReadAllText(configPath)) ?? Config;
            Server.PrintToConsole($"[ParticleDemo] Reloaded: Particle={Config.ParticleFile}, Method={Config.SpawnMethod}");
            if (player != null) player.PrintToChat("Particle config reloaded! Restart map for new particle file.");
        }
        catch (Exception ex)
        {
            Server.PrintToConsole($"[ParticleDemo] ERROR: Reload failed - {ex.Message}");
            if (player != null && Config.Debug) player.PrintToChat("Failed to reload config!");
        }
    }

        // ---- Core function to spawn a particle at a position
        private void SpawnParticle(Vector position, CCSPlayerController player)
        {
            var particle = Utilities.CreateEntityByName<CEnvParticleGlow>("env_particle_glow");
            if (particle == null || !particle.IsValid)
            {
                if (Config.Debug)
                {
                    Server.PrintToConsole("[ParticleDemo] ERROR: Particle creation failed");
                    player.PrintToChat("Failed to spawn particle!");
                }
                return;
            }

            particle.EffectName = Config.ParticleFile;
            particle.ColorTint = Color.Green; // ---- Optional tint
            particle.Teleport(position, new QAngle(0, 0, 0), null);
            particle.DispatchSpawn();
            particle.AcceptInput("Start");

            if (Config.Debug) Server.PrintToConsole($"[ParticleDemo] Spawned at ({position.X}, {position.Y}, {position.Z})");
        }

        // ---- 1: Spawn in front of player (100 units)
        private void SpawnInFront(CCSPlayerController player)
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn?.AbsOrigin == null || pawn.AbsRotation == null) return;

            float yaw = pawn.AbsRotation.Y * (float)(Math.PI / 180.0);
            var pos = new Vector(
                pawn.AbsOrigin.X + (float)(Math.Cos(yaw) * 100.0f),
                pawn.AbsOrigin.Y + (float)(Math.Sin(yaw) * 100.0f),
                pawn.AbsOrigin.Z
            );
            SpawnParticle(pos, player);
        }

        // ---- 2: Spawn at player’s feet (same location)
        private void SpawnAtFeet(CCSPlayerController player)
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn?.AbsOrigin == null) return;
            SpawnParticle(pawn.AbsOrigin, player);
        }

        // ---- 3: Spawn above player (50 units up)
        private void SpawnAbovePlayer(CCSPlayerController player)
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn?.AbsOrigin == null) return;
            var pos = new Vector(pawn.AbsOrigin.X, pawn.AbsOrigin.Y, pawn.AbsOrigin.Z + 50.0f);
            SpawnParticle(pos, player);
        }

        // ---- 4: Spawn circle around player (radius 100, 5 particles)
        private void SpawnCircleAroundPlayer(CCSPlayerController player)
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn?.AbsOrigin == null) return;
            int count = 5;
            float radius = 100.0f;
            for (int i = 0; i < count; i++)
            {
                float angle = (float)(2 * Math.PI * i / count);
                var pos = new Vector(
                    pawn.AbsOrigin.X + (float)(Math.Cos(angle) * radius),
                    pawn.AbsOrigin.Y + (float)(Math.Sin(angle) * radius),
                    pawn.AbsOrigin.Z
                );
                SpawnParticle(pos, player);
            }
        }

        // ---- 5: Spawn along line in front (100 units, 5 particles)
        private void SpawnAlongLine(CCSPlayerController player)
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn?.AbsOrigin == null || pawn.AbsRotation == null) return;
            float yaw = pawn.AbsRotation.Y * (float)(Math.PI / 180.0);
            int count = 5;
            float length = 100.0f;
            float step = length / count;
            for (int i = 1; i <= count; i++)
            {
                var pos = new Vector(
                    pawn.AbsOrigin.X + (float)(Math.Cos(yaw) * step * i),
                    pawn.AbsOrigin.Y + (float)(Math.Sin(yaw) * step * i),
                    pawn.AbsOrigin.Z
                );
                SpawnParticle(pos, player);
            }
        }

        // ---- Command handler for !pcfire
        public void OnParticleFire(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || !player.IsValid || !player.PawnIsAlive)
            {
                if (Config.Debug) Server.PrintToConsole("[ParticleDemo] ERROR: Player invalid or not alive");
                return;
            }

            // ---- Choose spawn method based on config
            switch (Config.SpawnMethod)
            {
                case 1: SpawnInFront(player); break;
                case 2: SpawnAtFeet(player); break;
                case 3: SpawnAbovePlayer(player); break;
                case 4: SpawnCircleAroundPlayer(player); break;
                case 5: SpawnAlongLine(player); break;
                default:
                    if (Config.Debug)
                    {
                        Server.PrintToConsole("[ParticleDemo] ERROR: Invalid SpawnMethod, using default (1)");
                        player.PrintToChat("Invalid spawn method in config, using in-front!");
                    }
                    SpawnInFront(player); break;
            }

            if (Config.Debug) player.PrintToChat("Particle spawned!");
        }
    }
}