using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopTest : MonoBehaviour
{
   public bool isPopAble { get; private set; }
   private bool isContact;
   private Collider cl;
   private Rigidbody rb;
   private void Start()
   {
      cl = GetComponent<Collider>();
      if (!gameObject.TryGetComponent<Rigidbody>(out rb))
         gameObject.AddComponent<Rigidbody>();
      rb = GetComponent<Rigidbody>();
      rb.isKinematic = true;
      rb.useGravity = false;
   }

   [ContextMenu("PopObj")]
   public void PopObject()
   {
      LookUpPointy();
      if (isContact)
      {
         isContact = false;
         gameObject.SetActive(false);
      }
   }

   public void LookUpPointy()
   {
      RaycastHit[] forwardHit;
      forwardHit = rb.SweepTestAll(Vector3.forward, 1f);
      foreach (var obj in forwardHit)
      {
         if (obj.collider.name == "Cactus")
         {
            isContact = true;
            return;
         }
         
      }
    
      RaycastHit[] backHit;
      backHit = rb.SweepTestAll(Vector3.back, 1f);
      foreach (var obj in backHit)
      {
         if (obj.collider.name == "Cactus")
         {
            isContact = true;
            return;
         }

      }
     
      RaycastHit[] leftHit;
      leftHit = rb.SweepTestAll(Vector3.left, 1f);
      foreach (var obj in leftHit)
      {
         if (obj.collider.name == "Cactus")
         {
            isContact = true;
            return;
         }
      }
      
      RaycastHit[] rightHit;
      rightHit = rb.SweepTestAll(Vector3.right, 1f);
      foreach (var obj in rightHit)
      {
         if (obj.collider.name == "Cactus")
         {
            isContact = true;
            return;
         }
      }
   }
   
   [ContextMenu("Test")]
   public void LookForPointy()
   {
      var centerPos = gameObject.GetComponent<Collider>().bounds.center;
      var extensPos = gameObject.GetComponent<Collider>().bounds.extents;
      
      RaycastHit hit;
      Ray front = new Ray(centerPos, Vector3.forward);
      Ray frontButtom = new Ray(transform.position, Vector3.forward);
      Ray back = new Ray(centerPos, Vector3.back);
      Ray backButtom = new Ray(transform.position,Vector3.back);
      Ray left = new Ray(centerPos, Vector3.left);
      Ray right = new Ray(centerPos, Vector3.right);
   
      if (Physics.Raycast(front, out hit, Mathf.Infinity))
      {
         if (hit.distance<(centerPos.z+extensPos.z)+.5f&& hit.transform.gameObject.name =="Cactus")
         {
            isContact = true;
            return;
         }
      }
      if (Physics.Raycast(back, out hit, Mathf.Infinity))
      {
         if (hit.distance < (centerPos.z + extensPos.z) + .5f&& hit.transform.gameObject.name =="Cactus")
         {
            isContact = true;
            return;
         }
      }
   
      if (Physics.Raycast(left, out hit, Mathf.Infinity))
      {
         if (hit.distance < (centerPos.x + extensPos.x) + .5f && hit.transform.gameObject.name =="Cactus")
         {
            isContact = true;
            return;
         }
      }
      if (Physics.Raycast(right, out hit, Mathf.Infinity))
      {
         if (hit.distance < (centerPos.x + extensPos.x) + .5f && hit.transform.gameObject.name =="Cactus")
         {
            isContact = true;
         }
      }
      
   }

   private void FixedUpdate()
   {
      PopObject();
      Debug.DrawLine(cl.bounds.center, 
         new Vector3(cl.bounds.center.x + cl.bounds.extents.x +.5f, cl.bounds.center.y, cl.bounds.center.z),Color.red );
      Debug.DrawLine(transform.position,new Vector3(cl.bounds.center.x + cl.bounds.extents.x +.5f, transform.position.y, cl.bounds.center.z),Color.magenta);
      
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
