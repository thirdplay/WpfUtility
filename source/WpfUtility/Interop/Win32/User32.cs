using System;
using System.Runtime.InteropServices;

namespace WpfUtility.Interop.Win32
{
    internal static class User32
    {
        /// <summary>
        /// 指定ウィンドウの矩形情報を返す。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lpRect">矩形情報の参照</param>
        /// <returns>成功ならtrue、失敗ならfalseを返します。</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// 子ウィンドウ、ポップアップウィンドウ、またはトップレベルウィンドウのサイズ、位置、および Z オーダーを変更します。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="hWndInsertAfter">Z オーダーを決めるためのウィンドウハンドル</param>
        /// <param name="X">ウィンドウの左上端の新しい x 座標（クライアント座標）</param>
        /// <param name="Y">ウィンドウの左上端の新しい y 座標（クライアント座標）</param>
        /// <param name="cx">ウィンドウの新しい幅（ピクセル単位）</param>
        /// <param name="cy">ウィンドウの新しい高さ（ピクセル単位）</param>
        /// <param name="uFlags">ウィンドウのサイズと位置の変更に関するフラグを</param>
        /// <returns>成功ならtrue、失敗ならfalseを返します。</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    }
}
