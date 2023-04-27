using UnityEngine;

namespace Scripts
{
    public class Enter : MonoBehaviour
    {
        public int Count { get; set; } = 0;

        public void Add()
        {
            Count++;
        }

        public void ResetCount()
        {
            Count = 0;
        }
    }
}