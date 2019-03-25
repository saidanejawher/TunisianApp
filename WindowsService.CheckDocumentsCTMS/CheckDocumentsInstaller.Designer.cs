namespace Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS
{
    partial class CheckDocumentsInstaller
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.CheckDocumentsServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.CheckDocumentsServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            // 
            // serviceInstaller1
            // 
            this.CheckDocumentsServiceInstaller.Description = "";
            this.CheckDocumentsServiceInstaller.DisplayName = "Check Documents CTMS Service";
            this.CheckDocumentsServiceInstaller.ServiceName = "CheckDocumentsCTMS";
            // 
            // serviceProcessInstaller1
            // 
            this.CheckDocumentsServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.CheckDocumentsServiceProcessInstaller.Password = null;
            this.CheckDocumentsServiceProcessInstaller.Username = null;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
                this.CheckDocumentsServiceInstaller,
                this.CheckDocumentsServiceProcessInstaller});
        }

        #endregion

        private System.ServiceProcess.ServiceInstaller CheckDocumentsServiceInstaller;
        private System.ServiceProcess.ServiceProcessInstaller CheckDocumentsServiceProcessInstaller;
    }
}