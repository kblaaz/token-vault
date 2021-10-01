using Aveva.Gdp.TokenVault.VM;
using System.Diagnostics;
using System.Windows;

namespace Aveva.Gdp.TokenVault.UI
{
    /// <summary>
    /// Interaction logic for TokenVaultSetPasswordWindow.xaml
    /// </summary>
    public partial class TokenVaultSetPasswordWindow : Window
    {
        #region Public Constructors

        public TokenVaultSetPasswordWindow()
        {
            InitializeComponent();
        }

        public TokenVaultSetPasswordWindow(Window owner) : this()
        {
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        #endregion

        #region Public Methods

        public void Initialize(TokenVaultSetPasswordVM vm)
        {
            Debug.Assert(vm != null);

            DataContext = vm;

            passwordBox.PasswordChanged += (s, e) => vm.Password = passwordBox.SecurePassword;
            repeatedPasswordBox.PasswordChanged += (s, e) => vm.RepeatedPassword = repeatedPasswordBox.SecurePassword;
        }

        #endregion
    }
}