using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pop : MonoBehaviour
{
   public bool isPopAble { get; private set; }
   private bool isContact;

   [ContextMenu("PopObj")]
   public void PopObject()
   {
      // print("here?");
      LookForPointy();
      if (isContact)
      {
         // print(isContact);
         isContact = false;
         gameObject.SetActive(false);
      }
   }

   
   [ContextMenu("Test")]
   public void LookForPointy()
   {
      var centerPos = gameObject.GetComponent<Collider>().bounds.center;
      var extensPos = gameObject.GetComponent<Collider>().bounds.extents;
      
      RaycastHit hit;
      Ray front = new Ray(centerPos, Vector3.forward);
      Ray back = new Ray(centerPos, Vector3.back);
      Ray left = new Ray(centerPos, Vector3.left);
      Ray right = new Ray(centerPos, Vector3.right);

      if (Physics.Raycast(front, out hit, Mathf.Infinity))
      {
         if (hit.distance<(centerPos.z+extensPos.z)+.5f&& hit.transform.gameObject.name =="Cactus")
         {
            // print("front check");
            isContact = true;
            return;
         }
      }
      if (Physics.Raycast(back, out hit, Mathf.Infinity))
      {
         if (hit.distance < (centerPos.z + extensPos.z) + .5f&& hit.transform.gameObject.name =="Cactus")
         {
            // print("back check");
            isContact = true;
            return;
         }
      }

      if (Physics.Raycast(left, out hit, Mathf.Infinity))
      {
         if (hit.distance < (centerPos.x + extensPos.x) + .5f && hit.transform.gameObject.name =="Cactus")
         {
            // print("left check");
            isContact = true;
            return;
         }
      }
      if (Physics.Raycast(right, out hit, Mathf.Infinity))
      {
         if (hit.distance < (centerPos.x + extensPos.x) + .5f && hit.transform.gameObject.name =="Cactus")
         {
            // print("right check");
            isContact = true;
            return;
         }
      }
      // print(isContact);
   }

   private void FixedUpdate()
   {
      PopObject();
   }


   public void MakeItPopable()
   {
      isPopAble = true;
   }

   bool CheckPopAble()
   {
      return isPopAble;
   }
}
