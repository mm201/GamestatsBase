/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GamestatsBase;

namespace Sample.tetrisds
{
    /// <summary>
    /// Summary description for store
    /// </summary>
    public class store : GamestatsHandler
    {
        public store() : base("Wo3vqrDoL56sAdveYeC1", 0, 0, 0, 0, "tetrisds", GamestatsRequestVersions.Version1, GamestatsResponseVersions.Version1)
        {

        }

        public override void ProcessGamestatsRequest(byte[] request, System.IO.MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {
            byte[] nameBytes = FromUrlSafeBase64String(context.Request.QueryString["name"]);
            char[] nameChars = new char[nameBytes.Length >> 1];

            Buffer.BlockCopy(nameBytes, 0, nameChars, 0, nameBytes.Length);
            String name = new String(nameChars);

            String region = context.Request.QueryString["region"];

            // todo: Figure out what the data contains and how to parse it, so
            // we can have a leaderboard.

            // todo: Write name, pid, region, and the mysteries contained
            // within the data blob to a database.

            // Since the correct response is actually blank, we don't need to
            // write anything to it here.
        }
    }
}
