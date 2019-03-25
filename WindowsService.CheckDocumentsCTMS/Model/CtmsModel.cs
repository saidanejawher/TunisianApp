using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS.Model
{
    public class AuthentificateCTMS
    {
        public long expiresIn { get; set; }
        public string login { get; set; }
        public string accessToken { get; set; }
        public long expiresAfter { get; set; }
    }
    public class FolderCtmsModel
    {
        public string reference { get; set; }
        public int idDossier { get; set; }
        public List<object> documents { get; set; }
        public object statut { get; set; }
        public object libelleStatut { get; set; }
        public bool cloture { get; set; }
    }

    public class CtmsResponse
    {
        public string reference { get; set; }
        public int idDossier { get; set; }
        public List<CtmsDocument> documents { get; set; }
        public string statut { get; set; }
        public string libelleStatut { get; set; }
        public bool cloture { get; set; }
    }

    public class CtmsDocument
    {
        public string type { get; set; }
        public string id { get; set; }
        public string codePiece { get; set; }
        public string libelleCodePiece { get; set; }
        public string identifiantDocument { get; set; }
        public string nomFichier { get; set; }
        public string contenu { get; set; }
    }


    public class ContenuCtmsID
    {
        public string nomNaissance { get; set; }
        public string dateNaissance { get; set; }
        public object commune { get; set; }
        public string sexe { get; set; }
        public object adresseTribunal { get; set; }
        public string nom { get; set; }
        public string prenoms { get; set; }
        public string dateExpirationMrz { get; set; }
        public string dateExpiration { get; set; }
        public object communeSiege { get; set; }
        public string dateDelivrance { get; set; }
        public object adressePersonnePhysique { get; set; }
        public object communeCAC { get; set; }
        public string nationalite { get; set; }
        public object adressePersonneMorale { get; set; }
        public object communeTribunal { get; set; }
        public string dateDelivranceMrz { get; set; }
        public string numeroDocument { get; set; }
        public List<string> mrz { get; set; }
        public object communePersonneMorale { get; set; }
        public string administrationDelivrance { get; set; }
        public object adresseSiege { get; set; }
        public object adresse { get; set; }
        public object adresseCAC { get; set; }
        public object communePersonnePhysique { get; set; }
        public string zoneDelivranceMrz { get; set; }
        public string lieuDelivrance { get; set; }
    }

    public class ContenuCtmsOtherDoc
    {
        public string capital { get; set; }
        public string dateNaissancePersonnePhysique { get; set; }
        public string lieuNaissancePersonnePhysique { get; set; }
        public string cpPersonnePhysique { get; set; }
        public string dateExtrait { get; set; }
        public string siren { get; set; }
        public string formeJuridique { get; set; }
        public object commune { get; set; }
        public string adresseTribunal { get; set; }
        public string raisonSociale { get; set; }
        public string communeSiege { get; set; }
        public string prenomPersonnePhysique { get; set; }
        public string dateEmission { get; set; }
        public string adressePersonnePhysique { get; set; }
        public object communeCAC { get; set; }
        public string numGestionTribunal { get; set; }
        public string dateImmatriculation { get; set; }
        public string cpTribunal { get; set; }
        public string dateDureePersonneMorale { get; set; }
        public object adressePersonneMorale { get; set; }
        public string cpSiege { get; set; }
        public string communeTribunal { get; set; }
        public string nomPersonnePhysique { get; set; }
        public object communePersonneMorale { get; set; }
        public string nomTribunal { get; set; }
        public string adresseSiege { get; set; }
        public object adresse { get; set; }
        public string typeGestionnaire { get; set; }
        public string communePersonnePhysique { get; set; }
        public object adresseCAC { get; set; }
    }

    public class ContenuCtmsJustifDomicile
    {
        public string dateDocument { get; set; }
        public object adressePersonneMorale { get; set; }
        public string prenomsConjoint { get; set; }
        public string commune { get; set; }
        public object adresseTribunal { get; set; }
        public string codePostal { get; set; }
        public object communeTribunal { get; set; }
        public string nom { get; set; }
        public string prenoms { get; set; }
        public object communePersonneMorale { get; set; }
        public string nomConjoint { get; set; }
        public object communeSiege { get; set; }
        public object adresseSiege { get; set; }
        public string adresse { get; set; }
        public object adressePersonnePhysique { get; set; }
        public string nomPrestataire { get; set; }
        public object communeCAC { get; set; }
        public object adresseCAC { get; set; }
        public object communePersonnePhysique { get; set; }
    }
}
