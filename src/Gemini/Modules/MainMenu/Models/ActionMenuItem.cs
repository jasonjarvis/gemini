using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gemini.Modules.MainMenu.Models
{
    /// <summary>
    /// A simple Action Menu item that takes a execute delegate and optional canExecute
    /// </summary>
    public class ActionMenuItem : StandardMenuItem
    {
        private string mText;
        ICommand mCommand;

        #region Constructors

        public ActionMenuItem(string text, System.Action<object> execute, Predicate<object> canExecute = null)
        {
            mText = text;
            mCommand = new RelayCommand(execute, canExecute);
        }

        public override ICommand Command
        {
            get
            {
                return mCommand;
            }
        }

        // we could implement this later
        public override Uri IconSource
        {
            get
            {
                return null;
            }
        }

        // we could implement this later
        public override string InputGestureText
        {
            get
            {
                return "";
            }
        }

        // actions do not show a check
        public override bool IsChecked
        {
            get
            {
                return false;
            }
        }

        // actions are always visible for now
        public override bool IsVisible
        {
            get
            {
                return true;
            }
        }

        public override string Text
        {
            get
            {
                return mText;
            }
        }

        #endregion
    }
}
