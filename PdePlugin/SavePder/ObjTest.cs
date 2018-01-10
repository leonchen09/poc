using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PdwPlugin
{
    public class ObjTest
    {
        public int age;
        public String name;
        public DateTime birthday;
        public bool sex;
        public DateTimeOffset dto;
        public List<String> strs;
        public List<Child> children;
    }

    public class Child
    {
        public String ttt = "tt22";
        public int num = 2;
    }
}
