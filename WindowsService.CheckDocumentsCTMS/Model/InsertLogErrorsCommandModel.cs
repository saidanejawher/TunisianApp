using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsideo.Finaveo2.WindowsService.CheckDocumentsCTMS
{
    class InsertLogErrorsCommandModel
    {
        public Guid ErrorId { get; set; }

        // USAGE
        public string EventAction { get; set; }

        // WHAT
        public string LogType { get; set; }
        public string Description { get; set; }

        // WHO
        public string UserId { get; set; }
        public string UserName { get; set; }

        // WHERE
        public string Layer { get; set; }
        public string Location { get; set; }
        public string Hostname { get; set; }
        public string HttpMethod { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Ip { get; set; }
        public string Env { get; set; }

        // PERFORMANCE
        public long? ElapsedMilliseconds { get; set; }  // only for performance entries


        // EXCEPTION
        public string Exception { get; set; }  // the exception for error logging
        public string StoredProc { get; set; }

    }
}
