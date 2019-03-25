using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS
{
    class Enum
    {
        public enum StatutOcr
        {
            [Description("En attente d'envoie OCR")]
            EnAttenteEnvoieOcr = 0,
            [Description("En cours de validation OCR")]
            EnCoursDeValidationOCR = 1,
            [Description("Validé par l'OCR")]
            Valide = 2,
            [Description("Non validé par l'OCR")]
            NonValide = 3,
            [Description("Validé manuellement")]
            ValideManuellement = 4,
            [Description("Erreur d'envoie à l'OCR")]
            ErreurEnvoieOcr = 5,
            PasConcerne = 6
        }
    }
}
