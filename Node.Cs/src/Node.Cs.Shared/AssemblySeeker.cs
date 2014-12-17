using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node.Cs
{
    public interface IAssemblySeeker
    {
        void AddSearchPath(string path);
        string FindAssembly(string dllPath);
    }

    public class AssemblySeeker : IAssemblySeeker
    {
        public AssemblySeeker(INodeExecutionContext context)
        {
            AddSearchPath(context.TempPath);
            AddSearchPath(context.NodeCsPackagesDirectory);
            AddSearchPath(context.NodeCsExtraBinDirectory);
            AddSearchPath(context.NodeCsExecutablePath);
            AddSearchPath(context.CurrentDirectory.Data);
        }

        private List<string> _searchPaths = new List<string>();

        public void AddSearchPath(string path)
        {
            _searchPaths.Add(path);
        }

        public string FindAssembly(string dllPath)
        {
            var path = dllPath.ToPath().
                Trim(Path.DirectorySeparatorChar);

            if (Path.IsPathRooted(path))
            {
                if (File.Exists(path))
                {
                    return path;
                }
                path = Path.GetFileName(path);
            }

            var ext = Path.GetExtension(path);
            if (ext != null && ext.ToLowerInvariant() != ".dll")
            {
                return null;
            }
            else if (ext == null)
            {
                path += ".dll";
            }

            for (int i = 0; i < _searchPaths.Count; i++)
            {
                var searchPath = _searchPaths[i];
                var fullPath = Path.Combine(searchPath, path);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            return null;
        }
    }
}
