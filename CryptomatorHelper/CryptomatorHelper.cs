//   Hack for version 8 vaults.
//See https://cryptomator.org/blog/2021/10/11/vault-format-8/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using RFC3394;
using CryptSharp.Utility;
using System.Security.Cryptography;

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
        public class VaultConfigHeader
        {
            public string kid { get; set; }
            public string typ { get; set; }
            public string alg { get; set; }
        }

        public class VaultConfigData
        {
            public int format { get; set; }
            public int shorteningThreshold { get; set; }
            public string jti { get; set; }
            public string cipherCombo { get; set; }
        }

        public class VaultConfig
        {
            public VaultConfigHeader vcH;
            public VaultConfigData vcD;
        }

        public class MasterKey
        {
            public string ScryptSalt { get; set; }
            public int ScryptCostParam { get; set; }
            public int ScryptBlockSize { get; set; }
            public string PrimaryMasterKey { get; set; }
            public string HmacMasterKey { get; set; }
            public string VersionMac { get; set; }
            public int Version { get; set; }
        }

        public class Keys
        {
            public byte[] masterKey;
            public byte[] macKey;
            public byte[] sivKey;
        }

        public abstract List<string> GetFiles(string virtualPath = "");
        public abstract List<string> GetDirs(string virtualPath);
        public abstract List<FolderInfo> GetFolders(string virtualPath);
        public abstract void DecryptFile(string virtualPath, string outFile);
        public abstract void DecryptFile(string virtualPath, Stream outputStream);
        public abstract string GetEncryptedFilePath(string virtualPath);

        public static CryptomatorHelper Create(string password, string vaultPath)
        {
            byte[] kek;
            byte[] JWTKey;

            string masterKeyPath = "";
            VaultConfig vaultConfig = null;

            string vaultConfigPath = Path.Combine(vaultPath, "vault.cryptomator");
            if (File.Exists(vaultConfigPath))
            {
                vaultConfig = LoadVaultConfig(vaultConfigPath);
                var kidParts = vaultConfig.vcH.kid.Split(':');
                if (kidParts.Length != 2 || kidParts[0] != "masterkeyfile")
                    throw new Exception($"vault config id parameter unsupported : {vaultConfig.vcH.kid}");
                else
                    masterKeyPath = Path.Combine(vaultPath, kidParts[1]);
            }
            else
                masterKeyPath = Path.Combine(vaultPath, "masterkey.cryptomator");

            if (!File.Exists(masterKeyPath))
                throw new FileNotFoundException("Missing master key file (masterkey.cryptomator)");

            var jsonString = File.ReadAllText(masterKeyPath);
            MasterKey mkey = JsonConvert.DeserializeObject<MasterKey>(jsonString);

            byte[] abPrimaryMasterKey = Convert.FromBase64String(mkey.PrimaryMasterKey);
            byte[] abHmacMasterKey = Convert.FromBase64String(mkey.HmacMasterKey);
            byte[] abScryptSalt = Convert.FromBase64String(mkey.ScryptSalt);

            kek = SCrypt.ComputeDerivedKey(Encoding.ASCII.GetBytes(password), abScryptSalt, mkey.ScryptCostParam, mkey.ScryptBlockSize, 1, 1, 32);

            Keys keys = new Keys();
            keys.masterKey = KeyWrapAlgorithm.UnwrapKey(kek, abPrimaryMasterKey);
            keys.macKey = KeyWrapAlgorithm.UnwrapKey(kek, abHmacMasterKey);
            keys.sivKey = keys.macKey.Concat(keys.masterKey).ToArray();
            JWTKey = keys.masterKey.Concat(keys.macKey).ToArray();

            //Validate vault config if present
            if (vaultConfig != null)
            {
                //Reload the vault config, this time verifying the signature
                vaultConfig = LoadVaultConfig(vaultConfigPath, true, JWTKey);
            }

            if (mkey.Version == 6)
                return new V6CryptomatorHelper(keys, vaultPath);
            else if (mkey.Version == 7)
                return new V7CryptomatorHelper(keys, vaultPath);
            else if (mkey.Version == 999)
            {
                //version must come from vault.cryptomator.  If v8, can handle as if version 7 
                //because there are no structural changes.
                if (vaultConfig == null)
                    throw new Exception("Missing required vault configuration");
                else if (vaultConfig.vcD.format == 8)
                    return new V7CryptomatorHelper(keys, vaultPath);
                else
                    throw new Exception($"Only format 8 vaults are currently support. Vault format is {vaultConfig.vcD.format}");
            }
            else
                throw new ArgumentException($"Vault version {mkey.Version} is unsupported");

        }


        static VaultConfig LoadVaultConfig(string vaultConfigPath, bool verify = false, byte[] key = null)
        {
            try
            {
                string token = File.ReadAllText(vaultConfigPath);
                VaultConfig vaultConfig = GetVaultConfigFromJWT(token, verify, key);
                return vaultConfig;
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot load vault configuration", ex);
            }
        }

        public static VaultConfig GetVaultConfigFromJWT(string token, bool verify = false, byte[] key = null)
        {
            try
            {
                var parts = token.Split('.');

                if (parts.Length != 3)
                    throw new Exception("Vault configuration JWT is invalid");

                var header = parts[0];
                var payload = parts[1];
                var jwtSignature = parts[2];

                var headerJson = Base64Url.DecodeToString(header);
                var vaultConfigHeader = JsonConvert.DeserializeObject<VaultConfigHeader>(headerJson);
                var payloadJson = Base64Url.DecodeToString(payload);
                var vaultConfigData = JsonConvert.DeserializeObject<VaultConfigData>(payloadJson);

                if (verify)
                {
                    HMAC hmac;
                    switch (vaultConfigHeader.alg)
                    {
                        case "HS256": hmac = new HMACSHA256(); break;
                        case "HS384": hmac = new HMACSHA384(); break;
                        case "HS512": hmac = new HMACSHA512(); break;
                        default:
                            throw new Exception("Unsupported vault configuration signature algorithm");
                    }
                    hmac.Key = key;
                    var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
                    var signature = hmac.ComputeHash(bytesToSign);
                    var computedJwtSignature = Base64Url.Encode(signature);
                    if (jwtSignature != computedJwtSignature)
                        throw new Exception("Vault signature is invalid");
                }
                VaultConfig vaultConfig = new VaultConfig();
                vaultConfig.vcH = vaultConfigHeader;
                vaultConfig.vcD = vaultConfigData;
                return vaultConfig;
            }
            catch (Exception ex)
            {

                throw new Exception("Vault configuration invalid or unsupported", ex);
            }
        }

    }
}
