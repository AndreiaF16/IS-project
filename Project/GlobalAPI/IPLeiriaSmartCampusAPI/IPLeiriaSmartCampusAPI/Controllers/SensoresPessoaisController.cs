using IPLeiriaSmartCampusAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;

namespace IPLeiriaSmartCampusAPI.Controllers
{
    public class SensoresPessoaisController : ApiController
    {

        [Route("api/sensores-users/")]
        [HttpGet]
        public HttpResponseMessage GetAllPessoaisSensores()
        {
            List<SensorPessoal> sensores = null;
            try
            {
                sensores = SensorPessoal.GetAllSensores();

                if (sensores == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, sensores);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception);
            }
        }


        [Route("api/sensores-users/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetPessoaisSensoresById(int id)
        {
            SensorPessoal sensor = null;
            try
            {
                sensor = SensorPessoal.GetSensorById(id);

                if (sensor == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, sensor);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception);
            }
        }


        [Route("api/sensores-users/{username}")]
        [HttpGet]
        public HttpResponseMessage GetPessoaisSensoresByUsername(string username)
        {
            List<SensorPessoal> sensores = null;
            try
            {
                sensores = SensorPessoal.GetSensoresByUsername(username);

                if (sensores == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, sensores);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception);
            }
        }

        [BasicAuthentication]
        [MyAuthorize(Roles = "User")]
        [Route("api/sensores-users/me")]
        [HttpGet]
        public HttpResponseMessage GetMyPessoaisSensores()
        {
            List<SensorPessoal> sensores = null;
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                int id = int.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault());

                sensores = SensorPessoal.GetMySensores(id);

                if (sensores == null)
                {
                    Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, sensores);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception);
            }
        }


        [BasicAuthentication]
        [MyAuthorize(Roles = "User")]
        [Route("api/sensores-users/")]
        [HttpPost]
        public HttpResponseMessage CreateNewSensorPessoal([FromBody]SensorPessoal sensor)
        {
            try
            {
                if (sensor == null || sensor.Temperatura == 0 || sensor.Humidade == 0 || sensor.Local == null)
                {
                    var form = new Dictionary<string, string>
                    {
                        { "Id", "(automatic) Type:Int" },
                        { "Temperatura", "(required) Type:Decimal" },
                        { "Humidade", "(required) Type:Decimal" },
                        { "Local", "(required) Type:String" },
                        { "Data", "(optional - DEFAULT Current DateTime) Type:Datetime" },
                        { "Valido", "(automatic - DEFAULT true) Type:Bool" },
                        { "Utilizador", "(automatic - DEFAULT Authenticated User), Type:Int" }
                    };

                    return Request.CreateResponse(HttpStatusCode.BadRequest, form);
                }

                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                int id = int.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault());

                SensorPessoal newSensor = SensorPessoal.CreateNewSensor(sensor, id);

                if (newSensor == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, newSensor);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception);
            }
        }


        [BasicAuthentication]
        [MyAuthorize(Roles = "User")]
        [Route("api/sensores-users/{id:int}")]
        [HttpPut]
        public HttpResponseMessage UpdateSensor([FromBody]SensorPessoal sensor, int id)
        {
            try
            {
                if (sensor == null)
                {
                    var form = new Dictionary<string, string>
                    {
                        { "Valido", "(optional - DEFAULT false) Type:Bool" },
                        { "Data", "(optional - DEFAULT Current DateTime) Type:Datetime" },
                    };

                    return Request.CreateResponse(HttpStatusCode.BadRequest, form);
                }

                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                int current_user = int.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault());

                SensorPessoal updatedSensor = SensorPessoalValidatedBy.UpdateValido(sensor, id, current_user);

                if (updatedSensor == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, updatedSensor);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception);
            }
        }
    }
}
