using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdePlugin
{
    [System.Xml.Serialization.XmlRoot("ObjTest")]
    public class ObjTest
    {
        public int age;
        public String name;
        public DateTime birthday;
        public int getAge()
        {
            return age;
        }
        public void setAge(int age)
        {
            this.age = age;
        }
        public String getName()
        {
            return name;
        }
        public void setName(String name)
        {
            this.name = name;
        }
        public DateTime getBirthday()
        {
            return birthday;
        }
        public void setBirthday(DateTime birthday)
        {
            this.birthday = birthday;
        }
        public bool isSex()
        {
            return sex;
        }
        public void setSex(bool sex)
        {
            this.sex = sex;
        }
        public bool sex;
    }
}
