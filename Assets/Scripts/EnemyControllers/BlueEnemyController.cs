using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueEnemyController : EnemyController
{

    protected override void Death()
    {

        if(transform.childCount > 0)
        {

            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                Transform tr = transform.GetChild(i);

                if (tr.gameObject.TryGetComponent<EnemyController>(out EnemyController subController))
                {

                    subController.health = health / transform.childCount;

                    tr.SetParent(transform.parent);
                    tr.gameObject.SetActive(true);

                }

            }

        }

        base.Death();

    }

}
