using IWshRuntimeLibrary;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WpfUtility.Desktop
{
    /// <summary>
    /// ショートカットの作成機能を提供します。
    /// </summary>
    public static class Shortcut
    {
        /// <summary>
        /// ショートカットファイルを作成します。
        /// </summary>
        /// <param name="path">ショートカットのパス</param>
        public static void Create(string path)
        {
            Create(path, Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// ショートカットファイルを作成します。
        /// </summary>
        /// <param name="path">ショートカットのパス</param>
        /// <param name="assembly">ショートカットのリンク先のアセンブリ</param>
        public static void Create(string path, Assembly assembly)
        {
            if (System.IO.File.Exists(path)) { return; }
            string targetPath = assembly.Location;

            // ショートカットのパスを指定して、WshShortcutを作成
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(path);
            try
            {
                shortcut.TargetPath = targetPath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(assembly.Location);
                shortcut.WindowStyle = 1;
                shortcut.IconLocation = assembly.Location + ",0";

                // ショートカットを作成
                shortcut.Save();
            }
            finally
            {
                Marshal.ReleaseComObject(shortcut);
            }
        }
    }
}
