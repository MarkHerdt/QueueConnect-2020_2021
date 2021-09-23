using System;

namespace Ganymed.Utils.Attributes
{
    /// <summary>
    /// Attribute allows to automatically set the value of a behaviours script execution without having to set it
    /// manually in the editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptOrder : Attribute
    {
        public readonly int order;
        public ScriptOrder(int order)
        {
            this.order = order;
        }
    }
}