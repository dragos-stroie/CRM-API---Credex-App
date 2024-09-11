using CRM_API___Credex.Common.CRM;
using CRM_API___Credex.Common;
using CRM_API___Credex.Filters;
using CRM_API___Credex.Models;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Web.Http.Results;

namespace CRM_API___Credex.Controlers
{
    
    [BasicAuthentication]
    [RoutePrefix("contracts")]
    //[RequireHttps]
    public class ContractController : ApiController
    {
        /// <summary>
        /// Gets the active tickets
        /// </summary>
        /// <param name="ContractNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{ContractNo}/tickets")]
        //[SwaggerResponseRemoveDefaults]
        //[SwaggerResponse(HttpStatusCode.Created, Description = "Returns the GUID of the created Ticket", Type = typeof(CustomGUID))]
        //[SwaggerResponse(HttpStatusCode.Conflict, Description = "The Ticket already exists in the application", Type = typeof(CustomError))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Attributes in incorrect format, or id not provided", Type = typeof(CustomError))]
        // POST: Toyota/Lead        
        public IHttpActionResult GetTickets(string ContractNo)
        {
            //return Content(HttpStatusCode.BadRequest, "Attributes in incorrect format, or id not provided");
            int contractNoInt;
            if (ContractNo == null || ContractNo == string.Empty /*|| !int.TryParse(ContractNo, out contractNoInt)*/)
                return Content(System.Net.HttpStatusCode.BadRequest, "Contract No not provided or is NaN");

            Ticket_Get entity = new Ticket_Get();
            List<Entity> lEntities = new List<Entity>();

            if (!entity.EntityInstanceExists("new_contract", "new_name", ContractNo))
                return Content(System.Net.HttpStatusCode.BadRequest, string.Format("Contract No {0} does not exist in the system.", ContractNo));
            else
            {
                Guid entityId = entity.GetEntityId("new_contract", "new_name", ContractNo);
                if (entityId == Guid.Empty)
                    return Content(System.Net.HttpStatusCode.BadRequest, string.Format("Contract No {0} does not exist in the system.", ContractNo));
                else
                {
                    lEntities = entity.RetrieveMultiple("sd_ticket", "sd_contract", entityId);
                    List<Ticket_Get> lTickets = new List<Ticket_Get>();
                    foreach (Entity entity1 in lEntities)
                    {
                        lTickets.Add(new Ticket_Get(entity1));
                    }
                    //return Content(System.Net.HttpStatusCode.OK, JsonConvert.SerializeObject(lTickets)); // with escape JSON response
                    return (IHttpActionResult)new ResponseMessageResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = (HttpContent)new JsonContent(JsonConvert.SerializeObject((object)lTickets, Formatting.Indented))

                    });
                }
            }
                
        }
    }
}