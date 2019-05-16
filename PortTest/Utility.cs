using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace PortTest
{
    public class Utility
    {
        public static bool CheckIP(string text)
        {
            //匹配ip
            if (!Regex.IsMatch(text, @"(^([1-9]|1[0-9]|1[1-9]{2}|2[0-4][0-9]|25[0-5])\.)(([0-9]{1,2}|1[1-9]{2}|2[0-4][0-9]|25[0-5])\.){2}([0-9]{1,2}|1[1-9]{2}|2[0-5][0-9]|25[0-4])$"))
            {
                return false;
            }
            return true;
        }

        public static ulong IP2ULong(string ip)
        {
            if (!CheckIP(ip))
            {
                //ip error
                MessageBox.Show("ip错误");
            }
            List<ulong> data = new List<ulong>();
            var ips = ip.Split('.');
            foreach (var item in ips)
            {
                data.Add(ulong.Parse(item));
            }
            ulong result = 0;
            ulong first = data[0] * 256 * 256 * 256;
            ulong second = data[1] * 256 * 256;
            ulong third = data[2] * 256;
            result = first + second + third + data[3];
            return result;
        }

        public static string ULong2IP(ulong ip)
        {
            ulong tmp = ip;
            ulong last = tmp % 256;
            tmp = tmp - last;
            ulong third = tmp % (256 * 256)/256;
            tmp = tmp - third;
            ulong second = tmp % (256 * 256 * 256)/256/256;
            tmp = tmp - third;
            ulong first = tmp % ((ulong)256 * 256 * 256 * 256)/256/256/256;

            return $"{first}.{second}.{third}.{last}";
        }
    }
}
