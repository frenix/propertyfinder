/*
 * Created by SharpDevelop.
 * User: durane
 * Date: 3/13/2015
 * Time: 9:29 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Nancy;
using Nancy.Responses;
using OHWebService.Models;

namespace OHWebService.Modules
{
	/// <summary>
	/// Description of ErrorBuilder.
	/// </summary>
	public class MsgBuilder
	{
		public static Nancy.Response MsgResponse(string url, string verb, HttpStatusCode code, string status, string infoMessage)
		{
			MsgBody e = new MsgBody
			{
				Url = url, 
				Operation = verb,
				Status = status,
				Message = infoMessage
			};
			// Build and return an object that the Nancy server knows about.
			Nancy.Response response = new Nancy.Responses.JsonResponse<MsgBody>(e, new DefaultJsonSerializer());
			response.StatusCode = code;
			return response;
		}

		public static Nancy.Response MsgJWTResponse(HttpStatusCode code, string token, string status, string msg, AgentModel agent)
		{
			JwtTokenMsg e = new JwtTokenMsg
			{
				Token = token,
				Status = status,
				Message = msg,
                Agent = agent
			};
			// Build and return an object that the Nancy server knows about.
			Nancy.Response response = new Nancy.Responses.JsonResponse<JwtTokenMsg>(e, new DefaultJsonSerializer());
			response.StatusCode = code;
			return response;
		}
	}
	
	// useful info to return in an error
    public class MsgBody
    {
        public string Url {get; set; }
        public string Operation { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
    
    // useful info for Auth module
    // this is a specific class for login module returning an agent data
    // todo: revise so as not to use AgentModel
    public class JwtTokenMsg
    {
        public string Token { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public AgentModel Agent { get; set; }
    }
}
