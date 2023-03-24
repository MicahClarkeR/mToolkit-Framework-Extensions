using mToolkitPlatformComponentLibrary;
using mToolkitPlatformComponentLibrary.Workspace.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace mToolkitFrameworkExtensions
{
    public static class mWorkflowFileExt
    {
        public static bool CreateFileCopy(mTool tool, string path, out mWorkspaceReferenceFile? file)
        {
            file = tool.CurrentWorkspace.CopyFileIntoWorkspace(path, path);
            return file != null;
        }
    }
}
