using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.Utilities
{
    public class TMPCharacterAnimation : MonoBehaviour
    {
        [SerializeField] private TMP_Text targetText;
        [SerializeField] private float scaleMultiplier = 1.5f;
        [SerializeField] private float animationDuration = 0.2f;
        [SerializeField] private float characterInterval = 0.05f;

        private Coroutine _animationCoroutine;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void StartAnimation()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            
            _animationCoroutine = StartCoroutine(AnimateCharacters());
        }

        private void OnDisable()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }

            ResetCharacterVertices();
            
            if (_button)
                _button.interactable = true;
        }

        private IEnumerator AnimateCharacters()
        {
            if (!targetText) yield break; 

            if(_button)
                _button.interactable = false;

            targetText.ForceMeshUpdate();

            TMP_TextInfo textInfo = targetText.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];

                if (!characterInfo.isVisible)
                    continue;

                int materialIndex = characterInfo.materialReferenceIndex;
                int vertexIndex = characterInfo.vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
                Vector3 center = (vertices[vertexIndex] + vertices[vertexIndex + 2]) * 0.5f;
                Vector3[] originalVertices = new Vector3[4];

                for (int j = 0; j < 4; j++)
                {
                    originalVertices[j] = vertices[vertexIndex + j];
                }

                float elapsed = 0f;

                while (elapsed < animationDuration)
                {
                    elapsed += Time.deltaTime;

                    float t = Mathf.Clamp01(elapsed / animationDuration);
                    float scale = Mathf.Lerp(1f, scaleMultiplier, Mathf.Sin(t * Mathf.PI));

                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] = center + (originalVertices[j] - center) * scale;
                    }

                    targetText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                    yield return null;
                }

                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] = originalVertices[j];
                }


                targetText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                yield return new WaitForSeconds(characterInterval);
            }
            
            if (_button)
                _button.interactable = true;

            _animationCoroutine = null;
        }

        private void ResetCharacterVertices()
        {
            if (!targetText)
                return;

            targetText.ForceMeshUpdate();

            TMP_TextInfo textInfo = targetText.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];

                if (!characterInfo.isVisible)
                    continue;

                int materialIndex = characterInfo.materialReferenceIndex;
                int vertexIndex = characterInfo.vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
                Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;
                Vector3[] originalVertices = new Vector3[4];

                for (int j = 0; j < 4; j++)
                {
                    originalVertices[j] = sourceVertices[vertexIndex + j];
                }

                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] = originalVertices[j];
                }
            }

            targetText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }
    }
}