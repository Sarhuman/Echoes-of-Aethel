using System;
using System.Linq;

namespace Aethel.Campaign
{
    public sealed class AbilityDefinition
    {
        public string ClassId { get; internal set; }
        public int Slot { get; internal set; }
        public int Mp { get; internal set; }
        public int CooldownMs { get; internal set; }
        public int Power { get; internal set; }
        public int Attribute { get; internal set; }
        public string Element { get; internal set; }
        public string Effect { get; internal set; }
        public int DurationMs { get; internal set; }
        public int WindupMs { get; internal set; }
        public bool Area { get; internal set; }
    }
    // GDD 01 baseline. Timing of the 250 ms hit tolerance is NEW/EXPERIMENTAL.
    public static class CombatKitCatalog
    {
        public const int HitToleranceMs = 250;
        public const int MaxActions = 32, MaxTargets = 128;
        private static AbilityDefinition D(string cls,int slot,int mp,int cd,int power,int attr,string element,string effect="",int duration=0,int windup=0,bool area=false)
        { return new AbilityDefinition { ClassId=cls,Slot=slot,Mp=mp,CooldownMs=cd,Power=power,Attribute=attr,Element=element,Effect=effect,DurationMs=duration,WindupMs=windup,Area=area }; }
        private static readonly AbilityDefinition[] All = {
            D("rune_weaver",1,0,800,6,2,"arcane"),D("rune_weaver",2,12,5000,18,2,"cold","slow",2000),
            D("rune_weaver",3,20,8000,12,2,"fire","burn",3000,0,true),D("rune_weaver",4,18,12000,20,2,"arcane","barrier",4000),
            D("shadow_sentinel",1,0,500,4,1,"physical"),D("shadow_sentinel",2,10,10000,0,1,"physical","conceal",2000),
            D("shadow_sentinel",3,14,7000,10,1,"physical","bleed",8000),D("shadow_sentinel",4,18,6000,22,1,"physical","slow",2000),
            D("thorn_summoner",1,0,800,5,2,"arcane"),D("thorn_summoner",2,22,14000,2,2,"physical","golem",12000),
            D("thorn_summoner",3,18,8000,0,2,"poison","poison",3000,0,true),D("thorn_summoner",4,24,16000,3,2,"arcane","beast",10000),
            D("sacred_flame_inquisitor",1,0,900,8,0,"physical"),D("sacred_flame_inquisitor",2,12,5000,16,3,"sacred","burn",3000),
            D("sacred_flame_inquisitor",3,20,10000,18,3,"sacred","heal"),D("sacred_flame_inquisitor",4,24,12000,28,3,"sacred","",0,700,true)
        };
        public static AbilityDefinition Find(string cls,int slot)
        { return All.FirstOrDefault(x=>x.ClassId==cls && x.Slot==slot); }
    }
}
