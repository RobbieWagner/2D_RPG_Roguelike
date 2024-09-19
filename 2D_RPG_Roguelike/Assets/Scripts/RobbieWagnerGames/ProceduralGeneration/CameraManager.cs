using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RobbieWagnerGames.TileSelectionGame
{
    public class CameraManager: MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        [SerializeField] private Vector3 cameraPosOffset;
        private Coroutine currentMovementCo;

        public static CameraManager instance { get; private set; }

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            if(gameCamera == null)
                gameCamera = Camera.main;
        }

        //private void CenterCameraOnSelection(Selectable selectable)
        //{
        //    if(currentMovementCo != null)
        //        StopCoroutine(currentMovementCo);
        //    currentMovementCo = StartCoroutine(CenterCameraOnSelectionCo(selectable));
        //}

        //private IEnumerator CenterCameraOnSelectionCo(Selectable selectable)
        //{
        //    Vector3 position = selectable.transform.position + cameraPosOffset;
        //    yield return gameCamera.transform.DOMove(position, .5f);
        //    currentMovementCo = null;    
        //}
    }
}