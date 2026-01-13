using System;
using UnityEngine;

namespace _Project.Systems._Core.Weapon_Tool_Handlers
{
    public class ToolHandler : MonoBehaviour
    {
        [Header("Assign ToolRoot here (not Hitbox)")] [SerializeField]
        private GameObject currentToolRoot;

        [Header("ToolLogic ")] private GameObject currentToolHitbox;
        public GameObject CurrentToolHitBox => currentToolHitbox;

        private ToolLogic.ToolLogic currentToolLogic;

        public ToolLogic.ToolLogic CurrentToolLogic => currentToolLogic;

        private void Start()
        {
            ToolLogic.ToolLogic toolLogic = currentToolRoot.GetComponentInChildren<ToolLogic.ToolLogic>(true);
            if (toolLogic == null)
            {
                Debug.LogError($"{name}: ToolLogic couldn't find in the children!", this);
                return;
            }

            currentToolLogic = toolLogic;
            currentToolHitbox = toolLogic.gameObject;
            currentToolHitbox.SetActive(false);
        }

        private void EnableTool()
        {
            if (currentToolHitbox != null)
                currentToolLogic.PerformLootAction();
        }

        private void DisableTool()
        {
            if (currentToolHitbox != null)
                currentToolLogic.EndLootAction();
        }
    }
}