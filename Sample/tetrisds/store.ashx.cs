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
        public store() : base("Wo3vqrDoL56sAdveYeC1", 0x00000000u, 0x00000000u, 0x00000000u, 0x00000000u, "tetrisds", GamestatsRequestVersions.Version1, GamestatsResponseVersions.Version1)
        {

        }

        public override void ProcessGamestatsRequest(byte[] request, System.IO.MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {
            byte[] nameBytes = FromUrlSafeBase64String(context.Request.QueryString["name"]);
            char[] nameChars = new char[nameBytes.Length >> 1];

            Buffer.BlockCopy(nameBytes, 0, nameChars, 0, nameBytes.Length);
            String name = new String(nameChars);

            // Since the correct response is actually blank, we don't need to
            // write anything to it here.
        }
    }
}
