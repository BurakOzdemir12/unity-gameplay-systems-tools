using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.UI.EnemyHud
{
    public class EnemyHUDPool : MonoBehaviour
    {
        public static EnemyHUDPool Instance { get; private set; }

        [Header("Settings")] [SerializeField] private EnemyHUDView hudPrefab;
        [SerializeField] private Transform hudParent;
        [SerializeField] private int initialPoolSize = 20;

        private Queue<EnemyHUDView> poolQueue = new Queue<EnemyHUDView>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this.gameObject);

            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewHUDAndEnqueue();
            }
        }

        private EnemyHUDView CreateNewHUDAndEnqueue()
        {
            EnemyHUDView hud = Instantiate(hudPrefab, hudParent);
            hud.gameObject.SetActive(false);
            poolQueue.Enqueue(hud);
            return hud;
        }

        public EnemyHUDView GetHUD()
        {
            if (poolQueue.Count == 0)
            {
                CreateNewHUDAndEnqueue();
            }

            EnemyHUDView hud = poolQueue.Dequeue();
            hud.ResetHUD();
            return hud;
        }

        public void ReturnHUD(EnemyHUDView hud)
        {
            if (hud == null) return;

            hud.gameObject.SetActive(false);
            poolQueue.Enqueue(hud);
        }
    }
}