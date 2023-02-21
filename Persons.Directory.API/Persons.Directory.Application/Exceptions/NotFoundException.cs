using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Persons.Directory.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public HttpStatusCode Code { get; set; }

        public bool ShowMessage { get; set; }

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, HttpStatusCode code) : base(message)
        {
            Code = code;
        }

        public NotFoundException(string message, bool showMessage = false) : base(message)
        {
            ShowMessage = showMessage;
        }
    }
}
