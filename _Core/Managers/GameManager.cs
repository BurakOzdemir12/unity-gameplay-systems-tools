using System;
using UnityEngine;

namespace _Project.Systems._Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private bool isDangerZone;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this.gameObject);
            Instance = this;
            
            
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            isDangerZone = false;
        }
    }
}