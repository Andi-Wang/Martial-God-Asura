using UnityEngine;
using System.Collections;

public class PlayerHitbox : MonoBehaviour {
    string currentSkill;
    int rank;
    
    public void setSkill(string skillName, int skillRank) {
        currentSkill = skillName;
        rank = skillRank;
    }
    
     
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy") {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            UnityStandardAssets._2D.PlatformerCharacter2D player = this.transform.parent.gameObject.GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>();
            AnimatorStateInfo state = player.getAnim().GetCurrentAnimatorStateInfo(0);

            if(currentSkill == "BasicPunch") {
                Skill.BasicPunch(rank, enemy);
            }

            
        }
    }
}
