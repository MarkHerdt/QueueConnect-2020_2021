using System;
using UnityEngine;

namespace QueueConnect.Tutorial
{
    [Serializable]
    public class TutorialText
    {
        #pragma warning disable 649
        [SerializeField][TextArea(5,5)] private string Text;
        #pragma warning restore 649

        public override string ToString()
        {
            return Text;
        }

        public static implicit operator string(TutorialText tutorialText)
        {
            return tutorialText.ToString();
        }
    }
}