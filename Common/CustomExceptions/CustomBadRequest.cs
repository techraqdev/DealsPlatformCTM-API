using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.CustomExceptions;

public sealed class CustomBadRequest :Exception 
{
    public CustomBadRequest(string message,int statusCode): base(message)
    {
        this.StatusCode = statusCode;
        this.ErrorDescription = message;
    }
    public int StatusCode { get; set; }
    public string ErrorDescription { get; set; }
}



