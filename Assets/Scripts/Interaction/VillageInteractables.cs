using UnityEngine;

[CreateAssetMenu(fileName = "Village Interactables", menuName = "RPG/Village Interactables")]
public class VillageInteractablesData : ScriptableObject
{
    [Header("Village Interactables Configuration")]
    public InteractionContent[] villageContents;
    
    [System.Serializable]
    public class VillageInteractableDefinition
    {
        public string objectName;
        public InteractionType type;
        public InteractionContent content;
        public Vector3 preferredPosition;
        public bool autoPlace = true;
    }
    
    public VillageInteractableDefinition[] interactableDefinitions;
}

public class VillageInteractables : MonoBehaviour
{
    [Header("Auto-Setup")]
    public bool autoCreateInteractables = true;
    public bool useExistingObjects = true;
    public bool enableDebugLogs = true;
    
    [Header("Default Settings")]
    public Material defaultHighlightMaterial;
    public AudioClip defaultInteractionSound;
    
    void Start()
    {
        if (autoCreateInteractables)
        {
            SetupVillageInteractables();
        }
    }
    
    public void SetupVillageInteractables()
    {
        if (enableDebugLogs)
        {
            Debug.Log("VillageInteractables: Setting up village interactables...");
        }
        
        SetupBuildings();
        SetupProps();
        SetupLoreObjects();
        
        if (enableDebugLogs)
        {
            Debug.Log("VillageInteractables: Setup complete");
        }
    }
    
    void SetupBuildings()
    {
        SetupTownHall();
        SetupShop();
        SetupInn();
        SetupBlacksmith();
        SetupChapel();
        SetupHouse();
    }
    
    void SetupProps()
    {
        SetupVillageWell();
        SetupNoticeBoard();
        SetupBarrels();
        SetupCrates();
        SetupFences();
        SetupRocks();
    }
    
    void SetupLoreObjects()
    {
        SetupStatue();
        SetupAncientStone();
        SetupMemorialPlaque();
    }
    
    void SetupTownHall()
    {
        GameObject townHall = FindObjectByName("Town Hall");
        if (townHall != null)
        {
            BuildingInteractable interactable = SetupBuildingInteractable(townHall);
            interactable.content = new InteractionContent
            {
                title = "Meadowbrook Town Hall",
                description = "The administrative heart of Meadowbrook, where Mayor Aldric Thornfield conducts village affairs. The hall's oak doors bear the carved seal of Aethermoor - a crowned tree surrounded by mystical runes.",
                loreText = "Built three centuries ago when Meadowbrook was granted village status by King Aetherius I, this hall has witnessed countless council meetings, harvest celebrations, and royal proclamations. The ceremonial gavel inside is said to be carved from the ancient Heartwood Tree.",
                category = "Municipal",
                showContentPanel = true,
                contentDisplayTime = 8f,
                canRepeatInteraction = true
            };
            interactable.hasInterior = true;
            interactable.interiorSceneName = "TownHallInterior";
        }
    }
    
    void SetupShop()
    {
        GameObject shop = FindObjectByName("Shop");
        if (shop != null)
        {
            BuildingInteractable interactable = SetupBuildingInteractable(shop);
            interactable.content = new InteractionContent
            {
                title = "Elderleaf Trading Post",
                description = "Merchant Gwendolyn Silverquill's establishment, filled with goods from across Aethermoor. Shelves overflow with healing herbs from the Whispering Woods, enchanted trinkets from the Crystal Caverns, and rare spices from the Southern Reaches.",
                loreText = "The Silverquill family has operated this trading post for seven generations. Gwendolyn's ancestor, Corvus Silverquill, was the first merchant to establish trade routes through the dangerous Shadowmere Passes, bringing prosperity to Meadowbrook.",
                category = "Commerce",
                showContentPanel = true,
                contentDisplayTime = 7f,
                canRepeatInteraction = true
            };
            interactable.hasInterior = true;
            interactable.interiorSceneName = "ShopInterior";
        }
    }
    
    void SetupInn()
    {
        GameObject inn = FindObjectByName("Inn");
        if (inn != null)
        {
            BuildingInteractable interactable = SetupBuildingInteractable(inn);
            interactable.content = new InteractionContent
            {
                title = "The Prancing Pegasus Inn",
                description = "Innkeeper Tobias Goldmane's warm hospitality welcomes weary travelers. The common room crackles with hearth fire, filled with the aroma of his famous mutton stew and honeyed ale. Upstairs rooms offer comfort after long journeys.",
                loreText = "Named after the legendary winged horse that once served the Aethermoor Royal Guard, this inn has sheltered heroes, merchants, and wanderers for over 200 years. The pegasus weathervane atop the roof is said to bring good fortune to all who stay here.",
                category = "Hospitality",
                showContentPanel = true,
                contentDisplayTime = 7f,
                canRepeatInteraction = true
            };
            interactable.hasInterior = true;
            interactable.interiorSceneName = "InnInterior";
        }
    }
    
    void SetupBlacksmith()
    {
        GameObject blacksmith = FindObjectByName("Blacksmith");
        if (blacksmith != null)
        {
            BuildingInteractable interactable = SetupBuildingInteractable(blacksmith);
            interactable.content = new InteractionContent
            {
                title = "Ironwright Forge",
                description = "Master Smith Gareth Ironwright's forge burns day and night, crafting everything from horseshoes to ceremonial weapons. The rhythmic hammer strikes echo through the village, and sparks dance in the smoky air.",
                loreText = "The Ironwright family forged the ceremonial sword presented to King Aetherius III during his coronation. Gareth continues the tradition of excellence, using techniques passed down through five generations and star-metal ore from the Celestial Peaks.",
                category = "Craftsmanship",
                showContentPanel = true,
                contentDisplayTime = 7f,
                canRepeatInteraction = true
            };
            interactable.hasInterior = true;
            interactable.interiorSceneName = "BlacksmithInterior";
        }
    }
    
    void SetupChapel()
    {
        GameObject chapel = FindObjectByName("Chapel");
        if (chapel != null)
        {
            BuildingInteractable interactable = SetupBuildingInteractable(chapel);
            interactable.content = new InteractionContent
            {
                title = "Chapel of Eternal Light",
                description = "Priestess Seraphina Dawnbringer tends to this sacred sanctuary dedicated to Solara, goddess of healing and harvest. Stained glass windows cast rainbow patterns across the altar, and holy water from the Sacred Springs fills the font.",
                loreText = "This chapel was consecrated during the Great Blight of 847, when Priestess Celeste Lightbringer's prayers to Solara ended the crop disease that threatened the kingdom. The blessed bells in the tower still ring at dawn and dusk, carrying protective magic across the village.",
                category = "Sacred",
                showContentPanel = true,
                contentDisplayTime = 8f,
                canRepeatInteraction = true
            };
            interactable.hasInterior = true;
            interactable.interiorSceneName = "ChapelInterior";
        }
    }
    
    void SetupHouse()
    {
        GameObject house = FindObjectByName("House");
        if (house != null)
        {
            BuildingInteractable interactable = SetupBuildingInteractable(house);
            interactable.content = new InteractionContent
            {
                title = "Willowmere Cottage",
                description = "The charming home of Farmer Elias Willowmere and his family. Window boxes overflow with colorful flowers, and the garden behind produces the finest vegetables in three counties. Smoke rises peacefully from the chimney.",
                loreText = "The Willowmere family has cultivated these lands for twelve generations, perfecting the art of moon-phase planting taught by the ancient druids. Their seeds are blessed by earth spirits, ensuring bountiful harvests even in difficult seasons.",
                category = "Residential",
                showContentPanel = true,
                contentDisplayTime = 6f,
                canRepeatInteraction = true
            };
            interactable.hasInterior = true;
            interactable.interiorSceneName = "HouseInterior";
        }
    }
    
    void SetupVillageWell()
    {
        GameObject well = FindObjectByName("Village Well");
        if (well != null)
        {
            LoreInteractable interactable = SetupLoreInteractable(well);
            interactable.content = new InteractionContent
            {
                title = "The Eternal Spring Well",
                description = "The heart of Meadowbrook, this ancient well draws from the Eternal Springs that flow deep beneath Aethermoor. The water is crystal clear and never runs dry, even during the worst droughts. Intricate stonework depicts flowing water and blessing runes.",
                loreText = "Legend speaks of the first settlers guided here by a silver doe, messenger of the water spirits. When they dug this well, they discovered it connects to the same mystical springs that flow beneath the royal palace, marking Meadowbrook as a place of destiny.",
                category = "Mystical",
                showContentPanel = true,
                contentDisplayTime = 8f,
                canRepeatInteraction = true
            };
            interactable.unlockJournalEntry = true;
            interactable.journalEntryId = "eternal_springs";
        }
    }
    
    void SetupNoticeBoard()
    {
        GameObject noticeBoard = FindObjectByName("Notice Board");
        if (noticeBoard != null)
        {
            LoreInteractable interactable = SetupLoreInteractable(noticeBoard);
            interactable.content = new InteractionContent
            {
                title = "Village Notice Board",
                description = "Weather-worn notices announce village events, missing livestock, and calls for aid. Recent posts mention strange lights in the Whispering Woods, a reward for retrieving Mrs. Applebottom's escaped chickens, and preparations for the Harvest Moon Festival.",
                loreText = "This board has carried news both mundane and momentous for over a century. The most famous notice was King Aetherius II's call for heroes to investigate the Dragon's Hollow - a quest that led to the founding of the legendary Silver Circle of knights.",
                category = "Information",
                showContentPanel = true,
                contentDisplayTime = 6f,
                canRepeatInteraction = true
            };
            interactable.unlockJournalEntry = true;
            interactable.journalEntryId = "village_notices";
        }
    }
    
    void SetupBarrels()
    {
        SetupPropWithContent("Barrel", "Merchant's Barrel", 
            "A sturdy oak barrel stamped with the Silverquill Trading Company seal. It likely contains preserved goods or supplies for the trading post. The wood shows signs of long journeys across kingdom roads.",
            "These barrels are crafted by the renowned Cooper's Guild in the capital city of Aetherspire. The mystical preservation runes carved into the wood keep contents fresh for months, essential for long-distance trade routes.",
            "Storage");
    }
    
    void SetupCrates()
    {
        SetupPropWithContent("Crate", "Supply Crate",
            "A reinforced wooden crate bearing shipping marks from various settlements across Aethermoor. Rope bindings and iron corners suggest it contains valuable or fragile cargo awaiting delivery.",
            "The standardized crate design was mandated by the Royal Commerce Guild to ensure efficient transport across the kingdom's vast trade network. Each crate can be traced to its origin through the mystical merchant marks.",
            "Storage");
    }
    
    void SetupFences()
    {
        SetupPropWithContent("Fence", "Village Boundary Fence",
            "Well-maintained fencing that marks property boundaries and keeps livestock contained. The wood is treated with protective oils and enchanted to resist rot and pest damage.",
            "Meadowbrook's fences follow the ancient property laws established by King Aetherius I, ensuring fair division of land while maintaining harmony between neighbors. The protective enchantments are renewed annually by traveling druids.",
            "Infrastructure");
    }
    
    void SetupRocks()
    {
        SetupPropWithContent("Rock", "Ancient Boundary Stone",
            "A weathered stone marker that has stood here since before the village's founding. Faint runic inscriptions are barely visible, worn smooth by centuries of wind and rain.",
            "These boundary stones were placed by the first human settlers, guided by ancient agreements with the forest spirits. The runes mark sacred boundaries that must never be moved, ensuring harmony between civilization and nature.",
            "Ancient");
    }
    
    void SetupStatue()
    {
        GameObject statue = CreateOrFindProp("Village Statue", Vector3.zero);
        if (statue != null)
        {
            LoreInteractable interactable = SetupLoreInteractable(statue);
            interactable.content = new InteractionContent
            {
                title = "Monument to the Silver Circle",
                description = "A majestic statue depicting a knight in gleaming armor, sword raised toward the sky. The inscription reads: 'In honor of Sir Galahad Brightshield and the Silver Circle, who defended Meadowbrook against the Shadow Drake of the Northern Wastes.'",
                loreText = "Erected 150 years ago to commemorate the heroic defense of Meadowbrook during the Shadow Drake's rampage. Sir Galahad and his companions held the beast at bay for three days until reinforcements arrived from the capital. Their sacrifice saved hundreds of lives.",
                category = "Memorial",
                showContentPanel = true,
                contentDisplayTime = 9f,
                canRepeatInteraction = true
            };
            interactable.unlockJournalEntry = true;
            interactable.journalEntryId = "silver_circle_history";
        }
    }
    
    void SetupAncientStone()
    {
        GameObject ancientStone = CreateOrFindProp("Ancient Runestone", new Vector3(5, 0, 8));
        if (ancientStone != null)
        {
            LoreInteractable interactable = SetupLoreInteractable(ancientStone);
            interactable.content = new InteractionContent
            {
                title = "Waymarker of the First Kingdom",
                description = "An ancient standing stone covered in intricate runic carvings that seem to shimmer in moonlight. The markings are far older than any known writing system, predating even the elvish settlements in the Silverleaf Forest.",
                loreText = "Archaeological scholars believe this stone was placed by the mysterious First Kingdom, a civilization that vanished over a millennium ago. The runes reportedly point toward hidden paths through the mystical Threshold Realm.",
                category = "Archaeological",
                showContentPanel = true,
                contentDisplayTime = 9f,
                canRepeatInteraction = true
            };
            interactable.unlockJournalEntry = true;
            interactable.journalEntryId = "first_kingdom_mystery";
        }
    }
    
    void SetupMemorialPlaque()
    {
        GameObject plaque = CreateOrFindProp("Memorial Plaque", new Vector3(-8, 0, -5));
        if (plaque != null)
        {
            LoreInteractable interactable = SetupLoreInteractable(plaque);
            interactable.content = new InteractionContent
            {
                title = "Memorial to the Lost Expedition",
                description = "A bronze plaque mounted on a simple stone cairn. The inscription honors the brave souls of the Thornwood Expedition who ventured into the Cursed Marshlands seeking the Lost Crown of Shadows and never returned.",
                loreText = "Fifteen years ago, renowned explorer Captain Marcus Thornwood led an expedition to recover the crown, believing it held the key to lifting an ancient curse. Though they never returned, their courage inspired new generations of adventurers to continue the quest.",
                category = "Memorial",
                showContentPanel = true,
                contentDisplayTime = 8f,
                canRepeatInteraction = true
            };
            interactable.unlockJournalEntry = true;
            interactable.journalEntryId = "thornwood_expedition";
        }
    }
    
    void SetupPropWithContent(string objectBaseName, string title, string description, string loreText, string category)
    {
        GameObject[] props = GameObject.FindGameObjectsWithTag("Prop");
        foreach (GameObject prop in props)
        {
            if (prop.name.Contains(objectBaseName))
            {
                PropInteractable interactable = SetupPropInteractable(prop);
                interactable.content = new InteractionContent
                {
                    title = title,
                    description = description,
                    loreText = loreText,
                    category = category,
                    showContentPanel = true,
                    contentDisplayTime = 5f,
                    canRepeatInteraction = true
                };
            }
        }
    }
    
    BuildingInteractable SetupBuildingInteractable(GameObject obj)
    {
        BuildingInteractable interactable = obj.GetComponent<BuildingInteractable>();
        if (interactable == null)
        {
            interactable = obj.AddComponent<BuildingInteractable>();
        }
        
        ApplyDefaultSettings(interactable);
        return interactable;
    }
    
    PropInteractable SetupPropInteractable(GameObject obj)
    {
        PropInteractable interactable = obj.GetComponent<PropInteractable>();
        if (interactable == null)
        {
            interactable = obj.AddComponent<PropInteractable>();
        }
        
        ApplyDefaultSettings(interactable);
        return interactable;
    }
    
    LoreInteractable SetupLoreInteractable(GameObject obj)
    {
        LoreInteractable interactable = obj.GetComponent<LoreInteractable>();
        if (interactable == null)
        {
            interactable = obj.AddComponent<LoreInteractable>();
        }
        
        ApplyDefaultSettings(interactable);
        return interactable;
    }
    
    void ApplyDefaultSettings(InteractableObject interactable)
    {
        if (defaultHighlightMaterial != null)
        {
            interactable.highlightMaterial = defaultHighlightMaterial;
        }
        
        if (defaultInteractionSound != null && interactable.interactionSound == null)
        {
            interactable.interactionSound = defaultInteractionSound;
        }
        
        interactable.displayName = interactable.gameObject.name;
        interactable.isInteractable = true;
    }
    
    GameObject FindObjectByName(string name)
    {
        return GameObject.Find(name);
    }
    
    GameObject CreateOrFindProp(string name, Vector3 position)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
        {
            return existing;
        }
        
        GameObject newProp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newProp.name = name;
        newProp.transform.position = position;
        newProp.transform.localScale = new Vector3(1f, 2f, 0.5f);
        newProp.tag = "Prop";
        
        return newProp;
    }
    
    [ContextMenu("Setup Village Interactables")]
    public void SetupVillageInteractablesContext()
    {
        SetupVillageInteractables();
    }
    
    [ContextMenu("Count Interactables")]
    public void CountInteractablesContext()
    {
        InteractableObject[] interactables = FindObjectsOfType<InteractableObject>();
        Debug.Log($"VillageInteractables: Found {interactables.Length} interactable objects in scene");
        
        foreach (InteractableObject interactable in interactables)
        {
            Debug.Log($"- {interactable.DisplayName} ({interactable.GetType().Name})");
        }
    }
}