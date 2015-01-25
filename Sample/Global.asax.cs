/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using GamestatsBase;

namespace Sample
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            String pathInfo, query;
            String targetUrl = RewriteUrl(Request.Url.PathAndQuery, out pathInfo, out query);

            if (targetUrl != null)
            {
                Context.RewritePath(targetUrl, pathInfo, query, false);
            }
        }

        void Application_EndRequest(object sender, EventArgs e)
        {
            // todo: find a way to make session expiry happen entirely in the
            // GamestatsBase library.
            // Currently, I run into a problem binding the EndRequest event:
            // http://stackoverflow.com/questions/2781346/why-can-event-handlers-only-be-bound-to-httpapplication-events-during-ihttpmodul
            GamestatsSessionManager.FromContext(Context).PruneSessions();
        }

        public static String RewriteUrl(String url, out String pathInfo, out String query)
        {
            int q = url.IndexOf('?');
            String path;
            pathInfo = "";

            if (q < 0)
            {
                path = url;
                query = "";
            }
            else
            {
                path = url.Substring(0, q);
                query = url.Substring(q + 1);
            }

            // Since our handler is ashx, not ASP classic, we need to rewrite
            // the .asp file extension so it will execute.
            if (path.Length < 4) return null;
            if (path.Substring(path.Length - 4).ToLowerInvariant() != ".asp") return null;
            return path.Substring(0, path.Length - 4) + ".ashx";
        }
    }
}
