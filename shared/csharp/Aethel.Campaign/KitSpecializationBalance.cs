using System;
using System.Collections.Generic;

namespace Aethel.Campaign
{
    // NEW/EXPERIMENTAL baseline; units and conditions in runtime-core-v3.md.
    // Damage percentages use basis points, time uses milliseconds, other fields use flat units.
    internal static class KitSpecializationBalance
    {
        private static readonly Dictionary<string,int[]> Values=new Dictionary<string,int[]> {
            {"rune_weaver.control",new[]{500,1000,3,1}},
            {"rune_weaver.burst",new[]{1000,4,1000,1500}},
            {"shadow_sentinel.precision",new[]{1000,3,1000,1000}},
            {"shadow_sentinel.ambush",new[]{1000,3,1,1500}},
            {"thorn_summoner.command",new[]{20,4,1500,2000}},
            {"thorn_summoner.sacrifice",new[]{25,3,1,10}},
            {"sacred_flame_inquisitor.protection",new[]{1000,4,20,20}},
            {"sacred_flame_inquisitor.judgment",new[]{1000,4,1000,1000}}
        };
        internal static int Applied(CampaignState s,string path,int tier)
        {
            string key=s.ClassId+"."+path;
            if(tier<1 || tier>4 || !Values.ContainsKey(key))throw new ArgumentException("Unknown specialization balance");
            return s.Nodes.Contains(key+".t"+tier)?Values[key][tier-1]:0;
        }
    }
}
