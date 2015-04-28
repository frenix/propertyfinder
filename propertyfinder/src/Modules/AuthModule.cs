﻿/*
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
            secretKey = System.Configuration.ConfigurationManager.AppSettings["SecretKey"];

            Post ["/"] = _ => LoginHandler(this.Bind<LoginRequest>());

            //PUT: Token JSON in body {"token":"XXXX"}
            Put["/"] = _ => LoginConfirmAcct(); 
            
        }

        /// <summary>
        /// Validates login credentials
        /// </summary>
        /// <param name="loginRequest"> a class that contains email and password</param>
        /// <returns>either a JWT token or Unauthorized</returns>
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
        
        
        Nancy.Response LoginConfirmAcct()
        {

            // debug code only
            // capture actual string posted in case the bind fails (as it will if the JSON is bad)
            // need to do it now as the bind operation will remove the data
            //String rawBody = this.GetBodyRaw(); 
            String rawBody = CommonModule.GetBodyRaw(this.Request);

            AgentModelToken agentauthkey = null;
            try
            {
                // bind the request body to the object
                agentauthkey = this.Bind<AgentModelToken>();

                AgentContext ctx = new AgentContext();

                AgentModel agent = ctx.GetByToken(agentauthkey.token);

                if (agent == null)
                {
                    return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "PUT", HttpStatusCode.NotFound, "NG", "Account not found!");
                }
                // if agent is found by authkey, update it
                agent.ConfirmFlg = "1"; //set flag to 1
                ctx.update(agent);
                // no content response
                return MsgBuilder.MsgResponse(this.Request.Url.ToString(), "PUT", HttpStatusCode.OK, "OK", "Update sucessfull!");
            }
            catch (Exception e)
            {
                String operation = String.Format("AgentModule.UpdateAgentByToken({0})", (agentauthkey == null) ? "No Model Data" : agentauthkey.token);
                return CommonModule.HandleException(e, HttpStatusCode.OK, operation, "NG", this.Request);
            }
        }


        /// <summary>
        /// Check if User credential exsits in the db
        /// </summary>
        /// <param name="email">Email Address</param>
        /// <param name="pswd">Password</param>
        /// <returns>Boolean</returns>
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
			else if (res.ConfirmFlg == "0")
            {
                //This user has not confirmed his account yet
                msgInfo = "Account not confirmed! Please check your email and confirm this account.";
                return false;
            }
            else
			{
				return IsEmailExist(email, pswd, res);
			}
				
		}
        
        /// <summary>
        /// Check db if Email and Password exists
        /// </summary>
        /// <param name="email">Email address</param>
        /// <param name="pswd">Password</param>
        /// <param name="agent">Agent details</param>
        /// <returns>Boolean</returns>
     	private bool IsEmailExist(string email, string pswd, AgentModel agent) 
		{
            bool ret = false;

        	if (email.Length == 0 && pswd.Length == 0)
        	{
        		msgInfo = "Empty parameters";
                return false;
        	}
        	
			// a null return means no object found
			if (agent == null) 
			{
				msgInfo = "Email does not exist";
                ret = false;
			} else {
				if(agent.Password != pswd) {
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
