using System.IO;

namespace TwitchShoppingNetworkLogger.Excel
{
    public class ExcelFileManager
    {
        public readonly string ExcelRootPath;

        public ExcelFileManager(string excelRootPath)
        {
            ExcelRootPath = excelRootPath;
            if (!ExcelRootPath.EndsWith("\\"))
                ExcelRootPath += "\\";

            Directory.CreateDirectory(ExcelRootPath);
        }

        public FileInfo GetFileInfo(string sessionId)
        {
            return new FileInfo($"{ExcelRootPath}TSN_{sessionId}.xlsx");
        }
    }
}
