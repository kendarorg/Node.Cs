namespace Node.Cs.Nugets
{
	public interface INugetVersionVerifier
	{
		string BuildODataQuery(string id, string version);
		int Compare(string versionA, string versionB);
		bool IsVersionMatching(string foundedVersion, string versionExpression);
	}
}
