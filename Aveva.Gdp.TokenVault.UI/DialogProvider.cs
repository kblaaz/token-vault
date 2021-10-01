using Aveva.Gdp.TokenVault.Api;
using Microsoft.Win32;
using System;
using System.Windows;

namespace Aveva.Gdp.TokenVault.UI
{
    public class DialogProvider : IDialogProvider
    {
        #region Public Constructors

        public DialogProvider(Window parent)
        {
            Parent = parent;
        }

        public DialogProvider()
        {
        }

        #endregion

        #region Public Properties

        public Window Parent { get; private set; }

        #endregion

        #region Public Methods

        public void ShowMessageCentered(string text, string caption, DialogIcon icon)
        {
            if (Parent == null)
                ShowMessage(text, caption, icon);
            else
                MessageBox.Show(Parent, text, caption, MessageBoxButton.OK, ToMessageBoxImage(icon));
        }

        public void ShowMessageCentered(string text, string caption)
        {
            if (Parent == null)
                ShowMessage(text, caption);
            else
                MessageBox.Show(Parent, text, caption);
        }

        public void ShowMessage(string text, string caption, DialogIcon icon)
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, ToMessageBoxImage(icon));
        }

        public void ShowMessage(string text, string caption)
        {
            MessageBox.Show(text, caption);
        }

        public DialogAnswer ShowReplaceFileQuestion(string text, string caption)
        {
            return ToDialogAnswer(MessageBox.Show(text, caption, ToMessageBoxButton(QuestionDialogButtons.YesNoCancel)));
        }

        public DialogAnswer ShowMessageWithQuestion(string text, string caption, QuestionDialogButtons buttons, DialogIcon icon)
        {
            return ToDialogAnswer(MessageBox.Show(text, caption, ToMessageBoxButton(buttons), ToMessageBoxImage(icon)));
        }

        public DialogAnswer ShowMessageWithQuestion(string text, string caption, QuestionDialogButtons buttons)
        {
            return ToDialogAnswer(MessageBox.Show(text, caption, ToMessageBoxButton(buttons)));
        }

        public DialogAnswer ShowMessageWithQuestionCentered(string text, string caption, QuestionDialogButtons buttons)
        {
            if (Parent == null)
                return ShowMessageWithQuestion(text, caption, buttons);
            else
                return ToDialogAnswer(MessageBox.Show(Parent, text, caption, ToMessageBoxButton(buttons)));
        }

        public DialogAnswer ShowMessageWithQuestionCentered(string text, string caption, QuestionDialogButtons buttons, DialogIcon icon)
        {
            if (Parent == null)
                return ShowMessageWithQuestion(text, caption, buttons, icon);
            else
                return ToDialogAnswer(MessageBox.Show(Parent, text, caption, ToMessageBoxButton(buttons), ToMessageBoxImage(icon)));
        }

        public FileDialogResult ShowOpenFileDialog(string title = "", string filter = "", bool multiSelect = false,
            string initialDirectory = "", string fileName = null, string defaultExt = "")
        {
            var fileDialog = new OpenFileDialog
            {
                Title = title,
                Filter = filter,
                Multiselect = multiSelect,
                InitialDirectory = initialDirectory,
                FileName = fileName,
                DefaultExt = defaultExt
            };

            var answer = fileDialog.ShowDialog();
            return new FileDialogResult(answer, fileDialog.FileNames);
        }

        public FileDialogResult ShowSaveFileDialog(string title = "", string filter = "",
            string initialDirectory = "", string fileName = null, string defaultExt = "")
        {
            var saveDialog = new SaveFileDialog
            {
                Title = title,
                Filter = filter,
                InitialDirectory = initialDirectory,
                FileName = fileName,
                DefaultExt = defaultExt
            };

            var answer = saveDialog.ShowDialog();
            return new FileDialogResult(answer, saveDialog.FileNames);
        }

        #endregion

        #region Private Methods

        private MessageBoxButton ToMessageBoxButton(QuestionDialogButtons buttons)
        {
            switch (buttons)
            {
                case QuestionDialogButtons.OK:
                    return MessageBoxButton.OK;

                case QuestionDialogButtons.OKCancel:
                    return MessageBoxButton.OKCancel;

                case QuestionDialogButtons.YesNoCancel:
                    return MessageBoxButton.YesNoCancel;

                case QuestionDialogButtons.YesNo:
                    return MessageBoxButton.YesNo;

                default:
                    throw new InvalidOperationException("Unknown QuestionDialogButtons value conversion!");
            }
        }

        private MessageBoxImage ToMessageBoxImage(DialogIcon icon)
        {
            switch (icon)
            {
                case DialogIcon.None:
                    return MessageBoxImage.None;

                case DialogIcon.Hand:
                    return MessageBoxImage.Hand;

                case DialogIcon.Question:
                    return MessageBoxImage.Question;

                case DialogIcon.Exclamation:
                    return MessageBoxImage.Exclamation;

                case DialogIcon.Asterisk:
                    return MessageBoxImage.Asterisk;

                case DialogIcon.Stop:
                    return MessageBoxImage.Stop;

                case DialogIcon.Error:
                    return MessageBoxImage.Error;

                case DialogIcon.Warning:
                    return MessageBoxImage.Warning;

                case DialogIcon.Information:
                    return MessageBoxImage.Information;

                default:
                    throw new InvalidOperationException("Unknown DialogIcon value conversion!");
            }
        }

        private DialogAnswer ToDialogAnswer(MessageBoxResult result)
        {
            switch (result)
            {
                case MessageBoxResult.None:
                    return DialogAnswer.None;

                case MessageBoxResult.OK:
                    return DialogAnswer.OK;

                case MessageBoxResult.Cancel:
                    return DialogAnswer.Cancel;

                case MessageBoxResult.Yes:
                    return DialogAnswer.Yes;

                case MessageBoxResult.No:
                    return DialogAnswer.No;

                default:
                    throw new InvalidOperationException("Unknown MessageBoxResult value conversion!");
            }
        }

        #endregion
    }
}