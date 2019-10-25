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
This program supports version 6 and version 7 vaults (Cryptomator 1.4.x and 1.5.x).


# License
This demo application and related source code are licensed under GPLv3 unless otherwise noted.
