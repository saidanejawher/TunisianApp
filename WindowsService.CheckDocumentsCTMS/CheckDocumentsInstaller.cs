using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS
{
    [RunInstaller(true)]
    public partial class CheckDocumentsInstaller : System.Configuration.Install.Installer
    {
        public CheckDocumentsInstaller()
        {
            InitializeComponent();
        }
    }
}
