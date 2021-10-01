using System.Windows;

namespace Aveva.Gdp.TokenVault.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Protected Methods

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var core = new TokenVaultEditorCore())
                core.Run();
        }

        #endregion
    }
}