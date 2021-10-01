using Aveva.Gdp.TokenVault.VM;
using System.Diagnostics;
using System.Windows;

namespace Aveva.Gdp.TokenVault.UI
{
    /// <summary>
    /// Interaction logic for TokenVaultAddTokenWindow.xaml
    /// </summary>
    public partial class TokenVaultAddTokenWindow : Window
    {
        #region Public Constructors

        public TokenVaultAddTokenWindow()
        {
            InitializeComponent();
        }

        public TokenVaultAddTokenWindow(Window owner) : this()
        {
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        #endregion

        #region Public Methods

        public void Initialize(TokenVaultAddTokenVM vm)
        {
            Debug.Assert(vm != null);

            DataContext = vm;
        }

        #endregion
    }
}