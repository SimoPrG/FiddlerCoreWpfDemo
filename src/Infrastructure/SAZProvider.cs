using System;

using Fiddler;

namespace Infrastructure
{
    internal class SAZProvider : ISAZProvider
    {
        public static Func<string> fnObtainPwd
        {
            get;
            set;
        }

        public ISAZWriter CreateSAZ(string sFilename)
        {
            return new SAZWriter(sFilename);
        }

        public ISAZReader LoadSAZ(string sFilename)
        {
            return new SAZReader(sFilename);
        }

        public bool BufferLocally
        {
            get
            {
                return false;
            }
        }

        public bool SupportsEncryption
        {
            get
            {
                return true;
            }
        }
    }
}
