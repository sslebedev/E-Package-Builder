# E-Package-Builder

## About `mock` branch
This is a place for mock applications that are intaracted in pipeline described in project specification.

It is a part of QC workflow

## Usage

### Unity

Basic Unity Editor console interface can be enriched by static functions, exported from `Editor` project:

``` C#
using UnityEditor;

static class S3dCommandLineBuild
{ 
    public static void ReleaseGame() {}
}
```

Binary output of mock project is `Unity.exe`. It can be called with following param set:

```
-batchmode -nographics -quit -projectPath "PATH_TO_PROJECT" -executeMethod S3dCommandLineBuild.<BuildName>Game -logFile "LOG_FILE_NAME"
```

where `<BuildName>` can be `Release` or `Standalone`. Any other param sets will be rejected.

Outpute results are Unity log file and non-empty output directory.

When running it from visual studio in debug mode command line args assign path to `C:\mockTemp`.

### Game Check

This app can be called with following param set

```
-grp "GAME_RELEASE_PATH" -gsp "GAME_SOURCES_PATH" -rf "PATH\gameCheckResult.xml"
```

Any other param sets will be regected.

Now first two params are considered always valid and are checked only to be not empty. Last parameter assign program's output `.xml` file.

When running it from visual studio in debug mode command line args assign path to `C:\mockTemp`.
