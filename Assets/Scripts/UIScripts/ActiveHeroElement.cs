using UnityEngine;
using UnityEngine.EventSystems;


public class ActiveHeroElement : MonoBehaviour, IPointerClickHandler {

    public HeroStatPanel heroPanel;
    //public Adventurer storedAdventurer;

    void Start()
    {
        heroPanel = GameObject.Find("HeroInfoPanel").GetComponent<HeroStatPanel>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        heroPanel.SetActiveHero(ActiveHeroPanel.Instance.AdventurerInSlot(gameObject));
    }
}
