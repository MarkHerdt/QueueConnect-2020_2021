namespace Ganymed.Utils.Editor
{
    public abstract class CleanEditor : UnityEditor.Editor
    {
        private static readonly string[] Exclude = new string[]{"m_Script"};
         
        public sealed override void OnInspectorGUI()
        {
            serializedObject.Update();
            OnBeforeDefaultInspector();
            OnDefaultInspector();
            OnAfterDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnDefaultInspector()
        {
            DrawPropertiesExcluding(serializedObject, Exclude);
        }
    
        protected virtual void OnBeforeDefaultInspector()
        {}
 
        protected virtual void OnAfterDefaultInspector()
        {}
    }
}
