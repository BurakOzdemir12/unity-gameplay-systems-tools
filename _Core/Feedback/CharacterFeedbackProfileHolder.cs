using UnityEngine;

namespace _Project.Systems._Core.Feedback
{
    public class CharacterFeedbackProfileHolder : MonoBehaviour
    {
        [SerializeField] private CharacterFeedbackProfile profile;
        public CharacterFeedbackProfile Profile => profile;
    }
}