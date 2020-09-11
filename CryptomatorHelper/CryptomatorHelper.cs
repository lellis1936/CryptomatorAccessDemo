using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace CryptomatorTools.Helpers
{

    public class FolderInfo
    {
        public string Name { get; internal set; }
        public string VirtualPath { get; internal set; }
        public bool HasChildren { get; internal set; }

    }

    public abstract class CryptomatorHelper
    {
        private class MasterKey
        {
            public string ScryptSalt { get; set; }
            public int ScryptCostParam { get; set; }
            public int ScryptBlockSize { get; set; }
            public string PrimaryMasterKey { get; set; }
            public string HmacMasterKey { get; set; }
            public string VersionMac { get; set; }
            public int Version { get; set; }
        }

        public abstract List<string> GetFiles(string virtualPath = "");
        public abstract List<string> GetDirs(string virtualPath);
        public abstract List<FolderInfo> GetFolders(string virtualPath);
        public abstract void DecryptFile(string virtualPath, string outFile);
        public abstract void DecryptFile(string virtualPath, Stream outputStream);
        public abstract string GetEncryptedFilePath(string virtualPath);

        public static CryptomatorHelper Create(string password, string vaultPath)
        {
            try
            {
                string masterKeyPath = Path.Combine(vaultPath, "masterkey.cryptomator");

                var jsonString = File.ReadAllText(masterKeyPath);
                MasterKey mkey = JsonConvert.DeserializeObject<MasterKey>(jsonString);

                if (mkey.Version == 6)
                    return new V6CryptomatorHelper(password, vaultPath);
                else if (mkey.Version == 7)
                    return new V7CryptomatorHelper(password, vaultPath);
                else
                    throw new ArgumentException($"Vault version {mkey.Version} is unsupported");

            }
            catch (System.IO.FileNotFoundException e)
            {
                throw new FileNotFoundException("Cannot open master key file (masterkey.cryptomator)", e);
            }

        }
    }
}
