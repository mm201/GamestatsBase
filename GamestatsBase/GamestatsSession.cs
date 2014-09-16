using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GamestatsBase
{
    public class GamestatsSession
    {
        protected static RNGCryptoServiceProvider m_rng;
        protected static SHA1 m_sha1;

        private int m_pid;
        private String m_url;
        private String m_token;
        private String m_hash;
        private DateTime m_expiry_date;

        public GamestatsSession(String gameId, String salt, int pid, String url)
        {
            PID = pid;
            URL = url;
            ExpiryDate = DateTime.UtcNow.AddMinutes(10);
            Token = CreateToken();
            Hash = CreateHash(Token, salt);
            GameId = gameId;
        }

        /// <summary>
        /// A PID is a user ID which is associated with a game cartridge.
        /// </summary>
        public int PID
        {
            get
            {
                return m_pid;
            }
            protected set
            {
                m_pid = value;
            }
        }

        /// <summary>
        /// The URL in which this session began
        /// </summary>
        public String URL
        {
            get
            {
                return m_url;
            }
            protected set
            {
                m_url = value;
            }
        }

        /// <summary>
        /// 32 chars of random data which both functions as a challenge
        /// and as a session ID
        /// </summary>
        public String Token
        {
            get
            {
                return m_token;
            }
            protected set
            {
                m_token = value;
            }
        }

        public String Hash
        {
            get
            {
                return m_hash;
            }
            protected set
            {
                m_hash = value;
            }
        }

        public DateTime ExpiryDate
        {
            get
            {
                return m_expiry_date;
            }
            protected set
            {
                m_expiry_date = value;
            }
        }

        public String GameId
        {
            get;
            protected set;
        }

        public object Tag
        {
            get;
            set;
        }

        public static String CreateToken()
        {
            if (m_rng == null) m_rng = new RNGCryptoServiceProvider();

            char[] token = new char[32];
            byte[] data = new byte[4];

            for (int x = 0; x < token.Length; x++)
            {
                // tokens have 62 possible chars: 0-9, A-Z, and a-z
                m_rng.GetBytes(data);
                uint rand = BitConverter.ToUInt32(data, 0) % 62u;

                if (rand < 10)
                    token[x] = (char)('0' + rand);
                else if (rand < 36)
                    token[x] = (char)('7' + rand); // 'A' + rand - 10
                else
                    token[x] = (char)('=' + rand); // 'a' + rand - 36
            }
            return new String(token);
        }

        public static String CreateHash(String token, String salt)
        {
            if (m_sha1 == null) m_sha1 = SHA1.Create();

            String longToken = salt + token;

            byte[] data = new byte[longToken.Length];
            MemoryStream stream = new MemoryStream(data);
            StreamWriter writer = new StreamWriter(stream, Encoding.ASCII);
            writer.Write(longToken);
            writer.Flush();

            return m_sha1.ComputeHash(data).ToHexStringLower();
        }
    }
}
