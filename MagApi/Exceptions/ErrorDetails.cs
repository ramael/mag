using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi.Exceptions
{
    public class ErrorDetails
    {
		public int StatusCode { get; set; }
		public string Message { get; set; }

		public ErrorDetails() { }

		public ErrorDetails(int StatusCode, string message)
		{
			this.StatusCode = StatusCode;
			this.Message = message;
		}
		
		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
		
	}
}
