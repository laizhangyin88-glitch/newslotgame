using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker.TestSuite
{
	[Serializable]
	public class TestCase
	{
	    public string caseId;
	    public string description;
	    public List<string> testSuites;
	    public Dictionary<string, object> customData;
	}

	[Serializable]
	public class TestSuite
	{
	    public string suiteId;
	    public string description;
	    public Dictionary<string, object> customData;
	}

	[Serializable]
	public class TestSuiteReportResult
	{
		public List<TestSuiteReportEntry> Results;
		public DateTime? NextReset;
	}

	[Serializable]
	public class TestSuiteReportEntry
	{
		public string ReportName;
		public int StatValue;
		public int Position;
	}

    [Serializable]
    public class ContentInfo
    {
        public int gameId;
        public string gameTitle;
        public string gameTitleName;
        public int version;
        public bool needCustomInGameScene;
        public bool ignore;
    }

    [Serializable]
    public class ContentTestInfo
    {
        public string reportId;
        public List<DebugSpin> debugSpins;
    }

    [Serializable]
    public class DebugSpin
    {
        public int code;
        public string description;
        public string title;
        public string tag;
        public List<DebugSequence> DebugSequenceList;
    }

    [Serializable]
    public class DebugSequence
    {
        public string type;
        public string debugParam;
    }

    [Serializable]
    public class DebugSequenceInfo
    {
        public List<DebugSequence> sequenceList;
    }

    [Serializable]
    public class DebugSpinData
    {
        public int id;
        public int gameId;
        public string rtpId;
        public string name;
        public string tag;
        public string description;
        public DebugSequenceInfo info;
    }

    [Serializable]
    public class DebugSpinParam
    {
        public string error;
        public List<DebugSpinData> data;
    }

    [Serializable]
    public class TestSuiteEditorSettings
    {
        public string id;
    }

    [Serializable]
    public class Manifest
    {
        public string name;
        public string version;
        public string displayName;
        public string description;
        public Dictionary<string, string> dependencies = new Dictionary<string, string>();
        public List<string> keywords = new List<string>();

        protected int majorVersion;
        protected int minorVersion;
        protected int patchVersion;

        public int GetMajorVersion() { return majorVersion; }
        public int GetMinorVersion() { return minorVersion; }
        public int GetPatchVersion() { return patchVersion; }

        public virtual void ParseVersion()
        {
            SplitSementicVersion(ref version, out majorVersion, out minorVersion, out patchVersion);
        }

        public static void SplitSementicVersion(ref string v, out int major, out int minor, out int patch)
        {
            if (string.IsNullOrEmpty(v)) v = "0.1.0";

            major = 0;
            minor = 0;
            patch = 0;

            string[] tokens = v.Split('.');
            if (tokens.Length > 0) int.TryParse(tokens[0], out major);
            if (tokens.Length > 1) int.TryParse(tokens[1], out minor);
            if (tokens.Length > 2) int.TryParse(tokens[2], out patch);

            v = string.Format("{0}.{1}.{2}", major, minor, patch);
        }

        public static int CompareSementicVersion(string x, string y)
        {
            int xMajorVersion, xMinorVersion, xPatchVersion;
            int yMajorVersion, yMinorVersion, yPatchVersion;

            SplitSementicVersion(ref x, out xMajorVersion, out xMinorVersion, out xPatchVersion);
            SplitSementicVersion(ref y, out yMajorVersion, out yMinorVersion, out yPatchVersion);

            int ret = xMajorVersion.CompareTo(yMajorVersion);
            if (ret != 0) return ret;
            ret = xMinorVersion.CompareTo(yMinorVersion);
            if (ret != 0) return ret;
            return xPatchVersion.CompareTo(yPatchVersion);
        }
    }

    [Serializable]
    public class GameManifest : Manifest
    {
        public int gameId;
        public string publishVersion;

        public enum Stage
        {
            Develop,
            Alpha,
            Beta,
            Verified
        };
        public Stage stage = Stage.Develop;

        protected int publishMajorVersion;
        protected int publishMinorVersion;
        protected int publishPatchVersion;

        public int GetPublishMajorVersion() { return publishMajorVersion; }
        public int GetPublishMinorVersion() { return publishMinorVersion; }
        public int GetPublishPatchVersion() { return publishPatchVersion; }

        public override void ParseVersion()
        {
            base.ParseVersion();
            SplitSementicVersion(ref publishVersion, out publishMajorVersion, out publishMinorVersion, out publishPatchVersion);
        }

        public static int Sort(GameManifest x, GameManifest y)
        {
            int ret = x.publishMajorVersion.CompareTo(y.publishMajorVersion);
            if (ret != 0) return ret;
            ret = x.publishMinorVersion.CompareTo(y.publishMinorVersion);
            if (ret != 0) return ret;
            ret = x.publishPatchVersion.CompareTo(y.publishPatchVersion);
            if (ret != 0) return ret;
            return x.gameId.CompareTo(y.gameId);
        }
    }
}
