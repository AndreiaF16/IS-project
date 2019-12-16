using IPLeiriaSmartCampusAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace IPLeiriaSmartCampusAPI.Controllers
{
    public class UtilizadoresController : ApiController
    {
        public object DataFormat { get; private set; }
        public object Method { get; private set; }
        public object ParameterType { get; private set; }

        [BasicAuthentication]
        [MyAuthorize(Roles = "Admin")]
        [Route("api/utilizadores/")]
        [HttpGet]
        public HttpResponseMessage GetAllUtilizadores()
        {
            List<Utilizador> utilizadores = null;
            try
            {
                utilizadores = Utilizador.GetAllUtilizadores();

                if(utilizadores == null)
                {
                    Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, utilizadores);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception);
            }
        }

        [BasicAuthentication]
        [MyAuthorize(Roles = "Admin,User")]
        [Route("api/utilizadores/me")]
        [HttpGet]
        public HttpResponseMessage GetMyself()
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                int id = int.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault());

                Utilizador utilizador = Utilizador.GetUtilizadorById(id);
                if (utilizador == null)
                {
                    Request.CreateResponse(HttpStatusCode.Gone);
                }

                return Request.CreateResponse(HttpStatusCode.OK, utilizador);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception);
            }
        }


        [Route("api/utilizadores/")]
        [HttpPost]
        public HttpResponseMessage CreateNewUser([FromBody]Authentication user)
        {
            try
            {
                var form = new Dictionary<string, string>
                {
                    { "Id", "(automatic) Type:Int" },
                    { "Nome", "(required) Type:String" },
                    { "Username", "(required UNIQUE) Type:String" },
                    { "Email", "(required UNIQUE) Type:String" },
                    { "Password", "(required) Type:String" },
                    { "Role", "(automatic - DEFAULT User) Type:String" }
                };

                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, form);
                }

                Utilizador utilizador = Utilizador.CreateNewUser(user);
                if (utilizador == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, form);
                }

                return Request.CreateResponse(HttpStatusCode.OK, utilizador);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception);
            }
        }

        [BasicAuthentication]
        [MyAuthorize(Roles = "Admin,User")]
        [Route("api/utilizadores/me")]
        [HttpPut]
        public HttpResponseMessage UpdateMyself([FromBody]Authentication user)
        {
            try
            {
                var form = new Dictionary<string, string>
                {
                    { "Nome", "(required) Type:String" },
                    { "Username", "(required UNIQUE) Type:String" },
                    { "Email", "(required UNIQUE) Type:String" },
                    { "Password", "(required) Type:String" }
                };

                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, form);
                }

                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                int id = int.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault());
                string role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();

                Utilizador utilizador = Utilizador.UpdateUtilizadorById(user, id, role);
                if (utilizador == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, form);
                }

                return Request.CreateResponse(HttpStatusCode.OK, utilizador);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception);
            }
        }

        [BasicAuthentication]
        [MyAuthorize(Roles = "Admin")]
        [Route("api/utilizadores/{id:int}")]
        [HttpPut]
        public HttpResponseMessage UpdateRole([FromBody]Authentication role, int id)
        {
            try
            {
                var form = new Dictionary<string, string>
                {
                    { "Role", "(required) Type:String" }
                };

                if (role.Role == null || (role.Role != "Admin" && role.Role != "User"))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, form);
                }

                Authentication utilizadorToUpdate = Authentication.GetUtilizadorById(id);
                utilizadorToUpdate.Role = role.Role;

                Utilizador utilizador = Utilizador.UpdateUtilizadorById(utilizadorToUpdate, id, "Admin");
                if (utilizador == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, form);
                }

                return Request.CreateResponse(HttpStatusCode.OK, utilizador);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception);
            }
        }
    }
}
