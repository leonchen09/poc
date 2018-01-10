using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PdePlugin
{
    [System.Xml.Serialization.XmlRoot("MapInfo")]
    public class MapInfo
    {
        public MapInfo()
        {
            Maps = new List<MapNode>();
        }
             
        [System.Xml.Serialization.XmlElement("Map")]
        public List<MapNode> Maps { get; set; }
    }
    [System.Xml.Serialization.XmlRoot("MapNode")]
    public class MapNode
    {

        public MapNode()
        {
            columns = new List<TabCol>();
        }

        [System.Xml.Serialization.XmlElement("treeNode")]
        public String treeNode { get; set; }
        [System.Xml.Serialization.XmlElement("type", typeof(MapType))]
        public MapType type { get; set; }
        [System.Xml.Serialization.XmlElement("target")]
        public string target { get; set; }
        [System.Xml.Serialization.XmlElement("columns")]
        public List<TabCol> columns { get; set; }

        public string xPath { get; set; }
    }

    public class TabCol
    {

        public TabCol()
        {
        }

        public TabCol(String column, String node)
        {
            this.columnName = column;
            this.treeNode = node;
        }

        [System.Xml.Serialization.XmlElement("columnName")]
        public string columnName { get; set; }
        [System.Xml.Serialization.XmlElement("treeNode")]
        public string treeNode { get; set; }

        public string xPath { get; set; }
    }

    public enum MapType
    {
        SingleCell,
        Table,
    }
}
