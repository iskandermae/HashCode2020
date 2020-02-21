namespace copete
{
    static class FileHelper
    {
        public static string ExeDirectory()
        {
            return $@"C:\Workspaces\VStudio 2019\copete\Data\";
        }
        public static string InputFileName(string inputFileName) {
            inputFileName = string.IsNullOrWhiteSpace(inputFileName)
                ? "in.txt"
                : inputFileName;
            return ExeDirectory()+inputFileName;
        }
        public static string OutputFileName(string inputFileName) {
            string OutputFileName = inputFileName;
            OutputFileName = OutputFileName.Replace(".txt", "_out.txt");
            
            return ExeDirectory() + OutputFileName;
        }
    }
}
