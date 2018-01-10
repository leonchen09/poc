using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlSerTest
{
    public class Obj1
    {
        public int id { get; set; }

        public string name { get; set; }

        public bool flag { get; set; }

        public DateTime birthday { get; set; }

        public float f1 { get; set; }

        public double d1 { get; set; }

        public decimal dec1 { get; set; }

        public objstatus status { get; set; }

        public List<string> strArray { get; set; }

        public List<int> intArray { get; set; }

        public byte[] datas { get; set; }

        public string ToString()
        {
            string result = "Obj1:";
            result += "\r\nid: " + id;
            result += "\r\nname: " + name;
            result += "\r\nbirthday: " + birthday;
            result += "\r\nflag: " + flag;
            result += "\r\nf1: " + f1;
            result += "\r\nd1: " + d1;
            result += "\r\ndec1: " + dec1;
            result += "\r\nstatus: " + status;
            result += "\r\nstrArray: " + strArray;
            return result;
        }

       
    }
    public enum objstatus
    {
        prepare = 0,
        initlize = 1,
        loading = 2,
        saving = 3
    }
}
