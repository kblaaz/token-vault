using Aveva.Gdp.TokenVault.Api;
using Aveva.Gdp.TokenVault.VM.Commands;
using System;

namespace Aveva.Gdp.TokenVault.VM
{
    public class TokenVaultEditTokenVM : BaseVM
    {
        #region Private Fields

        private readonly IToken tokenToEdit;

        private string description;

        private string message;

        private string id;

        private bool changeOnlyDescription;

        #endregion

        #region Public Constructors

        public TokenVaultEditTokenVM(IToken tokenToEdit)
        {
            this.tokenToEdit = tokenToEdit;
            ID = tokenToEdit.ID;
            Description = tokenToEdit.Description;
            ApplyCommand = new RelayCommand<string>(Apply);
        }

        #endregion

        #region Public Properties

        public string ID
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public RelayCommand<string> ApplyCommand { get; }
        public Action<IToken, string, string> TokenEditAction { get; set; }
        public Action CloseAction { get; set; }

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public bool ChangeOnlyDescription
        {
            get => changeOnlyDescription;
            set => SetProperty(ref changeOnlyDescription, value);
        }

        #endregion

        #region Protected Methods

        protected virtual void OnTokenEditedAction(string tokenValue, string description)
        {
            TokenEditAction?.Invoke(tokenToEdit, tokenValue, description);
            CloseAction?.Invoke();
        }

        #endregion

        #region Private Methods

        private void Apply(string tokenValue)
        {
            if (!ChangeOnlyDescription && string.IsNullOrWhiteSpace(tokenValue))
            {
                Message = "New Token field cannot be empty.";
                return;
            }

            if(ChangeOnlyDescription)
                OnTokenEditedAction(null, Description);
            else
                OnTokenEditedAction(tokenValue, Description);
        }

        #endregion
    }
}