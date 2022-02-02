using D2L.Build.BazelGenerator.Commands;

namespace D2L.Build.BazelGenerator {
	internal static class Program {
		public static void Main(string[] args) {
			GenerateFrameworkBuildFile.Run(
				targetFramework: args[ 0 ],
				referenceAssembliesPathRoot: args[ 1 ],
				referenceAssembliesInternalPath: args[ 2 ]
			);
		}
	}
}
