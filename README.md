# CryptomatorAccessDemo
Windows demo program for de-crypting files inside Cryptomator vaults

# Introduction
This demo application is provided to illustrate how C# can be used to decrypt Crytomator-encrypted vaults.

It provides only one-at-a-time single-file decryption, although a Windows-form front-end is provided which will render a tree-view of the vault to make file selection simple.  Encryption of plaintext files is not supported, and access to Cryptomator vaults is read-only.  No modifications are made either to the Cryptomator files nor the vault file structure.

# Disclaimer
I am not part of the official Cryptomator development team and this project is not supported or sanctioned by the team.  Use this program at your own risk.

# System Requirements
**.Net 4.6.1** on compatible Windows systems.  This program was not intended for, and will not run on other operating systems.

The project uses several NuGet packages, all of which are MIT-licensed or are public-domain:

- **Miscreant 0.3.3** 
- **Json.Net**
- **RFC3394**
- **SCrypt 2.0.0.2**

...and a variety of Microsoft .Net packages that are dependencies because the Miscreant library targets .Net Standard  1.3

# Supported Vault Formats
This program supports version 6-8 vaults (Cryptomator 1.4.x - 1.6.x).


# License
This demo application and related source code are licensed under GPLv3 unless otherwise noted.


# Change History

**4/1/2022** - Added support for version 8 vaults. 

**9/11/2020** - Added **Explore Encrypted File** right-click context menu option.  This will open a Windows file explorer at the *encrypted* file location.  This can facilitate restoring previous versions of the encrypted file in some cases (for example, when the vault is stored on **Microsoft OneDrive**).  Restore previous versions with caution and at your own risk.