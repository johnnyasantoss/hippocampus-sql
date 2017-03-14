#addin Cake.Coveralls

#tool "nuget:?package=coveralls.io"
#tool "nuget:?package=OpenCover"

var target = Argument("target", "");
var coverallsToken = Argument("coverallsToken", "");
var buildConfig = Argument("buildconfiguration", "Release");

var buildPath = "./artifacts/";

var coverageFile = File("./TestResults/coverage-result.xml");
var srcProject = Directory("./src/Hippocampus.SQL");
var srcProjectFile = srcProject.ToString() + "/Hippocampus.SQL.csproj";
var testsProjectFile = File("./tests/Hippocampus.SQL.Tests/Hippocampus.SQL.Tests.csproj");

var appVeyorVersion = EnvironmentVariable("APPVEYOR_BUILD_VERSION");
var appVeyorBuild = EnvironmentVariable("APPVEYOR_BUILD_NUMBER");

Task("SetVersion")
.Does(() => 
{
    // var file = srcProject + File("Properties/AssemblyInfo.cs");

    // var assemblyInfo = ParseAssemblyInfo("./SolutionInfo.cs");

    // var version = appVeyorVersion ?? assemblyInfo.AssemblyVersion;
    // var buildNo = appVeyorBuild ?? "0";
    // var semVersion = string.Concat(version, "-", buildNo);

    // CreateAssemblyInfo(file, new AssemblyInfoSettings {
    //     Product = "Hippocampus SQL",
    //     Version = version,
    //     FileVersion = version,
    //     InformationalVersion = semVersion,
    //     Copyright = string.Format("Copyright (c) Johnny Santos 2017 - {0}", DateTime.Now.Year)
    // });
});

Task("Build")
.IsDependentOn("Clean")
.IsDependentOn("SetVersion")
.Does(() => {
    Information("========== Restoring packages ==========");
    DotNetCoreRestore();

    Information("=============== Building ===============");
    var settings = new DotNetCoreBuildSettings
    {   
        Framework = "netstandard1.5",
        Configuration = buildConfig,
        OutputDirectory = buildPath
    };
    DotNetCoreBuild(srcProjectFile, settings);
})
.OnError(ex => {
    Information("================ ERROR =================");
    Information("Error.{0}{1}{0}", Environment.NewLine, ex);
    RunTarget("Clean");
});

Task("Build-Tests")
.IsDependentOn("Clean-Tests")
.Does(() => {
    Information("========== Restoring packages ==========");
    DotNetCoreRestore();

    Information("=============== Building ===============");
    var settings = new DotNetCoreBuildSettings
    {   
        Framework = "netstandard1.5",
        Configuration = buildConfig,
        OutputDirectory = buildPath
    };
    DotNetCoreBuild(srcProjectFile, settings);
})
.OnError(ex => {
    Information("================ ERROR =================");
    Information("Error.{0}{1}{0}", Environment.NewLine, ex);
    RunTarget("Clean");
});

Task("Clean-Tests").Does(() => {
    var totalKbs = 0M;
    Func<string, decimal> getKbs = path => 
    {
        var fileKbs = FileSize(path) / 1024;
        totalKbs = totalKbs + fileKbs;
        return fileKbs;
    };

    CleanDirectory(buildPath, 
        fsInfo => {
        if(!DirectoryExists(fsInfo.Path.FullPath))
        {
            Information("Excluding file \"{0}\". Size: {1} KB", fsInfo.Path.FullPath, getKbs(fsInfo.Path.FullPath));
        }
        return fsInfo.Exists;
        }
    );

    if (FileExists(coverageFile))
    {
        Information("Excluding coverage results. File size: {0} KB", getKbs(coverageFile));
        DeleteFile(coverageFile);
    }

    Information("Total deleted: {0} KB", totalKbs);
});

Task("Clean").Does(() => {
    var totalKbs = 0M;
    Func<string, decimal> getKbs = path => 
    {
        var fileKbs = FileSize(path) / 1024;
        totalKbs = totalKbs + fileKbs;
        return fileKbs;
    };

    CleanDirectory(buildPath, 
        fsInfo => {
        if(!DirectoryExists(fsInfo.Path.FullPath))
        {
            Information("Excluding file \"{0}\". Size: {1} KB", fsInfo.Path.FullPath, getKbs(fsInfo.Path.FullPath));
        }
        return fsInfo.Exists;
        }
    );

    if (FileExists(coverageFile))
    {
        Information("Excluding coverage results. File size: {0} KB", getKbs(coverageFile));
        DeleteFile(coverageFile);
    }

    Information("Total deleted: {0} KB", totalKbs);
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    Information("=============== Testing ================");
    DotNetCoreTest();
    Information("========================================");
});

Task("UploadCoverageReport")
    .IsDependentOn("GenerateCoverageReport")
    .Does(() =>
{
    if(string.IsNullOrEmpty(coverallsToken))
    {
        throw new ArgumentNullException("coverallsToken");
    }

    Information("======= UPLOADING COVERAGE DATA ========");
    CoverallsIo(new FilePath(coverageFile),
    new CoverallsIoSettings()
    {
        RepoToken = coverallsToken
    });
    Information("=============== FINISHED ===============");
})
.OnError(ex => {
    Information("================ ERROR =================");
    Information("Erro: {0}", ex);
});

Task("GenerateCoverageReport")
    .IsDependentOn("Build")
    .Does(() => 
{
    OpenCover(tool => tool.DotNetCoreTest(testsProjectFile),
    new FilePath(coverageFile),
    new OpenCoverSettings()
        .WithFilter("+[Hippocampus.SQL.Tests]*")
        .WithFilter("+[Hippocampus.SQL]*")
        .WithFilter("+[hippocampus-sql]*")
    );
});

if(!string.IsNullOrEmpty(target))
    RunTarget(target);