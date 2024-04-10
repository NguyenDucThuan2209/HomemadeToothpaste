#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditor.iOS.Xcode;

public class ATTIosPostBuild
{
    private const string PrivacyUserTrackingDescriptionKey  = "NSUserTrackingUsageDescription";
    private const string PrivacyUserTrackingDescription     = "Personalize Advertisement";
    [PostProcessBuildAttribute(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        var rootDict = plist.root;
        rootDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://ttpsdk.info");

        if(!rootDict.values.ContainsKey(PrivacyUserTrackingDescriptionKey))
        {
            rootDict.SetString(PrivacyUserTrackingDescriptionKey, PrivacyUserTrackingDescription);
        }
        File.WriteAllText(plistPath, plist.WriteToString());
    }
}
#endif