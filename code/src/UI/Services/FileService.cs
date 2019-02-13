using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Templates.UI.Services
{
    public static class FileService
    {
        public static readonly string ExecutingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace('\\', '/');

        public static string Read(string filePath)
        {
            return File.ReadAllText(Path.Combine(ExecutingDirectory, filePath));
        }

        public static string GetAbsolutePath(string filePath)
        {
            return Path.Combine(ExecutingDirectory, filePath);
        }
    }
}
