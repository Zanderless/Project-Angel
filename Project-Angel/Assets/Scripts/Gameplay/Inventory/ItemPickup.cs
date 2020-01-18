using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    public Item item;

    private bool inTrigger;

    private void Update()
    {

        if (inTrigger)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Inventory.Instance.AddItem(item);
                Destroy(transform.parent.gameObject);
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            inTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.tag == "Player")
            inTrigger = false;

    }

}
