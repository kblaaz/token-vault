using Aveva.Gdp.TokenVault.VM;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace Aveva.Gdp.TokenVault.UI
{
    /// <summary>
    /// Interaction logic for TokenVaultEditorWindow.xaml
    /// </summary>
    public partial class TokenVaultEditorWindow : Window
    {
        #region Private Fields

        private TokenVaultEditorVM vm;

        #endregion

        #region Public Constructors

        public TokenVaultEditorWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Methods

        public void Initialize(TokenVaultEditorVM vm)
        {
            Debug.Assert(vm != null);

            DataContext = this.vm = vm;

            Closing += TokenVaultEditorWindow_Closing;
        }

        #endregion

        #region Private Methods

        private void TokenVaultEditorWindow_Closing(object sender, CancelEventArgs e)
        {
            if (vm.CheckForChanges())
                vm.PromptSaveBeforeExecute(() => { }, () => e.Cancel = true);
        }

        #endregion
    }
}