using UnrealBuildTool;
using System.Collections.Generic;

public class EchoesOfAethelEditorTarget : TargetRules
{
    public EchoesOfAethelEditorTarget(TargetInfo Target) : base(Target)
    {
        Type = TargetType.Editor;
        DefaultBuildSettings = BuildSettingsVersion.V7;
        IncludeOrderVersion = EngineIncludeOrderVersion.Unreal5_8;
        ExtraModuleNames.Add("EchoesOfAethel");
    }
}
