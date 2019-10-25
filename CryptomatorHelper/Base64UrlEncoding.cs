using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Base64UrlSafeEncoding
{
    public static byte[] ToBytes(string input)
    {
        //Per RFC4648, replace + with - and / with _
        return Convert.FromBase64String(input.Replace('-', '+').Replace('_', '/'));
    }
    public static string ToString(byte[] input)
    {
        //Per RFC4648, replace + with - and / with _
        return Convert.ToBase64String(input).Replace('+', '-').Replace('/', '_');

    }

}
