using System;
using Ganymed.Utils.Attributes;

[AttributeUsage(AttributeTargets.Method)]
[TargetParamRestrictions(typeof(int), typeof(float), typeof(bool), AllowStrings = true)]
public class AssetUsageAttribute : Attribute
{
    
}
