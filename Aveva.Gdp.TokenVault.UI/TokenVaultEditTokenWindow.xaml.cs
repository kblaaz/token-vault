using Aveva.Gdp.TokenVault.VM;
using System.Diagnostics;
using System.Windows;

namespace Aveva.Gdp.TokenVault.UI
{
    /// <summary>
    /// Interaction logic for TokenVaultAddTokenWindow.xaml
    /// </summary>
    public partial class TokenVaultEditTokenWindow : Window
    {
        #region Public Constructors

        public TokenVaultEditTokenWindow()
        {
            InitializeComponent();
        }

        public TokenVaultEditTokenWindow(Window owner) : this()
        {
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        #endregion

        #region Public Methods

        public void Initialize(TokenVaultEditTokenVM vm)
        {
            Debug.Assert(vm != null);

            DataContext = vm;
        }

        #endregion
    }
}