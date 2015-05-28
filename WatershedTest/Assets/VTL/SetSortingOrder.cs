using UnityEngine;
using System.Collections;

namespace VTL
{
    public class SetSortingOrder : MonoBehaviour
    {
        public int newSortingOrder = 1;

        void Start()
        {
            GetComponent<Renderer>().sortingOrder = newSortingOrder;
        }
    }
}