using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using System.Diagnostics;

using Fiddler;

namespace Infrastructure
{
    internal class SAZReader : ISAZReader
    {
        private ZipFile _oZip;
        private string _sPassword;

        public string Filename { get; }

        public void Close()
        {
            _oZip.Dispose();
            _oZip = null;
        }

        public string Comment
        {
            get
            {
                return _oZip.Comment;
            }
        }
        public string EncryptionMethod { get; private set; }

        public string EncryptionStrength { get; private set; }

        private bool PromptForPassword()
        {
            Func<string> fnGetPwd = SAZProvider.fnObtainPwd;

            if (null == fnGetPwd) return false;
            _sPassword = fnGetPwd.Invoke();

            if (!String.IsNullOrEmpty(_sPassword))
            {
                _oZip.Password = _sPassword;
                return true;
            }

            return false;
        }

        public Stream GetFileStream(string sFilename)
        {
            ZipEntry oZE = _oZip[sFilename];
            if (null == oZE)
            {
                return null;
            }

            if ((oZE.UsesEncryption) && String.IsNullOrEmpty(_sPassword))
            {
                StoreEncryptionInfo(oZE.Encryption);
                if (!PromptForPassword()) { throw new OperationCanceledException("Password required."); }
            }

            Stream strmResult = null;

        RetryWithPassword:
            try
            {
                strmResult = oZE.OpenReader();
            }
            catch (BadPasswordException)
            {
                if (!PromptForPassword()) { throw new OperationCanceledException("Password required."); }
                goto RetryWithPassword;
            }
            catch (Exception eX)
            {
                Debug.Assert(false, eX.Message);
                FiddlerApplication.ReportException(eX, "Error saving SAZ");
            }

            return strmResult;
        }

        private void StoreEncryptionInfo(EncryptionAlgorithm oEA)
        {
            switch (oEA)
            {
                case EncryptionAlgorithm.PkzipWeak:
                    EncryptionMethod = "PKZip";
                    EncryptionStrength = "56";
                    break;
                case EncryptionAlgorithm.WinZipAes128:
                    EncryptionMethod = "WinZipAes";
                    EncryptionStrength = "128";
                    break;
                case EncryptionAlgorithm.WinZipAes256:
                    EncryptionMethod = "WinZipAes";
                    EncryptionStrength = "256";
                    break;
                default:
                    Debug.Assert(false, "Unknown encryption algorithm");
                    EncryptionMethod = "Unknown";
                    EncryptionStrength = "0";
                    break;
            }
        }

        public byte[] GetFileBytes(string sFilename)
        {
            Stream strmBytes = this.GetFileStream(sFilename);
            if (strmBytes == null) return null;

            byte[] arrData = Utilities.ReadEntireStream(strmBytes);
            strmBytes.Close();
            return arrData;
        }

        public string[] GetRequestFileList()
        {
            List<string> listFiles = new List<string>();

            foreach (ZipEntry oZE in _oZip)
            {
                if (!oZE.FileName.EndsWith("_c.txt", StringComparison.OrdinalIgnoreCase) || !oZE.FileName.StartsWith("raw/", StringComparison.OrdinalIgnoreCase))
                {
                    // Not a request. Skip it.
                    continue;
                }

                listFiles.Add(oZE.FileName);
            }

            return listFiles.ToArray();
        }

        internal SAZReader(string sFilename)
        {
            Filename = sFilename;
            _oZip = new ZipFile(sFilename);
            foreach (string s in _oZip.EntryFileNames)
            {
                Trace.WriteLine(s);

            }
        }
    }
}
