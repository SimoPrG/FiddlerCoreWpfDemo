using System;
using System.Text;
using System.IO;
using Ionic.Zip;
using System.Diagnostics;

using Fiddler;

namespace FiddlerCoreWpfDemo.Infrastructure
{
    class SAZWriter : ISAZWriter
    {
        private ZipFile _oZip;

        internal SAZWriter(string sFilename)
        {
            Filename = sFilename;
            _oZip = new ZipFile(sFilename);

            // We may need to use Zip64 format if the user saves more than 21844 sessions, because
            // each session writes 3 files and the non-Zip64 format is limited to 65535 files.
            _oZip.UseZip64WhenSaving = Zip64Option.AsNecessary;

            // Create the directory explicitly (not strictly required) because this matches
            // legacy behavior and some code checks for it.
            _oZip.AddDirectoryByName("raw");
        }

        private string _EncryptionMethod;
        public string EncryptionMethod
        {
            get
            {
                if (String.IsNullOrEmpty(_EncryptionMethod)) StoreEncryptionInfo(_oZip.Encryption);
                return _EncryptionMethod;
            }
        }

        private void StoreEncryptionInfo(EncryptionAlgorithm oEA)
        {
            switch (oEA)
            {
                case EncryptionAlgorithm.PkzipWeak:
                    _EncryptionMethod = "PKZip";
                    _EncryptionStrength = "56"; // Is that right?
                    break;
                case EncryptionAlgorithm.WinZipAes128:
                    _EncryptionMethod = "WinZipAes";
                    _EncryptionStrength = "128";
                    break;
                case EncryptionAlgorithm.WinZipAes256:
                    _EncryptionMethod = "WinZipAes";
                    _EncryptionStrength = "256";
                    break;
                default:
                    Debug.Assert(false, "Unknown encryption algorithm");
                    _EncryptionMethod = "Unknown";
                    _EncryptionStrength = "0";
                    break;
            }
        }

        private string _EncryptionStrength;
        public string EncryptionStrength
        {
            get
            {
                if (string.IsNullOrEmpty(_EncryptionStrength)) StoreEncryptionInfo(_oZip.Encryption);
                return _EncryptionStrength;
            }
        }

        public void AddFile(string sFilename, SAZWriterDelegate oSWD)
        {
            WriteDelegate oWD = (sFN, oS) =>
            {
                oSWD.Invoke(oS);
            };
            _oZip.AddEntry(sFilename, oWD);
        }

        /// <summary>
        /// Writes the ContentTypes XML to the ZIP so Packaging APIs can read it.
        /// See http://en.wikipedia.org/wiki/Open_Packaging_Conventions
        /// </summary>
        /// <param name="odfZip"></param>
        private void WriteODCXML()
        {
            const string oPFContentTypeXML =
               "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">\r\n" +
               "<Default Extension=\"htm\" ContentType=\"text/html\" />\r\n<Default Extension=\"xml\" ContentType=\"application/xml\" />\r\n<Default Extension=\"txt\" ContentType=\"text/plain\" />\r\n</Types>";

            _oZip.AddEntry("[Content_Types].xml", new WriteDelegate(delegate (string sn, Stream strmToWrite)
            {
                byte[] arrODCXML = Encoding.UTF8.GetBytes(oPFContentTypeXML);
                strmToWrite.Write(arrODCXML, 0, arrODCXML.Length);
            }));
        }

        public bool CompleteArchive()
        {
            WriteODCXML();
            _oZip.Save();
            _oZip = null;

            return true;
        }

        public string Filename { get; }

        public string Comment
        {
            get
            {
                return _oZip.Comment;
            }
            set
            {
                _oZip.Comment = value;
            }
        }

        public bool SetPassword(string sPassword)
        {
            if (!String.IsNullOrEmpty(sPassword))
            {
                if (CONFIG.bUseAESForSAZ)
                {
                    if (FiddlerApplication.Prefs.GetBoolPref("fiddler.saz.AES.Use256Bit", false))
                    {
                        _oZip.Encryption = EncryptionAlgorithm.WinZipAes256;
                    }
                    else
                    {
                        _oZip.Encryption = EncryptionAlgorithm.WinZipAes128;
                    }
                }
                _oZip.Password = sPassword;
            }

            return true;
        }
    }
}
