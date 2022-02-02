using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using D2L.Build.BazelGenerator.NewBuild;
using D2L.Build.BazelGenerator.NewBuild.Rules;

namespace D2L.Build.BazelGenerator.OldBuild {
	internal sealed class FrameworkAssembly {
		/// <summary>
		/// A DLL from the .NET framework we need to import
		/// </summary>
		/// <param name="packageRelativeDLLPath"></param>
		/// <param name="references"></param>
		public FrameworkAssembly(
			string targetFramework,
			string packageRelativeDLLPath,
			ImmutableArray<string> references
		) {
			Name = Path.GetFileNameWithoutExtension( packageRelativeDLLPath );
			TargetFramework = targetFramework;
			PackageRelativeDLLPath = packageRelativeDLLPath;
			References = references;
		}

		public string Name { get; }
		public string TargetFramework { get; }
		public string PackageRelativeDLLPath { get; }
		public ImmutableArray<string> References { get; }

		// Load a FrameworkAssembly from a DLL on disk
		public static FrameworkAssembly LoadFromFile(
			string targetFramework,
			string frameworkPackagePath,
			string dllPath
		) {
			var packageRelativeDLLPath = dllPath.Substring(
				frameworkPackagePath.Length + 1
			);

			if( !packageRelativeDLLPath.Contains( "Facades" ) ) {
				// So we don't _really_ need to tell Bazel about deps... but
				// for the Facade dlls we do. This is really weird and hacky
				// but it works out. The reason we're not output deps for
				// everybody here is because there are circular references
				// (wtf?) like System.Xml -> System.Data.SqlXml -> System.Xml
				// and Bazel is just not at all about that. We could bundle up
				// the cycles into a pool of DLLs that we import as one target,
				// maybe...
				return new FrameworkAssembly(
					targetFramework,
					packageRelativeDLLPath,
					Path.GetFileNameWithoutExtension( packageRelativeDLLPath ) == "mscorlib"
					? ImmutableArray<string>.Empty
					: ImmutableArray.Create( "mscorlib" )
				);
			}

			Assembly asm;
			try {
				asm = Assembly.ReflectionOnlyLoadFrom( dllPath );
			} catch( BadImageFormatException ) {
				// some DLLs are native DLLs. Just don't worry about those.
				return new FrameworkAssembly(
					targetFramework,
					packageRelativeDLLPath,
					ImmutableArray<string>.Empty
				);
			}

			var deps = asm.GetReferencedAssemblies()
				.Select( r => r.Name )
				.ToImmutableArray();

			return new FrameworkAssembly(
				targetFramework,
				packageRelativeDLLPath,
				deps
			);
		}

		public IEnumerable<INewBuildThing> Convert( ImmutableArray<Label> visibility ) {
			var deps = References
				.Select( r => new Label( TargetFramework, "", r ) )
				.ToImmutableArray();

			var refdll = new Label(
				TargetFramework,
				"",
				target: PackageRelativeDLLPath.Replace( '\\', '/' )
			);

			yield return new ImportLibrary(
				name: Name,
				targetFramework: TargetFramework,
				refdll: refdll,
				deps: deps,
				visibility: visibility
			);
		}
	}
}
