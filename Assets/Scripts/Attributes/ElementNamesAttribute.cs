using UnityEngine;

namespace QueueConnect.Attributes
{
    /// <summary>
    /// Set individual names of elements. Intended for IEnumerable properties or fields in editor
    /// </summary>
    public class ElementNamesAttribute : PropertyAttribute
    {
        //--------------------------------------------------
        #region ---PROPERTIES---

        public string[] ElementNames { get; }

        public string DefaultElementName { get; } = DEFAULT_NAME;

        public string IndexFormat { get; }

        public bool DisplayIndex { get; } = true;

        #endregion

        //--------------------------------------------------
        #region ---PRIVATE VARIABLES---

        private const string DEFAULT_FORMAT = "00";          //the default string format
        private const string DEFAULT_NAME = "element";       //the default element name

        #endregion

        #region ---CONSTRUCTOR---

        //--------------------------------------------------

        public ElementNamesAttribute(params string[] _ElementNames)
        {
            this.ElementNames = _ElementNames;
            this.IndexFormat = DEFAULT_FORMAT;
        }

        //--------------------------------------------------
        public ElementNamesAttribute(string[] _ElementNames, string _DefaultElementName)
        {
            this.ElementNames = _ElementNames;
            this.DefaultElementName = _DefaultElementName;
            this.IndexFormat = DEFAULT_FORMAT;
        }

        //--------------------------------------------------
        public ElementNamesAttribute(string[] _ElementNames, string _DefaultElementName, string _IndexFormat)
        {
            this.ElementNames = _ElementNames;
            this.DefaultElementName = _DefaultElementName;
            this.IndexFormat = _IndexFormat;
        }

        //--------------------------------------------------
        public ElementNamesAttribute(string[] _ElementNames, string _DefaultElementName, bool _DisplayIndex)
        {
            this.ElementNames = _ElementNames;
            this.DefaultElementName = _DefaultElementName;
            this.IndexFormat = DEFAULT_FORMAT;
            this.DisplayIndex = _DisplayIndex;
        }

        #endregion
    }
}