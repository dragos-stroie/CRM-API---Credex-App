using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Web;

namespace CRM_API___Credex.Common.CRM
{
    internal static class CRMConnecter
    {
        internal static IOrganizationService GetServiceForOrganization(string orgName)
        {
            /*
            Uri organizationUri = new Uri(string.Format(@"http://crm9srv.westeurope.cloudapp.azure.com/CollectionCredex/XRMServices/2011/Organization.svc", orgName));
            ClientCredentials credentials = new ClientCredentials();
            credentials.UserName.UserName = "splashdevcrm\\marius.andrei";
            credentials.UserName.Password = "eKVhwSc5e!";
            */


            // Credex UAT
            
            Uri organizationUri = new Uri(string.Format(@"https://dtvtstcrmspldev/CREDEXIFN-DEV/XRMServices/2011/Organization.svc", orgName));
            //"https://10.2.164.77/CREDEXIFN-DEV/XRMServices/2011/Organization.svc"
            ClientCredentials credentials = new ClientCredentials();
            credentials.UserName.UserName = "bucuresti\\splash_dev";
            credentials.UserName.Password = "Spl@d3v.1324";


            IServiceConfiguration<IOrganizationService> config = ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(organizationUri);
            using (OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(config, credentials))
            {
                IOrganizationService _service = (IOrganizationService)_serviceProxy;
                return _service;
            }
            /*
            IOrganizationService service = null;

            CrmServiceClient svc = new CrmServiceClient($@"AuthType=ClientSecret;url=https://credexifn-uat.crm4.dynamics.com;ClientId=0d7d770d-8431-49eb-b8aa-bcc453293d67;ClientSecret=U4p8Q~A46yUwmcYq4SDpgMtliN6.pJOlyYTnTah~");

            if (svc.IsReady)
            {
                if (svc.OrganizationServiceProxy != null)
                    service = (IOrganizationService)svc.OrganizationServiceProxy;
                else
                    service = (IOrganizationService)svc.OrganizationWebProxyClient;
            }
            return service;
            */
        }
        internal static IOrganizationService GetServiceForOrganization_Online(string orgName)
        {
            /*
            Uri organizationUri = new Uri(string.Format(@"http://crm9srv.westeurope.cloudapp.azure.com/CollectionCredex/XRMServices/2011/Organization.svc", orgName));
            ClientCredentials credentials = new ClientCredentials();
            credentials.UserName.UserName = "splashdevcrm\\marius.andrei";
            credentials.UserName.Password = "eKVhwSc5e!";
            */


            // Credex UAT
            /*
            Uri organizationUri = new Uri(string.Format(@"https://dtvtstcrmspldev/CREDEXIFN-DEV/XRMServices/2011/Organization.svc", orgName));
            //"https://10.2.164.77/CREDEXIFN-DEV/XRMServices/2011/Organization.svc"
            ClientCredentials credentials = new ClientCredentials();
            credentials.UserName.UserName = "bucuresti\\splash_dev";
            credentials.UserName.Password = "Spl@d3v.1324";


            IServiceConfiguration<IOrganizationService> config = ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(organizationUri);
            using (OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(config, credentials))
            {
                IOrganizationService _service = (IOrganizationService)_serviceProxy;
                return _service;
            }
            */
            IOrganizationService service = null;

            CrmServiceClient svc = new CrmServiceClient($@"AuthType=ClientSecret;url=https://credexifn-uat.crm4.dynamics.com;ClientId=0d7d770d-8431-49eb-b8aa-bcc453293d67;ClientSecret=a9K8Q~G69y1CQFOUCdBopq6PT8OtiHB.gIWdcaq6");
            //CrmServiceClient svc = new CrmServiceClient($@"AuthType=ClientSecret;url=https://credex-ifn.crm4.dynamics.com;ClientId=0d7d770d-8431-49eb-b8aa-bcc453293d67;ClientSecret=a9K8Q~G69y1CQFOUCdBopq6PT8OtiHB.gIWdcaq6");
            if (svc.IsReady)
            {
                if (svc.OrganizationServiceProxy != null)
                    service = (IOrganizationService)svc.OrganizationServiceProxy;
                else
                    service = (IOrganizationService)svc.OrganizationWebProxyClient;
            }
            return service;
            
        }
    }
}