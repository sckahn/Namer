using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pop : MonoBehaviour
{
   public bool isPopAble { get; private set; }
   private bool isContact;
   private Rigidbody rb;
   private void Start()
   {
      if (!gameObject.TryGetComponent<Rigidbody>(out rb) == false)
         gameObject.AddComponent<Rigidbody>();
      rb = GetComponent<Rigidbody>();
      rb.isKinematic = true;
      rb.useGravity = false;
   }

   [ContextMenu("PopObj")]
   public void PopObject()
   {
      // print("here?");
      LookUpPointy();
      if (isContact)
      {
         // print(isContact);
         isContact = false;
         gameObject.SetActive(false);
      }
   }

   
   [ContextMenu("Test")]
   public void LookUpPointy()
   {
      RaycastHit hit;
      if (rb.SweepTest(Vector3.forward,out hit, 1f))
      {
         if (hit.collider.name == "Cactus")
         {
            isContact = true;
            return;
         }
      }
      if (rb.SweepTest(Vector3.back,out hit, 1f))
      {
         if (hit.collider.name == "Cactus")
         {
            isContact = true;
            return;
         }
      }
      if (rb.SweepTest(Vector3.left,out hit, 1f))
      {
         if (hit.collider.name == "Cactus")
         {
            isContact = true;
            return;
         }
      }
      if (rb.SweepTest(Vector3.right,out hit, 1f))
      {
         if (hit.collider.name == "Cactus")
         {
            isContact = true;
            return;
         }
      }
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
