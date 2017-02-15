using Livet;
using Livet.Messaging;
using Livet.Messaging.Windows;
using System;
using System.ComponentModel;
using System.Windows;

namespace WpfUtility.Mvvm
{
    /// <summary>
    /// <see cref="Window"/> またはその派生型のためのデータを提供します。
    /// </summary>
    public class WindowViewModel : ViewModel
    {
        #region Title 変更通知プロパティ

        private string _Title;

        /// <summary>
        /// ウィンドウ タイトルを取得または設定します。
        /// </summary>
        public string Title
        {
            get { return this._Title; }
            set
            {
                if (this._Title != value)
                {
                    this._Title = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region CanClose 変更通知プロパティ

        private bool _CanClose = true;

        /// <summary>
        /// ウィンドウを閉じることができるかどうかを示す値を取得または設定します。
        /// </summary>
        public virtual bool CanClose
        {
            get { return this._CanClose; }
            set
            {
                if (this._CanClose != value)
                {
                    this._CanClose = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region IsClosed 変更通知プロパティ

        private bool _IsClosed;

        /// <summary>
        /// アタッチされたウィンドウが閉じられたかどうかを示す値を取得します。
        /// </summary>
        public bool IsClosed
        {
            get { return this._IsClosed; }
            private set
            {
                if (this._IsClosed != value)
                {
                    this._IsClosed = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// <see cref="InitializeCore"/> メソッドが呼ばれたかどうか (通常、これはアタッチされたウィンドウの
        /// <see cref="Window.ContentRendered"/> イベントによって呼び出されます) を示す値を取得します。
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// ダイアログの戻り値を示す値を取得します。
        /// </summary>
        public bool DialogResult { get; protected set; }

        /// <summary>
        /// ウィンドウ状態を取得または設定します。
        /// </summary>
        public WindowState WindowState { get; set; }

        /// <summary>
        /// このメソッドは、アタッチされたウィンドウで <see cref="Window.ContentRendered"/>
        /// イベントが発生したときに、Livet インフラストラクチャによって呼び出されます。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Initialize()
        {
            if (this.IsClosed) return;

            this.DialogResult = false;

            this.InitializeCore();
            this.IsInitialized = true;
        }

        /// <summary>
        /// 派生クラスでオーバーライドされると、アタッチされたウィンドウで <see cref="Window.ContentRendered"/>
        /// イベントが発生したときに呼び出される初期化処理を実行します。
        /// </summary>
        protected virtual void InitializeCore() { }

        /// <summary>
        /// このメソッドは、アタッチされたウィンドウで <see cref="Window.Closing"/>
        /// イベントがキャンセルされたときに、Livet インフラストラクチャによって呼び出されます。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CloseCanceledCallback()
        {
            this.CloseCanceledCallbackCore();
        }

        /// <summary>
        /// 派生クラスでオーバーライドされると、アタッチされたウィンドウで <see cref="Window.Closing"/>
        /// イベントがキャンセルされたときに <see cref="Livet.Behaviors.WindowCloseCancelBehavior"/>
        /// によって呼び出されるコールバック処理を実行します。
        /// </summary>
        protected virtual void CloseCanceledCallbackCore() { }


        /// <summary>
        /// ウィンドウをアクティブ化することを試みます。最小化されている場合は通常状態にします。
        /// </summary>
        public virtual void Activate()
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.SendWindowAction(WindowAction.Normal);
            }

            this.SendWindowAction(WindowAction.Active);
        }

        /// <summary>
        /// ウィンドウを閉じます。
        /// </summary>
        public virtual void Close()
        {
            if (this.IsClosed) return;

            this.SendWindowAction(WindowAction.Close);
        }

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        /// <param name="disposing">明示的に破棄したかを示す値</param>
        protected override void Dispose(bool disposing)
        {
            this.IsClosed = true;
            this.IsInitialized = false;

            base.Dispose(disposing);
        }

        /// <summary>
        /// ウィンドウの状態遷移の相互作用メッセージを送信します。
        /// </summary>
        /// <param name="action">Windowが遷移すべき状態を表すWindowAction列挙体</param>
        protected void SendWindowAction(WindowAction action)
        {
            this.Messenger.Raise(new WindowActionMessage(action, "Window.WindowAction"));
        }

        /// <summary>
        /// 画面遷移の相互作用メッセージを送信します。
        /// </summary>
        /// <param name="viewModel">新しいWindowのDataContextに設定するViewModel</param>
        /// <param name="windowType">新しいWindowの型</param>
        /// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。</param>
        /// <param name="isOwned">ウィンドウの所有者かどうかを示す値</param>
        protected void Transition(ViewModel viewModel, Type windowType, TransitionMode mode, bool isOwned)
        {
            var message = new TransitionMessage(windowType, viewModel, mode, isOwned ? "Window.Transition.Child" : "Window.Transition");
            this.Messenger.Raise(message);
        }

        /// <summary>
        /// 指定 <see cref="Action"/> をUIDispatcherで呼び出します。
        /// </summary>
        /// <param name="action">呼び出すアクション</param>
        protected void InvokeOnUIDispatcher(Action action)
        {
            DispatcherHelper.UIDispatcher.BeginInvoke(action);
        }
    }
}
