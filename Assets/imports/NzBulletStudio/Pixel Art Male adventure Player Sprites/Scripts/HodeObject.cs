using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NzBulletLookDev
{
    public class HodeObject : MonoBehaviour
    {
       public GameObject ObjectToHide;
        
    
        public void HideObject()
        {
            ObjectToHide.SetActive(false);
        }
        public void ShowObject()
        {
            ObjectToHide.SetActive(true);
        }
    }
}
