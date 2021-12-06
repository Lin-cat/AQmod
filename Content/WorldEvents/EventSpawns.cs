﻿using AQMod.NPCs.Monsters;
using Terraria.ModLoader;
using Terraria;
using AQMod.Content.WorldEvents.DemonSiege;
using AQMod.Content.WorldEvents.GaleStreams;
using AQMod.Content.WorldEvents.GlimmerEvent;
using System.Collections.Generic;
using AQMod.NPCs.Monsters.GaleStreams;

namespace AQMod.Content.WorldEvents
{
    public class EventSpawns : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.GetModPlayer<AQPlayer>().bossrush)
            {
                spawnRate *= 10;
                maxSpawns = (int)(maxSpawns * 0.1);
            }
            else if (AQMod.ShouldReduceSpawns())
            {
                spawnRate = 1000;
                maxSpawns = 0;
                return;
            }
            if (DemonSiege.DemonSiege.CloseEnoughToDemonSiege(player))
            {
                spawnRate *= 10;
                maxSpawns = (int)(maxSpawns * 0.1);
            }
            else
            {
                if (player.position.Y < AQMod.SpaceLayer - (40 * 16f))
                {
                    if (GaleStreams.GaleStreams.MeteorTime())
                    {
                        spawnRate /= 2;
                        maxSpawns *= 2;
                    }
                }
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (AQMod.CosmicEvent.SpawnsActive(spawnInfo.player))
            {
                int tileDistance = AQMod.CosmicEvent.GetTileDistance(spawnInfo.player);
                if (tileDistance < 30)
                {
                    pool.Clear();
                    return;
                }
                else if (tileDistance < GlimmerEvent.GlimmerEvent.MaxDistance)
                {
                    if (tileDistance > GlimmerEvent.GlimmerEvent.HyperStariteDistance) // shouldn't divide by 0...
                    {
                        float normalSpawnsMult = 1f - (1f / (tileDistance - GlimmerEvent.GlimmerEvent.HyperStariteDistance));
                        IEnumerator<int> keys = pool.Keys.GetEnumerator();
                        int[] keyValue = new int[pool.Count];
                        for (int i = 0; i < pool.Count; i++)
                        {
                            keyValue[i] = keys.Current;
                            if (!keys.MoveNext())
                            {
                                break;
                            }
                        }
                        keys.Dispose();
                        for (int i = 0; i < pool.Count; i++)
                        {
                            pool[keyValue[i]] *= normalSpawnsMult;
                        }
                    }
                    else
                    {
                        int[] keyValue = new int[pool.Count];
                        IEnumerator<int> keys = pool.Keys.GetEnumerator();
                        for (int i = 0; i < pool.Count; i++)
                        {
                            keyValue[i] = keys.Current;
                            if (!keys.MoveNext())
                            {
                                break;
                            }
                        }
                        for (int i = 0; i < pool.Count; i++)
                        {
                            pool[keyValue[i]] = 0f;
                        }
                    }
                    int layerIndex = GlimmerEvent.GlimmerEvent.GetLayerIndex(tileDistance);
                    if (layerIndex != -1)
                    {
                        for (int i = layerIndex - 1; i >= 0; i--)
                        {
                            pool.Add(GlimmerEvent.GlimmerEvent.Layers[i].NPCType, GlimmerEvent.GlimmerEvent.Layers[i].SpawnChance);
                        }
                        if (layerIndex == GlimmerEvent.GlimmerEvent.Layers.Count - 1)
                        {
                            pool.Add(GlimmerEvent.GlimmerEvent.Layers[layerIndex].NPCType, (AQUtils.GetGrad(0, GlimmerEvent.GlimmerEvent.Layers[layerIndex].Distance, tileDistance) * GlimmerEvent.GlimmerEvent.Layers[layerIndex].SpawnChance));
                        }
                        else
                        {
                            pool.Add(GlimmerEvent.GlimmerEvent.Layers[layerIndex].NPCType, 1f - (AQUtils.GetGrad(GlimmerEvent.GlimmerEvent.Layers[layerIndex + 1].Distance, GlimmerEvent.GlimmerEvent.Layers[layerIndex].Distance, tileDistance) * GlimmerEvent.GlimmerEvent.Layers[layerIndex].SpawnChance));
                        }
                    }
                }
            }
            if (spawnInfo.spawnTileY < AQMod.SpaceLayerTile - 40)
            {
                if (GaleStreams.GaleStreams.MeteorTime())
                {
                    pool.Add(ModContent.NPCType<Meteor>(), 2f);
                }
            }
            if (GaleStreams.GaleStreams.EventActive(spawnInfo.player))
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<Vraine>()))
                    pool.Add(ModContent.NPCType<Vraine>(), 1f);
                if (AQMod.SudoHardmode)
                {
                    pool[0] *= 0.1f;
                    if (!NPC.AnyNPCs(ModContent.NPCType<RedSprite>()))
                        pool.Add(ModContent.NPCType<RedSprite>(), 1f);
                }
            }
        }
    }
}