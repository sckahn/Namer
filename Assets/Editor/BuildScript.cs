using UnityEditor;
using System;
using System.Collections.Generic;

class BuildScript {
	static string[] SCENES = FindEnabledEditorScenes();

	static string APP_NAME = "Namer";
	static string TARGET_DIR = "target";

	static void PerformAllBuilds ()
	{
		PerformMacOSXBuild ();
		PerformWindowsBuild ();
		PerformWebGLBuild ();
		PerformAndroidBuild();
		PerformiOSBuild();
	}

	static void PerformMacOSXBuild ()
	{
		string target_dir = APP_NAME;
		GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.StandaloneOSX,BuildOptions.None);
	}

	static void PerformWindowsBuild ()
	{
		string target_dir = APP_NAME + ".exe";
		GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.StandaloneWindows,BuildOptions.None);
	}
	
	static void PerformWebGLBuild ()
	{
		string target_dir = "webgl";
		GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.WebGL,BuildOptions.None);
	}

	static void PerformAndroidBuild ()
	{
		string target_dir = APP_NAME + ".apk";
		GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.Android,BuildOptions.None);
	}

	static void PerformiOSBuild ()
	{
		string target_dir = "iOS";
		//We do not build the xcodeproject in the target directory, since we do not want to archive the
		//entire xcode project, but instead build with XCode, then output the .ipa through Jenkins
		GenericBuild(SCENES, target_dir, BuildTarget.iOS,BuildOptions.None);
	}

	private static string[] FindEnabledEditorScenes() {
		List<string> EditorScenes = new List<string>();
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if (!scene.enabled) continue;
			EditorScenes.Add(scene.path);
		}
		return EditorScenes.ToArray();
	}

	static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
		var res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
		if (res.summary.totalErrors > 0) {
			throw new Exception("BuildPlayer failure: " + res);
		}
	}
}
