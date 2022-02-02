namespace D2L.Build.BazelGenerator.NewBuild {
	internal static class Modules {
		public static Label CSharp { get; }
			= new Label( "d2l_rules_csharp", "csharp", "defs.bzl" );
	}
}
