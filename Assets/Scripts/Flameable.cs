using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Flameable : MonoBehaviour
{
   private bool isContact;

   public void ObjectOnFire()
   {
      LookUpFlame();
      if (isContact )
      {
         // print(isContact);
         isContact = false;
         gameObject.SetActive(false);
      }
   }


   private void LookUpFlame()
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
}
