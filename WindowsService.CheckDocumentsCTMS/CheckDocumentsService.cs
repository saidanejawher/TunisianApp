using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS.Model;
using static Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS.Enum;

namespace Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS
{
    public partial class CheckDocumentsService : ServiceBase
    {
        private System.Timers.Timer timer;
        public static string UrlWebApiVersion => ConfigurationManager.AppSettings.Get("Finaveo2.Url.WebApi.Tier.Version");
        public static string ApiDomaineCtms => ConfigurationManager.AppSettings.Get("Api.Domaine.Ctms");
        public static string ApiAuthentificationCtms => ConfigurationManager.AppSettings.Get("Api.Authentification.Ctms");
        public static string ApiCreatefolderCtms => ConfigurationManager.AppSettings.Get("Api.Create.Folder.Ctms");
        public static string ApiSendFileCheckCtms => ConfigurationManager.AppSettings.Get("Api.Send.File.Check.Ctms");
        public static string DematerializationRootPath => ConfigurationManager.AppSettings.Get("DematerializationRootPath");
        public static string UpsideoFinaveo2QueriesDocumentsWebApi => ConfigurationManager.AppSettings.Get("Upsideo.Finaveo2.Queries.Documents.WebApi");
        public static string UpsideoFinaveo2CommandsDocumentsWebApi => ConfigurationManager.AppSettings.Get("Upsideo.Finaveo2.Commands.Documents.WebApi");
        public static string UpsideoFinaveo2QueriesProspectClientWebApi => ConfigurationManager.AppSettings.Get("Upsideo.Finaveo2.Queries.ProspectClient.WebApi");
        public static string UpsideoFinaveo2QueriesOuvertureCompteWebApi => ConfigurationManager.AppSettings.Get("Upsideo.Finaveo2.Queries.Ouverture.Compte.WebApi");
        public static string UpsideoFinaveo2CommandsLogWebApi => ConfigurationManager.AppSettings.Get("Upsideo.Finaveo2.Commands.Logs.WebApi");
        public static string UrlWebApiGetDocumentsByStatutOcr => UrlWebApiVersion + ConfigurationManager.AppSettings.Get("Finaveo2.Url.WebApi.GetDocuments.By.StatutOcr");
        public static string UrlWebApiUpdateDocument => UrlWebApiVersion + ConfigurationManager.AppSettings.Get("Finaveo2.Url.WebApi.Update.Document");
        public static string UrlWebApiGetTiers => UrlWebApiVersion + ConfigurationManager.AppSettings.Get("Finaveo2.Url.WebApi.Tiers.GetTiersById");
        public static string UrlWebApiGetOuvertureCompte => UrlWebApiVersion + ConfigurationManager.AppSettings.Get("Finaveo2.Url.WebApi.Ouverture.Compte.GetOuvertureCompteById");
        public static string UrlWebApiLogsCreate => UrlWebApiVersion + ConfigurationManager.AppSettings.Get("Finaveo2.Url.WebApi.Logs.Create");
        public static string OcrCtmsLogin => ConfigurationManager.AppSettings.Get("OCR_CTMS_login");
        public static string OcrCtmsPassword => ConfigurationManager.AppSettings.Get("OCR_CTMS_password");


        public CheckDocumentsService()
        {
            InitializeComponent();
      
            timer = new Timer(1000 * 5); // One minute interval.
            timer.AutoReset = true;
            timer.Enabled = false;
        }

        protected override void OnStart(string[] args)
        {

            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                CtmsService();
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS is Done with Success" ,
                    Exception = "",
                    Action = "Check Document Windows Service",
                    LogType = "Informations",
                    EventAction = "Check Documents CTMS",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",

                };
                CreateLog(log);
            }
            catch (Exception exc)
            {
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS " + exc.Message,
                    Exception = exc.InnerException.Message,
                    Action = new StackTrace(exc).GetFrame(0).GetMethod().Name,
                    LogType = "Error",
                    EventAction = "Error",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",

                };
                CreateLog(log);
            }
            finally
            {

            }
        }
        protected override void OnStop()
        {
            timer.Enabled = false;
            timer.Elapsed -= new ElapsedEventHandler(timer_Elapsed);
        }
        private async void CtmsService()
        {
            try
            {
                // recupere tout les docs en attente d'envoie OCR
                var AllDocEnAttente = await GetAllDocumentsByStatutOcr();

                // modifier leur statutOcr En cours d'envoie OCR
                foreach (var item in AllDocEnAttente)
                {
                    var update = new UpdateDocumentJustificatifInputModel
                    {
                        Id = item.Id,
                        IdTiers = item.IdTiers,
                        IdOuvertureCompte = item.IdOuvertureCompte,

                        DocumentName = item.DocumentName,
                        DocPath = item.DocPath,
                        DocumentType = item.DocumentType,
                        DateUpload = item.DateUpload,
                        DateEmission = item.DateExpiration,
                        DateExpiration = item.DateExpiration,
                        NumeroPieceIdentite = item.NumeroPieceIdentite,
                        LieuDelivrancePieceIdentite = item.LieuDelivrancePieceIdentite,
                        Statut = item.Statut,
                        JustificatifDomicileCode = item.JustificatifDomicileCode,
                        UserId = item.UserId,
                        CgpUserRights = item.CgpUserRights,
                        ClientUserRights = item.ClientUserRights,
                        MOUserRights = item.MOUserRights,
                        TCUserRights = item.TCUserRights,

                        StatutOcr = StatutOcr.EnCoursDeValidationOCR.ToString(),

                    };
                    UpdateDocument(update);
                }

                // Get AccesToken and Create Folder in the first time :
                var AccesToken = GetTokenFromAuthentification(); // acces token to send
                var RefFolder = CreateFolderWithReference(AccesToken); // reference dolder to send

                // check document et modifier les statutOcr et ResponseOCr et ErreurDetail 
                foreach (var d in AllDocEnAttente)
                {

                    var StatutOcr = "";
                    var ResponseOcr = "";
                    var ErreurDetailOcr = "";

                    //Get Tiers :
                    string ResponseTiersGlobal = d.IdTiers != Guid.Empty ? await GetTiers(d.IdTiers) : await GetTiers( await GetIdTiersFromOuvertureCompte(d.IdOuvertureCompte));
                    //Tiers tiersGlobal = JsonConvert.DeserializeObject<Tiers>(ResponseTiersGlobal) ;
                    object tiersDetail = null;
                    //if (tiersGlobal != null && tiersGlobal.TypePersonne.Equals("PersonnePhysique"))
                    //{
                    //    tiersDetail = JsonConvert.DeserializeObject<TiersPP>(ResponseTiersGlobal);
                    //}
                    //else
                    //{
                    //    tiersDetail = JsonConvert.DeserializeObject<TiersPM>(ResponseTiersGlobal);
                    //}

                    var docPath = Path.Combine(DematerializationRootPath, d.DocPath);

                    if (File.Exists(docPath)  /*&& tiersDetail != null*/)
                    {
                        SendFileToCheck(docPath, tiersDetail, ref StatutOcr, ref ResponseOcr, ref ErreurDetailOcr, AccesToken, RefFolder);

                        // Update db set StatutOcr = Valide or NonValide 
                        var update = new UpdateDocumentJustificatifInputModel
                        {
                            Id = d.Id,
                            IdTiers = d.IdTiers,
                            IdOuvertureCompte = d.IdOuvertureCompte,

                            DocumentName = d.DocumentName,
                            DocPath = d.DocPath,
                            DocumentType = d.DocumentType,
                            DateUpload = d.DateUpload,
                            DateEmission = d.DateExpiration,
                            DateExpiration = d.DateExpiration,
                            NumeroPieceIdentite = d.NumeroPieceIdentite,
                            LieuDelivrancePieceIdentite = d.LieuDelivrancePieceIdentite,
                            Statut = d.Statut,
                            JustificatifDomicileCode = d.JustificatifDomicileCode,
                            UserId = d.UserId,
                            CgpUserRights = d.CgpUserRights,
                            ClientUserRights = d.ClientUserRights,
                            MOUserRights = d.MOUserRights,
                            TCUserRights = d.TCUserRights,

                            StatutOcr = StatutOcr,
                            ErreurDetailsOcr = ErreurDetailOcr,
                            ResponseOcr = ResponseOcr
                        };
                        UpdateDocument(update);
                    }
                }
            }
            catch (Exception exc)
            {
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS " + exc.Message,
                    Exception = exc.InnerException.Message,
                    Action = new StackTrace(exc).GetFrame(0).GetMethod().Name,
                    LogType = "Error",
                    EventAction = "Error",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",
                };
                CreateLog(log);
            }
        }

        private void SendFileToCheck(string FilePath, dynamic tiers, ref string StatutOcr, ref string ResponseOcr, ref string ErreurDetailsOcr, string AccesToken, string RefFolder)
        {
            try
            {
                //var AccesToken = GetTokenFromAuthentification(); // acces token to send
                //var RefFolder = CreateFolderWithReference(AccesToken); // reference dolder to send

                var ParamUrl = ApiSendFileCheckCtms + RefFolder + "/add/fichierSync";
                var DataToSendFile = new List<Tuple<object, string>>();

                var FileName = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss") + "_" + Path.GetFileName(FilePath);
                var stringContentFileName = new StringContent(FileName);
                var streamContent = new StreamContent(new MemoryStream(File.ReadAllBytes(FilePath)));
                DataToSendFile.Add(Tuple.Create(streamContent as object, "fichier"));
                DataToSendFile.Add(Tuple.Create(stringContentFileName as object, "identifiantDocument"));

                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"fichier\"",
                    FileName = FileName
                };

                var result = PostAsync(DataToSendFile, AccesToken, ApiDomaineCtms, ParamUrl).Result;
                var CtmsResponse = JsonConvert.DeserializeObject<CtmsResponse>(result);

                if (CtmsResponse.documents.Count > 0 && CtmsResponse.documents != null)
                {

                    if (CtmsResponse.documents.FirstOrDefault().type.Equals("JAP_KBIS"))
                    {
                        ResponseOcr = CtmsResponse.documents.FirstOrDefault().contenu.Replace(@"\", "");
                        var contenuCtmsOtherDoc = JsonConvert.DeserializeObject<ContenuCtmsOtherDoc>(ResponseOcr);
                        CompareTiersOtherDoc(tiers, contenuCtmsOtherDoc, ref StatutOcr, ref ErreurDetailsOcr);
                    }
                    else if (CtmsResponse.documents.FirstOrDefault().type.Contains("CNI") || CtmsResponse.documents.FirstOrDefault().type.Contains("PASSEPORT")) // for the moment just CNI and passeport
                    {
                        ResponseOcr = CtmsResponse.documents.FirstOrDefault().contenu;
                        var contenuId = JsonConvert.DeserializeObject<ContenuCtmsID>(ResponseOcr);
                        CompareTiersId(tiers, contenuId, ref StatutOcr, ref ErreurDetailsOcr);
                    }
                    else
                    {
                        ResponseOcr = CtmsResponse.documents.FirstOrDefault().contenu.Replace(@"\", "");
                        var contenuCtmsJustifDomicile = JsonConvert.DeserializeObject<ContenuCtmsJustifDomicile>(ResponseOcr);
                        CompareTiersJustificatifDomicile(tiers, contenuCtmsJustifDomicile, ref StatutOcr, ref ErreurDetailsOcr);
                    }

                }
                else
                {
                    StatutOcr = Enum.StatutOcr.ErreurEnvoieOcr.ToString();
                    ErreurDetailsOcr = "Erreur : Document invisible";

                    var log = new InsertLogErrorsCommandModel()
                    {
                        ErrorId = Guid.NewGuid(),
                        Description = "OCR CTMS response " ,
                        Exception = "La reponse de CTMS est NULL ",
                        Action = "Send File to CTMS",
                        LogType = "Errpr response CTMS",
                        EventAction = "Errpr response CTMS",
                        Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",


                    };
                    CreateLog(log);
                }
            }
            catch (Exception exc)
            {
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS " + exc.Message,
                    Exception = exc.InnerException.Message,
                    Action = new StackTrace(exc).GetFrame(0).GetMethod().Name,
                    LogType = "Error",
                    EventAction = "Error",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",


                };
                CreateLog(log);

            }

        }
        private void CompareTiersId(dynamic tiers, ContenuCtmsID contenuCtmsID, ref string StatutOcr, ref string ErreurDetailOcr)
        {
            try
            {
                var DateExpiration = Convert.ToDateTime(contenuCtmsID.dateExpiration);
                var NomTiers = tiers.TypePersonne == "PersonnePhysique" ? tiers.Nom.ToUpper() : tiers.Representant.Nom.ToUpper();
                var PrenomTiers = tiers.TypePersonne == "PersonnePhysique" ? tiers.Prenom.ToUpper() : tiers.Representant.Prenom.ToUpper();
                var DateNaissanceTiers = tiers.TypePersonne == "PersonnePhysique" ? tiers.DateNaissance.ToString("dd/MM/yyy") : tiers.Representant.DateNaissance.ToString("dd/MM/yyy");

                if ((string.IsNullOrEmpty(contenuCtmsID.nom) && string.IsNullOrEmpty(contenuCtmsID.nomNaissance)) || string.IsNullOrEmpty(contenuCtmsID.dateExpiration) || string.IsNullOrEmpty(contenuCtmsID.prenoms) || string.IsNullOrEmpty(contenuCtmsID.dateNaissance))
                {
                    StatutOcr = Enum.StatutOcr.NonValide.ToString();
                    ErreurDetailOcr = "Erreur : Document invisible";
                }
                else
                {
                    contenuCtmsID.nom = contenuCtmsID.nom ?? contenuCtmsID.nomNaissance; // fix data ctms!

                    if (contenuCtmsID.nom.Contains(NomTiers) && contenuCtmsID.prenoms.Contains(PrenomTiers) && contenuCtmsID.dateNaissance.Contains(DateNaissanceTiers) 
                        && (DateTime.Compare(DateExpiration, DateTime.Now) >0))
                    {
                        StatutOcr = Enum.StatutOcr.Valide.ToString();
                    }
                    else
                    {
                        StatutOcr = StatutOcr = Enum.StatutOcr.NonValide.ToString();
                        ErreurDetailOcr = "" /*"Erreur :"*/;
                        if (DateTime.Compare(DateExpiration, DateTime.Now) <= 0)
                            ErreurDetailOcr += $" La pièce d'identité est expirée depuis le {DateExpiration.ToString("dd/MM/yyy")} <br>";
                        if (!contenuCtmsID.nom.Contains(NomTiers))
                            ErreurDetailOcr += $"Le nom écrit sur le document ({contenuCtmsID.nom}) ne correspond pas au nom saisi dans le dossier  ({NomTiers}) <br>";
                        if (!contenuCtmsID.prenoms.Contains(PrenomTiers))
                            ErreurDetailOcr += $"Le prénom écrit sur le document ({contenuCtmsID.prenoms}) ne correspond pas au prénom saisi dans le dossier ({PrenomTiers}) <br>";
                        if (!contenuCtmsID.dateNaissance.Contains(DateNaissanceTiers))
                            ErreurDetailOcr += $"La date de naissance écrite sur le document ({contenuCtmsID.dateNaissance}) ne correspond pas à la date de naissance saisi dans le dossier ({DateNaissanceTiers}) <br>";
                    }
                }
            }
            catch (Exception exc)
            {
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS " + exc.Message,
                    Exception = exc.InnerException.Message,
                    Action = new StackTrace(exc).GetFrame(0).GetMethod().Name,
                    LogType = "Error",
                    EventAction = "Error",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",


                };
                CreateLog(log);

            }
        }

        private void CompareTiersJustificatifDomicile(dynamic tiers, ContenuCtmsJustifDomicile contenuCtmsJustifDomicile, ref string StatutOcr, ref string ErreurDetailOcr)
        {
            try
            {
                var dateDocument = Convert.ToDateTime(contenuCtmsJustifDomicile.dateDocument);
                DateTime dateAgo3Month = DateTime.Now.AddMonths(-3);
                string adresseNumero = tiers.AdresseNumero.ToUpper();
                string adresseTypeVoie = tiers.AdresseTypeVoie.ToUpper();
                string adresseLibelleVoie = tiers.AdresseLibelleVoie.ToUpper();
                string codePostal = tiers.CodePostal.ToUpper();
                string commune = tiers.CommuneResidence.ToUpper();
                string nomTiers = tiers.TypePersonne == "PersonnePhysique" ? tiers.Nom.ToUpper() : tiers.Representant.Nom;
                string prenomTiers = tiers.TypePersonne == "PersonnePhysique" ? tiers.Prenom.ToUpper() : tiers.Representant.Prenom;


                if (string.IsNullOrEmpty(contenuCtmsJustifDomicile.nom) || string.IsNullOrEmpty(contenuCtmsJustifDomicile.prenoms) || string.IsNullOrEmpty(contenuCtmsJustifDomicile.codePostal)
                    || string.IsNullOrEmpty(contenuCtmsJustifDomicile.commune) || string.IsNullOrEmpty(contenuCtmsJustifDomicile.adresse) || string.IsNullOrEmpty(contenuCtmsJustifDomicile.dateDocument))
                {
                    StatutOcr = StatutOcr = Enum.StatutOcr.NonValide.ToString();
                    ErreurDetailOcr = "Erreur : Document invisible";
                }
                else
                {
                    if (contenuCtmsJustifDomicile.nom.Contains(nomTiers) && contenuCtmsJustifDomicile.prenoms.Contains(prenomTiers) && contenuCtmsJustifDomicile.codePostal.Contains(codePostal) &&
                        contenuCtmsJustifDomicile.commune.Contains(commune) && contenuCtmsJustifDomicile.adresse.Contains(adresseNumero) && contenuCtmsJustifDomicile.adresse.Contains(adresseTypeVoie) &&
                        contenuCtmsJustifDomicile.adresse.Contains(adresseLibelleVoie) && ((DateTime.Compare(dateDocument, dateAgo3Month) > 0) && (DateTime.Compare(dateDocument, DateTime.Now) <= 0)))
                    {
                        StatutOcr = Enum.StatutOcr.Valide.ToString();
                    }
                    else
                    {
                        StatutOcr = StatutOcr = Enum.StatutOcr.NonValide.ToString();
                        ErreurDetailOcr = "" /* "Erreur : <br>"*/;
                        if (DateTime.Compare(dateDocument, dateAgo3Month) <= 0)
                            ErreurDetailOcr += $"Le justificatif de domicile doit être de moins de trois mois (date d'émission :{dateDocument.ToString("dd/MM/yyy")}) <br>";
                        if (!contenuCtmsJustifDomicile.nom.Contains(nomTiers))
                            ErreurDetailOcr += $"Le nom écrit sur le document ({contenuCtmsJustifDomicile.nom}) ne correspond pas au nom saisi dans le dossier ({nomTiers}) <br>";
                        if (!contenuCtmsJustifDomicile.prenoms.Contains(prenomTiers))
                            ErreurDetailOcr += $"Le prénom écrit sur le document {contenuCtmsJustifDomicile.prenoms} ne correspond pas au nom saisi dans le dossier ({prenomTiers}) <br>";
                        if (!contenuCtmsJustifDomicile.codePostal.Contains(codePostal))
                            ErreurDetailOcr += $"Le code postal écrit sur le document {contenuCtmsJustifDomicile.codePostal} ne correspond pas au code postal saisi dans le dossier ({codePostal}) <br>";
                        if (!contenuCtmsJustifDomicile.commune.Contains(commune))
                            ErreurDetailOcr += $"La commune écrite sur le document {contenuCtmsJustifDomicile.commune} ne correspond pas au commune saisi dans le dossier ({commune}) <br>";
                        if (!contenuCtmsJustifDomicile.adresse.Contains(adresseNumero))
                            ErreurDetailOcr += $" Numero d'adresse invalide : {adresseNumero} différent de ({contenuCtmsJustifDomicile.adresse}) <br>";
                        if (!contenuCtmsJustifDomicile.adresse.Contains(adresseTypeVoie))
                            ErreurDetailOcr += $" Type de voie d'adresse invalide : {adresseTypeVoie} différent de ({contenuCtmsJustifDomicile.adresse}) <br>";
                        if (!contenuCtmsJustifDomicile.adresse.Contains(adresseLibelleVoie))
                            ErreurDetailOcr += $" Libelle voie d'adresse invalide : {adresseLibelleVoie} différent de ({contenuCtmsJustifDomicile.adresse}) <br>";
                    }
                }
            }
            catch (Exception exc)
            {
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS " + exc.Message,
                    Exception = exc.InnerException.Message,
                    Action = new StackTrace(exc).GetFrame(0).GetMethod().Name,
                    LogType = "Error",
                    EventAction = "Error",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",


                };
                CreateLog(log);

            }
        }

        private void CompareTiersOtherDoc(dynamic tiers, ContenuCtmsOtherDoc contenuCtmsOtherDoc, ref string StatutOcr, ref string ErreurDetailOcr)
        {
            try
            {
                var RaisonSocial = tiers.RaisonSocialePM.ToUpper();
               
                if (string.IsNullOrEmpty(contenuCtmsOtherDoc.raisonSociale))
                {
                    StatutOcr = Enum.StatutOcr.NonValide.ToString();
                    ErreurDetailOcr = "Erreur : Document invisible";
                }
                else
                {
                    if (contenuCtmsOtherDoc.raisonSociale.Contains(RaisonSocial))
                    {
                        StatutOcr = Enum.StatutOcr.Valide.ToString();
                    }
                    else
                    {
                        StatutOcr = StatutOcr = Enum.StatutOcr.NonValide.ToString();
                        ErreurDetailOcr = $"La raison sociale écrit sur le Kbis ({contenuCtmsOtherDoc.raisonSociale}) ne correspond pas à celle saisie dans le dossier ({RaisonSocial})";
                    }
                }

            }
            catch (Exception exc)
            {
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS " + exc.Message,
                    Exception = exc.InnerException.Message,
                    Action = new StackTrace(exc).GetFrame(0).GetMethod().Name,
                    LogType = "Error",
                    EventAction = "Error",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",


                };
                CreateLog(log);

            }
        }

        private async Task<string> GetTiers(Guid IdTiers)
        {
            try
            {
                var ParamTiers = IdTiers.ToString();
                using (var client = new HttpClient { BaseAddress = new Uri(UpsideoFinaveo2QueriesProspectClientWebApi) })
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var UrlWebApi = UrlWebApiGetTiers + ParamTiers;
                    var response = client.GetAsync(UrlWebApi).Result;

                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        return responseData;
                    }
                    return null;
                }
            }
            catch (Exception exc)
            {
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS GetTiers :" + exc.Message,
                    Exception = exc.InnerException.Message,
                    Action = new StackTrace(exc).GetFrame(0).GetMethod().Name,
                    LogType = "Error",
                    EventAction = "Error",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",


                };
                CreateLog(log);
                return null;

            }
        }

        private async Task<Guid> GetIdTiersFromOuvertureCompte(Guid IdOuvertureCompte)
        {
            try
            {
                var ParamOuvertureCompte = IdOuvertureCompte.ToString();
                using (var client = new HttpClient { BaseAddress = new Uri(UpsideoFinaveo2QueriesOuvertureCompteWebApi) })
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var UrlWebApi = UrlWebApiGetOuvertureCompte + ParamOuvertureCompte;
                    var response = client.GetAsync(UrlWebApi).Result;

                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        //var OuvertureDeCompte = JsonConvert.DeserializeObject<OuvertureCompte>(responseData);

                        //return OuvertureDeCompte.IdTiersInitial.Value ;
                    }
                    return Guid.Empty;
                }
            }
            catch (Exception exc)
            {
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS GetIdTiersFromOuvertureCompte :" + exc.Message,
                    Exception = exc.InnerException.Message,
                    Action = new StackTrace(exc).GetFrame(0).GetMethod().Name,
                    LogType = "Error",
                    EventAction = "Error",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",
                };
                CreateLog(log);
                return Guid.Empty;

            }
        }
        private async Task<List<DocumentJustificatif>> GetAllDocumentsByStatutOcr()
        {
            try
            {
                var ParamStatutOcrEnAttente = StatutOcr.EnAttenteEnvoieOcr.ToString();
                using (var client = new HttpClient { BaseAddress = new Uri(UpsideoFinaveo2QueriesDocumentsWebApi) })
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = client.GetAsync(UrlWebApiGetDocumentsByStatutOcr + ParamStatutOcrEnAttente).Result;

                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var documents = JsonConvert.DeserializeObject<List<DocumentJustificatif>>(responseData);
                        return documents;
                    }
                    return null;
                }
            }
            catch (Exception exc)
            {
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS GetAllDocumentsByStatutOcr : " + exc.Message,
                    Exception = exc.InnerException.Message,
                    Action = new StackTrace(exc).GetFrame(0).GetMethod().Name,
                    LogType = "Error",
                    EventAction = "Error",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",


                };
                CreateLog(log);
                return null;
            }
        }

        private void UpdateDocument(UpdateDocumentJustificatifInputModel update)
        {
            try
            {
                using (var client = new HttpClient { BaseAddress = new Uri(UpsideoFinaveo2CommandsDocumentsWebApi) })
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, UrlWebApiUpdateDocument))
                    {
                        var json = Task.Run(() => JsonConvert.SerializeObject(update));
                        using (var stringContent = new StringContent(json.Result, Encoding.UTF8, "application/json"))
                        {
                            request.Content = stringContent;

                            var response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;

                            response.EnsureSuccessStatusCode();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS UpdateDocument :" + exc.Message,
                    Exception = exc.InnerException.Message,
                    Action = new StackTrace(exc).GetFrame(0).GetMethod().Name,
                    LogType = "Error",
                    EventAction = "Error",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",
                };
                CreateLog(log);
            }
        }
        private void CreateLog(InsertLogErrorsCommandModel log)
        {
            try
            {
                using (var client = new HttpClient { BaseAddress = new Uri(UpsideoFinaveo2CommandsLogWebApi) })
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, UrlWebApiLogsCreate))
                    {
                        var json = Task.Run(() => JsonConvert.SerializeObject(log));
                        using (var stringContent = new StringContent(json.Result, Encoding.UTF8, "application/json"))
                        {
                            request.Content = stringContent;

                            var response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;

                            response.EnsureSuccessStatusCode();


                        }
                    }
                }
            }
            catch (Exception e)
            {
               

            }
        }

        private string GetTokenFromAuthentification()
        {
            try
            {
                var DataToAuthentificate = new List<Tuple<object, string>>();
                var Login = new StringContent(OcrCtmsLogin);
                var Password = new StringContent(OcrCtmsPassword);
                DataToAuthentificate.Add(Tuple.Create(Login as object, "login"));
                DataToAuthentificate.Add(Tuple.Create(Password as object, "password"));
                var Authentification = JsonConvert.DeserializeObject<AuthentificateCTMS>(PostAsync(DataToAuthentificate, null, ApiDomaineCtms, ApiAuthentificationCtms).Result);
                var AccesToken = Authentification.accessToken;
                return AccesToken;
            }
            catch (Exception exc)
            {
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS GetTokenFromAuthentification" + exc.Message,
                    Exception = exc.InnerException.Message,
                    Action = new StackTrace(exc).GetFrame(0).GetMethod().Name,
                    LogType = "Error",
                    EventAction = "Error",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",
                };
                CreateLog(log);
                return null;
            }
        }

        private string CreateFolderWithReference(string AccesToken)
        {
            try
            {
                var DataToCreateNewFolder = new List<Tuple<object, string>>();
                var NewRef = "refe-Upsideo-" + DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");
                var stringContentRef = new StringContent(NewRef);
                DataToCreateNewFolder.Add(Tuple.Create(stringContentRef as object, "reference"));
                var folder = JsonConvert.DeserializeObject<FolderCtmsModel>(PostAsync(DataToCreateNewFolder, AccesToken, ApiDomaineCtms, ApiCreatefolderCtms).Result);
                return folder.reference;
            }
            catch (Exception exc)
            {
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS CreateFolderWithReference" + exc.Message,
                    Exception = exc.InnerException.Message,
                    Action = new StackTrace(exc).GetFrame(0).GetMethod().Name,
                    LogType = "Error",
                    EventAction = "Error",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",

                };
                CreateLog(log);
                return null;
            }
        }

        public async Task<string> PostAsync(List<Tuple<object, string>> data, string AccesToken, string domaine, string urlApi)
        {
            try
            {
                using (var client = new HttpClient() { BaseAddress = new Uri(domaine) })
                {
                    if (!string.IsNullOrEmpty(AccesToken))
                        client.DefaultRequestHeaders.Add("accessToken", AccesToken);
                    using (var content = new MultipartFormDataContent())
                    {
                        foreach (var d in data)
                        {
                            content.Add(d.Item1 as HttpContent, d.Item2);
                        }

                        var response = client.PostAsync(urlApi, content);
                        return await response.Result.Content.ReadAsStringAsync();

                    }
                }
            }
            catch (Exception exc)
            {
                var log = new InsertLogErrorsCommandModel()
                {
                    ErrorId = Guid.NewGuid(),
                    Description = "OCR CTMS PostAsync method : " + exc.Message,
                    Exception = exc.InnerException.Message,
                    Action = new StackTrace(exc).GetFrame(0).GetMethod().Name,
                    LogType = "Error",
                    EventAction = "Error",
                    Layer = "Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS",
                };
                CreateLog(log);
                return null;
            }
        }
    }
}
