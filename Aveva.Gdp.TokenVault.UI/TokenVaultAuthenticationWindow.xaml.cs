using Aveva.Gdp.TokenVault.VM;
using System.Diagnostics;
using System.Windows;

namespace Aveva.Gdp.TokenVault.UI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class TokenVaultAuthenticationWindow : Window
    {
        #region Public Constructors

        public TokenVaultAuthenticationWindow()
        {
            InitializeComponent();
        }

        public TokenVaultAuthenticationWindow(Window owner) : this()
        {
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        #endregion

        #region Public Methods

        public void Initialize(TokenVaultAuthenticationVM vm)
        {
            Debug.Assert(vm != null);

            DataContext = vm;

            passwordBox.PasswordChanged += (s, e) => vm.Password = passwordBox.SecurePassword;
        }

        #endregion
    }
}