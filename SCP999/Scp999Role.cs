using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using UnityEngine;
using MapEditorReborn.API.Features.Objects;
using System.Collections.Generic;
using System;
using Exiled.API.Features.Spawn;
using Exiled.API.Enums;
using YamlDotNet.Serialization;

namespace SCP999;
public class Scp999Role : CustomRole
{
    public override string Name { get; set; } = "SCP-999";
    public override string Description { get; set; } = "The tickle monster.";
    public override string CustomInfo { get; set; } = "SCP-999";
    public override uint Id { get; set; } = 999;
    public override int MaxHealth { get; set; } = 2000;
    public override Vector3 Scale { get; set; } = new(.5f, .5f, .5f);
    public override SpawnProperties SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>()
        {
            new() { Chance = 100, Location = SpawnLocationType.Inside330 } // another
        }
    };
    
    [YamlIgnore]
    public override RoleTypeId Role { get; set; } = RoleTypeId.Tutorial;
    public string SchematicName { get; set; } = "SCP999Model";
    private static SchematicObject _schematicObject;
    private static Animator _animator;
    //public static AudioPlayerBase audio;
    
    /// <summary>
    /// Adding the SCP-999 role to the player
    /// </summary>
    /// <param name="player">The player who should become SCP-999</param>
    public override void AddRole(Player player)
    {
        Log.Debug($"Player is NPC: {player.IsNPC}");
        Log.Debug($"Player Nickname: {player.Nickname}");
        Log.Debug($"Tracked Players Count: {TrackedPlayers.Count}");
        Log.Debug($"Spawn Limit: {SpawnProperties.Limit}");
        
        if (player.IsNPC || TrackedPlayers.Count >= SpawnProperties.Limit)
            return;
        
        _schematicObject = MerExtensions.SpawnSchematicForPlayer(SchematicName);
        if (_schematicObject == null)
            return;
        
        _animator = MerExtensions.GetAnimatorFromSchematic(_schematicObject);
        if (_animator == null)
            return;
        
        base.AddRole(player);
        player.Role.Set(Role, RoleSpawnFlags.None);
        player.Health = MaxHealth;
        player.IsGodModeEnabled = Plugin.Singleton.Config.IsGodModeEnabled;

        foreach (var ability in Plugin.Singleton.Config.Abilities)
        {
            player.AddItem(ability.Value.ItemType);
        }
        
        // Making the player invisible to all players
        MerExtensions.SendFakeSpawnMessage(player, Scale);
        
        _schematicObject.transform.parent = player.Transform;
        _schematicObject.transform.rotation = new Quaternion();
        _schematicObject.transform.position = player.Position + new Vector3(0, -.25f, 0);
    }

    /// <summary>
    /// Remove the role from the player
    /// </summary>
    /// <param name="player">A player who should become normal role</param>
    public override void RemoveRole(Player player)
    {
        try
        {
            _schematicObject.Destroy();

            //player.Scale = Vector3.one;
            //player.Kill(); <-

            TrackedPlayers.Remove(player);
        }
        catch { Exception ex; }
    }
}