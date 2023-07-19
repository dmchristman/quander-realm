
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_EDITOR_OSX
    using System.IO;
	using UnityEditor.iOS.Xcode;
#endif

namespace Editor
{
    public static class XcodeSwiftVersionPostProcess
    {
#if UNITY_EDITOR_OSX
        [PostProcessBuild(999)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                ModifyFrameworks(path);
                DisablingBitcodeiOS(path);
            }
        }

        private static void ModifyFrameworks(string path)
        {
            string projPath = PBXProject.GetPBXProjectPath(path);

            var project = new PBXProject();
            project.ReadFromFile(projPath);

            string targetGuid = project.GetUnityFrameworkTargetGuid();

            project.SetBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

            //string mainTargetGuid = project.GetUnityMainTargetGuid();
            //project.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");

            project.WriteToFile(projPath);
        }

        //https://forum.unity.com/threads/bitcode-bundle-could-not-be-generated-issue.897590/#post-5909051
        static void DisablingBitcodeiOS(string pathToBuildProject)
        {
            string projectPath = PBXProject.GetPBXProjectPath(pathToBuildProject);

            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);
#if UNITY_2019_3_OR_NEWER
            var targetGuid = pbxProject.GetUnityMainTargetGuid();
#else
            var targetName = PBXProject.GetUnityTargetName();
            var targetGuid = pbxProject.TargetGuidByName(targetName);
#endif
            pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            pbxProject.WriteToFile(projectPath);

            var projectInString = File.ReadAllText(projectPath);

            projectInString = projectInString.Replace("ENABLE_BITCODE = YES;",
                $"ENABLE_BITCODE = NO;");
            File.WriteAllText(projectPath, projectInString);
        }   

#endif
    }
}