using Aveva.Gdp.TokenVault.Api;
using Aveva.Gdp.TokenVault.VM.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Aveva.Gdp.TokenVault.VM
{
    public class TokenVaultEditorVM : BaseVM
    {
        #region Public Fields

        public const string TITLE = "Aveva Token Vault";

        #endregion

        #region Private Fields

        private readonly string coreDirectory;
        private bool isEdited;
        private BindingList<IToken> tokens;
        private IToken selectedToken;
        private ITokenVaultInfo tokenVaultInfo;
        private string path;

        #endregion

        #region Public Constructors

        public TokenVaultEditorVM(IDialogProvider dialogProvider, string coreDirectory)
        {
            DialogProvider = dialogProvider;
            this.coreDirectory = coreDirectory;

            OpenCommand = new RelayCommand(OpenExecute);
            NewCommand = new RelayCommand(NewExecute);
            SaveCommand = new RelayCommand(SaveExecute, () => IsEdited);
            SaveAsCommand = new RelayCommand(SaveAsExecute, () => IsEdited);
            CloseCommand = new RelayCommand(CloseExecute, () => IsEdited);
            ExitCommand = new RelayCommand(ExitExecute);
            AddCommand = new RelayCommand(AddExecute, () => IsEdited);
            RemoveCommand = new RelayCommand(RemoveExecute, () => IsEdited && SelectedToken != null);
            EditCommand = new RelayCommand(EditExecute, () => IsEdited && SelectedToken != null);
            Tokens = new BindingList<IToken>();
        }

        #endregion

        #region Public Properties

        public Action CloseAction { get; set; }

        public bool IsEdited
        {
            get => isEdited;
            set => SetProperty(ref isEdited, value);
        }

        public BindingList<IToken> Tokens
        {
            get => tokens;
            set => SetProperty(ref tokens, value);
        }

        public IToken SelectedToken
        {
            get => selectedToken;
            set => SetProperty(ref selectedToken, value);
        }

        public Func<string, ITokenVaultInfo> OpenExecuteFunc { get; set; }

        public Action<ITokenVaultInfo> SaveExecuteAction { get; set; }

        public Func<IToken> AddExecuteFunc { get; set; }

        public Func<IToken, IToken> EditExecuteFunc { get; set; }
        public RelayCommand OpenCommand { get; }

        public RelayCommand NewCommand { get; }

        public RelayCommand SaveCommand { get; }

        public RelayCommand SaveAsCommand { get; }

        public RelayCommand CloseCommand { get; }

        //public RelayCommand SaveAsCommand { get; }
        public RelayCommand ExitCommand { get; }

        public RelayCommand AddCommand { get; }

        public RelayCommand EditCommand { get; }

        public RelayCommand RemoveCommand { get; }

        public IDialogProvider DialogProvider { get; }

        public Func<string, IEnumerable<IToken>, ITokenVaultInfo> NewExecuteFunc { get; set; }

        public ITokenVaultInfo TokenVaultInfo
        {
            get => tokenVaultInfo;
            set => SetProperty(ref tokenVaultInfo, value);
        }

        public string Path
        {
            get => path;
            set => SetProperty(ref path, value);
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            TokenVaultInfo = null;
            Tokens.Clear();
        }

        public void PromptSaveBeforeExecute(Action action, Action cancelAction = null)
        {
            var result = DialogProvider.ShowMessageWithQuestion($"Do you want to save changes to '{TokenVaultInfo.Path}'?",
                                                   TITLE,
                                                   QuestionDialogButtons.YesNoCancel);

            switch (result)
            {
                case DialogAnswer.Yes:
                    SaveExecute();
                    action.Invoke();
                    break;

                case DialogAnswer.No:
                    action.Invoke();
                    break;

                case DialogAnswer.Cancel:
                    cancelAction?.Invoke();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public bool CheckForChanges()
        {
            if (!IsEdited)
                return false;
            return !TokenVaultInfo.Tokens.SequenceEqual(Tokens);
        }

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(string name)
        {
            switch (name)
            {
                case nameof(TokenVaultInfo):
                    IsEdited = TokenVaultInfo is null ? false : true;
                    break;
            }

            base.OnPropertyChanged(name);
        }

        protected virtual ITokenVaultInfo OnOpenExecuteFunc(string filePath)
        {
            return OpenExecuteFunc?.Invoke(filePath);
        }

        protected virtual IToken OnAddExecuteFunc()
        {
            return AddExecuteFunc?.Invoke();
        }

        protected virtual IToken OnEditExecuteFunc(IToken tokenToEdit)
        {
            return EditExecuteFunc?.Invoke(tokenToEdit);
        }

        protected virtual void OnSaveExecutedFunc()
        {
            var preSafeTokens = TokenVaultInfo.Tokens.ToList();
            TokenVaultInfo.Tokens.Clear();
            TokenVaultInfo.Tokens.AddRange(Tokens);
            try
            {
                SaveExecuteAction?.Invoke(TokenVaultInfo);
            }
            catch
            {
                var answer = DialogProvider.ShowMessageWithQuestion($"Unable to save token vault to file '{TokenVaultInfo.Path}'." +
                    $" Do you want to create a new token vault?", TITLE, QuestionDialogButtons.OKCancel);

                if (answer == DialogAnswer.OK)
                    SaveAsExecute();
                else
                {
                    TokenVaultInfo.Tokens.Clear();
                    TokenVaultInfo.Tokens.AddRange(preSafeTokens);
                }
            }
        }

        protected virtual ITokenVaultInfo OnNewExecuteFunc(string fileName, IEnumerable<IToken> tokens = null)
        {
            try
            {
                return NewExecuteFunc?.Invoke(fileName, tokens);
            }
            catch
            {
                DialogProvider.ShowMessageCentered($"Unable to save token vault to file '{fileName}'.", TITLE, DialogIcon.Error);
                return null;
            }
        }

        #endregion

        #region Private Methods

        private void ExitExecute()
        {
            if (CheckForChanges())
                PromptSaveBeforeExecute(OnClosed);
            else
                OnClosed();
        }

        private void OnClosed()
        {
            CloseAction?.Invoke();
        }

        private void SaveExecute() => OnSaveExecutedFunc();

        private void SaveAsExecute() => CreateNewTokenVaultFile(Tokens);

        private void EditExecute()
        {
            var token = OnEditExecuteFunc(SelectedToken);
            if (token is null)
                return;

            Tokens[Tokens.IndexOf(SelectedToken)] = token;
            SelectedToken = token;
        }

        private void CloseExecute()
        {
            if (CheckForChanges())
                PromptSaveBeforeExecute(Reset);
            else
                Reset();
        }

        private void OpenExecute()
        {
            var result = DialogProvider.ShowOpenFileDialog(title: "Open token vault",
                                              filter: "vault files (*.vault)|*.vault|All files (*.*)|*.*",
                                              initialDirectory: coreDirectory);

            if (result.Answer is false)
                return;
            else if (!File.Exists(result.FileName))
            {
                DialogProvider.ShowMessageCentered("File '{filePath}' does not exist.",
                                                   TITLE,
                                                   DialogIcon.Warning);
                return;
            }

            var tokenVaultInfo = OnOpenExecuteFunc(result.FileName);
            if (tokenVaultInfo is null)
                return;

            Reset();
            ApplyNewState(tokenVaultInfo);
        }

        private void ApplyNewState(ITokenVaultInfo tokenVaultInfo)
        {
            TokenVaultInfo = tokenVaultInfo;
            IsEdited = true;
            Tokens = new BindingList<IToken>(tokenVaultInfo.Tokens.ToList());
            Path = TokenVaultInfo.Path;
        }

        private void NewExecute()
        {
            if (TokenVaultInfo != null && CheckForChanges())
                PromptSaveBeforeExecute(() => CreateNewTokenVaultFile());
            else
                CreateNewTokenVaultFile();
        }

        private void CreateNewTokenVaultFile(IEnumerable<IToken> tokens = null)
        {
            var result = DialogProvider.ShowSaveFileDialog(title: "Create token vault",
                                                 filter: "vault files (*.vault)|*.vault|All files (*.*)|*.*",
                                                 initialDirectory: coreDirectory);

            if (result.Answer is false)
                return;

            var tokenVaultInfo = OnNewExecuteFunc(result.FileName, tokens);
            if (tokenVaultInfo is null)
                return;

            Reset();
            ApplyNewState(tokenVaultInfo);
        }

        private void AddExecute()
        {
            var token = OnAddExecuteFunc();
            if (token != null)
                Tokens.Add(token);
        }

        private void RemoveExecute()
        {
            if (!PromptBeforeRemove(SelectedToken))
                return;

            Tokens.Remove(SelectedToken);
        }

        private bool PromptBeforeRemove(IToken selectedToken)
        {
            var result = DialogProvider.ShowMessageWithQuestion($"Do you want to remove the token with ID '{selectedToken.ID}'?",
                                                   TITLE,
                                                   QuestionDialogButtons.YesNo);

            return result == DialogAnswer.Yes ? true : false;
        }

        #endregion
    }
}