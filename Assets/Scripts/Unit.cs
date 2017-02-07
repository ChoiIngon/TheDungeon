using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    public List<Buff> buffs;
    public int maxHealth;
    public float speed;
    // Use this for initialization
    void Start () {
        buffs = new List<Buff>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void OnDamage(int damage)
    {
    }
}
