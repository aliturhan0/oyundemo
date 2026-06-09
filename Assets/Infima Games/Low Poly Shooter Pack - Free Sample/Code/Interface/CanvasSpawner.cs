// Copyright 2021, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack.Interface
{
    /// <summary>
    /// Player Interface.
    /// </summary>
    public class CanvasSpawner : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Settings")]
        
        [Tooltip("Canvas prefab spawned at start. Displays the player's user interface.")]
        [SerializeField]
        private GameObject canvasPrefab;

        #endregion

        #region UNITY FUNCTIONS

        private void Awake()
        {
            //Spawn Interface.
            GameObject spawnedCanvas = Instantiate(canvasPrefab);

            // Watermark metinlerini bul ve gizle
            if (spawnedCanvas != null)
            {
                var texts = spawnedCanvas.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
                foreach (var text in texts)
                {
                    string content = text.text;
                    if (!string.IsNullOrEmpty(content) && 
                        (content.Contains("Low Poly") || content.Contains("Shooter Pack") || content.Contains("Infima")))
                    {
                        text.gameObject.SetActive(false);
                    }
                }
            }
        }

        #endregion
    }
}