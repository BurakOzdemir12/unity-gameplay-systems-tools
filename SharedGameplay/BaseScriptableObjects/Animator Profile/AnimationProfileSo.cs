using System.Collections.Generic;
using UnityEngine;

namespace _Project.Systems._Core.ScriptableObjects.Animator_Profile
{
    [CreateAssetMenu(fileName = "AnimationProfileData",
        menuName = "Scriptable Objects/Characters/Animation Profile/Animation Profile So")]
    public class AnimationProfileSo : ScriptableObject
    {
        public List<AnimList> Animations;
    }

    [System.Serializable]
    public class AnimList
    {
        public string animName;
        public string animationTag;
        public string Key;
    }
}