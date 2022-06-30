namespace ArgoStore.IntegrationTests
{
    public class IntegrationTestsConfiguration
    {
        public IntegrationTestsConfiguration()
        {
            string ramDiskPath = Environment.GetEnvironmentVariable("ram_disk_db_test_dir");
            string permaDiskPath = Environment.GetEnvironmentVariable("db_tests_dir");

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
}
