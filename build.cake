#addin Cake.Coveralls

#tool Cake.Coveralls
#tool "nuget:?package=OpenCover"

using System.Diagnostics
using System;

var target = Argument("target", "");
var coverallsToken = Argument("coverallsToken", "");
var buildConfig = Argument("buildconfiguration", "Debug");
var buildPath = "./bin";
var coverageFile = "./coverage-result.xml";

Task("Build")
.IsDependentOn("Clean")
.Does(() => {
	Information("========== Restoring packages ==========");
	DotNetCoreRestore();
	Information("=============== Building ===============");
	DotNetCoreBuild(".\\project.json");
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
	var testAssemblies = GetFiles("./bin/**/hippocampus-sql.dll");
	DotNetCoreTest();
	Information("========================================");
});

Task("UploadCoverageReport")
	.IsDependentOn("GenerateCoverageReport")
    .Does(() =>
{
	//if(string.IsNullOrEmpty(coverallsToken))
	//	throw new ArgumentNullException("coverallsToken");
	//#break
    CoverallsIo(new FilePath(coverageFile),
	new CoverallsIoSettings()
    {
        RepoToken = "" //implementar
    });
})
.OnError(ex => {
	Information("================ ERROR =================");
	Information("Erro: {0}", ex);
});

Task("GenerateCoverageReport")
	.IsDependentOn("Build")
	.Does(() => 
{
	OpenCover(tool => tool.DotNetCoreTest(),
	  new FilePath(coverageFile),
	  new OpenCoverSettings()
		.WithFilter("+[hippocampus-sql]*"));
});

if(!string.IsNullOrEmpty(target))
	RunTarget(target);