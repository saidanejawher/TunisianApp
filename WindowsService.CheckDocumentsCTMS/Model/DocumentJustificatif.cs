using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS.Model
{
    public partial class DocumentJustificatif /*: BaseEntity*/
    {
        public Guid Id { get; set; }

        public Guid IdTiers { get; set; }
        public Guid IdOuvertureCompte { get; set; }
        public string DocumentName { get; set; }

        public string DocumentType { get; set; }

        public string DocPath { get; set; }

        [StringLength(50)]
        public string Statut { get; set; }

        public DateTime? DateUpload { get; set; }

        public DateTime? DateEmission { get; set; }

        public DateTime? DateExpiration { get; set; }

        //public Tiers Tiers { get; set; }

      
        public string NumeroPieceIdentite { get; set; }
        public string LieuDelivrancePieceIdentite { get; set; }
        public string JustificatifDomicileCode { get; set; }
        public string StatutOcr { get; set; }
        public string ResponseOcr { get; set; }
        public string ErreurDetailsOcr { get; set; }
        public string UserId { get; set; }
        public string CgpUserRights { get; set; }
        public string MOUserRights { get; set; }
        public string TCUserRights { get; set; }
        public string ClientUserRights { get; set; }

    }
}
