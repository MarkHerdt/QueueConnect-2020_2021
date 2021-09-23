using UnityEngine;

namespace QueueConnect.Attributes
{
    /// <summary>
    /// Set custom name for elements. Intended for IEnumerable properties or fields in editor
    /// </summary>
    public class ElementNameAttribute : PropertyAttribute
    {
        //--------------------------------------------------
        #region ---PROPERTIES---

        public string ElementName { get; }

        public  string IndexFormat { get; }

        public  bool DisplayIndex { get; }

        #endregion

        //--------------------------------------------------
        #region ---PRIVATE VARIABLES---

        private const string DEFAULT_FORMAT = "00";           //the default string format
        #endregion


        #region ---CONSTRUCTOR---

        //--------------------------------------------------
        public ElementNameAttribute(string _ElementName)
        {
            this.ElementName = _ElementName;
            this.IndexFormat = DEFAULT_FORMAT;
            this.DisplayIndex = true;
        }

        //--------------------------------------------------
        public ElementNameAttribute(string _ElementName, string _IndexFormat = DEFAULT_FORMAT)
        {
            this.ElementName = _ElementName;
            this.IndexFormat = _IndexFormat;
            this.DisplayIndex = true;
        }

        //--------------------------------------------------
        public ElementNameAttribute(string _ElementName, bool _DisplayIndex = true, string _IndexFormat = DEFAULT_FORMAT)
        {
            this.ElementName = _ElementName;
            this.IndexFormat = _IndexFormat;
            this.DisplayIndex = _DisplayIndex;
        }

        #endregion
    }
}