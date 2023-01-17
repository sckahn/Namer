using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Flameable : MonoBehaviour
{
   private bool isContact;
   [Range(0,1)]public float sweepDistance = .5f;

   private void FixedUpdate()
   {
      ObjectOnFire();
   }

   public void ObjectOnFire()
   {
      LookUpFlame();
      if (isContact )
      {
         isContact = false;
         gameObject.SetActive(false);
         // print("Boom");
      }
   }


   private void LookUpFlame()
   {
      var neighbors = GameManager.GetInstance.GetCheckSurrounding.CheckNeighboursObjectsUsingSweepTest(gameObject,sweepDistance);

      for (int i = 0; i < neighbors.Count; i++)
      {
         if ((Dir)i != Dir.up && (Dir)i != Dir.down)
         {
            foreach (var item in neighbors[(Dir)i])
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
