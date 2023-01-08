namespace ArgoStore.IntegrationTests;

public class IntegrationTestsConfiguration
{
    public IntegrationTestsConfiguration()
    {
        string ramDiskPath = Environment.GetEnvironmentVariable("argotestdir");
        string permaDiskPath = Environment.GetEnvironmentVariable("argotestdir");

        try
        {
            if (!Directory.Exists(ramDiskPath))
            {
                Directory.CreateDirectory(ramDiskPath);
            }

            WorkingDir = ramDiskPath;
        }
        catch
        {
            WorkingDir = permaDiskPath;
        }

        RunIntegrationTests = !string.IsNullOrWhiteSpace(WorkingDir);

        if (RunIntegrationTests && !Directory.Exists(WorkingDir))
        {
            Directory.CreateDirectory(WorkingDir);
        }
    }

    public bool RunIntegrationTests { get; }
    public string WorkingDir { get; }
}