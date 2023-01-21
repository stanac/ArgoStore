namespace ArgoStore.IntegrationTests.TestInfra;

public class TestDir
{
    public static TestDir Instance { get; } = new TestDir();

    public string DirectoryPath { get; }

    public TestDir()
    {
        const string envVarName = "argotestdir";

        string dir = Environment.GetEnvironmentVariable(envVarName);

        if (string.IsNullOrEmpty(dir))
        {
            dir = Path.GetTempPath();
            dir = Path.Combine(dir, "argo-" + Guid.NewGuid().ToString("N"));
        }

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        DirectoryPath = dir;
    }
}