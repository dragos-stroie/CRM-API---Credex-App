using CRM_API___Credex.Common.CRM;
using CRM_API___Credex.Filters;
using CRM_API___Credex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;


namespace CRM_API___Credex.Controlers
{
    [BasicAuthentication]
    [RoutePrefix("tickets")]
    public class TicketController : ApiController
    {
        [HttpPost]
        [Route("")]
        //[SwaggerResponseRemoveDefaults]
        //[SwaggerResponse(HttpStatusCode.Created, Description = "Returns the GUID of the created Lead", Type = typeof(CustomGUID))]
        //[SwaggerResponse(HttpStatusCode.Conflict, Description = "The Lead already exists in the application", Type = typeof(CustomError))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Attributes in incorrect format, or id not provided", Type = typeof(CustomError))]
        // POST: Toyota/Lead        
        public IHttpActionResult Post([FromBody] Ticket_Post ticketToCreate)
        {
            //return Content(HttpStatusCode.BadRequest, "Attributes in incorrect format, or id not provided");
            if (ticketToCreate == null || !ModelState.IsValid || string.IsNullOrEmpty(ticketToCreate.externalKey.Value.ToString()) || string.IsNullOrWhiteSpace(ticketToCreate.externalKey.Value.ToString()))
                return Content(HttpStatusCode.BadRequest, "Attributes in incorrect format, or Id not provided");



            Guid? ticketGuid = ticketToCreate.CommitToCRM(TipActiune.Create);
            if (!ticketGuid.HasValue)
                return Content(HttpStatusCode.Conflict, string.Format("Ticket with Id {0} already exists", ticketToCreate.externalKey.Value));
            else
                return Content(HttpStatusCode.OK, ticketGuid.Value);
        }
    }
}