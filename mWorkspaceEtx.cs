using mToolkitPlatformComponentLibrary;
using mToolkitPlatformComponentLibrary.Workspace.Files;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace mToolkitFrameworkExtensions
{
    public static class mWorkspaceEtx
    {
        public static mWorkspaceFile[] FindFiles(mTool tool, string directory, string ext)
        {
            mWorkspaceFile[] files = new mWorkspaceFile[0];

            if (tool != null)
            {
                List<mWorkspaceFile> list = new List<mWorkspaceFile>();

                foreach(string file in tool.CurrentWorkspace.GetFilesAt(directory))
                {
                    FileInfo fi = new FileInfo(file);

                    if (fi.Extension.Equals(ext))
                        list.Add(tool.CurrentWorkspace.Create(file));

                }

                files = list.ToArray();
            }

            return files;
        }

        public static class mTempWorkspaceEtx
        {
            public static bool CreateTempFileCopy(mTool tool, string path, out mTemporaryReferenceFileEtx? file)
            {
                file = (mTemporaryReferenceFileEtx?) new mTemporaryReferenceFileEtx(tool.CurrentWorkspace.CopyFileIntoWorkspace(path, GenerateTempFromPath(path)));
                return file != null;
            }

            public static void CreateTempFile(mTool tool, string path, byte[] contents, out mTemporaryFileEtx? file)
            {
                path = GenerateTempFromPath(path);
                mWorkspaceFile fileW = tool.CurrentWorkspace.Create(path, contents, true);
                file = new mTemporaryFileEtx(fileW.Path);
                file.SetContents(contents);
            }

            public static string GenerateTempFromPath(string path)
            {
                return $"Temp\\{StringEtx.GetGUID()}-{new FileInfo(path).Name}";
            }

            public static void DeleteTempWorkspace(mTool tool)
            {
                string tempWorkspace = $"{tool.CurrentWorkspace.Path}Temp";
                DirectoryInfo tempDirectory = new DirectoryInfo(tempWorkspace);

                if (tempDirectory.Exists)
                {
                    tempDirectory.Delete(true);
                }
            }

            public class mTemporaryReferenceFileEtx : mTemporaryFileEtxI
            {
                public bool FinishedUse = false;
                public readonly mWorkspaceReferenceFile File;
                public FileSystemEventHandler? OnChange;

                public mTemporaryReferenceFileEtx(mWorkspaceReferenceFile file)
                {
                    File = file;
                    file.Stream.OnChange += new FileSystemEventHandler(TempFileDeleted);
                }

                private void TempFileDeleted(object sender, FileSystemEventArgs e)
                {
                    if (e.ChangeType == WatcherChangeTypes.Deleted || e.ChangeType == WatcherChangeTypes.Renamed)
                    {
                        SetFinishedUse();
                    }

                    OnChange?.Invoke(sender, e);
                }

                public bool IsFinishedUse()
                {
                    return FinishedUse;
                }

                public void SetFinishedUse()
                {
                    FinishedUse = true;
                }
            }

            public class mTemporaryFileEtx : mWorkspaceFile, mTemporaryFileEtxI
            {
                public bool FinishedUse = false;

                public mTemporaryFileEtx(string workspacePath) : base(workspacePath)
                {
                }

                public bool IsFinishedUse()
                {
                    return FinishedUse;
                }

                public void SetFinishedUse()
                {
                    FinishedUse = true;
                }
            }

            internal interface mTemporaryFileEtxI
            {
                public bool IsFinishedUse();
                public void SetFinishedUse();
            }
        }
    }
}
