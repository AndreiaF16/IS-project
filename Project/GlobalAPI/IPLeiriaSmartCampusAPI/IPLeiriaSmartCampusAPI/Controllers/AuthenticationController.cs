using IPLeiriaSmartCampusAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace IPLeiriaSmartCampusAPI.Controllers
{
    public class AuthenticationController : ApiController
    {

        [Route("api/utilizadores/login")]
        [HttpGet]
        public HttpResponseMessage GetBasicAuthenticationToken([FromBody]Authentication user)
        {
            try
            {
                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                Utilizador utilizador = Authentication.Login(user);
                if(utilizador != null)
                {
                    string url = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/");
                    string token = "Basic " + Authentication.EncodingToken(user.Email + ":" + user.Password);
                    var form = new Dictionary<string, string>
                    {
                        { "Message", "Bem-vindo(a) " + utilizador.Nome },
                        { "Description", "Para mais informações sobre IPLeiria SmartCampus API consultar: " +  url},
                        { "AuthType", "Basic Auth"},
                        { "Authorization", token }
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, form);
                }


                var form_bad_request = new Dictionary<string, string>
                {
                    { "Message", "Dados Errados" }
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, form_bad_request);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception);
            }
        }
    }
}
