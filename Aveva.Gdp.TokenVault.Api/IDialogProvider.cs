using System.Linq;

namespace Aveva.Gdp.TokenVault.Api
{
    public enum DialogIcon
    {
        None,
        Hand,
        Question,
        Exclamation,
        Asterisk,
        Stop,
        Error,
        Warning,
        Information
    }

    public enum DialogAnswer
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Abort = 3,
        Retry = 4,
        Ignore = 5,
        Yes = 6,
        No = 7
    }

    public enum QuestionDialogButtons
    {
        OK = 0,
        OKCancel = 1,
        AbortRetryIgnore = 2,
        YesNoCancel = 3,
        YesNo = 4,
        RetryCancel = 5
    }

    public interface IDialogProvider
    {
        //FolderBrowserResult ShowFolderBrowserDialog(string title, string initialDirectory);

        #region Public Methods

        void ShowMessage(string text, string caption);

        void ShowMessage(string text, string caption, DialogIcon icon);

        DialogAnswer ShowMessageWithQuestion(string text, string caption, QuestionDialogButtons buttons);

        DialogAnswer ShowMessageWithQuestion(string text, string caption, QuestionDialogButtons buttons, DialogIcon icon);

        void ShowMessageCentered(string text, string caption);

        void ShowMessageCentered(string text, string caption, DialogIcon icon);

        DialogAnswer ShowMessageWithQuestionCentered(string text, string caption, QuestionDialogButtons buttons);

        DialogAnswer ShowMessageWithQuestionCentered(string text, string caption, QuestionDialogButtons buttons, DialogIcon icon);

        FileDialogResult ShowOpenFileDialog(string title = "", string filter = "", bool multiSelect = false,
            string initialDirectory = "", string fileName = null, string defaultExt = "");

        FileDialogResult ShowSaveFileDialog(string title = "", string filter = "",
            string initialDirectory = "", string fileName = null, string defaultExt = "");

        DialogAnswer ShowReplaceFileQuestion(string title, string caption);

        #endregion
    }

    public class FileDialogResult
    {
        #region Public Constructors

        public FileDialogResult(bool? answer, string[] fileNames)
        {
            Answer = answer;
            FileNames = fileNames;
        }

        #endregion

        #region Public Properties

        public bool? Answer { get; private set; }
        public string FileName { get { return FileNames.FirstOrDefault(); } }
        public string[] FileNames { get; private set; }

        #endregion
    }

    public class FolderBrowserResult
    {
        #region Public Constructors

        public FolderBrowserResult(DialogAnswer answer, string selectedDirectory)
        {
            Answer = answer;
            SelectedDirectory = selectedDirectory;
        }

        #endregion

        #region Public Properties

        public DialogAnswer Answer { get; private set; }
        public string SelectedDirectory { get; private set; }

        #endregion
    }
}