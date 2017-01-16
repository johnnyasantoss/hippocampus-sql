#addin Cake.Coveralls
#addin Cake.Json

#tool "nuget:?package=coveralls.io"
#tool "nuget:?package=OpenCover"

using System;

var target = Argument("target", "");
var coverallsToken = Argument("coverallsToken", "");
var buildConfig = Argument("buildconfiguration", "Debug");
var buildPath = "./src/bin";
var coverageFile = "./src/coverage-result.xml";
var srcProjectFile = File("./src/project.json");
var testsProjectFile = File("./tests/project.json");

Task("SetVersion")
 .Does(() => 
 {
	var jObj = ParseJsonFromFile(srcProjectFile);
	jObj["version"] = EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? jObj.Value<string>("version");
	SerializeJsonToFile(srcProjectFile, jObj);
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
		Configuration = buildConfig,
		OutputDirectory = "./artifacts/"
	};
	DotNetCoreBuild(srcProjectFile);
	DotNetCoreBuild(testsProjectFile);
	Information("========================================");
})
.OnError(ex => {
	Information("================ ERROR =================");
	Information("Error.{0}{1}{0}", Environment.NewLine, ex);
	RunTarget("Clean");
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
	var testAssemblies = GetFiles("./tests/bin/**/hippocampus-sql.dll");
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
		.WithFilter("+[Hippocampus.SQL]*"));
});

if(!string.IsNullOrEmpty(target))
	RunTarget(target);