using UnrealBuildTool;
using System.Collections.Generic;

public class EchoesOfAethelTarget : TargetRules
{
    public EchoesOfAethelTarget(TargetInfo Target) : base(Target)
    {
        Type = TargetType.Game;
        ExtraModuleNames.Add("EchoesOfAethel");
    }
}
