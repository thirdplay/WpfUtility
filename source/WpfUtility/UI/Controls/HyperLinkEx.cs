using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace WpfUtility.UI.Controls
{
    /// <summary>
    /// ハイパーリンクの拡張機能を提供します。
    /// </summary>
    public class HyperlinkEx : Hyperlink
    {
        #region Uri 依存関係プロパティ

        /// <summary>
        /// ハイパーリングのURIを取得または設定します。
        /// </summary>
        public Uri Uri
        {
            get { return (Uri)this.GetValue(UriProperty); }
            set { this.SetValue(UriProperty, value); }
        }

        /// <summary>
        /// ハイパーリングのURI
        /// </summary>
        public static readonly DependencyProperty UriProperty =
            DependencyProperty.Register(nameof(Uri), typeof(Uri), typeof(HyperlinkEx), new UIPropertyMetadata(null));

        #endregion

        /// <summary>
        /// クリック時に発生するイベント。
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();

            if (this.Uri != null)
            {
                try
                {
                    Process.Start(Uri.ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}
