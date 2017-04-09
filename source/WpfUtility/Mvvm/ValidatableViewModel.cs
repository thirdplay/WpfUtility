using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using WpfUtility.Collection.Generic;

namespace WpfUtility.Mvvm
{
    /// <summary>
    /// <see cref="INotifyDataErrorInfo"/> またはその派生型のためのデータを提供します。
    /// </summary>
    public class ValidatableViewModel : WindowViewModel, INotifyDataErrorInfo
    {
        /// <summary>
        /// 各プロパティのエラーコンテナ
        /// </summary>
        private readonly OrderedDictionary<string, List<string>> errors = new OrderedDictionary<string, List<string>>();

        #region INotifyDataErrorInfo members

        /// <summary>
        /// 検証エラーイベント
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// 検証エラー変更イベントを発生させる。
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnErrorsChanged(string propertyName)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 指定したプロパティまたはエンティティ全体の検証エラーを取得します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>検証エラー</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (this.HasErrors)
            {
                if (string.IsNullOrEmpty(propertyName))
                {
                    var allErrors = new List<string>();
                    foreach (var errors in this.errors.Values)
                    {
                        allErrors.AddRange(errors);
                    }
                    return allErrors;
                }
                if (this.errors.ContainsKey(propertyName))
                {
                    return this.errors[propertyName];
                }
            }
            return null;
        }

        /// <summary>
        /// 検証エラーがあるかどうか取得します。
        /// </summary>
        public bool HasErrors => this.errors.Count > 0;

        #endregion

        /// <summary>
        /// 検証エラーがあるプロパティを取得します。
        /// </summary>
        /// <returns>プロパティ名のリスト</returns>
        public IEnumerable<string> GetErrorProperties()
        {
            if (this.errors.Count > 0)
            {
                var properties = new List<string>();
                for (var i = 0; i < this.errors.Count; i++)
                {
                    properties.Add(this.errors.KeyAt(i));
                }
                return properties;
            }
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// プロパティの入力値を検証します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <param name="groupName">検証を行うグループ</param>
        /// <returns>検証エラーがある場合はtrue、それ以外はfalse</returns>
        public bool Validate([CallerMemberName]string propertyName = null, string groupName = null)
        {
            if (!string.IsNullOrEmpty(propertyName) && IsContaindValidationGroup(propertyName, groupName))
            {
                object value = this.GetType().GetProperty(propertyName).GetValue(this);
                var context = new ValidationContext(this) { MemberName = propertyName };
                var validationErrors = new List<ValidationResult>();
                if (!Validator.TryValidateProperty(value, context, validationErrors))
                {
                    var errors = validationErrors.Select(error => error.ErrorMessage);
                    this.SetErrors(propertyName, errors);
                }
                else
                {
                    this.ClearErrors(propertyName);
                }
            }
            return !this.HasErrors;
        }

        /// <summary>
        /// プロパティの入力値を検証します。
        /// </summary>
        /// <param name="groupName">検証を行うグループ</param>
        /// <returns>検証エラーがある場合はtrue、それ以外はfalse</returns>
        public bool ValidateAll(string groupName = null)
        {
            this.ClearErrors();
            var context = new ValidationContext(this);
            var validationErrors = new List<ValidationResult>();
            if (!Validator.TryValidateObject(this, context, validationErrors, true))
            {
                var errors = validationErrors
                    .Where(x => x.MemberNames.Any())
                    .GroupBy(x => x.MemberNames.First())
                    .Where(x => IsContaindValidationGroup(x.Key, groupName));
                foreach (var error in errors)
                {
                    this.SetErrors(error.Key, error.Select(x => x.ErrorMessage));
                }
            }
            return !this.HasErrors;
        }

        /// <summary>
        /// 引数で指定されたプロパティに、指定されたエラーを登録します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <param name="error">エラーメッセージ</param>
        public void SetError(string propertyName, string error)
        {
            this.SetErrors(propertyName, new string[] { error });
        }

        /// <summary>
        /// 引数で指定されたプロパティに、指定されたエラーをすべて登録します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <param name="errors">エラーリスト</param>
        public void SetErrors(string propertyName, IEnumerable<string> errors)
        {
            var hasCurrentError = this.errors.ContainsKey(propertyName);
            var hasNewError = errors != null && errors.Count() > 0;

            if (!hasCurrentError && !hasNewError)
            {
                return;
            }

            if (hasNewError)
            {
                this.errors[propertyName] = new List<string>(errors);
            }
            else
            {
                this.errors.Remove(propertyName);
            }
            OnErrorsChanged(propertyName);
        }

        /// <summary>
        /// 引数で指定されたプロパティまたはエンティティ全体のエラーをすべて解除します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        public void ClearErrors(string propertyName = null)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (this.errors.ContainsKey(propertyName))
                {
                    this.errors.Remove(propertyName);
                    OnErrorsChanged(propertyName);
                }
            }
            else
            {
                while (this.errors.Count > 0)
                {
                    string key = this.errors.First().Key;
                    this.errors.Remove(key);
                    OnErrorsChanged(key);
                }
            }
        }

        /// <summary>
        /// 対象プロパティが、指定したグループに属するかの結果を返します。
        /// </summary>
        /// <param name="propertyName">対象プロパティ名</param>
        /// <param name="groupName">指定を行うグループ</param>
        /// <returns>グループに属する場合はtrue、それ以外はfalseを返す。</returns>
        private bool IsContaindValidationGroup(string propertyName, string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) return true;

            var displayAttr = Attribute.GetCustomAttribute(GetType().GetProperty(propertyName), typeof(DisplayAttribute)) as DisplayAttribute;
            if (displayAttr == null)
            {
                return false;
            }

            return displayAttr.GetGroupName()
                .Split(',').ToList()
                .Exists(x => x.Trim() == groupName);
        }
    }
}
