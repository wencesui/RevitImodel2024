using Bentley.ECObjects;
using Bentley.ECObjects.Instance;
using Bentley.ECObjects.Schema;
using Bentley.ECObjects.XML;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PublishImodel
{
    internal class ECDataXmlWriter : XmlTextWriter
    {
        private bool m_bSkipDocWrite;

        public bool SkipDocWrite
        {
            get
            {
                return this.m_bSkipDocWrite;
            }
            set
            {
                this.m_bSkipDocWrite = value;
            }
        }

        private ECDataXmlWriter(string filename, Encoding encoding) : base(filename, encoding)
        {
        }

        //WS: note how to write schema and instances into an XML file!
        public static void WriteData(string fileName, IECSchema schema, List<IECInstance> instList)
        {
            //ExpImpLogger.trace(string.Format("Export - write data to file {0}", fileName));

            ECDataXmlWriter eCDataXmlWriter = new ECDataXmlWriter(fileName, Encoding.UTF8);

            eCDataXmlWriter.WriteStartDocument();
            eCDataXmlWriter.WriteStartElement("Data");

            //ExpImpLogger.trace("Export - writing schema");

            eCDataXmlWriter.WriteStartElement("Schema");
            ECSchemaXmlWriter eCSchemaXmlWriter = new ECSchemaXmlWriter();
            eCDataXmlWriter.SkipDocWrite = true;
            eCSchemaXmlWriter.Serialize(eCDataXmlWriter, schema);

            eCDataXmlWriter.SkipDocWrite = false;
            eCDataXmlWriter.WriteEndElement();
            eCDataXmlWriter.WriteStartElement("Instances");
            eCDataXmlWriter.WriteAttributeString("Count", instList.Count.ToString());
            ECInstanceXmlWriter eCInstanceXmlWriter = new ECInstanceXmlWriter();

            //ExpImpLogger.trace("Export - writing instances");

            for (int i = 0; i < instList.Count; i++)
            {
                eCDataXmlWriter.WriteStartElement("Instance");
                eCDataXmlWriter.WriteAttributeString("Class", instList[i].ClassDefinition.Name);
                eCInstanceXmlWriter.Serialize(eCDataXmlWriter, instList[i]);
                eCDataXmlWriter.WriteEndElement();
            }
            eCDataXmlWriter.WriteEndElement();
            eCDataXmlWriter.WriteEndElement();
            eCDataXmlWriter.Flush();
            eCDataXmlWriter.Close();
        }

        public override void WriteEndDocument()
        {
            if (!this.m_bSkipDocWrite)
            {
                base.WriteEndDocument();
            }
        }

        public override void WriteStartDocument()
        {
            if (!this.m_bSkipDocWrite)
            {
                base.WriteStartDocument();
            }
        }
    }
}