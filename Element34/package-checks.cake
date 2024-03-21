//////////////////////////////////////////////////////////////////////
// SYNTAX FOR EXPRESSING CHECKS
//////////////////////////////////////////////////////////////////////

public static class Check
{
	public static void That(DirectoryPath testDirPath, IList<PackageCheck> checks)
	{
		if (checks == null)
			throw new ArgumentNullException(nameof(checks));

		bool allOK = true;

		foreach (var check in checks)
			allOK &= check.ApplyTo(testDirPath);

        if (!allOK) throw new Exception("Verification failed!");
    }
}

public static FileCheck HasFile(FilePath file) => HasFiles(new[] { file });
public static FileCheck HasFiles(params FilePath[] files) => new FileCheck(files);

public static DirectoryCheck HasDirectory(DirectoryPath dir) => new DirectoryCheck(dir);

public static DependencyCheck HasDependency(string packageId, string packageVersion = null) => new DependencyCheck(packageId, packageVersion);
public static DependencyCheck HasDependency(PackageReference packageReference) => new DependencyCheck(packageReference);

//////////////////////////////////////////////////////////////////////
// PACKAGECHECK CLASS
//////////////////////////////////////////////////////////////////////

public abstract class PackageCheck
{
	protected ICakeContext _context;

	public PackageCheck()
	{
		_context = BuildSettings.Context;
	}
	
	public abstract bool ApplyTo(DirectoryPath testDirPath);

	protected bool CheckDirectoryExists(DirectoryPath dirPath)
	{
		if (!_context.DirectoryExists(dirPath))
		{
			DisplayError($"Directory {dirPath} was not found.");
			return false;
		}

		return true;
	}

	protected bool CheckFileExists(FilePath filePath)
	{
		if (!_context.FileExists(filePath))
		{
			DisplayError($"File {filePath} was not found.");
			return false;
		}

		return true;
	}

	protected bool CheckFilesExist(IEnumerable<FilePath> filePaths)
	{
		bool isOK = true;

		foreach (var filePath in filePaths)
			isOK &= CheckFileExists(filePath);

		return isOK;
	}

	protected bool DisplayError(string msg)
	{
		_context.Error("  ERROR: " + msg);

		// The return value may be ignored or used as a shortcut
		// for an immediate return from ApplyTo as in
		//    return DisplayError(...)
		return false;
	}
}

//////////////////////////////////////////////////////////////////////
// FILECHECK CLASS
//////////////////////////////////////////////////////////////////////

public class FileCheck : PackageCheck
{
	FilePath[] _files;

	public FileCheck(FilePath[] files)
	{
		_files = files;
	}

	public override bool ApplyTo(DirectoryPath testDirPath)
	{
		return CheckFilesExist(_files.Select(file => testDirPath.CombineWithFilePath(file)));
	}
}

//////////////////////////////////////////////////////////////////////
// DIRECTORYCHECK CLASS
//////////////////////////////////////////////////////////////////////

public class DirectoryCheck : PackageCheck
{
	private DirectoryPath _relDirPath;
	private List<FilePath> _files = new List<FilePath>();

	public DirectoryCheck(DirectoryPath relDirPath)
	{
		_relDirPath = relDirPath;
	}

	public DirectoryCheck WithFiles(params FilePath[] files)
	{
		_files.AddRange(files);
		return this;
	}

    public DirectoryCheck AndFiles(params FilePath[] files)
    {
        return WithFiles(files);
    }

	public DirectoryCheck WithFile(FilePath file)
	{
		_files.Add(file);
		return this;
	}

    public DirectoryCheck AndFile(FilePath file)
    {
        return AndFiles(file);
    }

	public override bool ApplyTo(DirectoryPath testDirPath)
	{
		DirectoryPath absDirPath = testDirPath.Combine(_relDirPath);

		if (!CheckDirectoryExists(absDirPath))
			return false;

		return CheckFilesExist(_files.Select(file => absDirPath.CombineWithFilePath(file)));
	}
}

//////////////////////////////////////////////////////////////////////
// DEPENDENCYCHECK CLASS
//////////////////////////////////////////////////////////////////////

public class DependencyCheck : PackageCheck
{
	private string _packageId;
	private string _packageVersion;


	private List<DirectoryCheck> _directoryChecks = new List<DirectoryCheck>();
	private List<FilePath> _files = new List<FilePath>();

	public DependencyCheck(string packageId, string packageVersion)
	{
		_packageId = packageId;
		_packageVersion = packageVersion;
	}

	public DependencyCheck(PackageReference packageReference)
	{
		_packageId = packageReference.Id;
		_packageVersion = packageReference.Version;
	}

	public DependencyCheck WithFiles(params FilePath[] files)
	{
		_files.AddRange(files);
		return this;
	}

	public DependencyCheck WithFile(FilePath file)
	{
		_files.Add(file);
		return this;
	}

	public DependencyCheck WithDirectory(DirectoryPath relDirPath)
	{
		_directoryChecks.Add(new DirectoryCheck(relDirPath));
		return this;
	}

	public override bool ApplyTo(DirectoryPath testDirPath)
	{
		var packageInstallPath = testDirPath.GetParent();
		string pattern = packageInstallPath.Combine(_packageId) + ".*";
		var installedPackages = new List<string>(_context.GetDirectories(pattern).Select(p => p.FullPath));

		DirectoryPath packagePath = GetDependentPackagePath(packageInstallPath);
		if (packagePath == null)
			return false;

		bool isOK = CheckFilesExist(_files.Select(file => packagePath.CombineWithFilePath(file)));

		foreach (var directoryCheck in _directoryChecks)
			isOK &= directoryCheck.ApplyTo(packagePath);

		return isOK;

		DirectoryPath GetDependentPackagePath(DirectoryPath packageInstallPath)
		{
			if (_packageVersion != null)
			{
				var packagePath = packageInstallPath.Combine(_packageId + "." + _packageVersion);
				if (_context.DirectoryExists(packagePath))
				{
					_context.Information($"  Using version {_packageVersion} of package {_packageId}");
					return packagePath;
				}
			}

			// At this point, either no package version was specified or, if it was, it was not found.

			switch (installedPackages.Count)
			{
				case 0:
					DisplayError($"Package {_packageId} is not installed.");
					return null;

				case 1:
					if (_packageVersion == null)
					{
						var packagePath = installedPackages[0];
						var packageVersion = System.IO.Path.GetFileName(packagePath).Substring(_packageId.Length + 1);
						_context.Information($"  Using version {packageVersion} of package {_packageId}");

						return packagePath;
					}
					
					DisplayError($"Version {_packageVersion} of {_packageId} is not installed.");
					return null;

				default:
					DisplayError(_packageVersion == null
						? $"Version was not specified and multiple versions of package {_packageId} were found."
						: $"Version {_packageVersion} of {_packageId} is not installed.");
					return null;
			}
		}
	}
}
