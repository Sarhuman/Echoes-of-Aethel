using UnrealBuildTool;
using System.Collections.Generic;

public class EchoesOfAethelEditorTarget : TargetRules
{
    public EchoesOfAethelEditorTarget(TargetInfo Target) : base(Target)
    {
        Type = TargetType.Editor;
        ExtraModuleNames.Add("EchoesOfAethel");
    }
}
