using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace GamestatsBase
{
    public class GamestatsHandler : IHttpHandler, IRequiresSessionState
    {
        public GamestatsHandler(String initString, GamestatsRequestVersions reqVersion, 
            GamestatsResponseVersions respVersion)
        {
            if (initString.Length < 44) throw new FormatException();

            String salt = initString.Substring(0, 20);
            uint rngMul = UInt32.Parse(initString.Substring(20, 8), NumberStyles.AllowHexSpecifier);
            uint rngAdd = UInt32.Parse(initString.Substring(28, 8), NumberStyles.AllowHexSpecifier);
            uint rngMask = UInt32.Parse(initString.Substring(36, 8), NumberStyles.AllowHexSpecifier);
            uint hashMask = UInt32.Parse(initString.Substring(44, 8), NumberStyles.AllowHexSpecifier);
            String gameId = initString.Substring(52);

            Initialize(salt, rngMul, rngAdd, rngMask, hashMask, gameId, reqVersion, respVersion);
        }

        public GamestatsHandler(String salt, uint rngMul, uint rngAdd, uint rngMask, 
            uint hashMask,
            String gameId, GamestatsRequestVersions reqVersion, GamestatsResponseVersions respVersion)
        {
            Initialize(salt, rngMul, rngAdd, rngMask, hashMask, gameId, reqVersion, respVersion);
        }

        private void Initialize(String salt, uint rngMul, uint rngAdd, uint rngMask, 
            uint hashMask,
            String gameId, GamestatsRequestVersions reqVersion, GamestatsResponseVersions respVersion)
        {
            if (salt.Length != 20) throw new FormatException();
            Salt = salt;
            RngMul = rngMul;
            RngAdd = rngAdd;
            RngMask = rngMask;
            HashMask = hashMask;
            GameId = gameId;
            RequestVersion = reqVersion;
            ResponseVersion = respVersion;
        }

        public String Salt { get; private set; }
        public uint RngMul { get; private set; }
        public uint RngAdd { get; private set; }
        public uint RngMask { get; private set; }
        public uint HashMask { get; private set; }
        public String GameId { get; private set; }
        public GamestatsRequestVersions RequestVersion { get; private set; }
        public GamestatsResponseVersions ResponseVersion { get; private set; }

        public GamestatsSessionManager SessionManager
        {
            get;
            protected set;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.HttpMethod != "POST" &&
                context.Request.HttpMethod != "GET")
            {
                ShowError(context, 400);
                return;
            }

            NameValueCollection form = context.Request.HttpMethod == "POST" ?
                context.Request.Form : context.Request.QueryString;

            int pid;
            if (form["pid"] == null ||
                !Int32.TryParse(form["pid"], out pid))
            {
                // pid missing or bad format
                ShowError(context, 400);
                return;
            }

            String rawPath = context.Request.RawUrl;
            int qmPos = rawPath.IndexOf('?');
            if (qmPos >= 0)
                rawPath = rawPath.Substring(0, qmPos);

            SessionManager = GamestatsSessionManager.FromContext(context);

            if (form["data"] == null &&
                form["hash"] == null)
            {
                // this is a new session request
                GamestatsSession session = CreateSession(pid, rawPath);
                SessionManager.Add(session);

                context.Response.Write(session.Token);
                return;
            }
            else if (form.Count >= 3)
            {
                // this is a main request
                if (form["hash"] == null ||
                    form["data"] == null ||
                    form["data"].Length < 
                    ((RequestVersion != GamestatsRequestVersions.Version3) ? 12 : 16))
                {
                    // arguments missing, partial check for data length.

                    // In version 1-2, we require data to hold at least 7 bytes
                    // in this check. In reality, it must hold at least 8,
                    // which is checked for below after decoding

                    // In version 3, we require data to hold at least 10 bytes
                    // in this check, but it actually needs to hold 12.

                    // We do incomplete checks so we can fail as early as
                    // possible. In this case, before base64 decoding happens.

                    ShowError(context, 400);
                    return;
                }

                GamestatsSession session = null;

                if (SessionManager.Sessions.ContainsKey(form["hash"]))
                {
                    session = SessionManager.Sessions[form["hash"]];
                    if (session.GameId != GameId)
                    {
                        // matched wrong game. Highly unlikely
                        ShowError(context, 400);
                        return;
                    }
                }
                else if (context.Request.HttpMethod == "GET")
                {
                    // session hash not matched
                    ShowError(context, 400);
                    return;
                }

                byte[] data = null;
                try
                {
                    data = DecryptData(form["data"]);
                    if (data.Length < ((RequestVersion != GamestatsRequestVersions.Version3) 
                        ? 4 : 8))
                    {
                        // data too short to contain a pid
                        // We check for 4 bytes, not 8, since the decrypt seed
                        // isn't included in DecryptData's result.
                        // On version 2, we require 8 bytes (but not 12) to
                        // make room for the length field.
                        ShowError(context, 400);
                        return;
                    }
                }
                catch (FormatException)
                {
                    // fixme: Animal Crossing DS uses invalid base64 strings.
                    // It actually expects raw ASCII... >_________<
                    // We need to skip this check completely if RequestVersion
                    // is 1.

                    // data too short to contain a checksum,
                    // base64 format errors
                    ShowError(context, 400);
                    return;
                }

                int pid2 = BitConverter.ToInt32(data, 0);
                if (pid2 != pid)
                {
                    // packed pid doesn't match ?pid=
                    ShowError(context, 400);
                    return;
                }

                if (RequestVersion == GamestatsRequestVersions.Version3)
                {
                    int length = BitConverter.ToInt32(data, 4);
                    if (length + 8 != data.Length)
                    {
                        // message length is incorrect
                        ShowError(context, 400);
                        return;
                    }
                }

                // todo: we can save another copy by doing this trimming in
                // DecryptData.
                int trimLength = (RequestVersion != GamestatsRequestVersions.Version3) ? 4 : 8;
                byte[] dataTrim = new byte[data.Length - trimLength];
                Array.Copy(data, trimLength, dataTrim, 0, data.Length - trimLength);

                MemoryStream response = new MemoryStream();
                try
                {
                    ProcessGamestatsRequest(dataTrim, response, rawPath, pid, context, session);
                }
                catch (GamestatsException ex)
                {
                    ShowError(context, ex.ResponseCode, ex.Message);
                    return;
                }
                catch (Exception)
                {
                    ShowError(context, 500);
                    return;
                }

                response.Flush();
                byte[] responseArray = response.ToArray();
                response.Dispose();
                response = null;

                context.Response.OutputStream.Write(responseArray, 0, responseArray.Length);

                if (ResponseVersion == GamestatsResponseVersions.Version3)
                    context.Response.Write("done");
                if (ResponseVersion != GamestatsResponseVersions.Version1)
                    context.Response.Write(ResponseChecksum(responseArray));
            }
            else
            {
                // wrong number of querystring arguments
                ShowError(context, 400);
                return;
            }
        }

        // todo: keep the context on a field instead of passing it here
        public void ShowError(HttpContext context, int responseCode)
        {
            ShowError(context, responseCode, GamestatsException.defaultMessage(responseCode));
        }

        public void ShowError(HttpContext context, int responseCode, String message)
        {
            context.Response.StatusCode = responseCode;
            context.Response.Write(GamestatsException.defaultMessage(responseCode));
        }

        public virtual bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public virtual void ProcessGamestatsRequest(byte[] request, MemoryStream response, String url, int pid, HttpContext context, GamestatsSession session)
        {

        }

        /// <summary>
        /// Session factory if you need it. You probably don't.
        /// </summary>
        public virtual GamestatsSession CreateSession(int pid, String url)
        {
            return new GamestatsSession(GameId, Salt, pid, url);
        }

        /// <summary>
        /// Decrypts the NDS &data= querystring into readable binary data.
        /// The PID (little endian) is left at the start of the output
        /// but the (unencrypted) checksum is removed.
        /// </summary>
        private byte[] DecryptData(String data)
        {
            byte[] data2 = FromUrlSafeBase64String(data);
            if (RequestVersion == GamestatsRequestVersions.Version1) return data2;
            if (data2.Length < 4) throw new FormatException("Data must contain at least 4 bytes.");

            byte[] data3 = new byte[data2.Length - 4];
            int checksum = BitConverter.ToInt32(data2, 0);
            checksum = IPAddress.NetworkToHostOrder(checksum); // endian flip
            checksum ^= (int)HashMask;
            int rand = checksum | (checksum << 16);

            for (int pos = 0; pos < data3.Length; pos++)
            {
                rand = DecryptRNG(rand);
                data3[pos] = (byte)(data2[pos + 4] ^ (byte)(rand >> 16));
            }

            int checkedsum = 0;
            foreach (byte b in data3)
                checkedsum += b;

            if (checkedsum != checksum) throw new FormatException("Data checksum is incorrect.");

            return data3;
        }

        private static int DecryptRNG(int prev, uint mul, uint add, uint mask)
        {
            return (prev * (int)mul + (int)add) & ~(int)mask;
        }

        private int DecryptRNG(int prev)
        {
            return DecryptRNG(prev, RngMul, RngAdd, RngMask);
        }

        public static byte[] FromUrlSafeBase64String(String data)
        {
            return Convert.FromBase64String(data.Replace('-', '+').Replace('_', '/'));
        }

        public static String ToUrlSafeBase64String(byte[] data)
        {
            return Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_');
        }

        private static SHA1 m_sha1;

        private String ResponseChecksum(byte[] responseArray)
        {
            if (m_sha1 == null) m_sha1 = SHA1.Create();

            String toCheck = Salt + ToUrlSafeBase64String(responseArray) + Salt;

            byte[] data = new byte[toCheck.Length];
            MemoryStream stream = new MemoryStream(data);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(toCheck);
            writer.Flush();

            return m_sha1.ComputeHash(data).ToHexStringLower();
        }
    }

    public enum GamestatsRequestVersions
    {
        /// <summary>
        /// Version 1 has very little validation. Request data doesn't contain
        /// any of the headers found in later versions and is unencrypted.
        /// </summary>
        Version1,
        /// <summary>
        /// Data contains an obfuscated checksum and pid, and supports
        /// enryption.
        /// </summary>
        Version2,
        /// <summary>
        /// Data contains an obfuscated checksum, pid, and payload length, and
        /// supports encryption.
        /// </summary>
        Version3
    }

    public enum GamestatsResponseVersions
    {
        /// <summary>
        /// Response is plain raw binary data.
        /// </summary>
        Version1,
        /// <summary>
        /// Response contains a salted hash at the end, encoded in hex.
        /// </summary>
        Version2,
        /// <summary>
        /// Response contains the word "done" followed by the salted hash.
        /// </summary>
        Version3
    }

    public class GamestatsException : ApplicationException
    {
        public readonly int ResponseCode;

        public GamestatsException() : this(500)
        {
        }

        public GamestatsException(int responseCode)
            : base(defaultMessage(responseCode))
        {
            ResponseCode = responseCode;
        }

        public GamestatsException(int responseCode, String message)
            : base(message)
        {
            ResponseCode = responseCode;
        }

        public GamestatsException(int responseCode, String message, Exception innerException)
            : base(message, innerException)
        {
            ResponseCode = responseCode;
        }

        public static String defaultMessage(int responseCode)
        {
            switch (responseCode)
            {
                case 400:
                    return "Bad request";
                case 404:
                    return "This handler is not supported. (404)";
                case 500:
                default:
                    return "Server error";
            }
        }
    }
}
