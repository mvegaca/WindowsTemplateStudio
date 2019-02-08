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
        private static readonly string _executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace('\\', '/');

        public static string Read(string filePath)
        {
            return File.ReadAllText(Path.Combine(_executingDirectory, filePath));
        }
    }
}
