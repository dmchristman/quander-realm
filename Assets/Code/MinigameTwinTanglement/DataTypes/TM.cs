using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Labyrinth 
{ 
    public class TM : MonoBehaviour
    {
        public Tilemap bottom;
        public Tilemap walls;
        public Tilemap goal;
        public Tilemap overlay;

        public int type; //-1 for main, +1 for mirror

        public int translation(int size, float cent) {
            if (size % 2 == 0) { //if its even
                return (int)(type * (cent+2));
            }
            else { //if its odd
                return (int)(type * (cent+1));
            }
        }

        public void toggleRenderer(Tilemap map) {
            TilemapRenderer rend = map.GetComponent<TilemapRenderer>();
            if (rend.enabled == true) {
                rend.enabled = false;
            }
            else if (rend.enabled == false) {
                rend.enabled = true;
            }
        }

        public void toggleCollider(Tilemap map) {
            TilemapCollider2D coll = map.GetComponent<TilemapCollider2D>();
            if (coll.enabled == true) {
                coll.enabled = false;
            }
            else if (coll.enabled == false) {
                coll.enabled = true;
            }
        }

    }
}