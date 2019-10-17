using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Tools.VSTest;
using Nuke.Common.Tooling;

using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;
using static Nuke.Common.Tools.VSTest.VSTestTasks;

class Build : NukeBuild
{
    [Parameter("The folder where the build output is copied to")]
    static readonly AbsolutePath OutputDirectory = RootDirectory / "buildoutput";

    public static AbsolutePath SourceDirectory => RootDirectory / "src";
    public AbsolutePath BuildOutputDirectory => OutputDirectory;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    public static int Main() => Execute<Build>(x => x.BuildBackend);

    Target Clean => _ => _
        .Executes(() =>
        {
            EnsureCleanDirectory(OutputDirectory);
        });

    Target NugetRestore => _ => _
        .Executes(() =>
        {
            NuGetRestore(c => c
                .SetTargetPath(Solution)
             );
        });

    Target BuildBackend => _ => _
    .DependsOn(NugetRestore)
    .After(Clean)
    .Executes(() =>
    {
        MSBuild(s => s
            .SetProjectFile(Solution.Path)
            .SetTargets("Build")
            .SetVerbosity(MSBuildVerbosity.Quiet)
            .SetConfiguration(Configuration)
            .SetNodeReuse(false)
            .SetMaxCpuCount(4)
            .AddProperty("DeployOnBuild", true)
            .AddProperty("DeployDefaultTarget", "WebPublish")
            .AddProperty("SkipExtraFilesOnServer", true)
            .AddProperty("WebPublishMethod", "FileSystem")
            .AddProperty("publishUrl", BuildOutputDirectory)
        );
    });
}
