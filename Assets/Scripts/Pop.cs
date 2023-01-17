using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Pop : MonoBehaviour
{
   public bool isPopAble { get; private set; }
   private bool isContact;
   private void Start()
   {
      
   }

   [ContextMenu("PopObj")]
   public void PopObject()
   {
      LookUpPointy();
      if (isContact)
      {
         // print(isContact);
         isContact = false;
         gameObject.SetActive(false);
      }
   }



   public void LookUpPointy()
   {
      var neighbors = GameManager.GetInstance.GetCheckSurrounding.CheckNeighborsWithCollider(gameObject);

      for (int i = 0; i < neighbors.Count; i++)
      {
         if ((ObjDirection)i != ObjDirection.Up && (ObjDirection)i != ObjDirection.Down)
         {
            print((ObjDirection)i);
            foreach (var item in neighbors[(ObjDirection)i])
            {
               if (item.gameObject.name == "Cactus")
               {
                  isContact = true;
                  return;
               }
            }
         }
      }
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
