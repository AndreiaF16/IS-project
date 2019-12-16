using System;

namespace IPLeiriaSmartCampusAPI.Controllers
{
    internal class RestRequest
    {
        public object DateFormat { get; set; }
        public string Resource { get; set; }
        public object Method { get; set; }

        internal void AddParameter(string v, object p, object requestBody)
        {
            throw new NotImplementedException();
        }
    }
}