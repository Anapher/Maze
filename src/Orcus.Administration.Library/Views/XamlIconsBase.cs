using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace Orcus.Administration.Library.Views
{
    public abstract class XamlIconsBase
    {
        protected static Viewbox CreateImage(string s)
        {
            var stringReader = new StringReader(s);
            var xmlReader = XmlReader.Create(stringReader);
            return (Viewbox) XamlReader.Load(xmlReader);
        }
    }
}