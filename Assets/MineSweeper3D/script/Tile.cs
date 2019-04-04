using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Minesweeper3D
{
    public class Tile : MonoBehaviour
    {
        public int x, y, z;
        public bool isMine = false;
        public bool isRevealed = false;

        [Range(0, 1)]
        public float mineChance = 0.15f;

        public GameObject minePrefab;
        public GameObject textPrefab;

        public Gradient textGradient;

        private Collider col;
        private Animator anim;
        private GameObject mine, text;

        private Color originalColor;
        public Color flagColor;
        public Renderer rend;
        public bool isFlagged = false;
        // Use this for initialization
        void Awake()
        {
            anim = GetComponent<Animator>();
            col = GetComponent<Collider>();
        }
        GameObject Spawnchild(GameObject prefab)
        {
            //Spawn child and attach them
            GameObject child = Instantiate(prefab, transform);
            child.transform.localPosition = Vector3.zero;
            child.SetActive(false);
            return child;

        }
        void Start()
        {
            originalColor = rend.material.color;

            isMine = Random.value < mineChance;
            if (isMine)
            {
                mine = Spawnchild(minePrefab);
               
            }
            else
            {
                text = Spawnchild(textPrefab);
                
            }
           
        }
        // Update is called once per frame


        public void Reveal(int adjacentMines = 0)
        {
            isRevealed = true;
            anim.SetTrigger("Reveal");
            col.enabled = false;

            if (isMine)
            {
                mine.SetActive(true);
            }
            else
            {
                if (adjacentMines > 0)
                {
                    // Enabling text
                    text.SetActive(true);
                    // Setting text
                    TextMeshPro tmp = text.GetComponent<TextMeshPro>();
                    float time = adjacentMines / 9f; // lerp the adjacent mines
                    tmp.color = textGradient.Evaluate(time);

                    tmp.text = adjacentMines.ToString();
                }

            }


        }

        public void Flag()
        {
            isFlagged = !isFlagged;
            rend.material.color = isFlagged ? flagColor : originalColor;


        }
        
    }

}
