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
      rb = GetComponent<Rigidbody>();
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
   
   // [ContextMenu("Test")]
   // public void LookForPointy()
   // {
   //    var centerPos = gameObject.GetComponent<Collider>().bounds.center;
   //    var extensPos = gameObject.GetComponent<Collider>().bounds.extents;
   //    
   //    RaycastHit hit;
   //    Ray front = new Ray(centerPos, Vector3.forward);
   //    Ray frontButtom = new Ray(transform.position, Vector3.forward);
   //    Ray back = new Ray(centerPos, Vector3.back);
   //    Ray backButtom = new Ray(transform.position,Vector3.back);
   //    Ray left = new Ray(centerPos, Vector3.left);
   //    Ray right = new Ray(centerPos, Vector3.right);
   //
   //    if (Physics.Raycast(front, out hit, Mathf.Infinity))
   //    {
   //       if (hit.distance<(centerPos.z+extensPos.z)+.5f&& hit.transform.gameObject.name =="Cactus")
   //       {
   //          isContact = true;
   //          return;
   //       }
   //    }
   //    if (Physics.Raycast(back, out hit, Mathf.Infinity))
   //    {
   //       if (hit.distance < (centerPos.z + extensPos.z) + .5f&& hit.transform.gameObject.name =="Cactus")
   //       {
   //          isContact = true;
   //          return;
   //       }
   //    }
   //
   //    if (Physics.Raycast(left, out hit, Mathf.Infinity))
   //    {
   //       if (hit.distance < (centerPos.x + extensPos.x) + .5f && hit.transform.gameObject.name =="Cactus")
   //       {
   //          isContact = true;
   //          return;
   //       }
   //    }
   //    if (Physics.Raycast(right, out hit, Mathf.Infinity))
   //    {
   //       if (hit.distance < (centerPos.x + extensPos.x) + .5f && hit.transform.gameObject.name =="Cactus")
   //       {
   //          isContact = true;
   //       }
   //    }
   //    
   // }

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
