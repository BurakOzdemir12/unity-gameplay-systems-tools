using UnityEngine;

namespace _Project.Systems.SharedGameplay.Feedback
{
    public class CharacterFeedbackProfileHolder : MonoBehaviour
    {
        [SerializeField] private CharacterFeedbackProfile profile;
        public CharacterFeedbackProfile Profile => profile;
    }
}