using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NzBulletLookDev
{
    public class SetBool : MonoBehaviour
    {
        public Animator DoorAnimator;
        
    
        public void SetDoorOpenBool()
        {
            DoorAnimator.SetBool("Open", true);
        }
        public void SetDoorCloseBool()
        {
            DoorAnimator.SetBool("Open", false);
        }
    }
}
