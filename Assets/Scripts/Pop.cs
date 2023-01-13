using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Pop : MonoBehaviour
{
   public bool isPopAble { get; private set; }
   private bool isContact;
   private Rigidbody rb;
   [SerializeField]private float sweepDistance = 1f;
   private void Start()
   {
      if (!gameObject.TryGetComponent<Rigidbody>(out rb))
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
      print(isContact);
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
      RaycastHit[] forwardHit;
      forwardHit = rb.SweepTestAll(Vector3.forward, sweepDistance);
      foreach (var obj in forwardHit)
      {
         if (obj.collider.name == "Cactus")
         {
            isContact = true;
            return;
         }
         
      }
    
      RaycastHit[] backHit;
      backHit = rb.SweepTestAll(Vector3.back, sweepDistance);
      foreach (var obj in backHit)
      {
         if (obj.collider.name == "Cactus")
         {
            isContact = true;
            return;
         }

      }
     
      RaycastHit[] leftHit;
      leftHit = rb.SweepTestAll(Vector3.left, sweepDistance);
      foreach (var obj in leftHit)
      {
         if (obj.collider.name == "Cactus")
         {
            isContact = true;
            return;
         }
      }
      
      RaycastHit[] rightHit;
      rightHit = rb.SweepTestAll(Vector3.right, sweepDistance);
      foreach (var obj in rightHit)
      {
         if (obj.collider.name == "Cactus")
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
