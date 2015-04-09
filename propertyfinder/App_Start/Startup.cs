using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Cors;

using OHWebService.Authentication;

namespace propertyfinder
{
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app
                .UseCors(CorsOptions.AllowAll)
	            .Use(typeof(JwtOwinAuth))
                .UseNancy();
        }
    }
}