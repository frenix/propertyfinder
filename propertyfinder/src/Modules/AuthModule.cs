/*
 * Created by SharpDevelop.
 * User: Frenix
 * Date: 3/27/2015
 * Time: 8:44 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 * Modified: return of LoginHandler()
 * 
 */
using System;
using System.Collections.Generic;
//using System.Configuration;
using Nancy;
using Nancy.ModelBinding;
using JWT;
using OHWebService.Authentication;
using OHWebService.Models;

namespace OHWebService.Modules
{
	/// <summary>
	/// Description of AuthModule.
	/// </summary>
	public class AuthModule	: Nancy.NancyModule
	{
		private readonly string secretKey;
       // private readonly IUserService userService;
       private string msgInfo = "Access granted!";
               
        public AuthModule ()  : base ("/login")
        {

            Post ["/"] = _ => LoginHandler(this.Bind<LoginRequest>());

            secretKey = System.Configuration.ConfigurationManager.AppSettings ["SecretKey"];
        }

        public dynamic LoginHandler(LoginRequest loginRequest)
        {
            if (IsValidUser (loginRequest.email, loginRequest.password)) {
				
                var payload = new Dictionary<string, object> {
                    { "email", loginRequest.email },
                    { "userId", GuidCreator.New() }
                };

                var token = JsonWebToken.Encode (payload, secretKey, JwtHashAlgorithm.HS256);

                return MsgBuilder.MsgJWTResponse(HttpStatusCode.OK, token, "OK", msgInfo);
            } else {   
               return MsgBuilder.MsgJWTResponse(HttpStatusCode.Unauthorized, "", "NG", msgInfo);
            }
        }
        
        private bool IsValidUser(string email, string pswd) 
		{
        	if (email.Length == 0 && pswd.Length == 0)
        	{
        		return false;
        	}
        	//check expiry
            //https://github.com/jchannon/Owin.StatelessAuth/blob/master/src/Owin.StatelessAuthExample/MySecureTokenValidator.cs
            // create a connection to the PetaPoco orm and try to fetch and object with the given Id
			AgentContext ctx = new AgentContext();
			AgentModel res = ctx.GetByEmailAddAndPwd(email, pswd);
			// a null return means no object found
			if (res == null) 
			{
				msgInfo = "This profile does not exists!";
				return false;
			} 
			else
			{
				return IsEmailExist(email, pswd);
			}
				
		}
        
     	private bool IsEmailExist(string email, string pswd) 
		{
            bool ret = false;

        	if (email.Length == 0 && pswd.Length == 0)
        	{
        		msgInfo = "Empty parameters";
                return false;
        	}
        	
            // create a connection to the PetaPoco orm and try to fetch and object with the given Id
			AgentContext ctx = new AgentContext();
			AgentModel res = ctx.GetEmail(email);
			// a null return means no object found
			if (res == null) 
			{
				msgInfo = "Email does not exist";
                ret = false;
			} else {
				if(res.Password != pswd) {
					msgInfo = "Password mismatch";
                    ret = false;
				} else {
					msgInfo = "Email exists";
                    ret = true;
				}
			}

            return ret;			
		}

    } //end of AuthModule 

	

    public class LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }
	
}
