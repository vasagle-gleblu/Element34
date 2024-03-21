using System.Runtime.Versioning;

public class PluggableAgentFactory
{
	private const string README = "../../README.md";
	private const string LICENSE = "../../LICENSE.txt";
	private const string ICON = "../../testcentric.png";
	private const string CHOCO_VERIFICATION = "../../VERIFICATION.txt";

	private struct AgentInfo
	{
		public string LauncherName { get; }
		public FrameworkName TargetFramework { get; }
		public bool IsNetCore => TargetFramework.Identifier == ".NetCoreApp";
		public bool IsNetFramework => TargetFramework.Identifier == ".NetFramework";

		public AgentInfo(string launcherName, FrameworkName targetFramework)
		{
			LauncherName = launcherName;
			TargetFramework = targetFramework;
		}
	}

	private static SortedList<Version, AgentInfo> AvailableAgents = new SortedList<Version, AgentInfo>();

	// Set in constructor
	private FrameworkName TargetFramework { get; }
	private string NuGetId { get; }
	private string ChocoId { get; }
	private string TargetLauncherName { get; }
	private string TargetAgentName { get; }
	private string TargetLauncherFileNameWithoutExtension { get; }
	private string TargetLauncherFileName { get; }
	private string TargetAgentFileNameWithoutExtension { get; }
	private string TargetAgentFileName { get; }
	private string Title { get; }
	private string Description { get; }
	private string[] Tags { get; }
	private FilePath[] LauncherFiles { get; }
	private FilePath[] AgentFiles { get; }

	private string TargetIdentifier => TargetFramework.Identifier;
	private Version TargetVersion => TargetFramework.Version;
	private string TargetVersionWithoutDots => TargetVersion.ToString().Replace(".", "");
	
	private bool TargetIsNetFramework => TargetIdentifier == ".NetFramework";
	private bool TargetIsNetCore => TargetIdentifier == ".NetCoreApp";

	private List<PackageTest> PackageTests = new List<PackageTest>();

	/// <summary>
	/// Construct a factory for a particular runtime
	/// </summary>
	/// <param name="targetFramework">String in the form of a target framework moniker representing the target runtime for this factory.</param>
	public PluggableAgentFactory(string targetFramework)
	{
		TargetFramework = new FrameworkName(targetFramework);

		if (TargetIsNetFramework)
		{
			NuGetId = $"TestCentric.Extension.Net{TargetVersionWithoutDots}PluggableAgent";
			ChocoId = $"testcentric-extension-net{TargetVersionWithoutDots}-pluggable-agent";
			Title = $".NET {TargetVersion} Pluggable Agent";
			Description = $"TestCentric engine extension for running tests under .NET {TargetVersion}";
			TargetLauncherName = $"Net{TargetVersionWithoutDots}AgentLauncher";
			TargetLauncherFileNameWithoutExtension = $"net{TargetVersionWithoutDots}-agent-launcher";
			TargetAgentFileNameWithoutExtension = $"net{TargetVersionWithoutDots}-agent";
			TargetAgentFileName = TargetAgentFileNameWithoutExtension + ".exe";
			Tags = new [] { "testcentric", "pluggable", "agent", $"net{TargetVersionWithoutDots}" };
			AgentFiles = new FilePath[] {
				$"agent/{TargetAgentFileName}", $"agent/{TargetAgentFileNameWithoutExtension}.pdb", $"agent/{TargetAgentFileName}.config",
				"agent/TestCentric.Agent.Core.dll", "agent/TestCentric.Engine.Api.dll", "agent/TestCentric.Metadata.dll", "agent/TestCentric.Extensibility.dll", "agent/TestCentric.Extensibility.Api.dll", "agent/TestCentric.InternalTrace.dll" };

		}
		else
		{
			if (TargetVersion.Major <= 3)
			{
				NuGetId = $"TestCentric.Extension.NetCore{TargetVersionWithoutDots}PluggableAgent";
				ChocoId = $"testcentric-extension-netcore{TargetVersionWithoutDots}-pluggable-agent";
				Title = $".NET Core {TargetVersion} Pluggable Agent";
				Description = $"TestCentric engine extension for running tests under .NET Core {TargetVersion}";
				TargetLauncherName = $"NetCore{TargetVersionWithoutDots}AgentLauncher";
				TargetLauncherFileNameWithoutExtension = $"netcore{TargetVersionWithoutDots}-agent-launcher";
				TargetAgentFileNameWithoutExtension = $"netcore{TargetVersionWithoutDots}-agent";
				TargetAgentFileName = TargetAgentFileNameWithoutExtension + ".dll";
				AgentFiles = new FilePath[] {
					$"agent/{TargetAgentFileName}", $"agent/{TargetAgentFileNameWithoutExtension}.pdb", $"agent/{TargetAgentFileName}.config",
					$"agent/{TargetAgentFileNameWithoutExtension}.deps.json", $"agent/{TargetAgentFileNameWithoutExtension}.runtimeconfig.json",
					"agent/TestCentric.Agent.Core.dll", "agent/TestCentric.Engine.Api.dll", "agent/TestCentric.Metadata.dll",
					"agent/TestCentric.Extensibility.dll", "agent/TestCentric.Extensibility.Api.dll", "agent/TestCentric.InternalTrace.dll", "agent/Microsoft.Extensions.DependencyModel.dll" };
				Tags = new [] { "testcentric", "pluggable", "agent", $"netcoreapp{TargetVersion}" };
			}
			else
			{
				NuGetId = $"TestCentric.Extension.Net{TargetVersionWithoutDots}PluggableAgent";
				ChocoId = $"testcentric-extension-net{TargetVersionWithoutDots}-pluggable-agent";
				Title = $".NET {TargetVersion} Pluggable Agent";
				Description = $"TestCentric engine extension for running tests under .NET {TargetVersion}";
				TargetLauncherName = $"Net{TargetVersionWithoutDots}AgentLauncher";
				TargetLauncherFileNameWithoutExtension = $"net{TargetVersionWithoutDots}-agent-launcher";
				TargetAgentFileNameWithoutExtension = $"net{TargetVersionWithoutDots}-agent";
				TargetAgentFileName = TargetAgentFileNameWithoutExtension + ".dll";
				AgentFiles = new FilePath[] {
					$"agent/{TargetAgentFileName}", $"agent/{TargetAgentFileNameWithoutExtension}.pdb", $"agent/{TargetAgentFileName}.config",
					$"agent/{TargetAgentFileNameWithoutExtension}.deps.json", $"agent/{TargetAgentFileNameWithoutExtension}.runtimeconfig.json",
					"agent/TestCentric.Agent.Core.dll", "agent/TestCentric.Engine.Api.dll", "agent/TestCentric.Metadata.dll",
					"agent/TestCentric.Extensibility.dll", "agent/TestCentric.Extensibility.Api.dll", "agent/TestCentric.InternalTrace.dll", "agent/Microsoft.Extensions.DependencyModel.dll" };
				Tags = new [] { "testcentric", "pluggable", "agent", $"net{TargetVersion}" };
			}
		}

		TargetLauncherFileName = TargetLauncherFileNameWithoutExtension + ".dll";
		LauncherFiles = new FilePath[] {
			TargetLauncherFileName, TargetLauncherFileNameWithoutExtension + ".pdb", "nunit.engine.api.dll", "testcentric.engine.api.dll" };

		DefinePackageTests();
	}

	public NuGetPackage NuGetPackage =>
		new NuGetPackage(
			NuGetId,
			title: Title,
			description: Description,
			tags: Tags,
			packageContent: new PackageContent()
				.WithRootFiles(LICENSE, README, ICON)
				.WithDirectories(
					new DirectoryContent("tools").WithFiles( LauncherFiles ),
					new DirectoryContent("tools/agent").WithFiles( AgentFiles ) ),
			testRunner: new AgentRunner($"{BuildSettings.NuGetTestDirectory}{NuGetId}.{BuildSettings.PackageVersion}/tools/agent/{TargetAgentFileName}"),
			tests: PackageTests);
	
	public ChocolateyPackage ChocolateyPackage =>
		new ChocolateyPackage(
			ChocoId,
			title: Title,
			description: Description,
			tags: Tags,
			packageContent: new PackageContent()
				.WithRootFiles(ICON)
				.WithDirectories(
					new DirectoryContent("tools").WithFiles( LICENSE, README, CHOCO_VERIFICATION ).AndFiles( LauncherFiles ),
					new DirectoryContent("tools/agent").WithFiles( AgentFiles ) ),
			testRunner: new AgentRunner($"{BuildSettings.ChocolateyTestDirectory}{ChocoId}.{BuildSettings.PackageVersion}/tools/agent/{TargetAgentFileName}"),
			tests: PackageTests);

	public PackageDefinition[] Packages => new PackageDefinition[] { NuGetPackage, ChocolateyPackage };

	// Define Package Tests for both packages
	//   Level 1 tests are run each time we build the packages
	//   Level 2 tests are run for PRs and when packages will be published
	//   Level 3 tests are run only when publishing a release

	private void DefinePackageTests()
	{
		if (TargetIsNetFramework)
		{
			if (TargetVersion >= V_2_0)
			{
				PackageTests.Add(new PackageTest(
					1, "Net20PackageTest", "Run mock-assembly.dll targeting .NET 2.0",
					"tests/net20/mock-assembly.dll", MockAssemblyResult(V_2_0)));
			
				PackageTests.Add(new PackageTest(
					1, "Net35PackageTest", "Run mock-assembly.dll targeting .NET 3.5",
					"tests/net35/mock-assembly.dll", MockAssemblyResult(V_3_5)));
			}

			if (TargetVersion >= V_4_6_2)
				PackageTests.Add(new PackageTest(
					1, "Net462PackageTest", "Run mock-assembly.dll targeting .NET 4.6.2",
					"tests/net462/mock-assembly.dll", MockAssemblyResult(V_4_6_2)));
		}
		else if (TargetIsNetCore)
		{
			if (TargetVersion >= V_1_1)
				PackageTests.Add(new PackageTest(
					1, "NetCore11PackageTest", "Run mock-assembly.dll targeting .NET Core 1.1",
					"tests/netcoreapp1.1/mock-assembly.dll", MockAssemblyResult(V_1_1)));

			if (TargetVersion >= V_2_1)
				PackageTests.Add(new PackageTest(
					1, "NetCore21PackageTest", "Run mock-assembly.dll targeting .NET Core 2.1",
					"tests/netcoreapp2.1/mock-assembly.dll", MockAssemblyResult(V_2_1)));

			if (TargetVersion >= V_3_1)
				PackageTests.Add(new PackageTest(
					1, "NetCore31PackageTest", "Run mock-assembly.dll targeting .NET Core 3.1",
					"tests/netcoreapp3.1/mock-assembly.dll", MockAssemblyResult(V_3_1)));

			if (TargetVersion >= V_5_0)
				PackageTests.Add(new PackageTest(
					1, "Net50PackageTest", "Run mock-assembly.dll targeting .NET 5.0",
					"tests/net5.0/mock-assembly.dll", MockAssemblyResult(V_5_0)));

			if (TargetVersion >= V_6_0)
				PackageTests.Add(new PackageTest(
					1, "Net60PackageTest", "Run mock-assembly.dll targeting .NET 6.0",
					"tests/net6.0/mock-assembly.dll", MockAssemblyResult(V_6_0)));

			if (TargetVersion >= V_7_0)
				PackageTests.Add(new PackageTest(
					1, "Net70PackageTest", "Run mock-assembly.dll targeting .NET 7.0",
					"tests/net7.0/mock-assembly.dll", MockAssemblyResult(V_7_0)));

			// Special handling for target version > highest built-in version
			if (TargetVersion > V_7_0)
				PackageTests.Add(new PackageTest(
					1, $"Net{TargetVersionWithoutDots}PackageTest", $"Run mock-assembly.dll targeting .NET {TargetVersion}",
					$"tests/net{TargetVersion}/mock-assembly.dll", MockAssemblyResult(TargetVersion)));

			// Run AspNetCore test for target framework >= 3.1
			if (TargetVersion == V_3_1)
				PackageTests.Add(new PackageTest(
					1, $"AspNetCore{TargetVersionWithoutDots}Test", $"Run test using AspNetCore targeting .NET {TargetVersion}",
					$"tests/netcoreapp{TargetVersion}/aspnetcore-test.dll", AspNetCoreResult(TargetVersion)));

			if (TargetVersion > V_3_1)
				PackageTests.Add(new PackageTest(
					1, $"AspNetCore{TargetVersionWithoutDots}Test", $"Run test using AspNetCore targeting .NET {TargetVersion}",
					$"tests/net{TargetVersion}/aspnetcore-test.dll", AspNetCoreResult(TargetVersion)));

			// Run Windows test for target framework >= 5.0 (6.0 on AppVeyor)
			if (TargetVersion >= V_6_0 || TargetVersion >= V_5_0 && !BuildSettings.IsRunningOnAppVeyor)
				PackageTests.Add(new PackageTest(
					1, $"Net{TargetVersionWithoutDots}WindowsFormsTest", $"Run test using windows forms under .NET {TargetVersion}",
					$"tests/net{TargetVersion}-windows/windows-forms-test.dll", WindowsFormsResult(TargetVersion)));
		}
	}

	// Define expected results
	private ExpectedResult MockAssemblyResult(Version testVersion) => new ExpectedResult("Failed")
	{
		Total = 36, Passed = 23, Failed = 5, Warnings = 1, Inconclusive = 1, Skipped = 7,
		Assemblies = new ExpectedAssemblyResult[] { new ExpectedAssemblyResult("mock-assembly.dll") }
	};

	private ExpectedResult AspNetCoreResult(Version testVersion) => new ExpectedResult("Passed")
	{
		Total = 2, Passed = 2, Failed = 0, Warnings = 0, Inconclusive = 0, Skipped = 0,
		Assemblies = new ExpectedAssemblyResult[] { new ExpectedAssemblyResult("aspnetcore-test.dll") }
	};

	private ExpectedResult WindowsFormsResult(Version testVersion) => new ExpectedResult("Passed")
	{
		Total = 2, Passed = 2, Failed = 0, Warnings = 0, Inconclusive = 0, Skipped = 0,
		Assemblies = new ExpectedAssemblyResult[] {	new ExpectedAssemblyResult("windows-forms-test.dll") }
	};
}
