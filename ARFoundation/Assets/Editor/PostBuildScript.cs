﻿#if UNITY_IPHONE
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
#endif

namespace Editor
{
    public static class NGEPostBuildHelper
    {
#if UNITY_IPHONE
        [PostProcessBuild(5001)]
        public static void ModifyXcodeProject(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                Debug.Log("Starting Unity xCode adjustment");

                AdjustInfoPlist(pathToBuiltProject);

                var projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
                PBXProject pbxProject = new PBXProject ();
                pbxProject.ReadFromFile (projectPath);

                File.WriteAllText (projectPath, pbxProject.WriteToString ());

                //Debug.Log("Adding localizations");
                //var aLanguages = getLanguagesStringFromEnvironmentVariable("SUPPORTED_LANGUAGES");

                //if (aLanguages != null && aLanguages.Count > 0)
                //{
                //    var aLocalizationArray = plistRootDict.CreateArray("CFBundleLocalizations");
                //    for (int i = 0; i < aLanguages.Count; i++)
                //    {
                //        var aLanguage = aLanguages[i];
                //        Debug.Log("adding: " + aLanguage);
                //        aLocalizationArray.AddString(aLanguage);
                //    }
                //}

                //addFilesToPBXProject(new PBXProject(), pathToBuiltProject);



                // remove the Android ScreenshotGallery files which shouldn't be copied at all. Unknown why they are still copied though (PH)
                //var guid = pbxProject.FindFileGuidByProjectPath("Classes/UI/Keyboard.mm");


                Debug.Log("Finished xCode project adjustment");
            }
        }

        private static void AdjustInfoPlist(string pathToBuiltProject)
        {
            // Get plist
            string infoPlistPath = pathToBuiltProject + "/Info.plist";
            var infoPlist = new PlistDocument();
            //infoPlist.ReadFromString(File.ReadAllText(aPlistPath));
            infoPlist.ReadFromFile(infoPlistPath);

            // Get root
            PlistElementDict plistRootDict = infoPlist.root;

            plistRootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

            PlistElement plistElement;
            if (plistRootDict.values.TryGetValue("UIRequiredDeviceCapabilities", out plistElement))
            {
                var requiredDeviceCapabilitiesArray = plistElement as PlistElementArray;
                if (requiredDeviceCapabilitiesArray != null && requiredDeviceCapabilitiesArray.values != null)
                {
                    Debug.Log("Check the required device capabilities");
                    for (int i = requiredDeviceCapabilitiesArray.values.Count - 1; i >= 0; i--)
                    {
                        var requiredCapability = requiredDeviceCapabilitiesArray.values[i];
                        var capabilityName = requiredCapability.AsString();
                        Debug.Log("required: " + capabilityName);
                        if (!string.IsNullOrEmpty(capabilityName) && capabilityName.Equals("opengles-3"))
                        {
                            var removed = requiredDeviceCapabilitiesArray.values.Remove(requiredCapability);
                            Debug.Log("removing capability: " + capabilityName + ": " + removed);
                        }
                        if (!string.IsNullOrEmpty(capabilityName) && capabilityName.Equals("armv7"))
                        {
                            var removed = requiredDeviceCapabilitiesArray.values.Remove(requiredCapability);
                            Debug.Log("removing capability: " + capabilityName + ": " + removed);
                        }
                    }
                }
            }

            // Write to file
            File.WriteAllText(infoPlistPath, infoPlist.WriteToString());
        }
    }
}
#endif