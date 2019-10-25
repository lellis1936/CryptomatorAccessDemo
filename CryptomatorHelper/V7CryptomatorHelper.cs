//TODO Version 7 long filename handling.  DONE, but needs major refactoring.  Also need to check long directory names in v6
//TODO In version 6, sync up file handling with this code.
// Handle long directory names
// Review Exception handling

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;

using Newtonsoft.Json;
using Miscreant;
using RFC3394;
using CryptSharp.Utility;
using CryptomatorTools.Helpers;


public class V7CryptomatorHelper:CryptomatorHelper
{
    readonly string vaultPath;
    readonly byte[] kek;
    readonly byte[] masterKey;
    readonly byte[] macKey;
    readonly byte[] sivKey;
    readonly Aead siv;
    readonly string pathSeparator = Path.DirectorySeparatorChar.ToString();
    readonly string physicalPathRoot;

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

    private class DirInfo
    {
        public string Name { get; set; }
        public string VirtualPath { get; set; }
        public string PhysicalPath { get; set; }
        public string ParentDirId { get; set; }
        public int Level { get; set; }
    }

    private SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

    public V7CryptomatorHelper(string password, string vaultPath)
    {
        try
        {
            string masterKeyPath = PathJoin(vaultPath, "masterkey.cryptomator");

            var jsonString = File.ReadAllText(masterKeyPath);
            MasterKey mkey = JsonConvert.DeserializeObject<MasterKey>(jsonString);

            if (mkey.Version != 7)
                throw new ArgumentException("Only version 7 vaults are supported");

            byte[] abPrimaryMasterKey = Convert.FromBase64String(mkey.PrimaryMasterKey);
            byte[] abHmacMasterKey = Convert.FromBase64String(mkey.HmacMasterKey);
            byte[] abScryptSalt = Convert.FromBase64String(mkey.ScryptSalt);

            kek = SCrypt.ComputeDerivedKey(Encoding.ASCII.GetBytes(password), abScryptSalt, mkey.ScryptCostParam, mkey.ScryptBlockSize, 1, 1, 32);

            masterKey = KeyWrapAlgorithm.UnwrapKey(kek, abPrimaryMasterKey);
            macKey = KeyWrapAlgorithm.UnwrapKey(kek, abHmacMasterKey);
            sivKey = macKey.Concat(masterKey).ToArray();

            this.vaultPath = vaultPath;
            siv = Aead.CreateAesCmacSiv(sivKey);

            byte[] ciphertext = siv.Seal(new byte[0]);
            byte[] hash = sha1.ComputeHash(ciphertext);
            string fullDirName = Base32Encoding.ToString(hash);
            physicalPathRoot = PathJoin(fullDirName.Substring(0, 2), fullDirName.Substring(2));

        }
        catch (System.IO.FileNotFoundException e)
        {
            throw new FileNotFoundException("Cannot open master key file (masterkey.cryptomator)", e);
        }

        catch (CryptographicException e)
        {
            throw new CryptographicException("Cannot open vault, possible password error", e);
        }
    }

    public override List<string> GetFiles(string virtualPath = "")
    {
        string[] virtualDirHierarchy = GetDirHierarchy(virtualPath);

        Stack<DirInfo> stack = new Stack<DirInfo>();
        var fileList = new List<string>();

        stack.Push(GetRootDirInfo());

        while (stack.Count > 0)
        {
            var dir = stack.Pop();
            var di = new DirectoryInfo(dir.PhysicalPath);
            var fsis = di.EnumerateFileSystemInfos();

            foreach (FileSystemInfo fsi in fsis)
            {
                string encryptedFilename = fsi.Name;
                if (IsVirtualDirectory(fsi))
                {
                    //It's a directory...
                    if (dir.Level < virtualDirHierarchy.Length)
                    {
                        DirInfo newDir = CreateDirInfo(fsi.FullName, encryptedFilename, dir);
                        if (newDir.Name.ToLower() == virtualDirHierarchy[dir.Level].ToLower())
                        {
                            stack.Push(newDir);
                            break;
                        }
                    }
                }
                else
                {
                    //It's a file...  
                    if (dir.Level == virtualDirHierarchy.Length)
                    {
                        string Base64EncryptedName = encryptedFilename;
                        var filename = DecryptFileName(fsi.FullName, dir.ParentDirId);
                        fileList.Add(PathJoin(dir.VirtualPath, filename));
                    }
                }
            }
        }
        return fileList;
    }

    public override List<string> GetDirs(string virtualPath)
    {
        string[] virtualDirHierarchy = GetDirHierarchy(virtualPath);

        Stack<DirInfo> stack = new Stack<DirInfo>();
        var dirList = new List<string>();

        stack.Push(GetRootDirInfo());

        while (stack.Count > 0)
        {
            var dir = stack.Pop();
            var files = Directory.EnumerateDirectories(dir.PhysicalPath);

            foreach (string d in files)
            {
                string encryptedFilename = Path.GetFileName(d);
                if (IsVirtualDirectory(d))
                {
                    if (dir.Level < virtualDirHierarchy.Length)
                    {
                        DirInfo newDir = CreateDirInfo(d, encryptedFilename, dir);
                        if (newDir.Name.ToLower() == virtualDirHierarchy[dir.Level].ToLower())
                        {
                            stack.Push(newDir);
                            break;
                        }
                    }
                    else if (dir.Level == virtualDirHierarchy.Length)
                    {
                        DirInfo newDirInfo = CreateDirInfo(d, encryptedFilename, dir);
                        dirList.Add(PathJoin(dir.VirtualPath, newDirInfo.Name));
                    }
                }
            }
        }
        return dirList;
    }

    public override List<FolderInfo> GetFolders(string virtualPath)
    {
        string[] virtualDirHierarchy = GetDirHierarchy(virtualPath);

        Stack<DirInfo> stack = new Stack<DirInfo>();
        var folderList = new List<FolderInfo>();

        stack.Push(GetRootDirInfo());

        while (stack.Count > 0)
        {
            var dir = stack.Pop();
            var files = Directory.EnumerateDirectories(dir.PhysicalPath);

            foreach (string d in files)
            {
                string encryptedFilename = Path.GetFileName(d);
                if (IsVirtualDirectory(d))
                {
                    if (dir.Level < virtualDirHierarchy.Length)
                    {
                        DirInfo newDir = CreateDirInfo(d, encryptedFilename, dir);
                        if (newDir.Name.ToLower() == virtualDirHierarchy[dir.Level].ToLower())
                        {
                            stack.Push(newDir);
                            break;
                        }
                    }
                    else if (dir.Level == virtualDirHierarchy.Length)
                    {
                        DirInfo newDirInfo = CreateDirInfo(d, encryptedFilename, dir);
                        folderList.Add(
                            new FolderInfo
                            {
                                VirtualPath = PathJoin(dir.VirtualPath, newDirInfo.Name),
                                Name = newDirInfo.Name,
                                HasChildren = DirectoryHasChildren(newDirInfo.PhysicalPath)
                            });
                    }
                }
            }
        }
        return folderList;
    }

    private bool DirectoryHasChildren(string path)
    {
        return Directory.EnumerateFileSystemEntries(path).Any();
    }


    private bool IsVirtualDirectory(FileSystemInfo f)
    {
        if ((f.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            return IsVirtualDirectory(f.FullName);
        else
            return false;


    }

    private bool IsVirtualDirectory(string directoryFullName)
    {
        //Rule out that it is a file
        if (directoryFullName.EndsWith(".c9s") && File.Exists(PathJoin(directoryFullName, "contents.c9r")))
            return false;

        return true;

    }

    private string GetFilePhysicalPath(string virtualPath)
    {
        string[] virtualDirHierarchy = GetDirHierarchy(virtualPath);
        string searchFilename = virtualDirHierarchy[virtualDirHierarchy.Length - 1];
        virtualDirHierarchy = virtualDirHierarchy.Take(virtualDirHierarchy.Length - 1).ToArray();     //Remove filename portion of path at the end

        Stack<DirInfo> stack = new Stack<DirInfo>();

        stack.Push(GetRootDirInfo());

        while (stack.Count > 0)
        {
            var dir = stack.Pop();
            var di = new DirectoryInfo(dir.PhysicalPath);
            var fsis = di.EnumerateFileSystemInfos();

            foreach (FileSystemInfo fsi in fsis)
            {
                string encryptedFilename = fsi.Name;
                if(IsVirtualDirectory(fsi))
                {
                    //It's a directory...
                    if (dir.Level < virtualDirHierarchy.Length)
                    {
                        DirInfo newDir = CreateDirInfo(fsi.FullName, encryptedFilename, dir);
                        if (newDir.Name.ToLower() == virtualDirHierarchy[dir.Level].ToLower())
                        {
                            stack.Push(newDir);
                            break;
                        }
                    }
                }
                else
                {
                    //It's a file...  is the right one?
                    if (dir.Level == virtualDirHierarchy.Length)
                    {
                        string Base64EncryptedName = encryptedFilename;
                        var filename = DecryptFileName(fsi.FullName, dir.ParentDirId);
                        if (filename.ToLower() == searchFilename.ToLower())
                        {
                            if (fsi.FullName.EndsWith(".c9s"))
                                return PathJoin(fsi.FullName, "contents.c9r");
                            else
                                return fsi.FullName;
                        }
                    }
                }
            }
        }
        return "";
    }

    private DirInfo GetRootDirInfo()
    {
        return new DirInfo
        {
            VirtualPath = "",
            PhysicalPath = PathJoin(vaultPath, "d", physicalPathRoot),
            ParentDirId = "",
            Level = 0
        };

    }

    private DirInfo CreateDirInfo(string physicalDirFile, string base64Ciphertext, DirInfo parent)
    {

        var filename = DecryptFileName(physicalDirFile, parent.ParentDirId);

        string[] lines = File.ReadAllLines(PathJoin(physicalDirFile, "dir.c9r"));
        string dirID = lines[0];
        Debug.Assert(lines[0].Length == 36 && lines.Length == 1);

        var dirIdHash =
            Base32Encoding.ToString(sha1.ComputeHash(siv.Seal(Encoding.UTF8.GetBytes(dirID))));
        Debug.Assert(dirIdHash.Length == 32);

        var actualDirPath = PathJoin(dirIdHash.Substring(0, 2), dirIdHash.Substring(2));

        return new DirInfo
        {
            Name = filename,
            VirtualPath = PathJoin(parent.VirtualPath, filename),
            PhysicalPath = PathJoin(vaultPath, "d", actualDirPath),
            ParentDirId = dirID,
            Level = parent.Level + 1
        };

    }

    private string DecryptFileName(string fullFileName, string parentDirId)
    {
        string Base64EncryptedName = Path.GetFileName(fullFileName);

        if (Base64EncryptedName.EndsWith(".c9s"))
            Base64EncryptedName = GetEncryptedLongFilename(fullFileName);

        if (Base64EncryptedName.EndsWith(".c9r"))
            Base64EncryptedName = Base64EncryptedName.Substring(0, Base64EncryptedName.Length - 4);

        byte[] encryptedName = Base64UrlSafeEncoding.ToBytes(Base64EncryptedName);
        byte[] plaintextName = siv.Open(encryptedName, null, Encoding.UTF8.GetBytes(parentDirId));
        return Encoding.UTF8.GetString(plaintextName);

    }

    private string GetEncryptedLongFilename(string fullFileName)
    {
        string location = PathJoin(fullFileName, "name.c9s");
        string[] lines = File.ReadAllLines(location);
        return lines[0];

    }


    public override void DecryptFile(string virtualPath, string outFile)
    {
        using (FileStream outputStream = new FileStream(outFile, FileMode.Create))
        {
            DecryptFile(virtualPath, outputStream);
        }

    }

    public override void DecryptFile(string virtualPath, Stream outputStream)
    {
        var sw = new Stopwatch();

        sw.Start();
        string encryptedFilePath = GetFilePhysicalPath(virtualPath);
        if (String.IsNullOrEmpty(encryptedFilePath))
            throw new ArgumentException("Unable to locate encrypted file");
        sw.Stop();
#if DEBUG
        Console.WriteLine("Directory search took {0} ms", sw.ElapsedMilliseconds);
#endif

        using (FileStream encryptedStream = new FileStream(encryptedFilePath, FileMode.Open))
        using (BinaryReader reader = new BinaryReader(encryptedStream))
        {
            sw.Reset();
            sw.Start();
            Console.WriteLine("Decrypting...");
            DecryptStream(encryptedStream, outputStream);
            sw.Stop();
            Console.WriteLine("Decryption complete.  Time: {0} ms", sw.ElapsedMilliseconds);
        }

    }

    private string[] GetDirHierarchy(string virtualPath)
    {

        if (virtualPath.Contains(pathSeparator + pathSeparator))
            throw new ArgumentException("Invalid file path");

        if (virtualPath.StartsWith(pathSeparator))
            virtualPath = virtualPath.Substring(1);

        string[] dirList = virtualPath.Split(pathSeparator[0]);

        if (dirList[0] == "")
            return new string[0];               //root only, return empty hierarchy.
        else
            return dirList;

    }

    void DecryptStream(Stream encryptedStream, Stream output)
    {
        using (BinaryReader reader = new BinaryReader(encryptedStream))
        using (BinaryWriter writer = new BinaryWriter(output))
        {
            //Read file header
            byte[] headerNonce;
            byte[] ciphertextPayload;
            byte[] mac;
            byte[] contentKey;
            byte[] cleartextPayload;

            headerNonce = reader.ReadBytes(16);
            ciphertextPayload = reader.ReadBytes(40);
            mac = reader.ReadBytes(32);

            HMAC headerHmac = new HMAC(macKey);
            headerHmac.Update(headerNonce);
            headerHmac.DoFinal(ciphertextPayload);
            if (!headerHmac.Hash.SequenceEqual(mac))
                throw new IOException("Encrypted file fails integrity check.");

            cleartextPayload = AesCtr(ciphertextPayload, masterKey, headerNonce);
            contentKey = Slice(cleartextPayload, 8, 32);

            HMAC chunkHmac = new HMAC(macKey);

            //Process all chunks
            for (int blocknum = 0; ; ++blocknum)
            {
                //read file content payload
                byte[] chunk;
                chunk = reader.ReadBytes(32768 + 48);
                if (chunk.Length == 0)
                    break;

                var chunkNonce = Slice(chunk, 0, 16);
                var chunkpayload = Slice(chunk, chunkNonce.Length, chunk.Length - 48);
                var chunkmac = Slice(chunk, chunkNonce.Length + chunkpayload.Length, 32);


                byte[] beBlockNum = BitConverter.GetBytes((long)blocknum);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(beBlockNum);

                chunkHmac.Initialize();
                chunkHmac.Update(headerNonce);
                chunkHmac.Update(beBlockNum);
                chunkHmac.Update(chunkNonce);
                chunkHmac.DoFinal(chunkpayload);
                if (!chunkHmac.Hash.SequenceEqual(chunkmac))
                    throw new IOException("Encrypted file fails integrity check.");

                var decryptedContent = AesCtr(chunkpayload, contentKey, chunkNonce);
                writer.Write(decryptedContent);
            }
        }

    }

    static byte[] Slice(byte[] input, int offset, int length)
    {
        byte[] output = new byte[length];
        Array.Copy(input, offset, output, 0, length);
        return output;
    }

    static byte[] AesCtr(byte[] input, byte[] Key, byte[] IV)
    {
        IV = (byte[])IV.Clone();                      //use a copy to avoid updating original IV.

        //Since we're always decrypting an in-memory chunk we don't bother with streams
        using (Aes128CounterMode aesAlg = new Aes128CounterMode(IV))
        {
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(Key, IV);
            return decryptor.TransformFinalBlock(input, 0, input.Length);
        }
    }


    string PathJoin(params string[] values)
    {

        string result = String.Join(pathSeparator, values);

        //All returned paths are relative to root (ie. no leading backslash required)
        //so remove leading backslash if present (happens when valu[0] == "" [root])

        if (result.StartsWith(pathSeparator))
            result = result.Substring(1);

        return result;

    }

    private class HMAC : HMACSHA256
    {
        public HMAC(byte[] key) : base(key) { }

        public void Update(byte[] input)
        {
            TransformBlock(input, 0, input.Length, input, 0);
        }

        public byte[] DoFinal(byte[] input)
        {
            TransformFinalBlock(input, 0, input.Length);
            return Hash;

        }
    }

}
