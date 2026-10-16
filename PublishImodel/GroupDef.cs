using Bentley.ECObjects.Instance;
using Bentley.ECObjects.Schema;
using Bentley.ECObjects.UI;
using System;

namespace PublishImodel
{
    internal class GroupDef
    {
        private string m_strName;

        private string m_strLabel;

        private string m_strDescription;

        private int m_sortPriority;

        private bool m_bExpand;

        public GroupDef(string strName, int iGroupid)
        {
            //string str = strName;
            //string str1 = str;
            this.m_strDescription = strName;
            //string str2 = str1;
            //string str3 = str2;
            this.m_strLabel = strName;
            this.m_strName = strName;
            this.m_sortPriority = iGroupid;
            this.m_bExpand = true;
        }

        public IECInstance CreateCategory(IECSchema schema)
        {
            return ECPropertyPane.CreateCategory(this.m_strName, this.m_strLabel, this.m_strDescription, 
                                                this.m_sortPriority, this.m_bExpand);
        }
    }
}