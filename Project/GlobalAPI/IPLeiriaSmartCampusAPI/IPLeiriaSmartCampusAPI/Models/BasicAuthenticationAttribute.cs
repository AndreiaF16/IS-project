using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace IPLeiriaSmartCampusAPI.Models
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                string token = actionContext.Request.Headers.Authorization.Parameter;

                string decodedToken = Authentication.DencodingToken(token);

                Authentication user = new Authentication
                {
                    Email = decodedToken.Split(':')[0],
                    Password = decodedToken.Split(':')[1]
                };


                Utilizador utilizador = Authentication.Login(user);
                if (utilizador != null)
                {
                    Authentication.UpdateGenericIdentity(utilizador);
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
        }
    }
}